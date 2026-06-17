using Genomancy.Core.Definitions;
using Genomancy.Core.Genome;
using Genomancy.Core.Serialization;
using Genomancy.Core.Templates;
using Genomancy.Core.Variants;
using Genomancy.Storage.Common;

namespace Genomancy.Storage.Binary;

public sealed class BinaryFileStore
{
    private readonly BinaryFileStoreOptions _options;
    private readonly StoragePathResolver _resolver;

    public BinaryFileStore(string rootDirectory, BinaryFileStoreOptions? options = null)
    {
        _resolver = new StoragePathResolver(rootDirectory, "Binary");
        _options = options ?? new BinaryFileStoreOptions();
    }

    public string RootDirectory => _resolver.RootDirectory;

    public BinaryFileWriteResult WriteGenomeVersion(string relativePath, GenomeVersion version, bool? overwrite = null)
    {
        ArgumentNullException.ThrowIfNull(version);

        return Write(
            relativePath,
            stream => GenomeBinaryCodec.WriteVersion(stream, version),
            StoredResourceKind.GenomeVersion,
            version.IndividualId.Value,
            version.Id.Value,
            version.SystemDefinitionVersion.Value,
            overwrite);
    }

    public GenomeVersion ReadGenomeVersion(
        string relativePath,
        SystemDefinitionVersion expectedSystemDefinitionVersion,
        Func<SystemDefinitionVersion, SystemDefinitionVersion, GenomeCompatibilityResult>? compatibility = null)
    {
        return Read(relativePath, stream => GenomeBinaryCodec.ReadVersion(stream, expectedSystemDefinitionVersion, compatibility));
    }

    public BinaryFileWriteResult WritePopulationTemplate(
        string relativePath,
        PopulationTemplateVersion template,
        bool? overwrite = null)
    {
        ArgumentNullException.ThrowIfNull(template);

        return Write(
            relativePath,
            stream => PopulationTemplateBinaryCodec.Write(stream, template),
            StoredResourceKind.PopulationTemplateVersion,
            template.Id.Value,
            template.VersionId.Value,
            template.SystemDefinitionVersion.Value,
            overwrite);
    }

    public PopulationTemplateVersion ReadPopulationTemplate(
        string relativePath,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        return Read(relativePath, stream => PopulationTemplateBinaryCodec.Read(stream, expectedSystemDefinitionVersion));
    }

    public BinaryFileWriteResult WritePopulationTemplateGroup(
        string relativePath,
        PopulationTemplateGroupVersion group,
        bool? overwrite = null)
    {
        ArgumentNullException.ThrowIfNull(group);

        return Write(
            relativePath,
            stream => PopulationTemplateGroupBinaryCodec.Write(stream, group),
            StoredResourceKind.PopulationTemplateGroupVersion,
            group.Id.Value,
            group.VersionId.Value,
            group.SystemDefinitionVersion.Value,
            overwrite);
    }

    public PopulationTemplateGroupVersion ReadPopulationTemplateGroup(
        string relativePath,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        return Read(relativePath, stream => PopulationTemplateGroupBinaryCodec.Read(stream, expectedSystemDefinitionVersion));
    }

    public BinaryFileWriteResult WriteTemplatePopulationManifest(
        string relativePath,
        TemplatePopulationManifest manifest,
        bool? overwrite = null)
    {
        ArgumentNullException.ThrowIfNull(manifest);

        return Write(
            relativePath,
            stream => TemplatePopulationManifestBinaryCodec.Write(stream, manifest),
            StoredResourceKind.TemplatePopulationManifest,
            manifest.TemplateGroupId.Value,
            manifest.TemplateGroupVersionId.Value,
            manifest.SystemDefinitionVersion.Value,
            overwrite);
    }

    public TemplatePopulationManifest ReadTemplatePopulationManifest(
        string relativePath,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        return Read(relativePath, stream => TemplatePopulationManifestBinaryCodec.Read(stream, expectedSystemDefinitionVersion));
    }

    public BinaryFileWriteResult WriteRuntimeBodyPlanVariant(
        string relativePath,
        RuntimeBodyPlanVariant variant,
        bool? overwrite = null)
    {
        ArgumentNullException.ThrowIfNull(variant);

        return Write(
            relativePath,
            stream => RuntimeBodyPlanVariantBinaryCodec.Write(stream, variant),
            StoredResourceKind.RuntimeBodyPlanVariant,
            variant.Id.Value,
            variant.Id.Value,
            variant.SystemDefinitionVersion.Value,
            overwrite);
    }

    public RuntimeBodyPlanVariant ReadRuntimeBodyPlanVariant(
        string relativePath,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        return Read(relativePath, stream => RuntimeBodyPlanVariantBinaryCodec.Read(stream, expectedSystemDefinitionVersion));
    }

    private BinaryFileWriteResult Write(
        string relativePath,
        Action<Stream> write,
        StoredResourceKind kind,
        string resourceId,
        string resourceVersionId,
        string systemDefinitionVersion,
        bool? overwrite)
    {
        var fullPath = ResolveRelativePath(relativePath);

        try
        {
            var result = StorageFileOperations.Write(
                fullPath,
                write,
                new StoragePathPolicy(
                    "binary",
                    _options.CreateDirectories,
                    overwrite ?? _options.OverwriteExisting,
                    _options.TemporaryFileSuffix));
            return new BinaryFileWriteResult(
                result.FullPath,
                result.ByteCount,
                result.Sha256Hex,
                new StoredResourceEntry(
                    kind,
                    StoredResourceFormat.Binary,
                    relativePath,
                    result.FullPath,
                    resourceId,
                    resourceVersionId,
                    systemDefinitionVersion,
                    result.ByteCount,
                    result.Sha256Hex));
        }
        catch (StorageFileException exception)
        {
            throw new BinaryFileStoreException(exception.Message, exception);
        }
    }

    private T Read<T>(string relativePath, Func<Stream, T> read)
    {
        var fullPath = ResolveRelativePath(relativePath);

        try
        {
            return StorageFileOperations.Read(fullPath, read, "binary");
        }
        catch (StorageFileException exception)
        {
            throw new BinaryFileStoreException(exception.Message, exception);
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
            throw new BinaryFileStoreException(exception.Message, exception);
        }
    }
}
