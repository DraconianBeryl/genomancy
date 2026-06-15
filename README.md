# Genomancy

Genomancy is a C# implementation of the fantasy genetics system described in
`fantasy_genetics_system_requirements.md`.

The core library is intended to be usable from Godot and from non-Godot .NET
hosts. Domain behavior starts in `Genomancy.Core`; engine adapters, storage
providers, and resource testing infrastructure are separate project boundaries.

## Slice 0 Verification

Run the current build and implementation smoke tests with:

```sh
bash scripts/verify.sh
```

This command restores the solution, builds it, runs the implementation test
harness, and checks the expected project dependency boundary.

## Resource Test Batch CLI

The optional CLI host can execute a serialized JSON resource-test batch plan:

```sh
dotnet run --project src/Genomancy.Cli -- batch run \
  --plan path/to/batch-plan.json \
  --batch-result path/to/batch-result.json \
  --manifest path/to/result-manifest.json \
  --manifest-mode upsert \
  --run-result-root path/to/run-results \
  --report path/to/batch-report.txt
```

Exit codes are stable: `0` success, `1` resource-test failure, `2` usage error,
and `3` execution error.
