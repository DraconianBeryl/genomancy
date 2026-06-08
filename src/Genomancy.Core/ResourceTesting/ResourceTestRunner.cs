namespace Genomancy.Core.ResourceTesting;

public static class ResourceTestRunner
{
    public static ResourceTestRunResult Run(
        IEnumerable<ResourceTestDefinition> definitions,
        ResourceTestRunOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        options ??= new ResourceTestRunOptions();

        var results = definitions
            .Where(options.ShouldRun)
            .OrderBy(definition => definition.Id)
            .Select(RunCase)
            .ToArray();

        return new ResourceTestRunResult(results);
    }

    private static ResourceTestCaseResult RunCase(ResourceTestDefinition definition)
    {
        var diagnostics = new List<ResourceTestDiagnostic>();
        ResourceTestContext? context = null;

        try
        {
            context = new ResourceTestContext(definition, definition.CreateSystemDefinition());

            foreach (var step in definition.Steps)
            {
                try
                {
                    step.Execute(context);
                }
                catch (Exception exception)
                {
                    context.AddDiagnostic(
                        ResourceTestSeverity.Error,
                        "RESOURCE_TEST_STEP_EXCEPTION",
                        $"tests/{definition.Id}/steps/{step.Name}",
                        exception.Message);
                    break;
                }
            }

            diagnostics.AddRange(context.Diagnostics);
        }
        catch (Exception exception)
        {
            diagnostics.Add(new ResourceTestDiagnostic(
                ResourceTestSeverity.Error,
                "RESOURCE_TEST_FIXTURE_EXCEPTION",
                $"tests/{definition.Id}/fixture",
                exception.Message));
        }

        var status = diagnostics.Any(diagnostic => diagnostic.Severity == ResourceTestSeverity.Error)
            ? ResourceTestStatus.Failed
            : ResourceTestStatus.Passed;

        return new ResourceTestCaseResult(
            definition.Id,
            status,
            diagnostics,
            definition.Tags,
            context?.ReproducibilityPackets);
    }
}
