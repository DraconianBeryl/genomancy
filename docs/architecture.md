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
