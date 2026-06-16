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

It can also inspect stored JSON artifacts and update manifests from stored batch
results:

```sh
dotnet run --project src/Genomancy.Cli -- result show \
  --result path/to/run-result.json \
  --report path/to/run-report.txt

dotnet run --project src/Genomancy.Cli -- batch show \
  --batch-result path/to/batch-result.json \
  --report path/to/batch-report.txt

dotnet run --project src/Genomancy.Cli -- manifest show \
  --manifest path/to/result-manifest.json \
  --status failed \
  --tag smoke \
  --resolve-root path/to/run-results

dotnet run --project src/Genomancy.Cli -- manifest update \
  --manifest path/to/result-manifest.json \
  --from-batch-result path/to/batch-result.json \
  --manifest-mode upsert

dotnet run --project src/Genomancy.Cli -- manifest result show \
  --manifest path/to/result-manifest.json \
  --run-id run.smoke \
  --result-root path/to/run-results

dotnet run --project src/Genomancy.Cli -- manifest verify \
  --manifest path/to/result-manifest.json \
  --result-root path/to/run-results \
  --status failed

dotnet run --project src/Genomancy.Cli -- manifest repair \
  --manifest path/to/result-manifest.json \
  --result-root path/to/run-results \
  --dry-run
```
