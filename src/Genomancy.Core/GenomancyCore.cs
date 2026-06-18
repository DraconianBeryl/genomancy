using System;
using System.Reflection;

namespace Genomancy.Core;

/// <summary>
/// Provides stable assembly-level information for hosts and smoke tests.
/// </summary>
public static class GenomancyCore
{
    public const string AssemblyName = "Genomancy.Core";

    public static Assembly Assembly => typeof(GenomancyCore).Assembly;

    public static Version AssemblyVersion => Assembly.GetName().Version ?? new Version(0, 0, 0, 0);
}
