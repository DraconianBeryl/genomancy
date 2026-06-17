using Genomancy.Core.Definitions;
using Genomancy.Core.Genome;
using Genomancy.Core.Serialization;
using Genomancy.Core.Templates;
using Genomancy.Core.Variants;
using Genomancy.Storage.Common;

namespace Genomancy.Storage.Json;

public sealed class JsonFileStore
{
    private readonly JsonFileStoreOptions _options;
    private readonly StoragePathResolver _resolver;

    public JsonFileStore(string rootDirectory, JsonFileStoreOptions? options = null)
    {
        _resolver = new StoragePathResolver(rootDirectory, "JSON");
        _options = options ?? new JsonFileStoreOptions();
    }

    public string RootDirectory => _resolver.RootDirectory;

    public JsonFileWriteResult WriteGenomeVersion(string relativePath, GenomeVersion version, bool? overwrite = null)
    {
        ArgumentNullException.ThrowIfNull(version);

        return Write(relativePath, stream => GenomeJsonCodec.WriteVersion(stream, version), overwrite);
    }

    public GenomeVersion ReadGenomeVersion(
        string relativePath,
        SystemDefinitionVersion expectedSystemDefinitionVersion,
        Func<SystemDefinitionVersion, SystemDefinitionVersion, GenomeCompatibilityResult>? compatibility = null)
    {
        return Read(relativePath, stream => GenomeJsonCodec.ReadVersion(stream, expectedSystemDefinitionVersion, compatibility));
    }

    public JsonFileWriteResult WritePopulationTemplate(
        string relativePath,
        PopulationTemplateVersion template,
        bool? overwrite = null)
    {
        ArgumentNullException.ThrowIfNull(template);

        return Write(relativePath, stream => PopulationTemplateJsonCodec.Write(stream, template), overwrite);
    }

    public PopulationTemplateVersion ReadPopulationTemplate(
        string relativePath,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        return Read(relativePath, stream => PopulationTemplateJsonCodec.Read(stream, expectedSystemDefinitionVersion));
    }

    public JsonFileWriteResult WritePopulationTemplateGroup(
        string relativePath,
        PopulationTemplateGroupVersion group,
        bool? overwrite = null)
    {
        ArgumentNullException.ThrowIfNull(group);

        return Write(relativePath, stream => PopulationTemplateGroupJsonCodec.Write(stream, group), overwrite);
    }

    public PopulationTemplateGroupVersion ReadPopulationTemplateGroup(
        string relativePath,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        return Read(relativePath, stream => PopulationTemplateGroupJsonCodec.Read(stream, expectedSystemDefinitionVersion));
    }

    public JsonFileWriteResult WriteTemplatePopulationManifest(
        string relativePath,
        TemplatePopulationManifest manifest,
        bool? overwrite = null)
    {
        ArgumentNullException.ThrowIfNull(manifest);

        return Write(
            relativePath,
            stream =>
            {
                var buffer = TemplatePopulationManifestJsonCodec.WriteToBuffer(manifest);
                stream.Write(buffer);
            },
            overwrite);
    }

    public TemplatePopulationManifest ReadTemplatePopulationManifest(
        string relativePath,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        return Read(relativePath, stream =>
        {
            using var memory = new MemoryStream();
            stream.CopyTo(memory);
            return TemplatePopulationManifestJsonCodec.ReadFromBuffer(memory.ToArray(), expectedSystemDefinitionVersion);
        });
    }

    public JsonFileWriteResult WriteRuntimeBodyPlanVariant(
        string relativePath,
        RuntimeBodyPlanVariant variant,
        bool? overwrite = null)
    {
        ArgumentNullException.ThrowIfNull(variant);

        return Write(
            relativePath,
            stream =>
            {
                var buffer = RuntimeBodyPlanVariantJsonCodec.WriteToBuffer(variant);
                stream.Write(buffer);
            },
            overwrite);
    }

    public RuntimeBodyPlanVariant ReadRuntimeBodyPlanVariant(
        string relativePath,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        return Read(relativePath, stream =>
        {
            using var memory = new MemoryStream();
            stream.CopyTo(memory);
            return RuntimeBodyPlanVariantJsonCodec.ReadFromBuffer(memory.ToArray(), expectedSystemDefinitionVersion);
        });
    }

    private JsonFileWriteResult Write(string relativePath, Action<Stream> write, bool? overwrite)
    {
        var fullPath = ResolveRelativePath(relativePath);

        try
        {
            var result = StorageFileOperations.Write(
                fullPath,
                write,
                new StoragePathPolicy(
                    "JSON",
                    _options.CreateDirectories,
                    overwrite ?? _options.OverwriteExisting,
                    _options.TemporaryFileSuffix));
            return new JsonFileWriteResult(result.FullPath, result.ByteCount, result.Sha256Hex);
        }
        catch (StorageFileException exception)
        {
            throw new JsonFileStoreException(exception.Message, exception);
        }
    }

    private T Read<T>(string relativePath, Func<Stream, T> read)
    {
        var fullPath = ResolveRelativePath(relativePath);

        try
        {
            return StorageFileOperations.Read(fullPath, read, "JSON");
        }
        catch (StorageFileException exception)
        {
            throw new JsonFileStoreException(exception.Message, exception);
        }
    }

    private string ResolveRelativePath(string relativePath)
    {
        try
        {
            return _resolver.ResolveRelativePath(relativePath);
        }
        catch (StoragePathException exception)
        {
            throw new JsonFileStoreException(exception.Message, exception);
        }
    }
}
