# Genomancy

Genomancy is a C# implementation of the fantasy genetics system described in
`fantasy_genetics_system_requirements.md`.

The core library is intended to be usable from Godot and from non-Godot .NET
hosts. Domain behavior starts in `Genomancy.Core`; engine adapters, storage
providers, and resource testing infrastructure are separate project boundaries.

## Slice 0 Verification

Run the current build and implementation smoke tests with:

```sh
./scripts/verify.sh
```

This command builds the solution, runs the implementation test harness, and
checks the expected project dependency boundary.

The script intentionally executes the built managed test assembly with
`dotnet exec` instead of using `dotnet run` or the generated native apphost.
The current Gentoo source-built .NET SDK 9.0.111 environment has produced an
invalid Linux apphost during sandboxed verification: the apphost contained the
correct managed assembly name in one placeholder location but still retained
the `c3ab8ff...` apphost placeholder at the runtime-used application path slot.
Executing that native launcher failed with:

```text
The application to execute does not exist: '.../c3ab8ff13720e8ad9047dd39466b3c8974e592c2fa383d4a3960714caef0c4f2'.
```

Directly executing the managed DLL with `dotnet exec` passed the same smoke
tests. Treat generated apphost execution as untrusted for project verification
until the SDK/package behavior is revalidated.
