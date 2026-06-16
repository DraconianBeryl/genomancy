# Genomancy

Genomancy is a C# implementation of the fantasy genetics system described in
`fantasy_genetics_system_requirements.md`.

The core library is intended to be usable from Godot and from non-Godot .NET
hosts. Domain behavior starts in `Genomancy.Core`; engine adapters, storage
providers, and resource testing infrastructure are separate project boundaries.

`Genomancy.Storage.Json` is the first optional storage provider. It persists
core JSON resources to caller-owned filesystem paths without adding filesystem
dependencies to `Genomancy.Core`.

## Slice 0 Verification

Run the current build and implementation smoke tests with:

```sh
bash scripts/verify.sh
```

This command restores the solution, builds it, runs the implementation test
harness, and checks the expected project dependency boundary.
