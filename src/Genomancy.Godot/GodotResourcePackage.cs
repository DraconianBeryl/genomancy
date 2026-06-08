namespace Genomancy.Godot;

public sealed record GodotResourcePackage
{
    public GodotResourcePackage(string packageId, IEnumerable<GodotResourceDocument> resources)
    {
        if (string.IsNullOrWhiteSpace(packageId))
        {
            throw new ArgumentException("Godot resource package id must not be empty.", nameof(packageId));
        }

        ArgumentNullException.ThrowIfNull(resources);

        PackageId = packageId.Trim();
        Resources = Array.AsReadOnly(resources.OrderBy(resource => resource.Path).ToArray());

        var duplicatePath = Resources
            .GroupBy(resource => resource.Path)
            .FirstOrDefault(group => group.Count() > 1);

        if (duplicatePath is not null)
        {
            throw new ArgumentException(
                $"Godot resource package contains duplicate path '{duplicatePath.Key}'.",
                nameof(resources));
        }
    }

    public string PackageId { get; }

    public IReadOnlyList<GodotResourceDocument> Resources { get; }

    public GodotResourceDocument Get(GodotResourcePath path)
    {
        return Resources.Single(resource => resource.Path == path);
    }
}
