using System;
using System.Linq;
using System.Runtime.Versioning;
using Genomancy.Core;

Run("core assembly loads", CoreAssemblyLoads);
Run("core dependency boundary", CoreDependencyBoundary);
Run("core target framework", CoreTargetFramework);

Console.WriteLine("All Genomancy implementation tests passed.");

static void CoreAssemblyLoads()
{
    Assert.Equal("Genomancy.Core", GenomancyCore.Assembly.GetName().Name);
    Assert.Equal(GenomancyCore.AssemblyName, GenomancyCore.Assembly.GetName().Name);
    Assert.True(GenomancyCore.AssemblyVersion.Major >= 1);
}

static void CoreDependencyBoundary()
{
    string[] forbiddenPrefixes =
    [
        "Godot",
        "Microsoft.Data.Sqlite",
        "System.Data.SQLite",
        "xunit",
        "nunit",
        "MSTest"
    ];

    string[] referencedAssemblies = GenomancyCore.Assembly
        .GetReferencedAssemblies()
        .Select(reference => reference.Name ?? string.Empty)
        .OrderBy(name => name, StringComparer.Ordinal)
        .ToArray();

    string[] forbiddenReferences = referencedAssemblies
        .Where(name => forbiddenPrefixes.Any(prefix => name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
        .ToArray();

    Assert.Empty(forbiddenReferences);
}

static void CoreTargetFramework()
{
    TargetFrameworkAttribute? targetFramework = GenomancyCore.Assembly
        .GetCustomAttributes(typeof(TargetFrameworkAttribute), inherit: false)
        .OfType<TargetFrameworkAttribute>()
        .SingleOrDefault();

    Assert.Equal(".NETStandard,Version=v2.1", targetFramework?.FrameworkName);
}

static void Run(string name, Action test)
{
    try
    {
        test();
        Console.WriteLine($"PASS {name}");
    }
    catch (Exception exception)
    {
        Console.Error.WriteLine($"FAIL {name}");
        Console.Error.WriteLine(exception);
        Environment.ExitCode = 1;
        throw;
    }
}

internal static class Assert
{
    public static void Empty<T>(T[] values)
    {
        if (values.Length != 0)
        {
            throw new InvalidOperationException($"Expected empty collection, found: {string.Join(", ", values)}");
        }
    }

    public static void Equal<T>(T expected, T actual)
    {
        if (!Equals(expected, actual))
        {
            throw new InvalidOperationException($"Expected '{expected}', found '{actual}'.");
        }
    }

    public static void True(bool condition)
    {
        if (!condition)
        {
            throw new InvalidOperationException("Expected condition to be true.");
        }
    }
}
