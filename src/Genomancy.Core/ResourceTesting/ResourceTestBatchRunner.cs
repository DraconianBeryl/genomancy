namespace Genomancy.Core.ResourceTesting;

public static class ResourceTestBatchRunner
{
    public static ResourceTestBatchRunResult RunSpecifications(
        IEnumerable<ResourceTestBatchRunSpecification> specifications)
    {
        ArgumentNullException.ThrowIfNull(specifications);

        return Run(specifications.Select(specification => specification.ToRequest()));
    }

    public static ResourceTestBatchRunResult Run(IEnumerable<ResourceTestBatchRunRequest> requests)
    {
        ArgumentNullException.ThrowIfNull(requests);

        var materialized = requests
            .OrderBy(request => request.RunId)
            .ThenBy(request => request.ResultPath, StringComparer.Ordinal)
            .ToArray();

        var duplicateRunIds = materialized
            .GroupBy(request => request.RunId)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key.Value)
            .ToArray();

        if (duplicateRunIds.Length > 0)
        {
            throw new ArgumentException(
                $"Resource test batch contains duplicate run ids: {string.Join(", ", duplicateRunIds)}.",
                nameof(requests));
        }

        var records = materialized.Select(RunRequest).ToArray();
        return new ResourceTestBatchRunResult(records);
    }

    private static ResourceTestBatchRunRecord RunRequest(ResourceTestBatchRunRequest request)
    {
        var result = ResourceTestRunner.Run(request.Definitions, request.Options);
        var manifestTags = request.Tags
            .Concat(result.Cases.SelectMany(testCase => testCase.Tags))
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();
        var manifestEntry = ResourceTestResultManifestEntry.FromResult(
            request.RunId,
            request.ResultPath,
            result,
            request.CompletedAtUtc,
            request.Label,
            manifestTags);

        return new ResourceTestBatchRunRecord(
            request.RunId,
            request.ResultPath,
            result,
            manifestEntry);
    }
}
