using Genomancy.Core.ResourceTesting;

namespace Genomancy.Storage.JsonFile;

public sealed class ResourceTestBatchRunJsonFileWorkflow
{
    private readonly JsonFileStore<IReadOnlyList<ResourceTestBatchRunSpecification>> _planStore;
    private readonly JsonFileStore<ResourceTestRunResult> _runResultStore;
    private readonly JsonFileStore<ResourceTestBatchRunResult> _batchResultStore;
    private readonly ResourceTestResultManifestJsonFileUpdater _manifestUpdater;

    public ResourceTestBatchRunJsonFileWorkflow(
        JsonFileStore<IReadOnlyList<ResourceTestBatchRunSpecification>>? planStore = null,
        JsonFileStore<ResourceTestRunResult>? runResultStore = null,
        JsonFileStore<ResourceTestBatchRunResult>? batchResultStore = null,
        ResourceTestResultManifestJsonFileUpdater? manifestUpdater = null)
    {
        _planStore = planStore ?? ResourceTestBatchRunJsonFileStore.Create();
        _runResultStore = runResultStore ?? ResourceTestResultJsonFileStore.Create();
        _batchResultStore = batchResultStore ?? ResourceTestBatchRunResultJsonFileStore.Create();
        _manifestUpdater = manifestUpdater ?? new ResourceTestResultManifestJsonFileUpdater();
    }

    public ResourceTestBatchRunJsonFileExecutionResult ExecuteAndAppendManifest(
        string planPath,
        string batchResultPath,
        string manifestPath,
        string? runResultRootPath = null)
    {
        return Execute(
            planPath,
            batchResultPath,
            manifestPath,
            upsertManifest: false,
            runResultRootPath);
    }

    public ResourceTestBatchRunJsonFileExecutionResult ExecuteAndUpsertManifest(
        string planPath,
        string batchResultPath,
        string manifestPath,
        string? runResultRootPath = null)
    {
        return Execute(
            planPath,
            batchResultPath,
            manifestPath,
            upsertManifest: true,
            runResultRootPath);
    }

    public ResourceTestBatchRunJsonFileExecutionResult Execute(
        string planPath,
        string batchResultPath,
        string? manifestPath = null,
        bool upsertManifest = true,
        string? runResultRootPath = null)
    {
        if (string.IsNullOrWhiteSpace(planPath))
        {
            throw new ArgumentException("JSON batch plan path must not be empty.", nameof(planPath));
        }

        if (string.IsNullOrWhiteSpace(batchResultPath))
        {
            throw new ArgumentException("JSON batch result path must not be empty.", nameof(batchResultPath));
        }

        if (manifestPath is not null && string.IsNullOrWhiteSpace(manifestPath))
        {
            throw new ArgumentException("JSON result manifest path must not be empty.", nameof(manifestPath));
        }

        var plan = _planStore.Load(planPath);
        var result = ResourceTestBatchRunner.RunSpecifications(plan);
        var writtenRunResultPaths = SaveRunResults(planPath, result, runResultRootPath);

        _batchResultStore.Save(batchResultPath, result);

        var updatedManifest = manifestPath is null
            ? null
            : upsertManifest
                ? _manifestUpdater.UpsertBatchResult(manifestPath, result)
                : _manifestUpdater.AppendBatchResult(manifestPath, result);

        return new ResourceTestBatchRunJsonFileExecutionResult(
            planPath,
            batchResultPath,
            result,
            writtenRunResultPaths,
            manifestPath,
            updatedManifest);
    }

    private IReadOnlyList<string> SaveRunResults(
        string planPath,
        ResourceTestBatchRunResult result,
        string? runResultRootPath)
    {
        var rootPath = ResolveRunResultRoot(planPath, runResultRootPath);
        var writtenPaths = new List<string>();

        foreach (var run in result.Runs)
        {
            var writePath = ResolveRunResultPath(rootPath, run.ResultPath);
            _runResultStore.Save(writePath, run.Result);
            writtenPaths.Add(writePath);
        }

        return writtenPaths;
    }

    private static string ResolveRunResultRoot(string planPath, string? runResultRootPath)
    {
        if (!string.IsNullOrWhiteSpace(runResultRootPath))
        {
            return runResultRootPath.Trim();
        }

        return Path.GetDirectoryName(Path.GetFullPath(planPath)) ?? Directory.GetCurrentDirectory();
    }

    private static string ResolveRunResultPath(string rootPath, string resultPath)
    {
        return Path.IsPathRooted(resultPath)
            ? resultPath
            : Path.Combine(rootPath, resultPath);
    }
}
