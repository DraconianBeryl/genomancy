# Genomancy Architecture

## Project Boundaries

`Genomancy.Core` is the engine-neutral domain assembly. It owns the genetics
model, invariants, deterministic mechanics, serialization contracts, and policy
interfaces as they are implemented.

Implemented optional boundaries:

- `Genomancy.Godot` depends on `Genomancy.Core` and provides package-free
  Godot-facing resource documents, resource packages, import/export bridges, and
  runtime startup diagnostics. It currently bridges genomes, mosaic genomes,
  population templates, population template groups, resource-test
  specifications, and resource-test results. It intentionally does not move
  genetics behavior into adapter types.
- `Genomancy.Storage.JsonFile` depends on `Genomancy.Core` and provides generic
  atomic JSON-file persistence using caller-supplied core stream codecs.

The following boundaries are reserved for later slices:

- Optional custom binary-file and SQLite storage modules.
- GodotSharp `Resource` subclasses and editor/runtime plugin packaging that wrap
  the current adapter once package/runtime constraints are selected.

## Dependency Direction

Allowed dependencies:

- Optional adapters depend on `Genomancy.Core`.
- Optional storage modules depend on `Genomancy.Core`.
- Resource testing tools depend on `Genomancy.Core`.
- Implementation tests depend on `Genomancy.Core` and optional modules under
  test.

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
