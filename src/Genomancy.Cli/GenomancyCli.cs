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
        writer.WriteLine();
        writer.WriteLine("Commands:");
        writer.WriteLine("  batch run    Execute a serialized JSON resource-test batch plan.");
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
}
