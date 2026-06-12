using System.Text;
using Genomancy.Core.Definitions;
using Genomancy.Core.Genome;
using Genomancy.Core.Mosaicism;
using Genomancy.Core.ResourceTesting;
using Genomancy.Core.Serialization;
using Genomancy.Core.Templates;

namespace Genomancy.Godot;

public static class GodotResourceBridge
{
    public static GodotResourceDocument ExportGenomeVersion(GodotResourcePath path, GenomeVersion version)
    {
        ArgumentNullException.ThrowIfNull(version);

        return new GodotResourceDocument(
            path,
            GodotResourceKind.GenomeVersion,
            GenomeJsonCodec.WriteVersionToText(version),
            version.SystemDefinitionVersion.Value);
    }

    public static GodotAdapterResult<GenomeVersion> ImportGenomeVersion(
        GodotResourceDocument document,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        return Import(
            document,
            GodotResourceKind.GenomeVersion,
            () => GenomeJsonCodec.ReadVersionFromBuffer(Encoding.UTF8.GetBytes(document.PayloadJson), expectedSystemDefinitionVersion));
    }

    public static GodotResourceDocument ExportPopulationTemplate(
        GodotResourcePath path,
        PopulationTemplateVersion template)
    {
        ArgumentNullException.ThrowIfNull(template);

        return new GodotResourceDocument(
            path,
            GodotResourceKind.PopulationTemplate,
            PopulationTemplateJsonCodec.WriteToText(template),
            template.SystemDefinitionVersion.Value);
    }

    public static GodotAdapterResult<PopulationTemplateVersion> ImportPopulationTemplate(
        GodotResourceDocument document,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        return Import(
            document,
            GodotResourceKind.PopulationTemplate,
            () => PopulationTemplateJsonCodec.ReadFromBuffer(Encoding.UTF8.GetBytes(document.PayloadJson), expectedSystemDefinitionVersion));
    }

    public static GodotResourceDocument ExportPopulationTemplateGroup(
        GodotResourcePath path,
        PopulationTemplateGroupVersion group)
    {
        ArgumentNullException.ThrowIfNull(group);

        return new GodotResourceDocument(
            path,
            GodotResourceKind.PopulationTemplateGroup,
            PopulationTemplateGroupJsonCodec.WriteToText(group),
            group.SystemDefinitionVersion.Value);
    }

    public static GodotAdapterResult<PopulationTemplateGroupVersion> ImportPopulationTemplateGroup(
        GodotResourceDocument document,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        return Import(
            document,
            GodotResourceKind.PopulationTemplateGroup,
            () => PopulationTemplateGroupJsonCodec.ReadFromBuffer(Encoding.UTF8.GetBytes(document.PayloadJson), expectedSystemDefinitionVersion));
    }

    public static GodotResourceDocument ExportMosaicGenome(
        GodotResourcePath path,
        MosaicGenomeState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        return new GodotResourceDocument(
            path,
            GodotResourceKind.MosaicGenome,
            MosaicGenomeJsonCodec.WriteToText(state),
            state.PrimaryGenomeVersion.SystemDefinitionVersion.Value);
    }

    public static GodotAdapterResult<MosaicGenomeState> ImportMosaicGenome(
        GodotResourceDocument document,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        return Import(
            document,
            GodotResourceKind.MosaicGenome,
            () => MosaicGenomeJsonCodec.ReadFromBuffer(Encoding.UTF8.GetBytes(document.PayloadJson), expectedSystemDefinitionVersion));
    }

    public static GodotResourceDocument ExportResourceTests(
        GodotResourcePath path,
        IEnumerable<ResourceTestSpecification> specifications)
    {
        ArgumentNullException.ThrowIfNull(specifications);

        var materialized = specifications.ToArray();
        var versions = materialized
            .Select(specification => specification.SystemDefinitionVersion.Value)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();
        var tags = materialized
            .SelectMany(specification => specification.Tags)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();

        return new GodotResourceDocument(
            path,
            GodotResourceKind.ResourceTests,
            ResourceTestJsonCodec.WriteToText(materialized),
            string.Join(",", versions),
            tags);
    }

    public static GodotAdapterResult<IReadOnlyList<ResourceTestSpecification>> ImportResourceTests(GodotResourceDocument document)
    {
        return Import(
            document,
            GodotResourceKind.ResourceTests,
            () => ResourceTestJsonCodec.ReadFromBuffer(Encoding.UTF8.GetBytes(document.PayloadJson)));
    }

    public static GodotResourceDocument ExportResourceTestResult(
        GodotResourcePath path,
        ResourceTestRunResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        var versions = result.Cases
            .SelectMany(testCase => testCase.ReproducibilityPackets)
            .Select(packet => packet.ResourceSetVersion.Value)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();
        var tags = result.Cases
            .SelectMany(testCase => testCase.Tags)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();

        return new GodotResourceDocument(
            path,
            GodotResourceKind.ResourceTestResult,
            ResourceTestResultJsonCodec.WriteToText(result),
            string.Join(",", versions),
            tags);
    }

    public static GodotAdapterResult<ResourceTestRunResult> ImportResourceTestResult(GodotResourceDocument document)
    {
        return Import(
            document,
            GodotResourceKind.ResourceTestResult,
            () => ResourceTestResultJsonCodec.ReadFromBuffer(Encoding.UTF8.GetBytes(document.PayloadJson)));
    }

    public static GodotResourceDocument ExportResourceTestResultManifest(
        GodotResourcePath path,
        ResourceTestResultManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(manifest);

        var tags = manifest.Entries
            .SelectMany(entry => entry.Tags)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();

        return new GodotResourceDocument(
            path,
            GodotResourceKind.ResourceTestResultManifest,
            ResourceTestResultManifestJsonCodec.WriteToText(manifest),
            string.Empty,
            tags);
    }

    public static GodotAdapterResult<ResourceTestResultManifest> ImportResourceTestResultManifest(
        GodotResourceDocument document)
    {
        return Import(
            document,
            GodotResourceKind.ResourceTestResultManifest,
            () => ResourceTestResultManifestJsonCodec.ReadFromBuffer(Encoding.UTF8.GetBytes(document.PayloadJson)));
    }

    private static GodotAdapterResult<T> Import<T>(
        GodotResourceDocument document,
        string expectedKind,
        Func<T> read)
    {
        ArgumentNullException.ThrowIfNull(document);

        if (document.Kind != expectedKind)
        {
            return GodotAdapterResult<T>.Failure(new GodotAdapterDiagnostic(
                "GODOT_RESOURCE_KIND_MISMATCH",
                document.Path.Value,
                $"Expected resource kind '{expectedKind}', got '{document.Kind}'."));
        }

        try
        {
            return GodotAdapterResult<T>.Success(read());
        }
        catch (GenomeSerializationException exception)
        {
            return GodotAdapterResult<T>.Failure(new GodotAdapterDiagnostic(
                "GODOT_RESOURCE_IMPORT_FAILED",
                document.Path.Value,
                exception.Message));
        }
        catch (ArgumentException exception)
        {
            return GodotAdapterResult<T>.Failure(new GodotAdapterDiagnostic(
                "GODOT_RESOURCE_IMPORT_FAILED",
                document.Path.Value,
                exception.Message));
        }
    }

}
