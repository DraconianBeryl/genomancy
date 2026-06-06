# Genomancy Architecture

## Project Boundaries

`Genomancy.Core` is the engine-neutral domain assembly. It owns the genetics
model, invariants, deterministic mechanics, serialization contracts, and policy
interfaces as they are implemented.

The following boundaries are reserved for later slices:

- Serialization implementation that depends only on core abstractions.
- Optional storage modules for JSON files, custom binary files, and SQLite.
- Designer-authored resource testing infrastructure.
- A Godot adapter that converts between Godot-facing resources/nodes and core
  types without moving genetics behavior into Godot-specific classes.

## Dependency Direction

Allowed dependencies:

- Optional adapters depend on `Genomancy.Core`.
- Optional storage modules depend on `Genomancy.Core`.
- Resource testing tools depend on `Genomancy.Core`.
- Implementation tests depend on `Genomancy.Core`.

Disallowed dependencies:

- `Genomancy.Core` must not reference Godot assemblies.
- `Genomancy.Core` must not reference SQLite or other permanent-storage
  providers.
- `Genomancy.Core` must not depend on test frameworks or test projects.
- `Genomancy.Core` must not own a filesystem layout, database, cloud service, or
  save-game repository.

## Target Framework

Slice 0 targets `net9.0` because this repository is being developed with .NET
SDK 9.0.111 on Gentoo Linux and Godot 4.6.2 is installed locally. The core
library has no Godot dependency, so this target can be revisited if a later
Godot adapter or deployment target requires a narrower framework.

## Verification Toolchain Debt

The Slice 0 verification script avoids generated native apphost execution and
runs the built test assembly with `dotnet exec`.

This is a deliberate toolchain workaround, not a domain architecture choice. In
the current Gentoo source-built .NET SDK 9.0.111 environment, a generated Linux
apphost for `Genomancy.Tests` has been observed to build successfully but fail
at launch. Binary inspection showed that the apphost contained
`Genomancy.Tests.dll` at one apphost placeholder location while the
runtime-used application path slot still contained the apphost placeholder
string:

```text
c3ab8ff13720e8ad9047dd39466b3c8974e592c2fa383d4a3960714caef0c4f2
```

The native launcher then attempted to execute a file with that placeholder name
from the output directory. The managed test assembly itself ran correctly with
`dotnet exec`, so this does not indicate a failure in `Genomancy.Core`, the
test harness, or `net9.0` as a target framework.

Technical debt:

- Verification currently proves managed build output and smoke-test behavior,
  not generated native apphost launch behavior.
- Any future Genomancy command-line tools, packaged executables, or standalone
  distribution artifacts must explicitly test their generated launchers on the
  target SDK/runtime/package combination.
- Before relying on `dotnet run --no-build`, generated apphosts, or native
  launcher packaging in CI or release workflows, revalidate the SDK behavior or
  use a toolchain known not to produce this apphost placeholder issue.
- This workaround should be removed only after apphost generation has a
  recorded passing check in the target environment.
