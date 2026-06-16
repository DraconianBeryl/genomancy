using Genomancy.Core.ResourceTesting;
using Genomancy.Storage.JsonFile;

namespace Genomancy.Cli;

public static class GenomancyCli
{
    public static int Run(string[] args)
    {
        return Run(args, Console.Out, Console.Error);
    }

    public static int Run(string[] args, TextWriter output, TextWriter error)
    {
        ArgumentNullException.ThrowIfNull(args);
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(error);

        if (args.Length == 0 || IsHelp(args[0]))
        {
            WriteUsage(output);
            return (int)GenomancyCliExitCode.Success;
        }

        if (args.Length >= 2
            && string.Equals(args[0], "batch", StringComparison.Ordinal)
            && string.Equals(args[1], "run", StringComparison.Ordinal))
        {
            return RunBatch(args[2..], output, error);
        }

        if (args.Length >= 2
            && string.Equals(args[0], "batch", StringComparison.Ordinal)
            && string.Equals(args[1], "show", StringComparison.Ordinal))
        {
            return ShowBatchResult(args[2..], output, error);
        }

        if (args.Length >= 2
            && string.Equals(args[0], "result", StringComparison.Ordinal)
            && string.Equals(args[1], "show", StringComparison.Ordinal))
        {
            return ShowRunResult(args[2..], output, error);
        }

        if (args.Length >= 2
            && string.Equals(args[0], "manifest", StringComparison.Ordinal)
            && string.Equals(args[1], "show", StringComparison.Ordinal))
        {
            return ShowManifest(args[2..], output, error);
        }

        if (args.Length >= 3
            && string.Equals(args[0], "manifest", StringComparison.Ordinal)
            && string.Equals(args[1], "result", StringComparison.Ordinal)
            && string.Equals(args[2], "show", StringComparison.Ordinal))
        {
            return ShowManifestResult(args[3..], output, error);
        }

        if (args.Length >= 2
            && string.Equals(args[0], "manifest", StringComparison.Ordinal)
            && string.Equals(args[1], "verify", StringComparison.Ordinal))
        {
            return VerifyManifest(args[2..], output, error);
        }

        if (args.Length >= 2
            && string.Equals(args[0], "manifest", StringComparison.Ordinal)
            && string.Equals(args[1], "regenerate", StringComparison.Ordinal))
        {
            return RegenerateManifest(args[2..], output, error);
        }

        if (args.Length >= 2
            && string.Equals(args[0], "manifest", StringComparison.Ordinal)
            && string.Equals(args[1], "update", StringComparison.Ordinal))
        {
            return UpdateManifest(args[2..], output, error);
        }

        error.WriteLine("Unknown command.");
        WriteUsage(error);
        return (int)GenomancyCliExitCode.UsageError;
    }

    private static int RunBatch(string[] args, TextWriter output, TextWriter error)
    {
        if (args.Length == 0 || args.Any(IsHelp))
        {
            WriteBatchUsage(output);
            return (int)GenomancyCliExitCode.Success;
        }

        BatchRunOptions options;

        try
        {
            options = BatchRunOptions.Parse(args);
        }
        catch (ArgumentException exception)
        {
            error.WriteLine(exception.Message);
            WriteBatchUsage(error);
            return (int)GenomancyCliExitCode.UsageError;
        }

        try
        {
            var workflow = new ResourceTestBatchRunJsonFileWorkflow();
            var execution = workflow.Execute(
                options.PlanPath,
                options.BatchResultPath,
                options.ManifestPath,
                options.UpsertManifest,
                options.RunResultRootPath);

            WriteExecutionSummary(execution, output);

            if (options.ReportPath is not null)
            {
                var reportText = ResourceTestBatchRunTextReportFormatter.WriteToText(execution.BatchResult);
                WriteTextFile(options.ReportPath, reportText);
                output.WriteLine($"Report: {options.ReportPath}");
            }

            if (options.WriteReportToStdout)
            {
                output.Write(ResourceTestBatchRunTextReportFormatter.WriteToText(execution.BatchResult));
            }

            return execution.BatchResult.Status == ResourceTestStatus.Passed
                ? (int)GenomancyCliExitCode.Success
                : (int)GenomancyCliExitCode.TestFailure;
        }
        catch (Exception exception) when (exception is IOException
            or UnauthorizedAccessException
            or ArgumentException
            or InvalidOperationException)
        {
            error.WriteLine($"Batch execution failed: {exception.Message}");
            return (int)GenomancyCliExitCode.ExecutionError;
        }
    }

    private static int ShowBatchResult(string[] args, TextWriter output, TextWriter error)
    {
        if (args.Length == 0 || args.Any(IsHelp))
        {
            WriteBatchShowUsage(output);
            return (int)GenomancyCliExitCode.Success;
        }

        BatchShowOptions options;

        try
        {
            options = BatchShowOptions.Parse(args);
        }
        catch (ArgumentException exception)
        {
            error.WriteLine(exception.Message);
            WriteBatchShowUsage(error);
            return (int)GenomancyCliExitCode.UsageError;
        }

        try
        {
            var result = ResourceTestBatchRunResultJsonFileStore.Create().Load(options.BatchResultPath);
            var report = ResourceTestBatchRunTextReportFormatter.WriteToText(result);

            output.Write(report);

            if (options.ReportPath is not null)
            {
                WriteTextFile(options.ReportPath, report);
                output.WriteLine($"Report: {options.ReportPath}");
            }

            return result.Status == ResourceTestStatus.Passed
                ? (int)GenomancyCliExitCode.Success
                : (int)GenomancyCliExitCode.TestFailure;
        }
        catch (Exception exception) when (IsCliExecutionException(exception))
        {
            error.WriteLine($"Batch result show failed: {exception.Message}");
            return (int)GenomancyCliExitCode.ExecutionError;
        }
    }

    private static int ShowRunResult(string[] args, TextWriter output, TextWriter error)
    {
        if (args.Length == 0 || args.Any(IsHelp))
        {
            WriteResultShowUsage(output);
            return (int)GenomancyCliExitCode.Success;
        }

        ResultShowOptions options;

        try
        {
            options = ResultShowOptions.Parse(args);
        }
        catch (ArgumentException exception)
        {
            error.WriteLine(exception.Message);
            WriteResultShowUsage(error);
            return (int)GenomancyCliExitCode.UsageError;
        }

        try
        {
            var result = ResourceTestResultJsonFileStore.Create().Load(options.ResultPath);
            var report = ResourceTestTextReportFormatter.WriteToText(result);

            output.Write(report);

            if (options.ReportPath is not null)
            {
                WriteTextFile(options.ReportPath, report);
                output.WriteLine($"Report: {options.ReportPath}");
            }

            return result.Status == ResourceTestStatus.Passed
                ? (int)GenomancyCliExitCode.Success
                : (int)GenomancyCliExitCode.TestFailure;
        }
        catch (Exception exception) when (IsCliExecutionException(exception))
        {
            error.WriteLine($"Result show failed: {exception.Message}");
            return (int)GenomancyCliExitCode.ExecutionError;
        }
    }

    private static int ShowManifest(string[] args, TextWriter output, TextWriter error)
    {
        if (args.Length == 0 || args.Any(IsHelp))
        {
            WriteManifestShowUsage(output);
            return (int)GenomancyCliExitCode.Success;
        }

        ManifestShowOptions options;

        try
        {
            options = ManifestShowOptions.Parse(args);
        }
        catch (ArgumentException exception)
        {
            error.WriteLine(exception.Message);
            WriteManifestShowUsage(error);
            return (int)GenomancyCliExitCode.UsageError;
        }

        try
        {
            var manifest = ResourceTestResultManifestJsonFileStore.Create().Load(options.ManifestPath);
            var text = FormatManifest(manifest, options);

            output.Write(text);

            if (options.ReportPath is not null)
            {
                WriteTextFile(options.ReportPath, text);
                output.WriteLine($"Report: {options.ReportPath}");
            }

            return manifest.Entries.Any(entry => entry.Summary.Status == ResourceTestStatus.Failed)
                ? (int)GenomancyCliExitCode.TestFailure
                : (int)GenomancyCliExitCode.Success;
        }
        catch (Exception exception) when (IsCliExecutionException(exception))
        {
            error.WriteLine($"Manifest show failed: {exception.Message}");
            return (int)GenomancyCliExitCode.ExecutionError;
        }
    }

    private static int UpdateManifest(string[] args, TextWriter output, TextWriter error)
    {
        if (args.Length == 0 || args.Any(IsHelp))
        {
            WriteManifestUpdateUsage(output);
            return (int)GenomancyCliExitCode.Success;
        }

        ManifestUpdateOptions options;

        try
        {
            options = ManifestUpdateOptions.Parse(args);
        }
        catch (ArgumentException exception)
        {
            error.WriteLine(exception.Message);
            WriteManifestUpdateUsage(error);
            return (int)GenomancyCliExitCode.UsageError;
        }

        try
        {
            var batchResult = ResourceTestBatchRunResultJsonFileStore.Create().Load(options.BatchResultPath);
            var updater = new ResourceTestResultManifestJsonFileUpdater();
            var manifest = options.UpsertManifest
                ? updater.UpsertBatchResult(options.ManifestPath, batchResult)
                : updater.AppendBatchResult(options.ManifestPath, batchResult);

            WriteManifestUpdateSummary(manifest, options.ManifestPath, options.UpsertManifest, output);

            if (options.ReportPath is not null)
            {
                WriteTextFile(options.ReportPath, FormatManifest(manifest, ManifestShowOptions.Default(options.ManifestPath)));
                output.WriteLine($"Report: {options.ReportPath}");
            }

            return batchResult.Status == ResourceTestStatus.Passed
                ? (int)GenomancyCliExitCode.Success
                : (int)GenomancyCliExitCode.TestFailure;
        }
        catch (Exception exception) when (IsCliExecutionException(exception))
        {
            error.WriteLine($"Manifest update failed: {exception.Message}");
            return (int)GenomancyCliExitCode.ExecutionError;
        }
    }

    private static int ShowManifestResult(string[] args, TextWriter output, TextWriter error)
    {
        if (args.Length == 0 || args.Any(IsHelp))
        {
            WriteManifestResultShowUsage(output);
            return (int)GenomancyCliExitCode.Success;
        }

        ManifestResultShowOptions options;

        try
        {
            options = ManifestResultShowOptions.Parse(args);
        }
        catch (ArgumentException exception)
        {
            error.WriteLine(exception.Message);
            WriteManifestResultShowUsage(error);
            return (int)GenomancyCliExitCode.UsageError;
        }

        try
        {
            var manifest = ResourceTestResultManifestJsonFileStore.Create().Load(options.ManifestPath);
            var entry = FindManifestEntry(manifest, options.RunId);
            var resultPath = ResolveResultPath(options.ResultRootPath, entry.ResultPath);
            var result = ResourceTestResultJsonFileStore.Create().Load(resultPath);
            var summaryDiagnostics = CompareSummaries(entry.Summary, ResourceTestRunSummary.FromResult(result)).ToArray();

            if (summaryDiagnostics.Length > 0 && options.RequireSummaryMatch)
            {
                throw new InvalidOperationException(
                    $"Manifest entry '{entry.RunId}' does not match loaded result summary: {string.Join("; ", summaryDiagnostics)}.");
            }

            var report = FormatManifestResult(entry, resultPath, result, summaryDiagnostics);

            output.Write(report);

            if (options.ReportPath is not null)
            {
                WriteTextFile(options.ReportPath, report);
                output.WriteLine($"Report: {options.ReportPath}");
            }

            return result.Status == ResourceTestStatus.Passed
                ? (int)GenomancyCliExitCode.Success
                : (int)GenomancyCliExitCode.TestFailure;
        }
        catch (Exception exception) when (IsCliExecutionException(exception))
        {
            error.WriteLine($"Manifest result show failed: {exception.Message}");
            return (int)GenomancyCliExitCode.ExecutionError;
        }
    }

    private static int VerifyManifest(string[] args, TextWriter output, TextWriter error)
    {
        if (args.Length == 0 || args.Any(IsHelp))
        {
            WriteManifestVerifyUsage(output);
            return (int)GenomancyCliExitCode.Success;
        }

        ManifestVerifyOptions options;

        try
        {
            options = ManifestVerifyOptions.Parse(args);
        }
        catch (ArgumentException exception)
        {
            error.WriteLine(exception.Message);
            WriteManifestVerifyUsage(error);
            return (int)GenomancyCliExitCode.UsageError;
        }

        try
        {
            var manifest = ResourceTestResultManifestJsonFileStore.Create().Load(options.ManifestPath);
            var checks = VerifyManifestEntries(manifest, options).ToArray();
            var text = FormatManifestVerification(checks, options);

            output.Write(text);

            if (options.ReportPath is not null)
            {
                WriteTextFile(options.ReportPath, text);
                output.WriteLine($"Report: {options.ReportPath}");
            }

            if (checks.Any(check => check.Status == ManifestVerificationStatus.Missing
                    || check.Status == ManifestVerificationStatus.Mismatched))
            {
                return (int)GenomancyCliExitCode.ExecutionError;
            }

            return checks.Any(check => check.ResultStatus == ResourceTestStatus.Failed)
                ? (int)GenomancyCliExitCode.TestFailure
                : (int)GenomancyCliExitCode.Success;
        }
        catch (Exception exception) when (IsCliExecutionException(exception))
        {
            error.WriteLine($"Manifest verify failed: {exception.Message}");
            return (int)GenomancyCliExitCode.ExecutionError;
        }
    }

    private static int RegenerateManifest(string[] args, TextWriter output, TextWriter error)
    {
        if (args.Length == 0 || args.Any(IsHelp))
        {
            WriteManifestRegenerateUsage(output);
            return (int)GenomancyCliExitCode.Success;
        }

        ManifestRegenerateOptions options;

        try
        {
            options = ManifestRegenerateOptions.Parse(args);
        }
        catch (ArgumentException exception)
        {
            error.WriteLine(exception.Message);
            WriteManifestRegenerateUsage(error);
            return (int)GenomancyCliExitCode.UsageError;
        }

        try
        {
            var regenerator = new ResourceTestResultManifestJsonFileRegenerator();
            var manifest = regenerator.RegenerateFromBatchResults(
                options.ManifestPath,
                options.BatchResultPaths);

            WriteManifestRegenerateSummary(manifest, options, output);

            if (options.ReportPath is not null)
            {
                WriteTextFile(options.ReportPath, FormatManifest(manifest, ManifestShowOptions.Default(options.ManifestPath)));
                output.WriteLine($"Report: {options.ReportPath}");
            }

            return manifest.Entries.Any(entry => entry.Summary.Status == ResourceTestStatus.Failed)
                ? (int)GenomancyCliExitCode.TestFailure
                : (int)GenomancyCliExitCode.Success;
        }
        catch (Exception exception) when (IsCliExecutionException(exception))
        {
            error.WriteLine($"Manifest regenerate failed: {exception.Message}");
            return (int)GenomancyCliExitCode.ExecutionError;
        }
    }

    private static void WriteExecutionSummary(
        ResourceTestBatchRunJsonFileExecutionResult execution,
        TextWriter output)
    {
        var summary = ResourceTestBatchRunSummary.FromResult(execution.BatchResult);

        output.WriteLine($"Status: {summary.Status}");
        output.WriteLine($"Batch result: {execution.BatchResultPath}");
        output.WriteLine($"Run results: {execution.WrittenRunResultPaths.Count}");

        foreach (var path in execution.WrittenRunResultPaths)
        {
            output.WriteLine($"- {path}");
        }

        output.WriteLine(execution.ManifestPath is null
            ? "Manifest: none"
            : $"Manifest: {execution.ManifestPath}");
        output.WriteLine(
            $"Summary: runs={summary.TotalRuns} passed={summary.PassedRuns} failed={summary.FailedRuns} cases={summary.TotalCases} diagnostics={summary.TotalDiagnostics} packets={summary.ReproducibilityPackets}");
    }

    private static void WriteManifestUpdateSummary(
        ResourceTestResultManifest manifest,
        string manifestPath,
        bool upsertManifest,
        TextWriter output)
    {
        var failedEntries = manifest.Entries.Count(entry => entry.Summary.Status == ResourceTestStatus.Failed);
        var mode = upsertManifest ? "upsert" : "append";

        output.WriteLine($"Manifest: {manifestPath}");
        output.WriteLine($"Mode: {mode}");
        output.WriteLine($"Entries: {manifest.Entries.Count} total, {failedEntries} failed");
    }

    private static void WriteManifestRegenerateSummary(
        ResourceTestResultManifest manifest,
        ManifestRegenerateOptions options,
        TextWriter output)
    {
        var failedEntries = manifest.Entries.Count(entry => entry.Summary.Status == ResourceTestStatus.Failed);

        output.WriteLine($"Manifest: {options.ManifestPath}");
        output.WriteLine("Mode: regenerate");
        output.WriteLine($"Batch results: {options.BatchResultPaths.Count}");

        foreach (var path in options.BatchResultPaths.Order(StringComparer.Ordinal))
        {
            output.WriteLine($"- {path}");
        }

        output.WriteLine($"Entries: {manifest.Entries.Count} total, {failedEntries} failed");
    }

    private static string FormatManifest(ResourceTestResultManifest manifest, ManifestShowOptions options)
    {
        var selectedEntries = manifest.Entries
            .Where(entry => options.Status is null || entry.Summary.Status == options.Status)
            .Where(entry => options.Tag is null || entry.Tags.Contains(options.Tag, StringComparer.Ordinal))
            .ToArray();
        var totalFailed = manifest.Entries.Count(entry => entry.Summary.Status == ResourceTestStatus.Failed);
        using var writer = new StringWriter();

        writer.WriteLine("Resource test result manifest");
        writer.WriteLine($"Entries: {manifest.Entries.Count} total, {totalFailed} failed");
        writer.WriteLine($"Selected: {selectedEntries.Length}");

        if (options.Status is not null)
        {
            writer.WriteLine($"Status filter: {options.Status}");
        }

        if (options.Tag is not null)
        {
            writer.WriteLine($"Tag filter: {options.Tag}");
        }

        writer.WriteLine("Runs:");

        foreach (var entry in selectedEntries)
        {
            var summary = entry.Summary;

            writer.WriteLine(
                $"- {entry.RunId}: {summary.Status} path={entry.ResultPath} cases={summary.TotalCases} passed={summary.PassedCases} failed={summary.FailedCases}");
            writer.WriteLine(
                $"  Diagnostics: {summary.TotalDiagnostics} total, {summary.ErrorDiagnostics} error, {summary.WarningDiagnostics} warning, {summary.InfoDiagnostics} info");
            writer.WriteLine($"  Reproducibility packets: {summary.ReproducibilityPackets}");
            writer.WriteLine($"  Completed: {FormatCompletedAt(entry.CompletedAtUtc)}");
            writer.WriteLine($"  Label: {FormatOptional(entry.Label)}");
            writer.WriteLine($"  Tags: {FormatTags(entry.Tags)}");

            if (options.ResolveRootPath is not null)
            {
                writer.WriteLine($"  Resolved path: {ResolveResultPath(options.ResolveRootPath, entry.ResultPath)}");
            }
        }

        return writer.ToString();
    }

    private static string FormatManifestResult(
        ResourceTestResultManifestEntry entry,
        string resolvedPath,
        ResourceTestRunResult result,
        IReadOnlyCollection<string> summaryDiagnostics)
    {
        using var writer = new StringWriter();

        writer.WriteLine("Manifest result");
        writer.WriteLine($"Run: {entry.RunId}");
        writer.WriteLine($"Manifest path: {entry.ResultPath}");
        writer.WriteLine($"Resolved path: {resolvedPath}");
        writer.WriteLine($"Completed: {FormatCompletedAt(entry.CompletedAtUtc)}");
        writer.WriteLine($"Label: {FormatOptional(entry.Label)}");
        writer.WriteLine($"Tags: {FormatTags(entry.Tags)}");
        writer.WriteLine(summaryDiagnostics.Count == 0
            ? "Summary check: matched"
            : $"Summary check: mismatched ({string.Join("; ", summaryDiagnostics)})");
        writer.Write(ResourceTestTextReportFormatter.WriteToText(result));

        return writer.ToString();
    }

    private static IEnumerable<ManifestVerificationCheck> VerifyManifestEntries(
        ResourceTestResultManifest manifest,
        ManifestVerifyOptions options)
    {
        var entries = manifest.Entries
            .Where(entry => options.Status is null || entry.Summary.Status == options.Status)
            .Where(entry => options.Tag is null || entry.Tags.Contains(options.Tag, StringComparer.Ordinal))
            .ToArray();
        var resultStore = ResourceTestResultJsonFileStore.Create();

        foreach (var entry in entries)
        {
            var resolvedPath = ResolveResultPath(options.ResultRootPath, entry.ResultPath);

            if (!File.Exists(resolvedPath))
            {
                yield return new ManifestVerificationCheck(
                    entry,
                    resolvedPath,
                    ManifestVerificationStatus.Missing,
                    null,
                    ["Result file does not exist."]);
                continue;
            }

            ResourceTestRunResult? result = null;
            string? loadDiagnostic = null;

            try
            {
                result = resultStore.Load(resolvedPath);
            }
            catch (Exception exception) when (IsCliExecutionException(exception))
            {
                loadDiagnostic = exception.Message;
            }

            if (loadDiagnostic is not null)
            {
                yield return new ManifestVerificationCheck(
                    entry,
                    resolvedPath,
                    ManifestVerificationStatus.Mismatched,
                    null,
                    [$"Result file could not be loaded: {loadDiagnostic}"]);
                continue;
            }

            if (result is null)
            {
                throw new InvalidOperationException("Result file load returned no result.");
            }

            var resultSummary = ResourceTestRunSummary.FromResult(result);
            var diagnostics = CompareSummaries(entry.Summary, resultSummary).ToArray();

            yield return new ManifestVerificationCheck(
                entry,
                resolvedPath,
                diagnostics.Length == 0 ? ManifestVerificationStatus.Verified : ManifestVerificationStatus.Mismatched,
                result.Status,
                diagnostics);
        }
    }

    private static string FormatManifestVerification(
        IReadOnlyCollection<ManifestVerificationCheck> checks,
        ManifestVerifyOptions options)
    {
        using var writer = new StringWriter();
        var verified = checks.Count(check => check.Status == ManifestVerificationStatus.Verified);
        var missing = checks.Count(check => check.Status == ManifestVerificationStatus.Missing);
        var mismatched = checks.Count(check => check.Status == ManifestVerificationStatus.Mismatched);
        var failedResults = checks.Count(check => check.ResultStatus == ResourceTestStatus.Failed);

        writer.WriteLine("Resource test result manifest verification");
        writer.WriteLine($"Manifest: {options.ManifestPath}");
        writer.WriteLine($"Result root: {options.ResultRootPath}");
        writer.WriteLine($"Entries: {checks.Count} selected, {verified} verified, {missing} missing, {mismatched} mismatched, {failedResults} failed results");

        if (options.Status is not null)
        {
            writer.WriteLine($"Status filter: {options.Status}");
        }

        if (options.Tag is not null)
        {
            writer.WriteLine($"Tag filter: {options.Tag}");
        }

        writer.WriteLine("Runs:");

        foreach (var check in checks.OrderBy(check => check.Entry.RunId))
        {
            writer.WriteLine(
                $"- {check.Entry.RunId}: {check.Status} manifestStatus={check.Entry.Summary.Status} resultStatus={FormatResultStatus(check.ResultStatus)} path={check.ResolvedPath}");

            if (check.Diagnostics.Count == 0)
            {
                writer.WriteLine("  Diagnostics: none");
            }
            else
            {
                writer.WriteLine($"  Diagnostics: {string.Join("; ", check.Diagnostics)}");
            }
        }

        return writer.ToString();
    }

    private static ResourceTestResultManifestEntry FindManifestEntry(
        ResourceTestResultManifest manifest,
        ResourceTestId runId)
    {
        return manifest.Entries.SingleOrDefault(entry => entry.RunId == runId)
            ?? throw new InvalidOperationException($"Manifest does not contain run id '{runId}'.");
    }

    private static IEnumerable<string> CompareSummaries(
        ResourceTestRunSummary manifestSummary,
        ResourceTestRunSummary resultSummary)
    {
        if (manifestSummary.Status != resultSummary.Status)
        {
            yield return $"status manifest={manifestSummary.Status} result={resultSummary.Status}";
        }

        if (manifestSummary.TotalCases != resultSummary.TotalCases
            || manifestSummary.PassedCases != resultSummary.PassedCases
            || manifestSummary.FailedCases != resultSummary.FailedCases)
        {
            yield return $"cases manifest={manifestSummary.TotalCases}/{manifestSummary.PassedCases}/{manifestSummary.FailedCases} result={resultSummary.TotalCases}/{resultSummary.PassedCases}/{resultSummary.FailedCases}";
        }

        if (manifestSummary.TotalDiagnostics != resultSummary.TotalDiagnostics
            || manifestSummary.ErrorDiagnostics != resultSummary.ErrorDiagnostics
            || manifestSummary.WarningDiagnostics != resultSummary.WarningDiagnostics
            || manifestSummary.InfoDiagnostics != resultSummary.InfoDiagnostics)
        {
            yield return $"diagnostics manifest={manifestSummary.TotalDiagnostics}/{manifestSummary.ErrorDiagnostics}/{manifestSummary.WarningDiagnostics}/{manifestSummary.InfoDiagnostics} result={resultSummary.TotalDiagnostics}/{resultSummary.ErrorDiagnostics}/{resultSummary.WarningDiagnostics}/{resultSummary.InfoDiagnostics}";
        }

        if (manifestSummary.ReproducibilityPackets != resultSummary.ReproducibilityPackets)
        {
            yield return $"packets manifest={manifestSummary.ReproducibilityPackets} result={resultSummary.ReproducibilityPackets}";
        }
    }

    private static string ResolveResultPath(string rootPath, string resultPath)
    {
        return Path.IsPathRooted(resultPath)
            ? resultPath
            : Path.Combine(rootPath, resultPath);
    }

    private static string FormatCompletedAt(DateTimeOffset? completedAtUtc)
    {
        return completedAtUtc is null
            ? "none"
            : completedAtUtc.Value.ToUniversalTime().ToString("O");
    }

    private static string FormatOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? "none" : value;
    }

    private static string FormatTags(IReadOnlyCollection<string> tags)
    {
        return tags.Count == 0 ? "none" : string.Join(", ", tags);
    }

    private static string FormatResultStatus(ResourceTestStatus? status)
    {
        return status is null ? "unavailable" : status.ToString() ?? "unavailable";
    }

    private static bool IsCliExecutionException(Exception exception)
    {
        return exception is IOException
            or UnauthorizedAccessException
            or ArgumentException
            or InvalidOperationException;
    }

    private static void WriteTextFile(string path, string text)
    {
        var directory = Path.GetDirectoryName(Path.GetFullPath(path));

        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(path, text);
    }

    private static void WriteUsage(TextWriter writer)
    {
        writer.WriteLine("Usage:");
        writer.WriteLine("  genomancy batch run --plan <path> --batch-result <path> [options]");
        writer.WriteLine("  genomancy batch show --batch-result <path> [options]");
        writer.WriteLine("  genomancy result show --result <path> [options]");
        writer.WriteLine("  genomancy manifest show --manifest <path> [options]");
        writer.WriteLine("  genomancy manifest result show --manifest <path> --run-id <id> --result-root <path> [options]");
        writer.WriteLine("  genomancy manifest verify --manifest <path> --result-root <path> [options]");
        writer.WriteLine("  genomancy manifest regenerate --manifest <path> --from-batch-result <path> [options]");
        writer.WriteLine("  genomancy manifest update --manifest <path> --from-batch-result <path> [options]");
        writer.WriteLine();
        writer.WriteLine("Commands:");
        writer.WriteLine("  batch run    Execute a serialized JSON resource-test batch plan.");
        writer.WriteLine("  batch show   Render an aggregate batch-result report.");
        writer.WriteLine("  result show  Render an individual resource-test result report.");
        writer.WriteLine("  manifest     Inspect or update result manifests.");
    }

    private static void WriteBatchUsage(TextWriter writer)
    {
        writer.WriteLine("Usage:");
        writer.WriteLine("  genomancy batch run --plan <path> --batch-result <path> [options]");
        writer.WriteLine();
        writer.WriteLine("Options:");
        writer.WriteLine("  --manifest <path>             Update a JSON result manifest.");
        writer.WriteLine("  --manifest-mode <upsert|append>");
        writer.WriteLine("  --run-result-root <path>      Root for relative per-run result paths.");
        writer.WriteLine("  --report <path>               Write deterministic text report.");
        writer.WriteLine("  --stdout-report               Also print deterministic text report to stdout.");
    }

    private static void WriteBatchShowUsage(TextWriter writer)
    {
        writer.WriteLine("Usage:");
        writer.WriteLine("  genomancy batch show --batch-result <path> [options]");
        writer.WriteLine();
        writer.WriteLine("Options:");
        writer.WriteLine("  --report <path>               Also write deterministic text report.");
    }

    private static void WriteResultShowUsage(TextWriter writer)
    {
        writer.WriteLine("Usage:");
        writer.WriteLine("  genomancy result show --result <path> [options]");
        writer.WriteLine();
        writer.WriteLine("Options:");
        writer.WriteLine("  --report <path>               Also write deterministic text report.");
    }

    private static void WriteManifestShowUsage(TextWriter writer)
    {
        writer.WriteLine("Usage:");
        writer.WriteLine("  genomancy manifest show --manifest <path> [options]");
        writer.WriteLine();
        writer.WriteLine("Options:");
        writer.WriteLine("  --status <passed|failed>      Filter manifest entries by status.");
        writer.WriteLine("  --tag <tag>                   Filter manifest entries by tag.");
        writer.WriteLine("  --resolve-root <path>         Print resolved file paths for relative entries.");
        writer.WriteLine("  --report <path>               Also write deterministic text report.");
    }

    private static void WriteManifestUpdateUsage(TextWriter writer)
    {
        writer.WriteLine("Usage:");
        writer.WriteLine("  genomancy manifest update --manifest <path> --from-batch-result <path> [options]");
        writer.WriteLine();
        writer.WriteLine("Options:");
        writer.WriteLine("  --manifest-mode <upsert|append>");
        writer.WriteLine("  --report <path>               Write deterministic manifest report after update.");
    }

    private static void WriteManifestRegenerateUsage(TextWriter writer)
    {
        writer.WriteLine("Usage:");
        writer.WriteLine("  genomancy manifest regenerate --manifest <path> --from-batch-result <path> [--from-batch-result <path>...] [options]");
        writer.WriteLine();
        writer.WriteLine("Options:");
        writer.WriteLine("  --report <path>               Write deterministic manifest report after regeneration.");
    }

    private static void WriteManifestResultShowUsage(TextWriter writer)
    {
        writer.WriteLine("Usage:");
        writer.WriteLine("  genomancy manifest result show --manifest <path> --run-id <id> --result-root <path> [options]");
        writer.WriteLine();
        writer.WriteLine("Options:");
        writer.WriteLine("  --allow-summary-mismatch      Render the result even when manifest summary differs.");
        writer.WriteLine("  --report <path>               Also write deterministic text report.");
    }

    private static void WriteManifestVerifyUsage(TextWriter writer)
    {
        writer.WriteLine("Usage:");
        writer.WriteLine("  genomancy manifest verify --manifest <path> --result-root <path> [options]");
        writer.WriteLine();
        writer.WriteLine("Options:");
        writer.WriteLine("  --status <passed|failed>      Filter manifest entries by status.");
        writer.WriteLine("  --tag <tag>                   Filter manifest entries by tag.");
        writer.WriteLine("  --report <path>               Also write deterministic text report.");
    }

    private static bool IsHelp(string value)
    {
        return string.Equals(value, "--help", StringComparison.Ordinal)
            || string.Equals(value, "-h", StringComparison.Ordinal)
            || string.Equals(value, "help", StringComparison.Ordinal);
    }

    private sealed record BatchRunOptions(
        string PlanPath,
        string BatchResultPath,
        string? ManifestPath,
        bool UpsertManifest,
        string? RunResultRootPath,
        string? ReportPath,
        bool WriteReportToStdout)
    {
        public static BatchRunOptions Parse(IReadOnlyList<string> args)
        {
            string? planPath = null;
            string? batchResultPath = null;
            string? manifestPath = null;
            string? runResultRootPath = null;
            string? reportPath = null;
            var upsertManifest = true;
            var writeReportToStdout = false;

            for (var i = 0; i < args.Count; i++)
            {
                var option = args[i];

                switch (option)
                {
                    case "--plan":
                        planPath = ReadValue(args, ref i, option);
                        break;
                    case "--batch-result":
                        batchResultPath = ReadValue(args, ref i, option);
                        break;
                    case "--manifest":
                        manifestPath = ReadValue(args, ref i, option);
                        break;
                    case "--manifest-mode":
                        upsertManifest = ParseManifestMode(ReadValue(args, ref i, option));
                        break;
                    case "--run-result-root":
                        runResultRootPath = ReadValue(args, ref i, option);
                        break;
                    case "--report":
                        reportPath = ReadValue(args, ref i, option);
                        break;
                    case "--stdout-report":
                        writeReportToStdout = true;
                        break;
                    default:
                        throw new ArgumentException($"Unknown option '{option}'.");
                }
            }

            if (string.IsNullOrWhiteSpace(planPath))
            {
                throw new ArgumentException("Missing required option '--plan'.");
            }

            if (string.IsNullOrWhiteSpace(batchResultPath))
            {
                throw new ArgumentException("Missing required option '--batch-result'.");
            }

            return new BatchRunOptions(
                planPath.Trim(),
                batchResultPath.Trim(),
                NormalizeOptionalPath(manifestPath),
                upsertManifest,
                NormalizeOptionalPath(runResultRootPath),
                NormalizeOptionalPath(reportPath),
                writeReportToStdout);
        }

        private static string ReadValue(IReadOnlyList<string> args, ref int index, string option)
        {
            if (index + 1 >= args.Count || args[index + 1].StartsWith("--", StringComparison.Ordinal))
            {
                throw new ArgumentException($"Option '{option}' requires a value.");
            }

            index++;
            return args[index];
        }

        private static bool ParseManifestMode(string value)
        {
            if (string.Equals(value, "upsert", StringComparison.Ordinal))
            {
                return true;
            }

            if (string.Equals(value, "append", StringComparison.Ordinal))
            {
                return false;
            }

            throw new ArgumentException("Option '--manifest-mode' must be 'upsert' or 'append'.");
        }

        private static string? NormalizeOptionalPath(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }

    private sealed record BatchShowOptions(string BatchResultPath, string? ReportPath)
    {
        public static BatchShowOptions Parse(IReadOnlyList<string> args)
        {
            string? batchResultPath = null;
            string? reportPath = null;

            for (var i = 0; i < args.Count; i++)
            {
                switch (args[i])
                {
                    case "--batch-result":
                        batchResultPath = ReadValue(args, ref i, args[i]);
                        break;
                    case "--report":
                        reportPath = ReadValue(args, ref i, args[i]);
                        break;
                    default:
                        throw new ArgumentException($"Unknown option '{args[i]}'.");
                }
            }

            if (string.IsNullOrWhiteSpace(batchResultPath))
            {
                throw new ArgumentException("Missing required option '--batch-result'.");
            }

            return new BatchShowOptions(batchResultPath.Trim(), NormalizeOptionalPath(reportPath));
        }
    }

    private sealed record ResultShowOptions(string ResultPath, string? ReportPath)
    {
        public static ResultShowOptions Parse(IReadOnlyList<string> args)
        {
            string? resultPath = null;
            string? reportPath = null;

            for (var i = 0; i < args.Count; i++)
            {
                switch (args[i])
                {
                    case "--result":
                        resultPath = ReadValue(args, ref i, args[i]);
                        break;
                    case "--report":
                        reportPath = ReadValue(args, ref i, args[i]);
                        break;
                    default:
                        throw new ArgumentException($"Unknown option '{args[i]}'.");
                }
            }

            if (string.IsNullOrWhiteSpace(resultPath))
            {
                throw new ArgumentException("Missing required option '--result'.");
            }

            return new ResultShowOptions(resultPath.Trim(), NormalizeOptionalPath(reportPath));
        }
    }

    private sealed record ManifestShowOptions(
        string ManifestPath,
        ResourceTestStatus? Status,
        string? Tag,
        string? ResolveRootPath,
        string? ReportPath)
    {
        public static ManifestShowOptions Default(string manifestPath)
        {
            return new ManifestShowOptions(manifestPath, null, null, null, null);
        }

        public static ManifestShowOptions Parse(IReadOnlyList<string> args)
        {
            string? manifestPath = null;
            ResourceTestStatus? status = null;
            string? tag = null;
            string? resolveRootPath = null;
            string? reportPath = null;

            for (var i = 0; i < args.Count; i++)
            {
                switch (args[i])
                {
                    case "--manifest":
                        manifestPath = ReadValue(args, ref i, args[i]);
                        break;
                    case "--status":
                        status = ParseStatus(ReadValue(args, ref i, args[i]));
                        break;
                    case "--tag":
                        tag = ReadValue(args, ref i, args[i]);
                        break;
                    case "--resolve-root":
                        resolveRootPath = ReadValue(args, ref i, args[i]);
                        break;
                    case "--report":
                        reportPath = ReadValue(args, ref i, args[i]);
                        break;
                    default:
                        throw new ArgumentException($"Unknown option '{args[i]}'.");
                }
            }

            if (string.IsNullOrWhiteSpace(manifestPath))
            {
                throw new ArgumentException("Missing required option '--manifest'.");
            }

            return new ManifestShowOptions(
                manifestPath.Trim(),
                status,
                NormalizeOptionalPath(tag),
                NormalizeOptionalPath(resolveRootPath),
                NormalizeOptionalPath(reportPath));
        }
    }

    private sealed record ManifestUpdateOptions(
        string ManifestPath,
        string BatchResultPath,
        bool UpsertManifest,
        string? ReportPath)
    {
        public static ManifestUpdateOptions Parse(IReadOnlyList<string> args)
        {
            string? manifestPath = null;
            string? batchResultPath = null;
            string? reportPath = null;
            var upsertManifest = true;

            for (var i = 0; i < args.Count; i++)
            {
                switch (args[i])
                {
                    case "--manifest":
                        manifestPath = ReadValue(args, ref i, args[i]);
                        break;
                    case "--from-batch-result":
                        batchResultPath = ReadValue(args, ref i, args[i]);
                        break;
                    case "--manifest-mode":
                        upsertManifest = ParseManifestMode(ReadValue(args, ref i, args[i]));
                        break;
                    case "--report":
                        reportPath = ReadValue(args, ref i, args[i]);
                        break;
                    default:
                        throw new ArgumentException($"Unknown option '{args[i]}'.");
                }
            }

            if (string.IsNullOrWhiteSpace(manifestPath))
            {
                throw new ArgumentException("Missing required option '--manifest'.");
            }

            if (string.IsNullOrWhiteSpace(batchResultPath))
            {
                throw new ArgumentException("Missing required option '--from-batch-result'.");
            }

            return new ManifestUpdateOptions(
                manifestPath.Trim(),
                batchResultPath.Trim(),
                upsertManifest,
                NormalizeOptionalPath(reportPath));
        }
    }

    private sealed record ManifestRegenerateOptions(
        string ManifestPath,
        IReadOnlyList<string> BatchResultPaths,
        string? ReportPath)
    {
        public static ManifestRegenerateOptions Parse(IReadOnlyList<string> args)
        {
            string? manifestPath = null;
            var batchResultPaths = new List<string>();
            string? reportPath = null;

            for (var i = 0; i < args.Count; i++)
            {
                switch (args[i])
                {
                    case "--manifest":
                        manifestPath = ReadValue(args, ref i, args[i]);
                        break;
                    case "--from-batch-result":
                        batchResultPaths.Add(ReadValue(args, ref i, args[i]));
                        break;
                    case "--report":
                        reportPath = ReadValue(args, ref i, args[i]);
                        break;
                    default:
                        throw new ArgumentException($"Unknown option '{args[i]}'.");
                }
            }

            if (string.IsNullOrWhiteSpace(manifestPath))
            {
                throw new ArgumentException("Missing required option '--manifest'.");
            }

            var normalizedBatchResultPaths = batchResultPaths
                .Select(NormalizeOptionalPath)
                .OfType<string>()
                .ToArray();

            if (normalizedBatchResultPaths.Length == 0)
            {
                throw new ArgumentException("Missing required option '--from-batch-result'.");
            }

            return new ManifestRegenerateOptions(
                manifestPath.Trim(),
                Array.AsReadOnly(normalizedBatchResultPaths),
                NormalizeOptionalPath(reportPath));
        }
    }

    private sealed record ManifestResultShowOptions(
        string ManifestPath,
        ResourceTestId RunId,
        string ResultRootPath,
        string? ReportPath,
        bool RequireSummaryMatch)
    {
        public static ManifestResultShowOptions Parse(IReadOnlyList<string> args)
        {
            string? manifestPath = null;
            ResourceTestId? runId = null;
            string? resultRootPath = null;
            string? reportPath = null;
            var requireSummaryMatch = true;

            for (var i = 0; i < args.Count; i++)
            {
                switch (args[i])
                {
                    case "--manifest":
                        manifestPath = ReadValue(args, ref i, args[i]);
                        break;
                    case "--run-id":
                        runId = ResourceTestId.Parse(ReadValue(args, ref i, args[i]));
                        break;
                    case "--result-root":
                        resultRootPath = ReadValue(args, ref i, args[i]);
                        break;
                    case "--report":
                        reportPath = ReadValue(args, ref i, args[i]);
                        break;
                    case "--allow-summary-mismatch":
                        requireSummaryMatch = false;
                        break;
                    default:
                        throw new ArgumentException($"Unknown option '{args[i]}'.");
                }
            }

            if (string.IsNullOrWhiteSpace(manifestPath))
            {
                throw new ArgumentException("Missing required option '--manifest'.");
            }

            var parsedRunId = runId;

            if (parsedRunId is null)
            {
                throw new ArgumentException("Missing required option '--run-id'.");
            }

            if (string.IsNullOrWhiteSpace(resultRootPath))
            {
                throw new ArgumentException("Missing required option '--result-root'.");
            }

            return new ManifestResultShowOptions(
                manifestPath.Trim(),
                parsedRunId.Value,
                resultRootPath.Trim(),
                NormalizeOptionalPath(reportPath),
                requireSummaryMatch);
        }
    }

    private sealed record ManifestVerifyOptions(
        string ManifestPath,
        string ResultRootPath,
        ResourceTestStatus? Status,
        string? Tag,
        string? ReportPath)
    {
        public static ManifestVerifyOptions Parse(IReadOnlyList<string> args)
        {
            string? manifestPath = null;
            string? resultRootPath = null;
            ResourceTestStatus? status = null;
            string? tag = null;
            string? reportPath = null;

            for (var i = 0; i < args.Count; i++)
            {
                switch (args[i])
                {
                    case "--manifest":
                        manifestPath = ReadValue(args, ref i, args[i]);
                        break;
                    case "--result-root":
                        resultRootPath = ReadValue(args, ref i, args[i]);
                        break;
                    case "--status":
                        status = ParseStatus(ReadValue(args, ref i, args[i]));
                        break;
                    case "--tag":
                        tag = ReadValue(args, ref i, args[i]);
                        break;
                    case "--report":
                        reportPath = ReadValue(args, ref i, args[i]);
                        break;
                    default:
                        throw new ArgumentException($"Unknown option '{args[i]}'.");
                }
            }

            if (string.IsNullOrWhiteSpace(manifestPath))
            {
                throw new ArgumentException("Missing required option '--manifest'.");
            }

            if (string.IsNullOrWhiteSpace(resultRootPath))
            {
                throw new ArgumentException("Missing required option '--result-root'.");
            }

            return new ManifestVerifyOptions(
                manifestPath.Trim(),
                resultRootPath.Trim(),
                status,
                NormalizeOptionalPath(tag),
                NormalizeOptionalPath(reportPath));
        }
    }

    private sealed record ManifestVerificationCheck(
        ResourceTestResultManifestEntry Entry,
        string ResolvedPath,
        ManifestVerificationStatus Status,
        ResourceTestStatus? ResultStatus,
        IReadOnlyList<string> Diagnostics);

    private enum ManifestVerificationStatus
    {
        Verified,
        Missing,
        Mismatched,
    }

    private static string ReadValue(IReadOnlyList<string> args, ref int index, string option)
    {
        if (index + 1 >= args.Count || args[index + 1].StartsWith("--", StringComparison.Ordinal))
        {
            throw new ArgumentException($"Option '{option}' requires a value.");
        }

        index++;
        return args[index];
    }

    private static bool ParseManifestMode(string value)
    {
        if (string.Equals(value, "upsert", StringComparison.Ordinal))
        {
            return true;
        }

        if (string.Equals(value, "append", StringComparison.Ordinal))
        {
            return false;
        }

        throw new ArgumentException("Option '--manifest-mode' must be 'upsert' or 'append'.");
    }

    private static ResourceTestStatus ParseStatus(string value)
    {
        if (Enum.TryParse<ResourceTestStatus>(value, ignoreCase: true, out var status))
        {
            return status;
        }

        throw new ArgumentException("Option '--status' must be 'passed' or 'failed'.");
    }

    private static string? NormalizeOptionalPath(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
