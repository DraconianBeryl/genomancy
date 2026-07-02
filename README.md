# Genomancy

Genomancy is planned as a C# implementation of the fantasy genetics system
described in `fantasy_genetics_system_requirements.md`.

The core library is intended to be usable from Godot and from non-Godot .NET
hosts. The reset baseline contains the project skeleton and documentation only.
Genetics behavior is not implemented yet.

## Skeleton Verification

Run the current skeleton build with:

```sh
./scripts/verify.sh
```

This command builds the solution using repo-local writable .NET/NuGet/MSBuild
state directories so the repository can be checked in restricted sandbox
environments.

Generated native apphost execution is not covered by current skeleton
verification.
