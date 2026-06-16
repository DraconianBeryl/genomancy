using Genomancy.Core.Definitions;
using Genomancy.Core.Genome;
using Genomancy.Core.Serialization;
using Genomancy.Core.Templates;
using Genomancy.Core.Variants;

namespace Genomancy.Storage.Binary;

public sealed class BinaryFileStore
{
    private readonly BinaryFileStoreOptions _options;
    private readonly string _rootDirectory;

    public BinaryFileStore(string rootDirectory, BinaryFileStoreOptions? options = null)
    {
        if (string.IsNullOrWhiteSpace(rootDirectory))
        {
            throw new ArgumentException("Root directory must not be empty.", nameof(rootDirectory));
        }

        _rootDirectory = Path.GetFullPath(rootDirectory);
        _options = options ?? new BinaryFileStoreOptions();
    }

    public string RootDirectory => _rootDirectory;

    public BinaryFileWriteResult WriteGenomeVersion(string relativePath, GenomeVersion version, bool? overwrite = null)
    {
        ArgumentNullException.ThrowIfNull(version);

        return Write(relativePath, stream => GenomeBinaryCodec.WriteVersion(stream, version), overwrite);
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

        return Write(relativePath, stream => PopulationTemplateBinaryCodec.Write(stream, template), overwrite);
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

        return Write(relativePath, stream => PopulationTemplateGroupBinaryCodec.Write(stream, group), overwrite);
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

        return Write(relativePath, stream => TemplatePopulationManifestBinaryCodec.Write(stream, manifest), overwrite);
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

        return Write(relativePath, stream => RuntimeBodyPlanVariantBinaryCodec.Write(stream, variant), overwrite);
    }

    public RuntimeBodyPlanVariant ReadRuntimeBodyPlanVariant(
        string relativePath,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        return Read(relativePath, stream => RuntimeBodyPlanVariantBinaryCodec.Read(stream, expectedSystemDefinitionVersion));
    }

    private BinaryFileWriteResult Write(string relativePath, Action<Stream> write, bool? overwrite)
    {
        var fullPath = ResolveRelativePath(relativePath);
        var directory = Path.GetDirectoryName(fullPath)
            ?? throw new BinaryFileStoreException($"Path '{relativePath}' does not have a parent directory.");

        if (_options.CreateDirectories)
        {
            Directory.CreateDirectory(directory);
        }
        else if (!Directory.Exists(directory))
        {
            throw new BinaryFileStoreException($"Directory '{directory}' does not exist.");
        }

        var allowOverwrite = overwrite ?? _options.OverwriteExisting;

        if (!allowOverwrite && File.Exists(fullPath))
        {
            throw new BinaryFileStoreException($"File '{fullPath}' already exists.");
        }

        var temporaryPath = CreateTemporaryPath(fullPath);

        try
        {
            using (var stream = new FileStream(
                temporaryPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None))
            {
                write(stream);
            }

            File.Move(temporaryPath, fullPath, allowOverwrite);
            return new BinaryFileWriteResult(fullPath, new FileInfo(fullPath).Length);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            throw new BinaryFileStoreException($"Failed to write binary resource file '{fullPath}'.", exception);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }

    private T Read<T>(string relativePath, Func<Stream, T> read)
    {
        var fullPath = ResolveRelativePath(relativePath);

        try
        {
            using var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return read(stream);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            throw new BinaryFileStoreException($"Failed to read binary resource file '{fullPath}'.", exception);
        }
    }

    private string ResolveRelativePath(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            throw new ArgumentException("Relative path must not be empty.", nameof(relativePath));
        }

        if (Path.IsPathFullyQualified(relativePath) || Path.IsPathRooted(relativePath))
        {
            throw new BinaryFileStoreException("Binary storage paths must be relative to the store root.");
        }

        var segments = relativePath.Split([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar]);

        if (segments.Any(segment => segment is "" or "." or ".."))
        {
            throw new BinaryFileStoreException("Binary storage paths must not contain empty, current-directory, or parent-directory segments.");
        }

        var fullPath = Path.GetFullPath(Path.Combine(_rootDirectory, relativePath));
        var rootWithSeparator = _rootDirectory.EndsWith(Path.DirectorySeparatorChar)
            ? _rootDirectory
            : _rootDirectory + Path.DirectorySeparatorChar;

        if (!fullPath.StartsWith(rootWithSeparator, StringComparison.Ordinal))
        {
            throw new BinaryFileStoreException("Binary storage path escapes the store root.");
        }

        return fullPath;
    }

    private string CreateTemporaryPath(string fullPath)
    {
        var directory = Path.GetDirectoryName(fullPath)
            ?? throw new BinaryFileStoreException($"Path '{fullPath}' does not have a parent directory.");
        var fileName = Path.GetFileName(fullPath);
        return Path.Combine(directory, $"{fileName}.{Guid.NewGuid():N}{_options.TemporaryFileSuffix}");
    }
}
