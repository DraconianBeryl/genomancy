using System.Text.Json;
using System.Text.Json.Serialization;
using Genomancy.Core.Definitions;
using Genomancy.Core.Serialization;
using Genomancy.Core.Simulation;
using Genomancy.Core.Templates;

namespace Genomancy.Core.ResourceTesting;

public static class ResourceTestJsonCodec
{
    private const int EnvelopeVersion = 1;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };

    public static void Write(Stream stream, IEnumerable<ResourceTestSpecification> specifications)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(specifications);

        JsonSerializer.Serialize(stream, ToEnvelope(specifications), JsonOptions);
    }

    public static string WriteToText(IEnumerable<ResourceTestSpecification> specifications)
    {
        ArgumentNullException.ThrowIfNull(specifications);

        return JsonSerializer.Serialize(ToEnvelope(specifications), JsonOptions);
    }

    public static byte[] WriteToBuffer(IEnumerable<ResourceTestSpecification> specifications)
    {
        ArgumentNullException.ThrowIfNull(specifications);

        return JsonSerializer.SerializeToUtf8Bytes(ToEnvelope(specifications), JsonOptions);
    }

    public static IReadOnlyList<ResourceTestSpecification> Read(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        try
        {
            var envelope = JsonSerializer.Deserialize<ResourceTestEnvelope>(stream, JsonOptions)
                ?? throw new GenomeSerializationException("Resource test JSON envelope was empty.");

            return FromEnvelope(envelope);
        }
        catch (JsonException exception)
        {
            throw new GenomeSerializationException("Resource test JSON is malformed.", exception);
        }
    }

    public static IReadOnlyList<ResourceTestSpecification> ReadFromBuffer(ReadOnlySpan<byte> buffer)
    {
        using var stream = new MemoryStream(buffer.ToArray(), writable: false);
        return Read(stream);
    }

    public static IReadOnlyList<ResourceTestDefinition> ReadDefinitionsFromBuffer(ReadOnlySpan<byte> buffer)
    {
        return ReadFromBuffer(buffer)
            .Select(specification => specification.ToDefinition())
            .ToArray();
    }

    private static ResourceTestEnvelope ToEnvelope(IEnumerable<ResourceTestSpecification> specifications)
    {
        return new ResourceTestEnvelope(
            EnvelopeVersion,
            specifications
                .OrderBy(specification => specification.Id)
                .Select(ToTestEnvelope)
                .ToArray());
    }

    private static ResourceTestCaseEnvelope ToTestEnvelope(ResourceTestSpecification specification)
    {
        return new ResourceTestCaseEnvelope(
            specification.Id.Value,
            specification.DisplayName,
            specification.SystemDefinitionVersion.Value,
            specification.Tags.Order(StringComparer.Ordinal).ToArray(),
            ToFixtureEnvelope(specification.Fixture),
            specification.Steps.Select(ToStepEnvelope).ToArray());
    }

    private static FixtureEnvelope ToFixtureEnvelope(ResourceTestFixtureSpecification fixture)
    {
        return new FixtureEnvelope(
            fixture.Alleles.Select(allele => new AlleleEnvelope(allele.Id.Value, allele.DisplayName)).ToArray(),
            fixture.Genes.Select(gene => new GeneEnvelope(
                gene.Id.Value,
                gene.AlleleIds.Select(id => id.Value).ToArray(),
                gene.DisplayName,
                gene.RequiredAlleleCount,
                gene.ExpressionStrategy.ToString())).ToArray(),
            fixture.Groups.Select(group => new GroupEnvelope(
                group.Id.Value,
                group.GeneIds.Select(id => id.Value).ToArray(),
                group.SubgroupIds.Select(id => id.Value).ToArray(),
                group.DependencyGroupIds.Select(id => id.Value).ToArray(),
                group.DisplayName)).ToArray(),
            fixture.BodyPlans.Select(bodyPlan => new BodyPlanEnvelope(
                bodyPlan.Id.Value,
                bodyPlan.RequiredGroupIds.Select(id => id.Value).ToArray(),
                bodyPlan.OptionalGroupIds.Select(id => id.Value).ToArray(),
                bodyPlan.SharedGroupIds.Select(id => id.Value).ToArray(),
                bodyPlan.DisplayName)).ToArray());
    }

    private static StepEnvelope ToStepEnvelope(ResourceTestStepSpecification step)
    {
        return new StepEnvelope(
            step.Kind,
            step.Name == step.Kind ? null : step.Name,
            step.ExpectedValid,
            step.DiagnosticCode,
            step.MustBePresent,
            ToPopulationTemplateFrequencyEnvelope(step.PopulationTemplateFrequencyAssertion));
    }

    private static IReadOnlyList<ResourceTestSpecification> FromEnvelope(ResourceTestEnvelope envelope)
    {
        if (envelope.EnvelopeVersion != EnvelopeVersion)
        {
            throw new GenomeSerializationException($"Unsupported resource test envelope version '{envelope.EnvelopeVersion}'.");
        }

        return (envelope.Tests ?? [])
            .Select(test => new ResourceTestSpecification(
                ResourceTestId.Parse(Required(test.Id, "id")),
                SystemDefinitionVersion.Parse(Required(test.SystemDefinitionVersion, "systemDefinitionVersion")),
                FromFixtureEnvelope(test.Fixture ?? new FixtureEnvelope(null, null, null, null)),
                (test.Steps ?? []).Select(FromStepEnvelope),
                test.DisplayName ?? string.Empty,
                test.Tags ?? []))
            .OrderBy(test => test.Id)
            .ToArray();
    }

    private static ResourceTestFixtureSpecification FromFixtureEnvelope(FixtureEnvelope fixture)
    {
        return new ResourceTestFixtureSpecification(
            (fixture.Alleles ?? []).Select(allele => new AlleleResourceSpecification(
                ResourceId.Parse(Required(allele.Id, "allele.id")),
                allele.DisplayName ?? string.Empty)),
            (fixture.Genes ?? []).Select(gene => new GeneResourceSpecification(
                ResourceId.Parse(Required(gene.Id, "gene.id")),
                (gene.AlleleIds ?? []).Select(ResourceId.Parse),
                gene.DisplayName ?? string.Empty,
                gene.RequiredAlleleCount <= 0 ? 1 : gene.RequiredAlleleCount,
                ParseExpressionStrategy(gene.ExpressionStrategy))),
            (fixture.Groups ?? []).Select(group => new GroupResourceSpecification(
                ResourceId.Parse(Required(group.Id, "group.id")),
                (group.GeneIds ?? []).Select(ResourceId.Parse),
                (group.SubgroupIds ?? []).Select(ResourceId.Parse),
                (group.DependencyGroupIds ?? []).Select(ResourceId.Parse),
                group.DisplayName ?? string.Empty)),
            (fixture.BodyPlans ?? []).Select(bodyPlan => new BodyPlanResourceSpecification(
                ResourceId.Parse(Required(bodyPlan.Id, "bodyPlan.id")),
                (bodyPlan.RequiredGroupIds ?? []).Select(ResourceId.Parse),
                (bodyPlan.OptionalGroupIds ?? []).Select(ResourceId.Parse),
                (bodyPlan.SharedGroupIds ?? []).Select(ResourceId.Parse),
                bodyPlan.DisplayName ?? string.Empty)));
    }

    private static ResourceTestStepSpecification FromStepEnvelope(StepEnvelope step)
    {
        return new ResourceTestStepSpecification(
            Required(step.Kind, "step.kind"),
            step.Name ?? string.Empty,
            step.ExpectedValid,
            step.DiagnosticCode,
            step.MustBePresent ?? true,
            FromPopulationTemplateFrequencyEnvelope(step.PopulationTemplateFrequencyAssertion));
    }

    private static PopulationTemplateFrequencyAssertionEnvelope? ToPopulationTemplateFrequencyEnvelope(
        PopulationTemplateFrequencyAssertionSpecification? assertion)
    {
        if (assertion is null)
        {
            return null;
        }

        return new PopulationTemplateFrequencyAssertionEnvelope(
            ToPopulationTemplateEnvelope(assertion.Template),
            assertion.GroupId.Value,
            assertion.GeneId.Value,
            assertion.AlleleId.Value,
            assertion.SampleCount,
            assertion.Seed,
            assertion.Tolerance.ExpectedProportion,
            assertion.Tolerance.AbsoluteTolerance,
            assertion.Limits?.MaximumSamples);
    }

    private static PopulationTemplateFrequencyAssertionSpecification? FromPopulationTemplateFrequencyEnvelope(
        PopulationTemplateFrequencyAssertionEnvelope? assertion)
    {
        if (assertion is null)
        {
            return null;
        }

        var template = FromPopulationTemplateEnvelope(assertion.Template
            ?? throw new GenomeSerializationException("Population template frequency assertion requires 'template'."));

        return new PopulationTemplateFrequencyAssertionSpecification(
            template,
            ResourceId.Parse(Required(assertion.GroupId, "populationTemplateFrequencyAssertion.groupId")),
            ResourceId.Parse(Required(assertion.GeneId, "populationTemplateFrequencyAssertion.geneId")),
            ResourceId.Parse(Required(assertion.AlleleId, "populationTemplateFrequencyAssertion.alleleId")),
            assertion.SampleCount,
            assertion.Seed,
            new StatisticalTolerance(assertion.ExpectedProportion, assertion.AbsoluteTolerance),
            assertion.MaximumSamples is null ? null : new SimulationResourceLimits(assertion.MaximumSamples.Value));
    }

    private static PopulationTemplateEnvelope ToPopulationTemplateEnvelope(PopulationTemplateVersion template)
    {
        return new PopulationTemplateEnvelope(
            template.Id.Value,
            template.VersionId.Value,
            template.SystemDefinitionVersion.Value,
            template.ChangeSummary,
            template.GroupTemplates
                .OrderBy(group => group.GroupId)
                .Select(group => new PopulationTemplateGroupEnvelope(
                    group.GroupId.Value,
                    group.GeneTemplates
                        .OrderBy(gene => gene.GeneId)
                        .Select(gene => new PopulationTemplateGeneEnvelope(
                            gene.GeneId.Value,
                            gene.AlleleCount,
                            gene.AlleleFrequencies
                                .OrderBy(frequency => frequency.AlleleId)
                                .Select(frequency => new PopulationTemplateAlleleEnvelope(
                                    frequency.AlleleId.Value,
                                    frequency.Weight,
                                    frequency.NumericValue))
                                .ToArray()))
                        .ToArray()))
                .ToArray());
    }

    private static PopulationTemplateVersion FromPopulationTemplateEnvelope(PopulationTemplateEnvelope template)
    {
        return new PopulationTemplateVersion(
            PopulationTemplateId.Parse(Required(template.Id, "populationTemplateFrequencyAssertion.template.id")),
            PopulationTemplateVersionId.Parse(Required(template.VersionId, "populationTemplateFrequencyAssertion.template.versionId")),
            SystemDefinitionVersion.Parse(Required(template.SystemDefinitionVersion, "populationTemplateFrequencyAssertion.template.systemDefinitionVersion")),
            (template.Groups ?? []).Select(group => new GroupTemplate(
                ResourceId.Parse(Required(group.GroupId, "populationTemplateFrequencyAssertion.template.groupId")),
                (group.Genes ?? []).Select(gene => new GeneTemplate(
                    ResourceId.Parse(Required(gene.GeneId, "populationTemplateFrequencyAssertion.template.geneId")),
                    gene.AlleleCount,
                    (gene.Alleles ?? []).Select(allele => new AlleleFrequency(
                        ResourceId.Parse(Required(allele.AlleleId, "populationTemplateFrequencyAssertion.template.alleleId")),
                        allele.Weight,
                        allele.NumericValue)))))),
            template.ChangeSummary ?? string.Empty);
    }

    private static GeneExpressionStrategy ParseExpressionStrategy(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? GeneExpressionStrategy.StrictDominance
            : Enum.Parse<GeneExpressionStrategy>(value, ignoreCase: true);
    }

    private static string Required(string? value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new GenomeSerializationException($"Resource test JSON property '{propertyName}' is required.");
        }

        return value;
    }

    private sealed record ResourceTestEnvelope(
        int EnvelopeVersion,
        IReadOnlyList<ResourceTestCaseEnvelope>? Tests);

    private sealed record ResourceTestCaseEnvelope(
        string? Id,
        string? DisplayName,
        string? SystemDefinitionVersion,
        IReadOnlyList<string>? Tags,
        FixtureEnvelope? Fixture,
        IReadOnlyList<StepEnvelope>? Steps);

    private sealed record FixtureEnvelope(
        IReadOnlyList<AlleleEnvelope>? Alleles,
        IReadOnlyList<GeneEnvelope>? Genes,
        IReadOnlyList<GroupEnvelope>? Groups,
        IReadOnlyList<BodyPlanEnvelope>? BodyPlans);

    private sealed record AlleleEnvelope(string? Id, string? DisplayName);

    private sealed record GeneEnvelope(
        string? Id,
        IReadOnlyList<string>? AlleleIds,
        string? DisplayName,
        int RequiredAlleleCount,
        string? ExpressionStrategy);

    private sealed record GroupEnvelope(
        string? Id,
        IReadOnlyList<string>? GeneIds,
        IReadOnlyList<string>? SubgroupIds,
        IReadOnlyList<string>? DependencyGroupIds,
        string? DisplayName);

    private sealed record BodyPlanEnvelope(
        string? Id,
        IReadOnlyList<string>? RequiredGroupIds,
        IReadOnlyList<string>? OptionalGroupIds,
        IReadOnlyList<string>? SharedGroupIds,
        string? DisplayName);

    private sealed record StepEnvelope(
        string? Kind,
        string? Name,
        bool? ExpectedValid,
        string? DiagnosticCode,
        bool? MustBePresent,
        PopulationTemplateFrequencyAssertionEnvelope? PopulationTemplateFrequencyAssertion);

    private sealed record PopulationTemplateFrequencyAssertionEnvelope(
        PopulationTemplateEnvelope? Template,
        string? GroupId,
        string? GeneId,
        string? AlleleId,
        int SampleCount,
        ulong Seed,
        double ExpectedProportion,
        double AbsoluteTolerance,
        int? MaximumSamples);

    private sealed record PopulationTemplateEnvelope(
        string? Id,
        string? VersionId,
        string? SystemDefinitionVersion,
        string? ChangeSummary,
        IReadOnlyList<PopulationTemplateGroupEnvelope>? Groups);

    private sealed record PopulationTemplateGroupEnvelope(
        string? GroupId,
        IReadOnlyList<PopulationTemplateGeneEnvelope>? Genes);

    private sealed record PopulationTemplateGeneEnvelope(
        string? GeneId,
        int AlleleCount,
        IReadOnlyList<PopulationTemplateAlleleEnvelope>? Alleles);

    private sealed record PopulationTemplateAlleleEnvelope(
        string? AlleleId,
        double Weight,
        double? NumericValue);
}
