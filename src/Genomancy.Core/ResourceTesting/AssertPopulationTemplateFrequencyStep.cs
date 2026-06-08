using Genomancy.Core.Definitions;
using Genomancy.Core.Simulation;
using Genomancy.Core.Templates;

namespace Genomancy.Core.ResourceTesting;

public sealed class AssertPopulationTemplateFrequencyStep : IResourceTestStep
{
    private readonly PopulationTemplateVersion _template;
    private readonly ResourceId _groupId;
    private readonly ResourceId _geneId;
    private readonly ResourceId _alleleId;
    private readonly int _sampleCount;
    private readonly ulong _seed;
    private readonly StatisticalTolerance _tolerance;
    private readonly SimulationResourceLimits _limits;

    public AssertPopulationTemplateFrequencyStep(
        PopulationTemplateVersion template,
        ResourceId groupId,
        ResourceId geneId,
        ResourceId alleleId,
        int sampleCount,
        ulong seed,
        StatisticalTolerance tolerance,
        SimulationResourceLimits? limits = null)
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(tolerance);

        _template = template;
        _groupId = groupId;
        _geneId = geneId;
        _alleleId = alleleId;
        _sampleCount = sampleCount;
        _seed = seed;
        _tolerance = tolerance;
        _limits = limits ?? new SimulationResourceLimits();
    }

    public string Name => "assert-population-template-frequency";

    public void Execute(ResourceTestContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var result = PopulationTemplateSimulationService.MeasureAlleleFrequency(
            _template,
            _groupId,
            _geneId,
            _alleleId,
            _sampleCount,
            _seed,
            _tolerance,
            _limits,
            context.Definition.Id.Value);

        if (result.FailurePacket is null)
        {
            return;
        }

        context.AddReproducibilityPacket(result.FailurePacket);
        context.AddDiagnostic(
            ResourceTestSeverity.Error,
            "RESOURCE_TEST_STATISTICAL_TOLERANCE_FAILED",
            result.FailurePacket.OperationPath,
            result.FailurePacket.Diagnostic);
    }
}
