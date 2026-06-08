using System.Text;
using Genomancy.Core.Definitions;
using Genomancy.Core.Genome;
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
