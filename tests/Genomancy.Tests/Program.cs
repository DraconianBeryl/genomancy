using System.Reflection;
using Genomancy.Core;

var tests = new (string Name, Action Test)[]
{
    ("Core assembly exposes stable name", CoreAssemblyExposesStableName),
    ("Core assembly is Godot independent", CoreAssemblyIsGodotIndependent),
    ("Core assembly has no forbidden dependencies", CoreAssemblyHasNoForbiddenDependencies),
};

var failures = new List<string>();

foreach (var (name, test) in tests)
{
    try
    {
        test();
        Console.WriteLine($"PASS {name}");
    }
    catch (Exception exception)
    {
        failures.Add($"{name}: {exception.Message}");
        Console.Error.WriteLine($"FAIL {name}: {exception.Message}");
    }
}

if (failures.Count > 0)
{
    Console.Error.WriteLine();
    Console.Error.WriteLine("Implementation test failures:");

    foreach (var failure in failures)
    {
        Console.Error.WriteLine($"- {failure}");
    }

    return 1;
}

Console.WriteLine("All implementation smoke tests passed.");
return 0;

static void CoreAssemblyExposesStableName()
{
    AssertEqual("Genomancy.Core", GenomancyCoreAssembly.Name);
}

static void CoreAssemblyIsGodotIndependent()
{
    AssertTrue(GenomancyCoreAssembly.IsGodotIndependent, "Core assembly must be marked Godot independent.");
}

static void CoreAssemblyHasNoForbiddenDependencies()
{
    var forbiddenPrefixes = new[]
    {
        "Godot",
        "Microsoft.Data.Sqlite",
        "System.Data.SQLite",
        "SQLite",
        "xunit",
        "NUnit",
        "MSTest",
    };

    var references = typeof(GenomancyCoreAssembly)
        .Assembly
        .GetReferencedAssemblies()
        .Select(reference => reference.Name ?? string.Empty)
        .ToArray();

    foreach (var reference in references)
    {
        if (forbiddenPrefixes.Any(prefix => reference.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Forbidden core dependency found: {reference}");
        }
    }
}

static void AssertTrue(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}

static void AssertEqual<T>(T expected, T actual)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
    {
        throw new InvalidOperationException($"Expected '{expected}', got '{actual}'.");
    }
}
