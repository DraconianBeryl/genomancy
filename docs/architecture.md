# Genomancy Architecture

## Slice 0 dependency boundaries

Genomancy is organized around an engine-neutral core library.

- `Genomancy.Core` contains domain types and behavior. It must not reference Godot, storage providers, database providers, file-system storage modules, or test frameworks.
- `Genomancy.Tests` is the implementation test runner for the repository. It may reference `Genomancy.Core`.
- Future serialization, storage, resource-testing, and Godot integration assemblies may depend on `Genomancy.Core`.
- `Genomancy.Core` must not depend on those future adapter assemblies.

The core library currently targets `netstandard2.1` to keep the domain assembly usable from non-Godot .NET hosts and from a future Godot adapter. Test and tooling projects target the SDK available in this repository environment, currently `net9.0`.

## Verification command

Run the repository verification script from the repository root:

```bash
./scripts/verify.sh
```

The script keeps .NET, NuGet, and MSBuild caches inside `.dotnet-work/` so restore, build, and test commands do not depend on writable user-home directories.
