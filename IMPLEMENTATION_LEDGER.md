# Genomancy Implementation Ledger

## Ledger control

| Field | Value |
|---|---|
| Project | Genomancy |
| Ledger status | Active; initial planning baseline |
| Requirements source | `fantasy_genetics_system_requirements.md` |
| Source revision | Initial repository version; no implementation existed when this ledger was created |
| Target language | C# |
| Integration target | Godot-compatible, with no Godot dependency in the core library |
| Last ledger update | 2026-06-15 |
| Current implementation slice | Slice 33 - Batch-plan Godot bridge and batch-run text reports (verified); later hardening/release work is next |

This file is the persistent requirements and progress ledger for Genomancy. Update it in the same change that alters scope, architecture, implementation status, or test coverage. Do not mark a requirement complete solely because a type or API exists; completion requires its acceptance criteria and tests to pass.

## Status vocabulary

- **Planned**: accepted scope, not implemented.
- **In progress**: implementation has begun but acceptance criteria are not all met.
- **Implemented**: behavior is present but required verification is incomplete.
- **Verified**: implementation and required tests satisfy the current acceptance criteria.
- **Deferred**: intentionally postponed to a named later slice.
- **Out of scope**: explicitly excluded from this project boundary.
- **Blocked**: cannot progress without a recorded decision or external change.

## Scope baseline

### In scope

- A reusable C# domain library implementing the fantasy genetics model in the requirements source.
- A strict design/runtime mode boundary. Authored definitions are editable in design mode and frozen after validated runtime loading.
- Stable authored-resource identifiers and system-definition versions.
- Individuals, genomes, immutable genome versions, transient current genome copies, genes, alleles, ranked allele sets, groups, body plans, developmental phases, and runtime-derived body-plan variants.
- Policy-controlled expression, activation, inheritance, reproduction, mutation, repair, compatibility, traces, non-ploidal inheritance, and population generation.
- Variable ploidy, multi-parent contribution, mosaicism/chimerism, linkage, dependencies, generated complements, and nonstandard reproduction.
- Immutable population-template and template-group versions, blending, sampling, and simulation.
- Stable JSON and binary serialization at multiple resource/state granularities.
- In-memory stream, buffer, byte-array, and text serialization APIs in the core library.
- Optional non-core storage modules for JSON files, custom binary files, and SQLite.
- A first-class designer-authored resource testing framework, distinct from implementation tests.
- Deterministic behavior where seeded randomness is involved, including separated random streams where required.
- A thin, optional Godot integration layer that adapts the engine-neutral core without moving domain rules into Godot types.

### Explicitly out of scope

- Molecular biology simulation: DNA bases, RNA, transcription, translation, protein folding, cellular biochemistry, and full embryology.
- Identity, souls, reincarnation, possession identity, prophecy, fate, social caste, legal species identity, blood-purity ideology, political legitimacy, true names, and related metaphysics or social systems.
- Ecological selection, survival selection, mating behavior, geography, economy, and culture unless supplied as external context.
- A mandatory database, filesystem layout, cloud service, asset database, save repository, networking model, user interface, or game-specific entity model in the core library.
- Godot-specific inheritance or serialization as a requirement of core domain objects.
- Unconstrained mutation or automatic invention of body structures outside authored policies.

### Scope injections and changes

| Date | Change | Source | Impact | Status |
|---|---|---|---|---|
| 2026-06-05 | Select C# as the implementation language. | Project request | Constrains solution structure and public API design. | Accepted |
| 2026-06-05 | Require Godot usability without exclusive Godot coupling. | Project request | Core assemblies must not reference Godot; integration belongs in an adapter assembly. | Accepted |
| 2026-06-05 | Require incremental-refinement planning. | Project request | Near-term slices have detailed deliverables/tests; distant slices remain outcome-level and must be refined before work starts. | Accepted |
| 2026-06-06 | Use .NET SDK 9.0.111 and target `net9.0` for the initial core/test projects. | Slice 0 implementation | Establishes current build target; may be revisited if a later Godot adapter requires a narrower target. | Accepted |
| 2026-06-06 | Use repo-local ignored `.dotnet-work/` directories for .NET, NuGet, XDG, and MSBuild writable state during verification. | Slice 0 implementation | Makes verification work in restricted Linux/Codex sandbox environments without relying on writable home directories or shared `/tmp` paths. | Accepted |
| 2026-06-06 | Force single-node/non-parallel MSBuild restore/build in `scripts/verify.sh`. | Slice 1 implementation | Codex Linux sandbox breaks MSBuild's parallel restore graph probe, causing `_IsProjectRestoreSupported` to fail with `0 Error(s)`; `-m:1`, `RestoreBuildInParallel=false`, and `BuildInParallel=false` make the failure mode disappear and expose normal compiler/test output. | Accepted |
| 2026-06-06 | Execute the compiled test DLL with `dotnet exec` instead of `dotnet run --no-build`. | Slice 1 implementation | Avoids sandbox-sensitive apphost path resolution after build. | Accepted |
| 2026-06-06 | Implement Slice 2 binary serialization as a preliminary binary envelope containing the canonical Slice 2 JSON payload. | Slice 2 implementation | Satisfies stream/buffer binary round trips now while deferring a compact stable binary layout decision to the serialization finalization slice. | Accepted |
| 2026-06-06 | Add `GeneDefinition.RequiredAlleleCount` and `GeneDefinition.ExpressionStrategy` as defaulted definition fields. | Slice 3 implementation | Enables initial ploidy/arity checks and expression strategies without breaking existing constructor call sites. | Accepted |
| 2026-06-06 | Use named deterministic random streams derived from a root seed with FNV-1a stream-name hashing and SplitMix64 draws. | Slice 4 implementation | Gives cross-runtime deterministic stream separation without relying on `System.Random` as a serialized behavior contract. | Accepted |
| 2026-06-06 | Refine Slice 5 to a current-copy mutation service with allele, numeric, copy-count, and group add/remove operations. | Slice 5 implementation | Covers first mutation/repair lifecycle without introducing full mutation event history, serialized mutation policy resources, or metaphysical source semantics. | Accepted |
| 2026-06-06 | Add `HeritableObjectState` to `GenomeVersion` for non-ploidal objects and traces. | Slice 6 implementation | Integrates non-ploidal and trace state into immutable versioning and existing genome JSON/binary codecs without adding a separate persistence boundary. | Accepted |
| 2026-06-06 | Refine Slice 7 to compatibility metadata, inviable reproduction, clonal-copy reproduction, development timelines, gestation context, and numeric maternal effects. | Slice 7 implementation | Advances compatibility/development/expanded reproduction while deferring full hybrid morphology construction, nonstandard reproduction families, and gestational simulation. | Accepted |
| 2026-06-06 | Refine Slice 8 to numeric sum/weighted-average expression, deterministic generated complements, and JSON-serializable runtime body-plan variants. | Slice 8 implementation | Advances advanced expression and variant state while deferring full expression policy language, generated complement resource graphs, and, until Slice 21, binary variant codecs. | Accepted |
| 2026-06-06 | Refine Slice 9 to regional genome assignments, region-scoped expression, inheritance-site source resolution, and distinct chimeric material state. | Slice 9 implementation | Advances mosaic/chimera modeling while deferring region geometry, chimeric serialization, and full inheritance workflows. | Accepted |
| 2026-06-06 | Refine Slice 10 to immutable statistical population template versions with deterministic sampling, blending, template-from-individual, population generation, and JSON codecs. | Slice 10 implementation | Advances template workflows while deferring biased inheritance/mutation hooks, full statistical tolerances, and binary template codecs. | Accepted |
| 2026-06-07 | Refine Slice 11 to immutable population template-group versions, weighted direct/nested selection, optional deterministic cross-template blending, and structure-preserving generated genome metadata. | Slice 11 implementation | Advances nested population simulation while deferring template-group serialization, authored resource validation, statistical tolerance reports, and biased inheritance/mutation hooks. | Accepted |
| 2026-06-07 | Refine Slice 12 to an in-memory resource-test runner with fixture factories, validation/freeze operations, validation assertions, custom step extensibility, deterministic result ordering, and structured diagnostics. | Slice 12 implementation | Establishes the first resource-testing framework surface while deferring serialized test resources, resource loading, snapshots, fuzz/matrix execution, reproducibility packets, and statistical assertions. | Accepted |
| 2026-06-08 | Refine Slice 13 to typed serialized resource-test specifications, deterministic JSON codecs, materialization into executable definitions, and tag include/exclude filtering. | Slice 13 implementation | Adds a designer-authored resource-test boundary while deferring full resource-pack loading, snapshots, fuzz/matrix execution, statistical assertions, and, until Slice 18, serialized result/failure packets. | Accepted |
| 2026-06-08 | Refine Slice 14 to shared preliminary binary envelopes for genomes/templates/resource tests and an optional atomic JSON-file storage adapter in a separate assembly. | Slice 14 implementation | Advances serialization and storage boundaries without adding provider dependencies to core; SQLite, migrations, custom compact binary storage, and remaining model codecs are deferred. | Accepted |
| 2026-06-08 | Refine Slice 15 to a package-free `Genomancy.Godot` adapter with Godot-style resource documents/packages, core codec import/export, runtime startup diagnostics, and package metadata. | Slice 15 implementation | Advances Godot integration without adding a GodotSharp dependency; actual Godot `Resource` subclasses, editor plugins, and export packaging are deferred. | Accepted |
| 2026-06-08 | Refine Slice 16 to bounded population-template allele-frequency simulation, absolute statistical tolerances, resource-test statistical assertions, and deterministic JSON reproducibility packets. | Slice 16 implementation | Establishes reusable statistical/reproducibility primitives while deferring reproduction distributions, template-group simulation reports, confidence models, outlier policies, and, until Slice 17, serialized statistical step specifications. | Accepted |
| 2026-06-08 | Refine Slice 17 to serialized resource-test specifications for the Slice 16 population-template frequency assertion. | Slice 17 implementation | Makes designer-authored JSON/binary resource-test specs able to carry embedded population-template statistical assertions while deferring broader statistical registries, external template references, and, until Slice 18, result codecs. | Accepted |
| 2026-06-08 | Refine Slice 18 to deterministic JSON and preliminary binary codecs for resource-test run results, diagnostics, and reproducibility packets. | Slice 18 implementation | Makes resource-test outcomes portable without defining resource-pack layout or persistence policy; storage integration, manifests, retention, and compact binary schemas remain deferred. | Accepted |
| 2026-06-09 | Refine Slice 19 to deterministic JSON and preliminary binary codecs for embedded population template-group versions. | Slice 19 implementation | Makes nested template groups portable while preserving Slice 11's embedded-version model; external template/group registries, resource-pack references, compact binary schemas, and structure-level statistical reports remain deferred. | Accepted |
| 2026-06-09 | Refine Slice 20 to package-free Godot adapter import/export for population template groups and resource-test run results. | Slice 20 implementation | Extends the existing Godot-facing DTO bridge to newer core codecs without adding GodotSharp resource classes, editor plugins, binary import/export, or persistence policy. | Accepted |
| 2026-06-09 | Refine Slice 21 to a preliminary binary codec for runtime body-plan variants. | Slice 21 implementation | Closes the current runtime-variant binary round-trip gap using the shared JSON-wrapped binary envelope while deferring compact binary schemas and variant persistence in genome versions. | Accepted |
| 2026-06-09 | Refine Slice 22 to standalone JSON and preliminary binary codecs for mosaic genome state. | Slice 22 implementation | Makes ID-based mosaic/chimeric runtime state portable as a separate resource while deferring genome-version embedding, regional geometry, overlapping expression, and full chimeric workflows. | Accepted |
| 2026-06-09 | Refine Slice 23 to package-free Godot adapter import/export for standalone mosaic genome state. | Slice 23 implementation | Extends the existing Godot-facing DTO bridge to mosaic resources without adding GodotSharp resources, binary import/export, persistence policy, or richer mosaic behavior. | Accepted |
| 2026-06-10 | Refine Slice 24 to resource-test diagnostic severity filtering. | Slice 24 implementation | Adds report-focused diagnostic filtering while preserving execution and pass/fail semantics; runtime-safe subsets, resource-pack loading, and broader reporting remain deferred. | Accepted |
| 2026-06-11 | Refine Slice 25 to deterministic resource-test text report formatting. | Slice 25 implementation | Adds a human-readable report export over existing run results while relying on existing JSON result codecs for machine-readable reports; CLI, storage, editor integration, and resource-pack loading remain deferred. | Accepted |
| 2026-06-11 | Refine Slice 26 to JSON-file storage for resource-test results. | Slice 26 implementation | Adds an optional storage-module factory for persisted resource-test run results using existing core codecs and atomic JSON-file storage; manifests, retention, naming policy, and resource-pack integration remain deferred. | Accepted |
| 2026-06-11 | Refine Slice 27 to derived resource-test run summaries. | Slice 27 implementation | Adds a reusable deterministic summary surface for result status/counts to support reports, future manifests, and future CLI status without implementing manifests, CLI output, or persistence policy. | Accepted |
| 2026-06-12 | Refine Slice 28 to resource-test result manifests. | Slice 28 implementation | Adds deterministic manifest entries with stored-result paths and embedded summaries to support future batch/CLI indexes without defining execution, retention, or project file layout policy. | Accepted |
| 2026-06-12 | Refine Slice 29 to package-free Godot adapter import/export for resource-test result manifests. | Slice 29 implementation | Extends the Godot-facing DTO bridge to Slice 28 manifests without adding GodotSharp resources, editor plugins, binary import/export, CLI integration, or result retention policy. | Accepted |
| 2026-06-12 | Refine Slice 30 to resource-test batch runs with manifest generation. | Slice 30 implementation | Adds core batch-run orchestration over existing resource-test definitions/options/results and produces a deterministic manifest without introducing CLI commands, filesystem layout, retention, or storage writes. | Accepted |
| 2026-06-12 | Refine Slice 31 to serialized resource-test batch-run plans. | Slice 31 implementation | Adds deterministic JSON plans for multiple batch runs by embedding existing resource-test specification JSON and per-run options, without adding CLI commands, filesystem writes, binary plan codecs, or resource-pack loading. | Accepted |
| 2026-06-15 | Refine Slice 32 to batch-plan binary codec and JSON-file storage. | Slice 32 implementation | Adds a preliminary binary envelope codec for serialized batch-run plans and an optional typed JSON-file store factory without defining CLI execution, result writes, retention, or project layout. | Accepted |
| 2026-06-15 | Expand scope to Slice 33 batch-plan Godot bridge and batch-run text reports. | Project request to implement the next generous slice and allow scope expansion | Adds package-free Godot document import/export for serialized resource-test batch-run plans and deterministic human-readable batch-run result reports, without adding CLI commands, result-file writes, manifest persistence, or GodotSharp resources. | Accepted |

## Architectural decisions and constraints

| ID | Decision or constraint | Rationale | Status |
|---|---|---|---|
| ARC-001 | Domain behavior lives in a Godot-independent `Genomancy.Core` assembly. | Supports Godot, tests, tools, servers, and other .NET hosts without engine coupling. | Accepted |
| ARC-002 | Godot integration will be an optional adapter assembly. | Prevents Godot resource/node lifecycles from defining core semantics. | Accepted |
| ARC-003 | Authored definitions and runtime instance state use distinct types or clearly enforced ownership boundaries. | Runtime may mutate individual genetic state but not interpretation rules. | Accepted |
| ARC-004 | Permanent versions are immutable value graphs; changes are produced through controlled builders/services. | Enforces genome/template version invariants and improves reproducibility. | Accepted |
| ARC-005 | Policies are explicit inputs to mechanics; there is no implicit global mutation or expression behavior. | Matches opt-in mechanics and designer authority requirements. | Accepted |
| ARC-006 | Randomness is injected through named deterministic streams, never read from ambient global randomness. | Enables repeatability and stream separation for tests and simulations. | Accepted |
| ARC-007 | Core persistence APIs target streams/buffers/text, not paths or databases. | Preserves the required storage boundary. | Accepted |
| ARC-008 | Serialized authored references use stable IDs; runtime variants reference frozen definitions and policies. | Preserves compatibility and avoids embedding mutable authoring definitions. | Accepted |
| ARC-009 | Public APIs will use ordinary .NET types unless an adapter requires conversion. | Keeps the core portable across Godot and non-Godot hosts. | Accepted |
| ARC-010 | The initial target framework is `net9.0`; binary encoding and SQLite provider remain open until serialization/storage refinement. | .NET SDK 9.0.111 and Godot 4.6.2 are installed locally; the core has no Godot dependency, so retargeting remains possible. | Partially resolved |
| ARC-011 | Seeded random mechanics use named streams derived from a root seed and a specified SplitMix64 implementation. | Keeps reproduction deterministic and prevents unrelated random draws from perturbing allele selection. | Accepted |

## Requirements register

The source specification remains authoritative for detailed behavior. The IDs below provide implementation and test traceability; they do not replace the source text.

| ID | Requirement family | Source sections | Status | Planned slice | Verification |
|---|---|---:|---|---|---|
| REQ-MODE | Fixed design/runtime operating mode, startup ordering, validation, freeze, and system-definition versions. | 3, 27, 30 | In progress | 1 | Unit + integration |
| REQ-MODE-FREEZE | Runtime definitions become immutable after successful validation/migration and before ordinary runtime state loads/operations. | 3.3-3.6, 28.3 | In progress | 1 | Unit + negative tests |
| REQ-ID | Stable identifiers and compatibility references for authored resources and serialized state. | 3.6, 31.3 | In progress | 1 | Unit + serialization tests |
| REQ-MODEL | Core object hierarchy from allele/value through nested population structures. | 4, 24 | In progress | 1-2 | Unit tests |
| REQ-GENOME | Individuals, genomes, immutable permanent versions, and mutable/provisional current copies remain distinct. | 4.1-4.4, 16, 28.1 | In progress | 2 | Unit + invariant tests |
| REQ-GENE | Categorical and numeric genes, alleles, ranked sets, and opt-in expression patterns. | 4.5-4.7, 5 | In progress | 2-3 | Unit + resource tests |
| REQ-GROUP | Groups, subgroups, presence, completeness, sharing, dependencies, linkage, and generated complements. | 6 | In progress | 3, 8 | Unit + graph validation |
| REQ-BODY | Authored body plans, active/dormant/primary states, families, activation, and temporary shapeshifting. | 4.12, 7.1-7.7 | In progress | 3 | Unit + integration |
| REQ-VARIANT | Deterministic, serializable, policy-created runtime body-plan variants as instance state. | 4.13, 7.8, 28.4 | In progress | 8 | Unit + serialization + resource tests |
| REQ-DEVELOP | Developmental sequencing, gestation, post-birth requirements, and maternal/gestational effects. | 8 | In progress | 7 | Integration + resource tests |
| REQ-COMPAT | Explicit compatibility layers with fertile, sterile, inviable, and hybrid-morphology outcomes. | 9 | In progress | 7 | Unit + resource tests |
| REQ-PLOIDY | Default/variable ploidy, ranked interpretation, shared-group models, and multi-parent roles. | 10, 28.8 | In progress | 4 | Unit + matrix tests |
| REQ-REPRO | Policy-driven allele/group/object selection, ordinary and nonstandard reproduction, germline/generation sites. | 11 | In progress | 4, 7 | Unit + integration + simulation |
| REQ-MOSAIC | Mosaic/chimeric regions, expression, inheritance, and absorbed-twin genetic handling. | 12 | In progress | 9 | Unit + integration |
| REQ-NONPLOID | Flags, counters, accumulators, archives, markers, weights, activation, transmission, mutation, and decay. | 4.10, 13 | In progress | 6 | Unit + resource tests |
| REQ-TRACE | Trace structure, weighted transmission, activation, allele replacement effects, mutation, degradation, and loss. | 4.11, 14 | In progress | 6 | Unit + resource tests |
| REQ-MUTATION | Policy-controlled event source, timing, targets, scope, value selection, overwrite, numeric update, copy number, and structural changes. | 15, 28.9-28.10 | In progress | 5 | Unit + policy coverage |
| REQ-VERSION | Permanent version creation, commit policies, current-copy behavior, repair, reversion, and visibility. | 16 | In progress | 2, 5 | Unit + integration |
| REQ-ACQUIRED | External systems may request authored heritable changes through mutation interfaces without becoming core metaphysics. | 17, 20, 32 | In progress | 5 | Integration + negative tests |
| REQ-EXPR | Contextual expression and activation using body plan, phase, ploidy, dependencies, epigenetics, and external context. | 18, 28.5-28.7 | In progress | 3, 8 | Unit + resource tests |
| REQ-EXTERNAL | Container, genealogy, birth order, lineage facts, possession, and identity remain external inputs. | 19, 20, 28.7 | In progress | 3+ | Boundary tests |
| REQ-TEMPLATE | Immutable statistical templates, random generation, blending, biased inheritance, mutation, and simulation. | 21, 28.2, 28.11 | In progress | 10 | Unit + statistical tests |
| REQ-TGROUP | Nested template groups, weights, cross-template blending, generation simulation, and structure preservation. | 22 | In progress | 11 | Unit + simulation tests |
| REQ-TFROMIND | Create statistical templates from individuals without conflating templates and genomes. | 23 | In progress | 10 | Unit tests |
| REQ-POLICY | Explicit policy categories, granularity, inputs, and outputs. | 25 | In progress | 1 onward | Unit + coverage tests |
| REQ-RTEST | First-class immutable-input resource test definitions, fixtures, operations, assertions, diagnostics, and runners. | 26, 27 | In progress | 12-13 | Self-tests + integration |
| REQ-RANDOM | Deterministic execution, separated random streams, reproducibility packets, and statistical tolerances. | 26.13-26.14, 26.24, 26.26 | In progress | 4, 12 | Determinism + statistical tests |
| REQ-VALIDATE | Resource graph, reachability, policy coverage, invariants, negative cases, and required baseline content tests. | 26.19-26.22, 26.39 | In progress | 1, 12-13 | Validation + resource tests |
| REQ-SERIAL | Stable JSON and binary formats at multiple granularities, including versions, variants, templates, tests, and failure packets. | 31.1-31.3 | In progress | 2 onward; finalized 14 | Round-trip + compatibility |
| REQ-STORAGE | Core has no permanent storage; optional JSON-file, binary-file, and SQLite modules depend on core. | 31.4-31.7 | In progress | 14 | Integration tests |
| REQ-GODOT | Optional Godot adapter consumes core APIs without redefining genetics behavior. | Project scope injection | In progress | 15 | Build + adapter tests |

## Incremental implementation plan

The next five slices are deliberately detailed. Slices 5 and later are progressively less specific and must be refined before entering **In progress**. Refinement may split a slice, but it must preserve requirement IDs and record scope changes above.

### Slice 0 - Project foundation

**Status:** Verified on 2026-06-06.

**Objective:** Establish a buildable, testable C# repository with dependency direction that protects the engine-neutral core.

**Deliverables**

- Create a solution and initial projects for core implementation and implementation tests.
- Reserve clear extension points or project boundaries for serialization, optional storage, resource testing, and Godot integration without implementing their domain features yet.
- Select and document the target .NET framework based on supported Godot C# compatibility and non-Godot host usability.
- Enable nullable reference types, deterministic builds, warnings suitable for library code, and consistent formatting.
- Add a minimal CI-capable command that restores, builds, and runs implementation tests.
- Add an architecture note documenting allowed dependencies: adapters and storage depend on core; core does not depend on them.

**Acceptance criteria**

- A clean checkout can restore, build, and test with one documented command.
- `Genomancy.Core` has no Godot, SQLite, filesystem-storage, or test-framework dependency.
- A smoke test loads the core assembly.
- Project names and namespaces are stable enough for Slice 1 public types.

**Tests**

- One implementation smoke test.
- Build verification for all created projects.
- Dependency/reference inspection, automated if practical.

**Explicit deferrals**

- No genetics behavior, serialization format, Godot resource type, or storage provider is implemented.

### Slice 1 - Definition kernel, modes, IDs, and validation

**Status:** Verified on 2026-06-06 for the Slice 1 acceptance criteria. Broader requirement families remain **In progress** where later slices add behavior.

**Objective:** Represent a small authored definition graph and enforce the design/runtime boundary.

**Deliverables**

- Introduce validated value types for stable resource IDs and system-definition versions.
- Define the fixed operating mode selected when a system instance is created.
- Implement the minimum authored definitions: allele, gene, group, and body-plan definitions, plus references among them.
- Implement a design-time builder/editor boundary and an immutable runtime definition snapshot.
- Implement graph validation for duplicate IDs, missing references, invalid basic group/body-plan references, and dependency cycles where cycles are not explicitly supported.
- Implement runtime startup ordering as an API: load/build, validate, optionally migrate through an explicit hook, then freeze.
- Reject definition mutation in runtime mode and after freeze with typed diagnostics/errors.
- Establish the policy contract shape and policy identity, but implement only no-op/test policies needed to exercise the boundary.

**Acceptance criteria**

- Operating mode cannot change during a system instance lifetime.
- Runtime operations cannot observe an unfrozen definition graph.
- Frozen definition collections and nested values cannot be mutated through retained design references.
- All authored references resolve by stable ID, not object identity.
- Validation returns deterministic, structured diagnostics with resource IDs and paths.
- A minimal valid human body-plan resource graph validates and freezes.
- Invalid graphs fail before runtime state is accepted.

**Tests**

- ID/version value semantics and invalid-input tests.
- Design-mode edit success tests.
- Runtime and post-freeze mutation rejection tests, including retained-reference attempts.
- Missing/duplicate reference and dependency-cycle validation tests.
- Deterministic diagnostic ordering test.
- Startup-order integration test.

**Requirements advanced:** REQ-MODE, REQ-MODE-FREEZE, REQ-ID, REQ-MODEL, REQ-POLICY, REQ-VALIDATE.

### Slice 2 - Genome state, immutable versions, and baseline serialization

**Status:** Verified on 2026-06-06 for the Slice 2 acceptance criteria. Broader requirement families remain **In progress** where later slices add expression, mutation policy, repair, storage modules, and final serialization formats.

**Objective:** Model an individual genome and its permanent/transient lifecycle without yet implementing full expression or reproduction.

**Deliverables**

- Implement ranked allele entries/sets and genome group state sufficient for categorical and numeric allele values.
- Implement individual identity references as opaque external IDs; do not encode personhood or genealogy.
- Implement immutable genome versions containing system-definition version, stable version ID, parent-version reference where applicable, and genome state.
- Implement a mutable or replacement-based current genome copy that is explicitly derived from a permanent version.
- Implement controlled commit behavior that creates a new immutable version; repeated temporary changes do not auto-commit.
- Implement historical comparison metadata sufficient for later repair/audit features without committing to full mutation history shape.
- Define versioned JSON envelopes and a preliminary binary codec for the Slice 2 model using streams/buffers only.
- Ensure deserialization validates system-definition version compatibility through an explicit compatibility result/hook.

**Acceptance criteria**

- Permanent versions cannot be changed after creation, including through collection aliasing.
- A current copy can diverge from its base version without changing version history.
- Commit creates a distinct immutable version; discard restores the latest permanent state.
- Serialization round trips preserve IDs, ranking, numeric/categorical values, version ancestry, and definition-version dependency.
- Core serialization performs no filesystem access.
- JSON output ordering/canonicalization is deterministic where the format contract requires it.

**Tests**

- Immutability and collection-aliasing tests.
- Current-copy edit, discard, and commit lifecycle tests.
- No-version-spam test for uncommitted temporary changes.
- JSON and binary round-trip tests, malformed-input tests, and unknown-version tests.
- In-memory stream/buffer-only boundary tests.

**Implemented**

- `ExternalIndividualId` stores opaque caller-owned individual references without genealogy, identity, or personhood semantics.
- `GenomeVersionId`, `RankedAlleleEntry`, `RankedAlleleSet`, `GenomeGroupState`, and `GenomeState` model ranked categorical allele IDs and optional numeric allele values.
- `GenomeVersion` captures stable version ID, system-definition version, external individual ID, optional parent version ID, immutable genome state, and a minimal `ChangeSummary` comparison/audit hint.
- `CurrentGenomeCopy` derives from a permanent version, permits replacement-based temporary edits, supports discard, and creates a distinct immutable child version only on explicit commit.
- `GenomeJsonCodec` reads/writes versioned deterministic JSON envelopes through streams, buffers, and text.
- `GenomeBinaryCodec` reads/writes a preliminary binary stream/buffer payload with a binary magic header and embedded canonical Slice 2 JSON.
- Deserialization enforces system-definition version compatibility by default and accepts caller-supplied compatibility hooks.

**Implementation simplification choices**

- Numeric gene support is represented as an optional numeric value on ranked allele entries; numeric expression and validation against authored numeric gene definitions are deferred to Slice 3 and later gene-policy work.
- Current genome edits replace immutable state fragments instead of exposing mutable collections.
- Historical comparison metadata is limited to parent-version ancestry plus a free-form change summary; full mutation/repair/audit event history is deferred to Slice 5.
- Binary serialization is intentionally preliminary and wraps the deterministic JSON payload instead of defining the final compact binary format.
- Serialization compatibility checks compare the embedded system-definition version to a caller-provided expected version unless a caller-provided hook overrides the result.

**Not yet implemented**

- No expression, body-plan activation, group completeness, ploidy validation, reproduction, mutation, repair, reversion, or visibility policy behavior.
- No validation that genome allele IDs are allowed by a frozen system definition; this arrives with expression/group validation slices.
- No permanent filesystem, database, repository, or Godot serialization integration.
- No final cross-version JSON/binary compatibility contract beyond the Slice 2 envelope version and system-definition version hook.

**Requirements advanced:** REQ-MODEL, REQ-GENOME, REQ-GENE, REQ-VERSION, REQ-SERIAL, REQ-STORAGE.

### Slice 3 - Group completeness, body-plan activation, and basic expression

**Status:** Verified on 2026-06-06 for the Slice 3 acceptance criteria. Broader requirement families remain **In progress** where later slices add richer policies, generated complements, runtime variants, and advanced expression.

**Objective:** Interpret a genome in a body-plan context using a deliberately limited first expression feature set.

**Deliverables**

- Evaluate group presence and completeness against required genes, ranked allele arity, ploidy, and direct dependencies.
- Support authored required, optional, and shared groups on body plans.
- Represent active, dormant, unavailable, and incomplete body-plan availability results with structured reasons.
- Require at least one active body plan for a valid individual runtime state.
- Implement activation/deactivation as expression state, separate from genome mutation and version creation.
- Implement initial expression strategies: strict dominance hierarchy, codominance, and numeric midpoint blending.
- Pass body plan, developmental phase, group state, and explicit external context into expression evaluation even when initial strategies do not consume every field.
- Keep external facts opaque and caller-supplied.

**Simplification for this slice**

- Defer polygenic traits, epistasis, pleiotropy, penetrance, variable expressivity, sex/parent-of-origin rules, generated complements, and runtime-derived variants.
- Support direct acyclic dependencies first; richer policy-dependent dependency behavior is deferred.

**Acceptance criteria**

- The same shared group can produce body-plan-specific evaluation context without duplicating genome data.
- Incomplete or wrong-ploidy groups prevent full body-plan availability with precise diagnostics.
- Temporary activation never creates a genome version.
- Dominance, codominance, and midpoint outputs are deterministic and distinguishable.
- Expression cannot be evaluated without an explicit body-plan context.

**Tests**

- Complete, absent, partial, dormant, and dependency-failed group cases.
- Shared-group evaluation under two body plans.
- Body-plan activation and temporary shapeshift version-count tests.
- Table-driven dominance, codominance, and numeric midpoint tests.
- Boundary test proving external context is read-only opaque input.

**Implemented**

- `GeneDefinition` now carries defaulted `RequiredAlleleCount` and `ExpressionStrategy` fields.
- `DevelopmentalPhaseId`, `ExpressionExternalContext`, and `ExpressionEvaluationContext` make body-plan, phase, group-state, and external facts explicit expression inputs.
- `GroupCompletenessEvaluator` reports complete, missing, incomplete, wrong-ploidy, and dependency-failed group states with deterministic `ExpressionDiagnostic` entries.
- `BodyPlanExpressionState` tracks active body plans separately from genome state and supports activation/deactivation without creating genome versions.
- `BodyPlanAvailabilityEvaluator` returns active, dormant, unavailable, and incomplete body-plan availability against required, optional, and shared groups.
- `IndividualExpressionStateValidator` checks the Slice 3 runtime invariant that at least one body plan is active and available.
- `GeneExpressionEvaluator` implements strict dominance, codominance, and numeric midpoint strategies.

**Implementation simplification choices**

- Ploidy is represented by exact ranked allele entry count per gene through `RequiredAlleleCount`; variable ploidy mapping and lower-arity interpretation are deferred to Slice 4.
- Activation state is only a set of active body-plan IDs; primary body-plan selection, timed activation, and temporary shapeshift policy details remain deferred.
- Developmental phase and external context are passed through and preserved but are not yet consumed by the initial expression strategies.
- Dependency checks cover direct frozen group dependencies already validated as acyclic; policy-dependent dependencies and generated complements remain deferred.
- Numeric midpoint requires every ranked allele entry to carry a numeric value and does not yet validate numeric ranges against authored allele/gene policy.

**Not yet implemented**

- No polygenic traits, epistasis, pleiotropy, penetrance, variable expressivity, parent-of-origin behavior, generated complements, runtime variants, or advanced body-plan family rules.
- No serialization for expression state or expression results yet.
- No designer-authored resource tests; coverage remains in implementation tests.
- No mutation, reproduction, repair, compatibility, developmental sequencing, or random-stream behavior.

**Requirements advanced:** REQ-GROUP, REQ-BODY, REQ-EXPR, REQ-EXTERNAL, REQ-GENE.

### Slice 4 - Deterministic inheritance and ordinary reproduction

**Status:** Verified on 2026-06-06 for the Slice 4 acceptance criteria. Broader requirement families remain **In progress** where later slices add nonstandard reproduction, full compatibility, mutation during inheritance, gestation, non-ploidal objects, traces, resource tests, and reproduction-specific statistical tolerances.

**Objective:** Produce offspring genome versions through explicit parent roles and deterministic selection.

**Deliverables**

- Define reproduction requests with ordered/named genetic parent roles, source genome versions, reproductive policy, and random-stream bundle.
- Implement ranked allele selection for ordinary diploid inheritance and generalized N-parent/N-ploid contribution.
- Implement unambiguous lower-arity interpretation of higher-ranked allele sets.
- Permit the same individual/version to fill multiple parent roles without conflating the roles.
- Implement equal and weighted allele transmission.
- Implement basic group selection while preserving group coherence.
- Return structured success, sterile, incompatible, and invalid-request results; detailed compatibility biology remains deferred.
- Create offspring as a new individual genetic state tied to the frozen system-definition version.

**Simplification for this slice**

- Cover ordinary parent-role reproduction only. Parthenogenesis, budding, fission, spawning, vegetative reproduction, germline sites, traces, non-ploidal objects, mutation during inheritance, and gestational effects remain deferred.
- Compatibility outcomes are supplied by a minimal policy stub; full compatibility genes and hybrid morphology arrive later.

**Acceptance criteria**

- Identical definitions, parent versions, policy inputs, and seeds produce byte-equivalent offspring serialization.
- Separate named random streams prevent unrelated selection operations from perturbing allele selection.
- Ranked selection is deterministic and rejects ambiguous ploidy mappings.
- Weighted selection honors zero-weight exclusion and validated positive finite weights.
- Parent genome versions remain unchanged.

**Tests**

- Seeded diploid inheritance golden cases.
- Multi-parent and variable-ploidy matrix tests.
- Same-individual/multiple-role test.
- Weighted transmission boundary and distribution sanity tests.
- Random-stream separation regression test.
- Parent immutability and offspring serialization tests.

**Implemented**

- `DeterministicRandomStream` implements specified SplitMix64 draws and stable FNV-1a stream-name derivation.
- `NamedRandomStreams` derives independent streams from one root seed and caller-visible stream names.
- `ReproductionParentRole`, `ReproductionPolicy`, `TransmissionWeight`, `ReproductionRequest`, `ReproductionResult`, and diagnostics model ordinary reproduction inputs and structured outcomes.
- `OrdinaryReproductionService` produces immutable offspring `GenomeVersion` instances tied to the frozen system-definition version.
- Parent roles are explicit and role-name based; the same `GenomeVersion` may appear in multiple distinct roles.
- Equal transmission chooses among ranked parent allele entries through deterministic streams.
- Weighted transmission supports per-role/group/gene/allele weights, excludes zero-weight alleles, and rejects negative, infinite, or NaN weights.
- Variable ploidy is represented by `GeneDefinition.RequiredAlleleCount`; reproduction succeeds when contributor count exactly matches the gene requirement.
- Lower-arity reproduction from more roles is accepted only when `ReproductionPolicy.ContributingRoleNames` explicitly selects the contributing roles; otherwise it is rejected as ambiguous.
- Basic group selection preserves group coherence by producing offspring group states from frozen group definitions and required gene sets.
- Compatibility policy stub reports fertile/success, sterile, and incompatible outcomes without implementing full biological compatibility.

**Implementation simplification choices**

- Ordinary reproduction only; no parthenogenesis, budding, fission, spawning, vegetative reproduction, germline sites, gestational effects, traces, non-ploidal inheritance, or mutation during inheritance.
- Reproduction policy is an in-memory request object, not a serialized designer-authored policy resource.
- Offspring version ancestry is summarized in `ChangeSummary`; the existing `GenomeVersion.ParentVersionId` remains unused for multi-parent offspring because it is single-parent-shaped from Slice 2.
- Allele selection currently chooses from each contributing parent role independently; crossover/linkage and whole-object group selection are deferred.
- Weighted-selection distribution has deterministic boundary coverage; Slice 16 later adds the first template-focused statistical tolerance framework, but reproduction-specific distribution tolerances remain deferred.

**Not yet implemented**

- Full compatibility genes/layers, fertile/sterile/inviable/hybrid morphology rules, development and gestation, and expanded reproduction modes.
- Mutation, repair, acquired heritable changes, non-ploidal inheritance, traces, mosaicism, and chimerism.
- Designer-authored reproduction policy resources or resource-test runner coverage.
- Serialized reproduction requests/results or reproducibility packets beyond deterministic offspring genome serialization.

**Requirements advanced:** REQ-PLOIDY, REQ-REPRO, REQ-RANDOM, REQ-SERIAL.

### Slice 5 - Mutation, commit policy, repair, and acquired heritable changes

**Status:** Verified on 2026-06-06 for the refined Slice 5 acceptance criteria. Broader requirement families remain **In progress** where later slices add full mutation event history, resource-authored mutation policies, non-ploidal mutation, traces, and advanced structural policies.

**Objective:** Apply controlled heritable genome changes through explicit mutation requests while preserving the permanent/current lifecycle.

**Deliverables**

- Define mutation requests with explicit source kind, source ID, application mode, operations, optional commit version ID, and change summary.
- Implement first operation set: allele replacement, numeric value update, copy-count adjustment, group add, and group remove.
- Validate mutation targets against frozen definitions and current genome state.
- Enforce protected group/gene targets through mutation policy.
- Support current-only mutation without creating permanent genome versions.
- Support committed mutation that creates an immutable child genome version through `CurrentGenomeCopy.Commit`.
- Implement repair from the current base version for a group or gene.
- Implement revert/discard for all current-only changes.
- Expose external mutation sources as opaque caller-supplied labels without identity, metaphysics, spell-system, or social semantics.

**Acceptance criteria**

- Temporary mutation changes current state and leaves base version history untouched.
- Committed mutation creates a distinct immutable child version with parent-version linkage.
- Protected targets and invalid allele choices are rejected before state changes.
- Numeric, copy-count, and structural group mutations update current state deterministically.
- Repair restores the requested group/gene from the current base version without touching unrelated current changes.
- Revert restores the complete current state from the base version.
- External mutation requests are explicit and policy-controlled.

**Tests**

- Current-only mutation divergence without version creation.
- Committed mutation child-version creation and parent linkage.
- Protected-target and invalid-allele rejection tests.
- Numeric update, copy-count adjustment, group add, and group remove tests.
- Repair and revert lifecycle tests.
- External source allowed/blocked boundary tests.

**Implemented**

- `GenomeMutationOperation`, `GenomeMutationTarget`, `GenomeMutationRequest`, `GenomeMutationPolicy`, `GenomeMutationResult`, diagnostics, and status enums.
- `GenomeMutationService.Apply` for current-only and committed mutation application over `CurrentGenomeCopy`.
- `GenomeMutationService.RepairFromBase` for base-version group/gene repair.
- `GenomeMutationService.RevertCurrent` for full current-copy discard/reversion.
- Protected group/gene policy checks and external-source allow/deny policy checks.
- Definition-aware validation for gene existence, group state, gene state, allele rank, and allowed allele IDs.

**Implementation simplification choices**

- Mutation policy is an in-memory request-time policy object, not a serialized designer-authored resource.
- Mutation source is an opaque source-kind/source-ID pair; no spell, magic, identity, possession, soul, or social semantics are represented in core.
- Copy-count mutation extends by duplicating the highest-ranked current allele and truncates by rank; richer copy-number policies are deferred.
- Structural mutation is limited to adding/removing whole group state; subgroup surgery, generated complements, protected subtargets, and authored structural constraints remain deferred.
- Repair uses the current base version only; arbitrary historical repair/reversion and full audit trails remain deferred.
- Mutation operations are applied deterministically in request order after whole-request validation.

**Not yet implemented**

- Serialized mutation requests/results, mutation event history, mutation probability/timing/randomness, policy-authored target selection, protected target hierarchies, structural body-plan validation, non-ploidal object mutation, trace mutation/degradation, and statistical mutation simulation.
- Resource-test coverage for mutation policies.
- Repair from arbitrary historical versions or partial repair with conflict reporting.

**Requirements advanced:** REQ-MUTATION, REQ-VERSION, REQ-ACQUIRED, REQ-GROUP.

### Slice 6 - Non-ploidal inheritance and traces

**Status:** Verified on 2026-06-06 for the refined Slice 6 acceptance criteria. Broader requirement families remain **In progress** where later slices add designer-authored policies, trace effects on allele replacement/mutation, non-ploidal mutation, resource tests, and richer decay/loss rules.

**Objective:** Persist and transmit heritable state that is not interpreted as ranked allele sets.

**Deliverables**

- Define typed non-ploidal object state for flags, counters, accumulators, markers, weights, and archives.
- Define trace state with source ID, strength, active flag, age, and degradation rate.
- Add heritable non-ploidal/trace state to immutable genome versions.
- Preserve heritable object state through JSON and preliminary binary genome codecs.
- Implement deterministic weighted inheritance of non-ploidal objects from parent roles.
- Support inactive-object exclusion by default and explicit inclusion when requested.
- Support trace activation, strength replacement, and degradation.
- Let ordinary reproduction optionally inherit non-ploidal objects and traces.

**Acceptance criteria**

- Non-ploidal inheritance is deterministic for identical parent roles, weights, and seed.
- Zero-weight non-ploidal objects do not transmit.
- Inactive non-ploidal objects do not transmit unless explicitly included.
- Trace activation, replacement, and degradation preserve stable IDs and source references.
- Reproduction can create offspring genome versions with inherited non-ploidal objects and degraded traces.
- JSON and binary genome round trips preserve non-ploidal objects and traces.

**Tests**

- Weighted deterministic non-ploidal inheritance and seed-sweep sanity test.
- Inactive and zero-weight transmission boundary test.
- Trace activation, replacement, and degradation test.
- Ordinary reproduction integration test for inherited non-ploidal objects and traces.
- JSON and binary genome serialization round-trip test for heritable object state.

**Implemented**

- `NonPloidalObjectState`, `NonPloidalObjectKind`, `TraceState`, and `HeritableObjectState`.
- `NonPloidalInheritanceService` for deterministic weighted object selection and trace inheritance.
- `TraceUpdateService` for activation, strength replacement, and degradation.
- `GenomeVersion.HeritableObjects` with default empty state.
- JSON and preliminary binary codec integration through the existing genome-version envelope.
- `ReproductionPolicy` options for inherited non-ploidal object state, inactive object inclusion, and inherited trace degradation.
- `OrdinaryReproductionService` integration for optional heritable-object offspring state.

**Implementation simplification choices**

- Non-ploidal definitions are runtime state only in this slice; no authored non-ploidal resource definitions or validation graph are implemented.
- Weighted object inheritance selects at most one object state per object ID from parent roles.
- Trace inheritance deduplicates by trace ID using highest strength, then applies fixed degradation steps.
- Trace activation/degradation do not yet affect allele expression, mutation targets, or repair behavior.
- Binary serialization remains the preliminary JSON-wrapped codec from Slice 2.

**Not yet implemented**

- Non-ploidal mutation operations, trace mutation/degradation policies, trace loss rules, activation effects, allele replacement effects, resource-authored trace/non-ploidal policies, statistical trace tests, and resource-test coverage.

**Requirements advanced:** REQ-NONPLOID, REQ-TRACE, REQ-REPRO, REQ-SERIAL.

### Slice 7 - Compatibility, development, and expanded reproduction

**Status:** Verified on 2026-06-06 for the refined Slice 7 acceptance criteria. Broader requirement families remain **In progress** where later slices add full compatibility resources, gestational simulation, hybrid morphology construction, and additional nonstandard reproduction modes.

**Objective:** Add the first compatibility/development boundaries and one expanded reproduction mode without moving external identity or gestational systems into core.

**Deliverables**

- Represent compatibility evaluations with fertile, sterile, incompatible, and inviable outcomes.
- Represent hybrid morphology input weights as stable body-plan ID references.
- Add inviable reproduction outcome handling.
- Add clonal-copy reproduction mode with explicit single-contributor validation.
- Represent development stages, ordered development plans, and gestation context.
- Validate that plans requiring gestation receive explicit gestation context.
- Apply simple numeric maternal effects to genome state without mutating source versions.

**Acceptance criteria**

- Compatibility rules can produce layered outcomes with hybrid morphology contribution metadata.
- Inviable reproduction returns a structured inviable result and does not create offspring.
- Clonal-copy reproduction creates offspring by copying one source genome state without allele recombination.
- Invalid clonal-copy requests with multiple contributors are rejected.
- Development timelines are deterministically ordered and report missing gestation context.
- Maternal effects return a new genome state and leave source versions unchanged.

**Tests**

- Compatibility evaluation with hybrid body-plan contribution metadata.
- Inviable reproduction outcome test.
- Clonal-copy success and invalid multi-contributor tests.
- Development timeline ordering and missing-gestation validation tests.
- Maternal numeric-effect immutability test.

**Implemented**

- `CompatibilityEvaluation`, `CompatibilityRule`, `CompatibilityService`, and `HybridMorphologyContribution`.
- `ReproductionCompatibility.Inviable` and `ReproductionResultStatus.Inviable`.
- `ReproductionMode` and `ReproductionPolicy.Mode`.
- `OrdinaryReproductionService` support for inviable outcomes and clonal-copy reproduction.
- `DevelopmentStageDefinition`, `DevelopmentPlan`, `GestationContext`, `MaternalEffect`, `DevelopmentTimeline`, and `DevelopmentService`.

**Implementation simplification choices**

- Compatibility rules are in-memory and role-name based; no resource-authored compatibility graph exists yet.
- Hybrid morphology is represented as weighted body-plan references only; no morphology construction or expression integration yet.
- Clonal copy is the only expanded reproduction mode in this slice.
- Gestation context is opaque and caller-supplied; core does not model containers, pregnancy state, birth, or external entity lifecycles.
- Maternal effects are limited to deterministic numeric deltas on allele entries.

**Not yet implemented**

- Full compatibility layers, sterile/fertile policy resources, hybrid body construction, inviable embryo state, gestation simulation, post-birth requirements, maternal effect timing, germline/generation-site behavior, parthenogenesis, budding, fission, spawning, vegetative reproduction, and resource-test coverage.

**Requirements advanced:** REQ-COMPAT, REQ-DEVELOP, REQ-REPRO.

### Slice 8 - Advanced expression, generated complements, and runtime variants

**Status:** Verified on 2026-06-06 for the refined Slice 8 acceptance criteria. Broader requirement families remain **In progress** where later slices add full expression policy language, resource-authored generated complements, compact/final binary variant schemas, and resource tests.

**Objective:** Extend expression and body-plan interpretation with deterministic generated complements and first-class runtime variant state.

**Deliverables**

- Add additional numeric expression strategies beyond midpoint.
- Generate missing complement group state from explicit policy inputs.
- Validate generated complement group/gene/allele references against frozen definitions.
- Represent runtime body-plan variants with stable IDs, system-definition version, base body-plan ID, group overrides, and change summary.
- Evaluate runtime body-plan variants against genome state and active body-plan expression state.
- Serialize runtime body-plan variants to deterministic JSON with system-version compatibility checks.

**Acceptance criteria**

- Numeric sum and weighted-average expression results are deterministic and distinguishable.
- Generated complements add missing groups and do not regenerate existing groups.
- Generated complements reject unknown or disallowed references before changing state.
- Runtime variants can require generated groups and produce incomplete/active availability results.
- Runtime variant JSON round trips preserve IDs, group overrides, base body-plan reference, change summary, and system-definition version.
- Runtime variant deserialization rejects incompatible system-definition versions.

**Tests**

- Numeric sum and weighted-average expression tests.
- Generated complement success, idempotence, and invalid-reference tests.
- Runtime body-plan variant availability test with generated required group.
- Runtime body-plan variant JSON round-trip and version-rejection tests.

**Implemented**

- `GeneExpressionStrategy.NumericSum` and `GeneExpressionStrategy.NumericWeightedAverage`.
- `GeneratedComplementPolicy`, `GeneratedComplementResult`, and `GeneratedComplementService`.
- `BodyPlanVariantId`, `RuntimeBodyPlanVariant`, `RuntimeBodyPlanVariantService`, and `RuntimeBodyPlanVariantJsonCodec`.
- Structural equality for runtime variants with collection-valued group overrides.

**Implementation simplification choices**

- Generated complement policies are request-time objects, not authored resource graph nodes.
- Generated complements only add whole missing group state; no partial subgroup generation or policy-driven allele synthesis.
- Runtime variants reference a base body plan and add required/optional/shared groups; they do not remove authored base groups.
- Runtime variant serialization is JSON-only in Slice 8; Slice 21 adds a preliminary JSON-wrapped binary codec while compact/final binary schemas remain deferred.
- Advanced expression adds two numeric strategies only; full expression policy language and contextual trace/non-ploidal effects remain deferred.

**Not yet implemented**

- Resource-authored generated complement policies, generated body structures beyond group state, runtime variant persistence in genome versions, compact/final binary variant schemas, variant migration, advanced expression policy language, trace/non-ploidal expression effects, and resource-test coverage.

**Requirements advanced:** REQ-GENE, REQ-GROUP, REQ-BODY, REQ-VARIANT, REQ-EXPR.

### Slice 9 - Mosaicism and chimerism

**Status:** Verified on 2026-06-06 for the refined Slice 9 acceptance criteria. Broader requirement families remain **In progress** where later slices add regional geometry, serialization, chimeric inheritance workflows, and resource tests.

**Objective:** Represent regional genome assignment and chimeric material without conflating chimerism with integrated body-plan variants.

**Deliverables**

- Define stable mosaic region IDs and inheritance site IDs.
- Represent mosaic region assignments from region IDs to immutable genome versions.
- Represent chimeric material as genetically distinct genome material with expressed region IDs.
- Resolve expression in a region using the genome assigned to that region, falling back to the primary genome.
- Resolve inheritance source genome from explicit inheritance-site assignments.
- Keep chimeric material explicitly distinct from runtime body-plan variants.

**Acceptance criteria**

- Region-scoped expression uses the assigned region genome version.
- Unassigned regions fall back to the primary genome version.
- Inheritance sites can resolve to regional genome sources and fall back to primary when unmapped.
- Chimeric material references a distinct genome version and is not treated as a runtime body-plan variant.

**Tests**

- Regional expression test for assigned and fallback regions.
- Inheritance-site source resolution test.
- Chimeric material distinction from integrated runtime variants test.

**Implemented**

- `MosaicRegionId`, `InheritanceSiteId`, `MosaicRegionAssignment`, `MosaicGenomeState`, `InheritanceSiteAssignment`, and `ChimericMaterialState`.
- `MosaicExpressionService` for region-scoped gene expression using existing expression context/evaluators.
- `MosaicInheritanceService` for inheritance-site genome-source resolution.

**Implementation simplification choices**

- Regions are stable IDs only; no geometry, tissue hierarchy, proportions beyond assignment coverage, or overlap resolution is implemented.
- Mosaic state is not serialized as part of genome versions; Slice 22 adds standalone state-resource codecs.
- Chimeric material is explicit runtime state and may mark whether it is integrated, but it does not yet participate in expression automatically.
- Inheritance-site resolution returns a genome version only; it does not yet execute reproduction from region/site sources.

**Not yet implemented**

- Regional geometry, multi-region blending, overlapping mosaic expression, chimeric expression integration, absorbed-twin inheritance rules, genome-version-embedded mosaic/chimera persistence, mutation/repair over regions, reproduction workflows from inheritance sites, and resource-test coverage.

**Requirements advanced:** REQ-MOSAIC, REQ-EXPR, REQ-REPRO.

### Slice 10 - Population templates

**Status:** Verified on 2026-06-06 for the refined Slice 10 acceptance criteria. Broader requirement families remain **In progress** where later slices add template groups, biased inheritance/mutation hooks, statistical tolerances, and binary codecs.

**Objective:** Create immutable statistical population-template versions that can deterministically produce genome versions.

**Deliverables**

- Define stable population-template IDs and template-version IDs.
- Represent immutable group/gene/allele frequency profiles with optional numeric allele values.
- Sample genome versions from templates using deterministic named random streams.
- Generate multiple sampled genome versions with stable version and individual ID prefixes.
- Blend two templates targeting the same system-definition version.
- Create a population template from an individual genome without conflating the template and genome.
- Serialize population template versions through deterministic JSON streams, buffers, and text.

**Acceptance criteria**

- Identical template, seed, genome ID, and individual ID produce byte-equivalent sampled genome serialization.
- Generated populations use stable version/individual IDs and preserve system-definition version.
- Blending combines allele weights deterministically and rejects incompatible system-definition versions.
- Template-from-individual creates frequency-one entries from the source genome and does not mutate the genome.
- JSON round trips preserve template ID, version ID, system-definition version, group/gene/allele frequencies, numeric values, and change summary.
- JSON deserialization rejects incompatible system-definition versions.

**Tests**

- Deterministic sampling and sampled genome serialization test.
- Blend weight combination test.
- Stable generated population version/individual ID test.
- Template-from-individual test.
- Population template JSON round-trip and version-rejection test.

**Implemented**

- `PopulationTemplateId`, `PopulationTemplateVersionId`, `AlleleFrequency`, `GeneTemplate`, `GroupTemplate`, and `PopulationTemplateVersion`.
- `PopulationTemplateService.SampleGenome`, `GeneratePopulation`, `Blend`, and `FromGenome`.
- `PopulationTemplateJsonCodec` for stream, buffer, and text JSON operations with system-version compatibility checks.

**Implementation simplification choices**

- Sampling independently draws each allele rank from a gene template; linkage and correlated traits remain deferred.
- Template blending combines allele weights and uses the maximum allele count for matching genes.
- Population generation is deterministic repeated sampling, not a statistical simulation framework with tolerance reporting.
- Biased inheritance and mutation hooks are not implemented in this slice.
- Template serialization is JSON-only; binary template codecs remain deferred to serialization hardening.

**Not yet implemented**

- Template groups, nested populations, cross-template blend rates, structure-preserving nested simulation, biased inheritance/mutation hooks, statistical tolerance reports, binary template codecs, storage modules, and resource-test coverage.

**Requirements advanced:** REQ-TEMPLATE, REQ-TFROMIND, REQ-RANDOM, REQ-SERIAL.

### Slice 11 - Template groups and nested populations

**Status:** Verified on 2026-06-07 for the refined Slice 11 acceptance criteria. Broader requirement families remain **In progress** where later slices add template-group serialization, resource-authored validation, statistical tolerance reports, and inheritance/mutation hooks.

**Objective:** Add immutable template-group versions that can generate nested population samples while preserving the chosen group/template structure.

**Deliverables**

- Define stable population-template-group IDs and group-version IDs.
- Represent immutable group versions containing weighted direct templates and weighted child template groups.
- Validate template-group system-definition version compatibility across direct templates and children.
- Add optional deterministic cross-template blend policy with a blend occurrence rate and second-template blend weight.
- Generate genome versions from a template group through deterministic weighted selection.
- Generate multiple structure-preserving population samples with stable genome and individual ID prefixes.
- Return immutable generated-output metadata containing the selected group path and primary/secondary source template IDs.

**Acceptance criteria**

- Nested group generation preserves the root-to-leaf template-group path in generated metadata.
- Zero-weight direct templates and child groups are excluded from selection.
- A blend rate of one deterministically blends with a distinct positive-weight direct template when one exists.
- Identical group, seed, genome ID, and individual ID produce structurally equal generated outputs.
- Template-group constructors reject incompatible system-definition versions.
- Template-group versions and generated metadata are isolated from caller collection aliases.

**Tests**

- Nested weighted generation and generated path preservation test.
- Forced deterministic cross-template blend test.
- Version compatibility, zero-positive-weight rejection, and source-alias isolation test.

**Implemented**

- `PopulationTemplateGroupId` and `PopulationTemplateGroupVersionId`.
- `WeightedPopulationTemplate`, `WeightedPopulationTemplateGroup`, and `CrossTemplateBlendPolicy`.
- `PopulationTemplateGroupVersion` with direct templates, child groups, immutable copied collections, system-version validation, and structural equality.
- `GeneratedTemplateGenome` with generated genome version, group path, selected source template IDs, blend metadata, immutable copied collections, and structural equality.
- `PopulationTemplateGroupService.SampleGenome` and `GeneratePopulation` for deterministic weighted nested selection and optional cross-template blending.

**Implementation simplification choices**

- Child groups are embedded immutable group-version objects, not external references resolved through a resource registry.
- Cross-template blending applies only among direct templates in the selected group; child-group selection delegates to the child group's own blend policy.
- Blend policy uses a single occurrence rate and a fixed second-template weight, not pair-specific blend matrices.
- Generated population simulation returns generated members and provenance metadata, not statistical tolerance reports or aggregate distributions.
- Template-group serialization is not implemented in this slice.

**Not yet implemented**

- JSON/binary template-group codecs remain deferred until Slice 19; external template-group reference resolution, resource-authored template-group validation, pair-specific blend matrices, structure-level statistical reports, biased inheritance/mutation hooks, and resource-test coverage remain deferred.

**Requirements advanced:** REQ-TGROUP, REQ-TEMPLATE, REQ-RANDOM.

### Slice 12 - Resource testing framework foundation

**Status:** Verified on 2026-06-07 for the refined Slice 12 acceptance criteria. Broader requirement families remain **In progress** where later slices add serialized resources, broader operations/assertions, snapshots, fuzz/matrix execution, reproducibility packets, and statistical assertions.

**Objective:** Establish a first-class, engine-neutral resource-test runner that can exercise authored definition fixtures without depending on the implementation test harness.

**Deliverables**

- Define stable resource-test IDs.
- Represent in-memory resource-test definitions with fixture factories, ordered steps, display names, and sorted tags.
- Provide a resource-test runner that executes cases in deterministic ID order.
- Provide resource-test context state for a fresh system-definition fixture, latest validation result, optional frozen definition, and structured diagnostics.
- Provide validation and freeze steps for system definitions.
- Provide validation-result and validation-diagnostic assertion steps.
- Support custom test steps through a public step interface.
- Return deterministic case/run results with pass/fail status and ordered diagnostics.

**Acceptance criteria**

- Valid definition fixtures can validate, assert success, assert absent diagnostics, and freeze.
- Invalid fixtures can assert expected validation failure and expected diagnostics.
- Unexpected validation results and missing diagnostics produce structured resource-test errors.
- Cases run in deterministic resource-test ID order.
- Each resource-test run receives a fresh fixture instance.
- Custom steps can inspect and mutate the fixture through the same context used by built-in steps.

**Tests**

- Valid baseline resource-test fixture with validation assertions and freeze.
- Invalid/expected-failure fixture plus unexpected-valid failure diagnostics and deterministic case/diagnostic ordering.
- Custom step fixture-isolation test across repeated runner invocations.

**Implemented**

- `ResourceTestId`, `ResourceTestDefinition`, `IResourceTestStep`, `ResourceTestContext`, `ResourceTestDiagnostic`, `ResourceTestCaseResult`, `ResourceTestRunResult`, and status/severity enums.
- `ResourceTestRunner.Run` for deterministic in-memory case execution.
- `ValidateSystemDefinitionStep`, `FreezeSystemDefinitionStep`, `ExpectValidationResultStep`, and `ExpectValidationDiagnosticStep`.
- Structured resource-test diagnostics for fixture exceptions, step exceptions, failed freeze, missing validation results, validation-result mismatches, and validation-diagnostic mismatches.

**Implementation simplification choices**

- Resource-test fixtures are in-memory factories returning `SystemDefinitionBuilder`; serialized designer-authored test resources are deferred.
- Runner execution is sequential and deterministic; no parallel, matrix, fuzz, or long-running simulation execution is implemented.
- Built-in operations cover only system-definition validation and freeze.
- Built-in assertions cover only validation success/failure and validation diagnostic presence/absence.
- Reproducibility packets are represented only by deterministic test IDs, paths, and diagnostics, not by a serialized failure packet format.

**Not yet implemented**

- JSON/binary resource-test codecs, resource-pack loading, fixture references, operation/assertion registries by serialized kind, snapshots, runtime-safe subsets, fuzz/matrix execution, statistical assertions beyond the Slice 17 serialized population-template frequency assertion, validation reachability/policy coverage assertions, and integration with storage or Godot adapters. Tag filtering arrives in Slice 13 and diagnostic severity filtering arrives in Slice 24.

**Requirements advanced:** REQ-RTEST, REQ-VALIDATE, REQ-RANDOM.

### Slice 13 - Resource testing framework expansion

**Status:** Verified on 2026-06-08 for the refined Slice 13 acceptance criteria. Broader requirement families remain **In progress** where later slices add resource-pack loading, snapshots, fuzz/matrix execution, statistical assertions, and result persistence.

**Objective:** Add a typed serialized resource-test specification boundary that can materialize designer-authored validation tests into the Slice 12 runner.

**Deliverables**

- Define typed resource-test specifications for test metadata, system-definition fixtures, and supported steps.
- Represent serializable allele, gene, group, and body-plan fixture entries.
- Materialize resource-test specifications into executable `ResourceTestDefinition` instances.
- Add a versioned deterministic JSON codec for resource-test specifications over streams, buffers, and text.
- Reject unsupported serialized step kinds and malformed required JSON properties.
- Add runner tag include/exclude filtering while preserving deterministic case ordering.

**Acceptance criteria**

- Resource-test specifications round trip through JSON with deterministic ordering.
- Round-tripped specifications can materialize and execute as normal resource-test definitions.
- Serialized valid and expected-invalid fixtures can both pass their resource-test assertions.
- Unsupported step kinds are rejected before execution.
- Missing required serialized fields are rejected with structured serialization errors.
- Include/exclude tag filtering runs only matching cases in deterministic ID order.

**Tests**

- Resource-test JSON round-trip and materialized execution test.
- Unsupported step and malformed JSON rejection test.
- Deterministic tag include/exclude filtering test.

**Implemented**

- `ResourceTestSpecification`, `ResourceTestFixtureSpecification`, `ResourceTestStepSpecification`, and fixture resource specification records.
- `ResourceTestJsonCodec` for versioned deterministic JSON stream/buffer/text operations.
- Materialization from serialized step kinds to validation, freeze, validation-result, and validation-diagnostic steps.
- `ResourceTestRunOptions` and `ResourceTestRunner.Run` filtering by include/exclude tags.

**Implementation simplification choices**

- Serialized fixtures cover the current core authored definition kernel only: alleles, genes, groups, and body plans.
- Serialized steps cover the Slice 12 built-in validation/freeze/assertion operations only.
- JSON is the only resource-test serialization format in this slice; binary test codecs remain deferred.
- Tag filtering is include/exclude only in Slice 13; Slice 24 adds diagnostic severity filtering while runtime-safe subsets remain deferred.
- Resource-test results/failure packets are still in-memory only in Slice 13; Slice 18 adds result codecs while persistence remains deferred.

**Not yet implemented**

- Resource-pack loading, external fixture references, snapshots, fuzz/matrix execution, serialized/broader statistical assertions beyond the Slice 17 population-template frequency assertion, broader operation/assertion registries, reachability/policy coverage assertions, runtime-safe subset enforcement, result persistence, and integration with storage or Godot adapters.

**Requirements advanced:** REQ-RTEST, REQ-VALIDATE, REQ-RANDOM, REQ-SERIAL.

### Slice 14 - Serialization hardening and optional storage modules

**Status:** Verified on 2026-06-08 for the refined Slice 14 acceptance criteria. Broader requirement families remain **In progress** where later work adds compact binary formats, remaining model codecs, SQLite/custom-binary storage, and migrations.

**Objective:** Harden the existing preliminary binary-envelope pattern and establish the first optional filesystem storage module without introducing storage dependencies into core.

**Deliverables**

- Extract shared binary-envelope framing for core codecs with magic headers, payload lengths, and truncated-payload errors.
- Preserve existing genome binary behavior through the shared envelope implementation.
- Add preliminary binary stream/buffer codecs for population templates and resource-test specifications.
- Add a separate `Genomancy.Storage.JsonFile` assembly that depends on core.
- Provide generic JSON-file save/load using caller-supplied stream codecs.
- Write files through a same-directory temporary file followed by atomic replacement where supported by the host filesystem.
- Add the storage assembly to the solution and verification build.

**Acceptance criteria**

- Genome binary round trips and error handling continue to pass after shared framing extraction.
- Population-template binary round trips preserve immutable template state and reject invalid/truncated headers and incompatible system versions.
- Resource-test binary round trips preserve deterministic JSON-equivalent specifications and remain executable.
- JSON-file storage persists and loads resource-test specifications that execute successfully.
- `Genomancy.Core` retains no filesystem-storage or SQLite provider dependency.
- The complete solution builds with the optional storage assembly.

**Tests**

- Existing genome binary regression coverage.
- Population-template binary round-trip, truncated-header, and system-version rejection test.
- Resource-test binary round-trip, executable materialization, and truncated-header test.
- JSON-file storage save/load and loaded-resource execution test using an isolated temporary directory.

**Implemented**

- Internal `BinaryEnvelopeCodec` shared by preliminary binary codecs.
- `PopulationTemplateBinaryCodec` and `ResourceTestBinaryCodec`.
- Refactored `GenomeBinaryCodec` to use shared binary framing without changing its public API.
- `Genomancy.Storage.JsonFile` optional project and generic `JsonFileStore<T>`.
- Same-directory temporary writes with overwrite move and cleanup.
- Solution/test project references covering the new storage assembly.

**Implementation simplification choices**

- Binary codecs still wrap deterministic JSON payloads; no compact field-level binary schema is finalized.
- JSON-file storage is generic and caller-configured with stream codecs rather than exposing resource-specific repositories.
- Atomic replacement semantics depend on the host filesystem behavior of `File.Move(..., overwrite: true)`.
- No file locking, concurrent-writer coordination, directory manifest, resource-pack layout, or migration framework is implemented.
- SQLite and custom binary-file storage modules are deferred rather than adding package/native-provider dependencies in this slice.

**Not yet implemented**

- Compact stable binary schemas for body-plan variants/template groups/mosaic state, custom binary-file storage, SQLite storage/provider selection, schema migrations, resource-pack manifests, concurrency controls, untrusted-input size limits, and storage integration fixtures beyond JSON resource tests.

**Requirements advanced:** REQ-SERIAL, REQ-STORAGE, REQ-ID.

### Slice 15 - Godot adapter and packaging

**Status:** Verified on 2026-06-08 for the refined Slice 15 acceptance criteria. Broader Godot integration remains **In progress** where later work adds GodotSharp `Resource` subclasses, editor plugins, and export/package integration.

**Objective:** Add a thin optional adapter assembly that exposes Godot-facing resource documents and startup diagnostics while preserving a Godot-free core.

**Deliverables**

- Add separate `Genomancy.Godot` adapter assembly that depends on `Genomancy.Core`.
- Define validated Godot-style resource paths for `res://` and `user://` paths.
- Define Godot-facing resource document and package DTOs with deterministic ordering and duplicate-path rejection.
- Export/import genome versions, population templates, and resource-test specifications through existing core JSON codecs.
- Convert import failures and runtime startup validation failures into stable Godot adapter diagnostics.
- Add basic package metadata for the adapter project.
- Add the adapter assembly to the solution and verification build.

**Acceptance criteria**

- Genome and population-template documents round trip through the adapter.
- Resource-test specifications export/import through a Godot resource document and remain executable.
- Resource packages order documents deterministically and reject duplicate paths.
- Kind mismatches and serialization/version failures return diagnostics instead of throwing through the adapter boundary.
- Runtime startup through the adapter returns a runtime system for valid definitions and validation diagnostics for invalid definitions.
- `Genomancy.Core` remains Godot-free; adapter dependency direction is one-way from adapter to core.

**Tests**

- Godot adapter genome/template export/import round-trip test.
- Godot adapter kind mismatch and import failure diagnostics test.
- Godot adapter resource-test export/import, resource package, and runtime startup diagnostics test.

**Implemented**

- `Genomancy.Godot` optional adapter project with NuGet package metadata.
- `GodotResourcePath`, `GodotResourceKind`, `GodotResourceDocument`, and `GodotResourcePackage`.
- `GodotAdapterDiagnostic` and `GodotAdapterResult<T>`.
- `GodotResourceBridge` for genome, population-template, and resource-test import/export.
- `GodotRuntimeBridge` for runtime startup with validation diagnostics.
- Solution/test references covering the adapter assembly.

**Implementation simplification choices**

- The adapter is package-free and does not reference GodotSharp in this slice.
- Godot resources are represented as ordinary DTOs that can be wrapped by Godot scripts or future `Resource` subclasses.
- Import/export uses existing JSON codecs only; binary Godot resource import/export is deferred.
- Package metadata is basic project metadata; no NuGet package creation, Godot addon folder layout, or editor plugin is produced.

**Not yet implemented**

- GodotSharp `Resource` subclasses, editor inspector integration, Godot addon folder layout, exported `.tres`/`.res` resources, Godot runtime node helpers, binary resource import/export, Godot-specific package publishing, and engine-version compatibility matrices.

**Requirements advanced:** REQ-GODOT, REQ-MODE, REQ-SERIAL.

### Slice 16 - Statistical simulation and reproducibility hardening

**Status:** Verified on 2026-06-08 for the refined Slice 16 acceptance criteria. Broader statistical simulation remains **In progress** where later work adds reproduction distributions, template-group reports, confidence models, and broader statistical test registries.

**Objective:** Establish the first bounded statistical simulation and failure-reproduction path using existing deterministic population-template sampling.

**Deliverables**

- Define absolute statistical proportion tolerances with explicit expected, minimum, and maximum values.
- Define configurable simulation sample-count limits and reject invalid or over-limit workloads before sampling.
- Measure a target allele's observed frequency across all ranked allele entries generated from a population template.
- Preserve seeded determinism by using the existing template sampler and stable per-sample seed sequence.
- Return aggregate sample count, observation count, match count, observed proportion, tolerance, and optional failure packet.
- Define minimal reproducibility packets containing resource-set version, test ID, seed, operation path, serialized template input, failed assertion, expected/actual values, and diagnostic.
- Serialize reproducibility packets through deterministic versioned JSON stream/buffer/text APIs.
- Add a resource-test statistical assertion step that records a structured diagnostic and reproducibility packet on tolerance failure.
- Expose resource-test failure packets on case results.

**Acceptance criteria**

- Identical template, target, seed, sample count, tolerance, and limits produce structurally equal simulation results.
- Allele frequency is measured over ranked allele observations rather than only generated individual count.
- Passing simulations produce no failure packet.
- Failed statistical resource-test assertions produce one structured diagnostic and an inspectable reproducibility packet.
- Reproducibility packet JSON round trips deterministically.
- Zero, negative, and over-limit sample counts are rejected before simulation.

**Tests**

- Deterministic population-template simulation within tolerance with ranked-observation accounting.
- Configured maximum-sample rejection.
- Resource-test statistical failure diagnostic and packet creation.
- Reproducibility packet JSON round trip and deterministic output.

**Implemented**

- `StatisticalTolerance` with clamped absolute proportion bounds.
- `SimulationResourceLimits` with a default maximum of 100,000 samples.
- `PopulationTemplateSimulationService.MeasureAlleleFrequency` and `PopulationTemplateSimulationResult`.
- `ReproducibilityPacket` and `ReproducibilityPacketJsonCodec`.
- `AssertPopulationTemplateFrequencyStep`.
- Reproducibility packet collection on `ResourceTestContext` and `ResourceTestCaseResult`.

**Implementation simplification choices**

- This slice simulates population-template allele frequencies only; it does not add reproduction, mutation, trace, template-group, or multi-generation simulation.
- Tolerances use an absolute proportion range only; confidence intervals, advisory severity, and maximum-outlier semantics remain deferred.
- Each ranked allele entry is one observation. Correlated/linkage-aware observations remain deferred with richer template simulation.
- The simulation runs sequentially and stores aggregate counts only; it does not retain generated genomes unless a future failing-invariant operation requires them.
- Reproducibility packets embed canonical population-template JSON as the input fixture and expose JSON only.
- The statistical resource-test step started as an in-memory custom step in Slice 16; Slice 17 adds serialized specifications for this specific step while broader statistical registries remain deferred.

**Not yet implemented**

- Reproduction/transmission distribution simulation, template-group aggregate reports, multi-generation simulation, mutation/trace simulation, confidence interpretations, maximum outlier policies, advisory statistical failures, parallel execution, elapsed-time/allocation limits, broader serialized statistical step registries, or resource-pack failure-packet persistence.

**Requirements advanced:** REQ-RANDOM, REQ-RTEST, REQ-TEMPLATE, REQ-REPRO.

### Slice 17 - Serialized statistical resource-test assertions

**Status:** Verified on 2026-06-08 for the refined Slice 17 acceptance criteria. Broader statistical and resource-pack behavior remains **In progress**.

**Objective:** Allow designer-authored resource-test JSON/binary specifications to carry the Slice 16 population-template allele-frequency assertion and materialize it into an executable resource-test step.

**Deliverables**

- Add a typed serialized step kind for population-template allele-frequency assertions.
- Represent the required statistical assertion payload in resource-test specifications: embedded template, target group/gene/allele, sample count, seed, tolerance, and optional sample-count limit.
- Extend the deterministic resource-test JSON codec to read/write the statistical assertion payload.
- Preserve binary resource-test support through the existing JSON-wrapped binary resource-test codec.
- Materialize serialized statistical assertions into `AssertPopulationTemplateFrequencyStep`.

**Acceptance criteria**

- Resource-test JSON round trips a population-template frequency assertion deterministically.
- A round-tripped serialized statistical assertion materializes into an executable resource-test definition.
- Executing the materialized assertion produces the same structured statistical failure diagnostic and reproducibility packet path as the in-memory step.
- The serialized payload uses stable resource IDs and embedded immutable population-template content.

**Tests**

- Resource-test JSON round trip for a serialized population-template frequency assertion.
- Materialized serialized statistical assertion execution with expected failure diagnostic and reproducibility packet metadata.
- Full build/test verification through `scripts/verify.sh`.

**Implemented**

- `ResourceTestStepSpecification.AssertPopulationTemplateFrequencyKind`.
- `PopulationTemplateFrequencyAssertionSpecification`.
- Resource-test JSON envelope support for embedded population-template frequency assertion payloads.
- Materialization of serialized statistical assertions into `AssertPopulationTemplateFrequencyStep`.
- Existing resource-test binary codec compatibility via the JSON payload.

**Implementation simplification choices**

- Population templates are embedded directly in the resource-test step payload instead of referenced from a resource pack or fixture registry.
- Only the Slice 16 population-template allele-frequency assertion is serialized; broader statistical operation registries remain deferred.
- Optional limits serialize only the maximum sample count currently supported by `SimulationResourceLimits`.
- Result/failure-packet persistence remains runtime result state only in Slice 17; Slice 18 adds resource-test result codecs without adding persistence.

**Not yet implemented**

- External template fixture references, template-group statistical assertions, reproduction/transmission distribution assertions, mutation/trace statistical assertions, confidence/outlier statistical policies, resource-pack loading, or resource-pack failure-packet persistence.

**Requirements advanced:** REQ-RTEST, REQ-RANDOM, REQ-TEMPLATE, REQ-SERIAL.

### Slice 18 - Resource-test result and failure-packet codecs

**Status:** Verified on 2026-06-08 for the refined Slice 18 acceptance criteria. Resource-pack persistence and broader reporting remain **In progress**.

**Objective:** Serialize resource-test run outcomes, diagnostics, and reproducibility packets through deterministic core stream/buffer APIs without assigning filesystem or resource-pack ownership to core.

**Deliverables**

- Define a versioned deterministic JSON envelope for resource-test run results.
- Preserve run/case status, stable test IDs, ordered tags, ordered diagnostics, and embedded reproducibility packets.
- Validate that serialized aggregate run status matches the status derived from serialized case results.
- Add a preliminary binary result codec using the shared binary envelope and canonical result JSON payload.
- Expose stream and buffer operations without filesystem access.

**Acceptance criteria**

- JSON round trips preserve passed and failed cases, diagnostics, tags, and reproducibility packets.
- Canonical JSON output is deterministic before and after round trip.
- Binary round trips produce the same canonical JSON result representation.
- Unknown JSON envelope versions, inconsistent aggregate status, and truncated binary payloads are rejected.
- Core result codecs perform no filesystem access.

**Tests**

- Mixed passing/failing resource-test result JSON round trip.
- Statistical failure diagnostic and reproducibility packet preservation.
- Preliminary binary result round trip.
- Unknown JSON envelope version, inconsistent status, and truncated binary rejection.
- Full build/test verification through `scripts/verify.sh`.

**Implemented**

- `ResourceTestResultJsonCodec`.
- `ResourceTestResultBinaryCodec`.
- Deterministic case, tag, diagnostic, and packet ordering in serialized results.
- Aggregate status consistency validation during JSON reads.

**Implementation simplification choices**

- Binary result serialization wraps canonical JSON in the existing preliminary binary envelope.
- Reproducibility packets are embedded in each case result rather than referenced from external files.
- Result codecs serialize current result fields only; timestamps, durations, allocation metrics, host metadata, and attachments are not introduced.
- Persistence location, retention, naming, and resource-pack manifest integration remain outside core.

**Not yet implemented**

- Resource-pack result persistence, result manifests, external packet files, result history/retention, timestamps/durations, richer execution metrics, report rendering, compact binary result schemas, or storage-backed result adapters.

**Requirements advanced:** REQ-RTEST, REQ-RANDOM, REQ-SERIAL, REQ-STORAGE.

### Slice 19 - Population template-group codecs

**Status:** Verified on 2026-06-09 for the refined Slice 19 acceptance criteria. Broader template-group authoring, reference registries, and statistical reports remain **In progress**.

**Objective:** Serialize immutable population template-group versions, including direct templates, nested child groups, weights, and cross-template blend policy, through deterministic core stream/buffer APIs.

**Deliverables**

- Define a versioned deterministic JSON envelope for `PopulationTemplateGroupVersion`.
- Preserve group ID, group-version ID, system-definition version, change summary, direct weighted templates, weighted child groups, and cross-template blend policy.
- Embed current `PopulationTemplateVersion` and child `PopulationTemplateGroupVersion` content directly, matching the Slice 11 embedded-version model.
- Reject incompatible system-definition versions during deserialization.
- Add a preliminary binary template-group codec using the shared binary envelope and canonical JSON payload.

**Acceptance criteria**

- JSON round trips nested template groups deterministically.
- Binary round trips produce the same canonical JSON group representation.
- Round-tripped template groups produce deterministic generated samples equivalent to the source group for identical inputs.
- Cross-template blend policy and child-group weights survive round trip.
- Unknown JSON envelope versions, incompatible system-definition versions, and truncated binary payloads are rejected.

**Tests**

- Nested template-group JSON round trip with embedded direct templates and child groups.
- Preliminary binary template-group round trip.
- Deterministic sample equality after JSON round trip.
- Cross-template blend policy preservation.
- Unknown envelope, incompatible system version, and truncated binary rejection.
- Full build/test verification through `scripts/verify.sh`.

**Implemented**

- `PopulationTemplateGroupJsonCodec`.
- `PopulationTemplateGroupBinaryCodec`.
- Deterministic ordering for embedded direct templates, child groups, template groups, genes, and allele frequencies.
- System-definition version compatibility validation for the root group and embedded templates/groups.

**Implementation simplification choices**

- Direct templates and child groups are embedded in the serialized payload instead of referenced from a resource registry.
- Binary serialization wraps canonical JSON in the existing preliminary binary envelope.
- The codec preserves current template-group model fields only; no external fixture references, manifests, or statistical reports are introduced.

**Not yet implemented**

- External template/template-group reference registries, resource-pack manifests, resource-authored template-group validation, pair-specific blend matrices, structure-level statistical reports, compact binary template-group schemas, or storage-backed template-group adapters.

**Requirements advanced:** REQ-TGROUP, REQ-TEMPLATE, REQ-SERIAL, REQ-RANDOM.

### Slice 20 - Godot adapter coverage for template groups and test results

**Status:** Verified on 2026-06-09 for the refined Slice 20 acceptance criteria. GodotSharp resource classes, editor tooling, and persistence/export packaging remain **In progress**.

**Objective:** Expose the Slice 18 and Slice 19 core codecs through the existing package-free Godot-facing resource document bridge.

**Deliverables**

- Add Godot resource kind constants for population template groups and resource-test run results.
- Export/import `PopulationTemplateGroupVersion` documents using the core template-group JSON codec and system-definition version metadata.
- Export/import `ResourceTestRunResult` documents using the core result JSON codec.
- Preserve resource-test result tag metadata and reproducibility-packet system-version metadata on Godot resource documents.
- Keep adapter diagnostics consistent for kind mismatches and import failures.

**Acceptance criteria**

- Population template-group Godot documents round trip through the adapter.
- Resource-test result Godot documents round trip through the adapter.
- Template-group documents expose the expected system-definition version metadata.
- Resource-test result documents expose sorted tag metadata and packet-derived system-definition version metadata.
- Importing a document through the wrong Godot resource kind returns a stable adapter diagnostic.

**Tests**

- Godot adapter template-group export/import round trip.
- Godot adapter resource-test result export/import round trip.
- Resource package lookup for the new document kinds.
- Kind-mismatch diagnostic for importing a resource-test result as a template group.
- Full build/test verification through `scripts/verify.sh`.

**Implemented**

- `GodotResourceKind.PopulationTemplateGroup`.
- `GodotResourceKind.ResourceTestResult`.
- `GodotResourceBridge.ExportPopulationTemplateGroup` and `ImportPopulationTemplateGroup`.
- `GodotResourceBridge.ExportResourceTestResult` and `ImportResourceTestResult`.
- Godot adapter tests for template-group and result documents.

**Implementation simplification choices**

- The adapter continues to use package-free `GodotResourceDocument` DTOs, not GodotSharp `Resource` subclasses.
- Import/export uses JSON payloads only; binary Godot document import/export remains deferred.
- Resource-test result document system-version metadata is derived from embedded reproducibility packets, so passing results without packets may have empty version metadata.
- No Godot editor plugin, addon layout, save path convention, or export packaging behavior is added.

**Not yet implemented**

- GodotSharp `Resource` subclasses for template groups/results, editor inspectors/importers, binary Godot resource import/export, addon layout, `.tres`/`.res` export, runtime node helpers, save/package persistence, and Godot engine-version matrices.

**Requirements advanced:** REQ-GODOT, REQ-TGROUP, REQ-RTEST, REQ-SERIAL.

### Slice 21 - Runtime body-plan variant binary codec

**Status:** Verified on 2026-06-09 for the refined Slice 21 acceptance criteria. Compact binary schemas and genome-version variant persistence remain **In progress**.

**Objective:** Add preliminary binary stream/buffer serialization for runtime body-plan variants using the established shared binary envelope pattern.

**Deliverables**

- Add a binary codec for `RuntimeBodyPlanVariant`.
- Preserve runtime variant ID, system-definition version, base body-plan ID, required/optional/shared group IDs, and change summary.
- Reuse the existing deterministic runtime-variant JSON payload inside the shared binary envelope.
- Reject truncated/invalid binary envelopes.
- Preserve system-definition version compatibility checks through binary reads.

**Acceptance criteria**

- Binary round trips preserve runtime body-plan variant value equality.
- Binary round trips produce the same canonical JSON representation as the source variant.
- Truncated binary payloads are rejected.
- Binary reads reject incompatible expected system-definition versions.

**Tests**

- Runtime body-plan variant binary round trip.
- Canonical JSON equality after binary round trip.
- Truncated binary rejection.
- Incompatible expected system-definition version rejection.
- Full build/test verification through `scripts/verify.sh`.

**Implemented**

- `RuntimeBodyPlanVariantBinaryCodec`.
- Preliminary binary framing for runtime body-plan variants using the shared binary envelope.
- Implementation-test coverage for binary round trip and validation failures.

**Implementation simplification choices**

- Binary serialization wraps the deterministic Slice 8 JSON payload rather than defining a compact variant-specific binary schema.
- Runtime variants remain standalone runtime state; persistence inside genome versions remains deferred.
- The codec does not introduce variant migration or resource-pack reference behavior.

**Not yet implemented**

- Compact/final binary variant schema, runtime variant persistence in genome versions, variant migration rules, resource-pack variant references, or Godot binary variant import/export.

**Requirements advanced:** REQ-VARIANT, REQ-BODY, REQ-SERIAL.

### Slice 22 - Standalone mosaic genome codecs

**Status:** Verified on 2026-06-09 for the refined Slice 22 acceptance criteria. Genome-version embedding, regional geometry, and richer chimeric workflows remain **In progress**.

**Objective:** Serialize the current ID-based mosaic/chimeric genome state as a standalone runtime state resource using deterministic core stream/buffer APIs.

**Deliverables**

- Add a deterministic JSON codec for `MosaicGenomeState`.
- Preserve primary genome, regional genome assignments, coverage values, chimeric material IDs, chimeric genomes, expressed region IDs, and integrated-variant flags.
- Reuse existing genome-version JSON payloads for embedded primary/regional/chimeric genome versions.
- Reject incompatible system-definition versions from any embedded genome.
- Add a preliminary binary mosaic codec using the shared binary envelope and canonical JSON payload.

**Acceptance criteria**

- JSON round trips preserve primary, regional, and chimeric genome state deterministically.
- Binary round trips produce the same canonical JSON mosaic representation.
- Region resolution after deserialization returns regional genomes and primary fallback correctly.
- Chimeric material IDs, expressed regions, and integration flags survive round trip.
- Unknown JSON envelope versions, incompatible embedded genome versions, and truncated binary payloads are rejected.

**Tests**

- Mosaic genome JSON and binary round trips.
- Regional assignment and fallback resolution after deserialization.
- Chimeric material state preservation.
- Unknown envelope, incompatible system-version, and truncated binary rejection.
- Full build/test verification through `scripts/verify.sh`.

**Implemented**

- `MosaicGenomeJsonCodec`.
- `MosaicGenomeBinaryCodec`.
- Deterministic ordering for regions, chimeric materials, and chimeric expressed-region IDs.
- System-definition version compatibility validation through embedded genome codec reads.

**Implementation simplification choices**

- Mosaic state is serialized as a standalone resource, not embedded into `GenomeVersion`.
- Embedded genome versions are stored as deterministic genome JSON strings inside the mosaic envelope.
- Binary serialization wraps canonical mosaic JSON in the existing preliminary binary envelope.
- The codec preserves current ID-based regions only; no geometry, overlap model, or chimeric expression behavior is introduced.

**Not yet implemented**

- Genome-version-embedded mosaic persistence, regional geometry, overlapping/multi-region expression, automatic chimeric expression integration, absorbed-twin inheritance workflows, mutation/repair over regions, reproduction workflows from inheritance sites, resource-pack references, compact binary mosaic schemas, or storage-backed mosaic adapters.

**Requirements advanced:** REQ-MOSAIC, REQ-SERIAL, REQ-GENOME.

### Slice 23 - Godot adapter coverage for mosaic genome state

**Status:** Verified on 2026-06-09 for the refined Slice 23 acceptance criteria. GodotSharp resources, binary Godot import/export, and storage-backed mosaic adapters remain **In progress**.

**Objective:** Expose the Slice 22 standalone mosaic genome codec through the existing package-free Godot-facing resource document bridge.

**Deliverables**

- Add a Godot resource kind constant for standalone mosaic genome state.
- Export/import `MosaicGenomeState` documents using the core mosaic JSON codec.
- Preserve system-definition version metadata from the primary genome version.
- Keep adapter diagnostics consistent for kind mismatches and import failures.
- Support mosaic genome documents in existing `GodotResourcePackage` lookup behavior.

**Acceptance criteria**

- Mosaic genome Godot documents round trip through the adapter.
- Mosaic genome documents expose the expected system-definition version metadata.
- Imported mosaic documents preserve canonical mosaic JSON representation.
- Mosaic genome documents can be included in package-free Godot resource packages.
- Importing a mosaic document through the wrong Godot resource kind returns a stable adapter diagnostic.

**Tests**

- Godot adapter mosaic genome export/import round trip.
- Godot mosaic document metadata verification.
- Godot resource package lookup for mosaic documents.
- Kind-mismatch diagnostic for importing a mosaic genome as a genome version.
- Full build/test verification through `scripts/verify.sh`.

**Implemented**

- `GodotResourceKind.MosaicGenome`.
- `GodotResourceBridge.ExportMosaicGenome` and `ImportMosaicGenome`.
- Godot adapter tests for mosaic genome documents.

**Implementation simplification choices**

- The adapter continues to use package-free `GodotResourceDocument` DTOs, not GodotSharp `Resource` subclasses.
- Import/export uses JSON payloads only; binary Godot document import/export remains deferred.
- Mosaic document system-version metadata is derived from the primary genome version.
- No Godot editor plugin, addon layout, save path convention, export packaging behavior, or richer mosaic runtime behavior is added.

**Not yet implemented**

- GodotSharp `Resource` subclasses for mosaic state, editor inspectors/importers, binary Godot resource import/export, addon layout, `.tres`/`.res` export, runtime node helpers, save/package persistence, storage-backed mosaic adapters, and Godot engine-version matrices.

**Requirements advanced:** REQ-GODOT, REQ-MOSAIC, REQ-SERIAL.

### Slice 24 - Resource-test diagnostic severity filtering

**Status:** Verified on 2026-06-10 for the refined Slice 24 acceptance criteria. Runtime-safe subsets, resource-pack loading, and richer report generation remain **In progress**.

**Objective:** Let resource-test callers filter reported diagnostics by severity without changing which tests run or whether a case/run passes or fails.

**Deliverables**

- Extend `ResourceTestRunOptions` with an optional maximum diagnostic severity.
- Filter diagnostics included in `ResourceTestCaseResult` when the option is set.
- Keep tag include/exclude filtering behavior unchanged.
- Preserve case/run pass-fail status based on all produced diagnostics, including diagnostics filtered out of the report.
- Preserve deterministic diagnostic ordering through existing result construction.

**Acceptance criteria**

- Error-only filtering reports only error diagnostics.
- Warning filtering reports warning and error diagnostics, but omits info diagnostics.
- Unfiltered runs report all diagnostics.
- A failed case remains failed even when lower-priority diagnostics are filtered out.
- Existing tag filtering behavior remains compatible.

**Tests**

- Resource-test runner severity filtering with error, warning, and info diagnostics.
- Failure status preservation under diagnostic filtering.
- Existing tag-filtering regression coverage remains passing.
- Full build/test verification through `scripts/verify.sh`.

**Implemented**

- `ResourceTestRunOptions.MaximumDiagnosticSeverity`.
- `ResourceTestRunOptions.FilterDiagnostics`.
- Resource-test runner application of diagnostic filtering at result construction time.
- Implementation-test helper for deterministic diagnostic emission.

**Implementation simplification choices**

- Severity filtering is report-focused and does not alter step execution, diagnostic generation, or pass/fail status calculation.
- Reproducibility packets are not filtered by diagnostic severity.
- The filter is a maximum enum threshold using current severity ordering: `Error`, then `Warning`, then `Info`.

**Not yet implemented**

- Runtime-safe resource-test subsets, serialized run-option resources, report rendering, severity-based test selection, resource-pack loading, snapshots, fuzz/matrix execution, or storage-backed result persistence.

**Requirements advanced:** REQ-RTEST, REQ-VALIDATE.

### Slice 25 - Resource-test human-readable text reports

**Status:** Verified on 2026-06-11 for the refined Slice 25 acceptance criteria. CLI execution, editor integration, report persistence, and resource-pack loading remain **In progress**.

**Objective:** Let callers export a deterministic human-readable report from a `ResourceTestRunResult` without changing test execution, result serialization, or storage boundaries.

**Deliverables**

- Add a text report formatter for resource-test run results.
- Include aggregate run status, case counts, diagnostic counts by severity, and reproducibility packet count.
- Include deterministic per-case report sections ordered by existing result ordering.
- Include tags, diagnostics, and reproducibility packet references for each case.
- Preserve the existing JSON result codec as the machine-readable report format.

**Acceptance criteria**

- Failed and passed case counts are summarized.
- Error, warning, and info diagnostic counts are summarized.
- Diagnostic details include severity, code, path, and message.
- Cases with no diagnostics or packets explicitly report `none`.
- Reproducibility packet report entries include operation path, seed, resource-set version, and failed assertion.
- Case ordering is deterministic by resource-test ID.

**Tests**

- Resource-test text report summary over mixed passing/failing cases.
- Diagnostic detail rendering for multiple severities.
- Empty diagnostic and reproducibility packet section rendering.
- Reproducibility packet reference rendering.
- Full build/test verification through `scripts/verify.sh`.

**Implemented**

- `ResourceTestTextReportFormatter.WriteToText`.
- Aggregate report counts for cases, diagnostics, and reproducibility packets.
- Per-case text sections with tags, diagnostics, and reproducibility packet references.

**Implementation simplification choices**

- Text reports are generated from already-materialized `ResourceTestRunResult` instances.
- Machine-readable reporting continues to use `ResourceTestResultJsonCodec`; Slice 25 does not introduce another structured report envelope.
- The formatter does not write files, choose output paths, run tests, or manage process exit codes.

**Not yet implemented**

- CLI/batch runner, editor integration, report persistence, custom report templates, resource-pack loading, changed-resource test selection, or machine-readable formats beyond the existing JSON result codec.

**Requirements advanced:** REQ-RTEST, REQ-RANDOM, REQ-VALIDATE.

### Slice 26 - JSON-file storage for resource-test results

**Status:** Verified on 2026-06-11 for the refined Slice 26 acceptance criteria. Result manifests, retention policy, resource-pack integration, and CLI/editor output routing remain **In progress**.

**Objective:** Let optional JSON-file storage callers persist and reload `ResourceTestRunResult` values through the existing core result JSON codec without giving `Genomancy.Core` filesystem ownership.

**Deliverables**

- Add a typed JSON-file storage factory for resource-test run results in `Genomancy.Storage.JsonFile`.
- Reuse `ResourceTestResultJsonCodec` for the on-disk JSON payload.
- Preserve diagnostics, tags, case status, and reproducibility packets across save/load.
- Keep atomic file-write behavior centralized in the existing generic `JsonFileStore<T>`.
- Keep core free of filesystem, path, and storage-provider dependencies.

**Acceptance criteria**

- A failed resource-test run result can be saved and loaded through the optional JSON-file storage module.
- Loaded results retain failed status, diagnostics, and reproducibility packet seed data.
- Canonical core result JSON is unchanged by the storage round trip.
- The requested JSON file is created by the storage adapter.
- No new storage dependency is added to `Genomancy.Core`.

**Tests**

- JSON-file resource-test result persistence with a statistical failure result.
- Diagnostics preservation across the file boundary.
- Reproducibility packet preservation across the file boundary.
- Canonical JSON equality after save/load.
- Full build/test verification through `scripts/verify.sh`.

**Implemented**

- `ResourceTestResultJsonFileStore.Create`.
- Typed composition of `JsonFileStore<ResourceTestRunResult>` with `ResourceTestResultJsonCodec`.
- Architecture documentation for selected typed JSON-file storage factories.

**Implementation simplification choices**

- The typed factory does not define filenames, directories, manifests, retention rules, timestamps, or result history.
- Persistence remains optional and outside core.
- The file payload remains the existing deterministic resource-test result JSON envelope.

**Not yet implemented**

- Resource-pack result manifests, report/result retention policy, output path conventions, CLI/editor output routing, concurrent-writer coordination, or binary/SQLite result stores.

**Requirements advanced:** REQ-RTEST, REQ-STORAGE, REQ-SERIAL, REQ-RANDOM.

### Slice 27 - Resource-test run summaries

**Status:** Verified on 2026-06-11 for the refined Slice 27 acceptance criteria. Serialized manifests, CLI status files, and retention policy remain **In progress**.

**Objective:** Provide a reusable deterministic summary over `ResourceTestRunResult` so reports, future manifests, and future batch/CLI status surfaces share the same derived counts.

**Deliverables**

- Add an immutable resource-test run summary model.
- Derive run status, passed/failed case counts, diagnostic counts by severity, and reproducibility packet count from an existing run result.
- Keep summaries deterministic and independent of filesystem, timestamps, durations, or output routing.
- Refactor the human-readable text report formatter to use the shared summary.

**Acceptance criteria**

- Empty run results summarize as passed with zero cases, diagnostics, and packets.
- Mixed pass/fail results summarize total, passed, and failed case counts.
- Error, warning, and info diagnostic counts are derived independently.
- Reproducibility packet count is derived from all cases.
- Existing text report output remains compatible after using the shared summary.

**Tests**

- Resource-test run summary over an empty result.
- Resource-test run summary over mixed pass/fail cases.
- Diagnostic severity count coverage for error, warning, and info diagnostics.
- Reproducibility packet count coverage.
- Existing text-report regression coverage remains passing.
- Full build/test verification through `scripts/verify.sh`.

**Implemented**

- `ResourceTestRunSummary`.
- `ResourceTestRunSummary.FromResult`.
- Text-report formatter usage of `ResourceTestRunSummary`.

**Implementation simplification choices**

- Summaries are derived in memory from `ResourceTestRunResult`; Slice 27 does not add a serialized summary envelope.
- Summaries do not include timestamps, durations, file paths, command exit-code policy, or resource-pack metadata.
- The summary type is a reusable core value, not a storage manifest.

**Not yet implemented**

- Serialized result manifests, CLI status output, report indexes, duration/timing metrics, retention policy, or storage-backed summary catalogs.

**Requirements advanced:** REQ-RTEST, REQ-VALIDATE, REQ-RANDOM.

### Slice 28 - Resource-test result manifests

**Status:** Verified on 2026-06-12 for the refined Slice 28 acceptance criteria. CLI/batch execution, project file layout, manifest merge/update policy, and retention policy remain **In progress**.

**Objective:** Provide a deterministic manifest/catalog model for resource-test result artifacts so future batch, CLI, and editor surfaces can index stored results without inventing their own summary format.

**Deliverables**

- Add immutable resource-test result manifest entries with a stable run ID, stored result path, optional label, optional completion timestamp, deterministic tags, and an embedded `ResourceTestRunSummary`.
- Add an immutable resource-test result manifest collection with deterministic entry ordering and duplicate run-ID rejection.
- Add deterministic JSON stream/text/buffer codecs for manifests.
- Add a typed optional JSON-file storage factory for manifests in `Genomancy.Storage.JsonFile`.
- Keep manifest paths caller-authored strings; do not define repository layout, retention, naming policy, or result execution.

**Acceptance criteria**

- Manifest entries can be derived from existing `ResourceTestRunResult` values and preserve the shared Slice 27 summary counts.
- Manifest entries are sorted deterministically by run ID and result path.
- Duplicate run IDs are rejected.
- Tags are normalized, de-duplicated, and sorted deterministically.
- JSON round trips are canonical and reject unsupported envelope versions and invalid completion timestamps.
- The optional JSON-file storage module can persist and reload manifests using the core codec.

**Tests**

- Manifest summary derivation, deterministic entry ordering, tag normalization, UTC timestamp normalization, and duplicate run-ID rejection.
- Manifest JSON canonical round trip and invalid JSON/envelope validation.
- Manifest JSON-file storage round trip through `ResourceTestResultManifestJsonFileStore`.
- Full build/test verification through `scripts/verify.sh`.

**Implemented**

- `ResourceTestResultManifestEntry`.
- `ResourceTestResultManifest`.
- `ResourceTestResultManifestJsonCodec`.
- `ResourceTestResultManifestJsonFileStore.Create`.
- Package-free implementation tests for manifest model, codec, and storage behavior.

**Implementation simplification choices**

- Manifest result paths are opaque caller-supplied strings; the core does not interpret relative paths, URIs, or filesystem roots.
- Completion timestamps are optional and caller-supplied; the core does not read a clock or measure durations.
- Manifests embed derived summaries only; they do not duplicate full diagnostics or reproducibility packets from result files.
- The storage factory persists one manifest JSON file only and does not add retention, merge, locking, or catalog-update policy.

**Not yet implemented**

- CLI/batch execution, editor integration, project file layout, result retention/naming policy, manifest merge/update workflows, duration metrics, external packet files, binary manifest codecs, SQLite/custom-binary manifest storage, or resource-pack result loading.

**Requirements advanced:** REQ-RTEST, REQ-STORAGE, REQ-SERIAL, REQ-RANDOM.

### Slice 29 - Godot bridge for resource-test result manifests

**Status:** Verified on 2026-06-12 for the refined Slice 29 acceptance criteria. GodotSharp resources, editor integration, binary import/export, project file layout, manifest merge/update policy, and retention policy remain **In progress**.

**Objective:** Expose Slice 28 resource-test result manifests through the existing package-free Godot adapter document bridge so Godot-facing callers can import/export manifest JSON without duplicating core serialization logic.

**Deliverables**

- Add a Godot resource kind for resource-test result manifests.
- Add package-free `GodotResourceBridge` export/import methods for `ResourceTestResultManifest`.
- Preserve manifest JSON by delegating to the core manifest codec.
- Derive Godot document tags from manifest entry tags in deterministic sorted order.
- Leave `SystemDefinitionVersion` metadata empty because manifest entries currently embed summaries and opaque result paths, not result payloads or packet-level system versions.
- Support ordinary `GodotResourcePackage` lookup for manifest documents through the existing document model.

**Acceptance criteria**

- Exported manifest documents use the manifest resource kind and canonical core manifest JSON payload.
- Imported manifest documents round trip to the same canonical core manifest JSON.
- Manifest document tags are de-duplicated and sorted.
- Manifest document system-definition metadata is empty by design.
- Importing a manifest document through a different resource-kind importer reports a kind-mismatch diagnostic.
- Existing Godot adapter document/package behavior remains compatible.

**Tests**

- Godot adapter manifest document export/import round trip.
- Manifest document tag metadata sorting/de-duplication.
- Manifest document package lookup.
- Manifest-as-result kind-mismatch diagnostic.
- Full build/test verification through `scripts/verify.sh`.

**Implemented**

- `GodotResourceKind.ResourceTestResultManifest`.
- `GodotResourceBridge.ExportResourceTestResultManifest`.
- `GodotResourceBridge.ImportResourceTestResultManifest`.
- Package-free implementation tests for manifest document bridge behavior.

**Implementation simplification choices**

- Manifest bridge support remains DTO/document-only and does not introduce GodotSharp `Resource` classes.
- The bridge does not dereference manifest result paths or inspect linked result files.
- The bridge does not infer system-definition versions from manifests because Slice 28 manifests do not contain packet/result payloads.
- Binary Godot import/export and editor/runtime plugin integration remain deferred.

**Not yet implemented**

- GodotSharp manifest resources, editor inspectors/importers/exporters, resource-pack manifest loading, binary manifest resources, result-file dereferencing, retention/naming policy, manifest merge/update workflows, CLI/batch execution, or project file layout.

**Requirements advanced:** REQ-GODOT, REQ-RTEST, REQ-SERIAL.

### Slice 30 - Resource-test batch runs with manifest generation

**Status:** Verified on 2026-06-12 for the refined Slice 30 acceptance criteria. CLI commands, filesystem output layout, result persistence, manifest merge/update policy, and retention policy remain **In progress**.

**Objective:** Add a core batch-run orchestration surface that can execute multiple named resource-test runs and produce a deterministic manifest from their results, giving future CLI/editor tooling a shared engine-neutral primitive.

**Deliverables**

- Add immutable batch run requests with run ID, caller-supplied result path, definitions, per-run options, optional label, optional completion timestamp, and caller tags.
- Add immutable batch run records containing the run result and generated manifest entry.
- Add immutable batch run results containing all run records, aggregate status, and generated result manifest.
- Add a deterministic batch runner that orders runs by run ID/result path, applies each request's `ResourceTestRunOptions`, executes through the existing `ResourceTestRunner`, and builds manifest entries from the results.
- Reject duplicate batch run IDs before execution.
- Merge caller tags with executed case tags for each generated manifest entry.
- Keep result paths opaque and do not write files.

**Acceptance criteria**

- Multiple named batch runs execute deterministically and expose their individual `ResourceTestRunResult` values.
- Aggregate batch status is failed when any contained run fails and passed otherwise.
- Generated manifests preserve run IDs, caller result paths, labels/timestamps, derived summaries, and merged sorted tags.
- Per-run tag filters affect which cases execute and which case tags appear in the generated manifest.
- Duplicate run IDs are rejected.
- No filesystem writes, CLI behavior, or storage policy is introduced in core.

**Tests**

- Batch execution over passing and failing named runs with deterministic run ordering and manifest generation.
- Aggregate failed batch status when a contained run fails.
- Manifest entry summary and tag generation from batch results.
- Per-run include-tag option filtering.
- Duplicate batch run-ID rejection.
- Full build/test verification through `scripts/verify.sh`.

**Implemented**

- `ResourceTestBatchRunRequest`.
- `ResourceTestBatchRunRecord`.
- `ResourceTestBatchRunResult`.
- `ResourceTestBatchRunner`.
- Package-free implementation tests for batch execution, manifest generation, per-run options, and duplicate run-ID validation.

**Implementation simplification choices**

- Batch run requests carry caller-supplied result paths but do not write result files.
- Completion timestamps are optional caller inputs; the batch runner does not read the clock or measure durations.
- Batch execution is sequential and deterministic.
- The batch runner consumes in-memory `ResourceTestDefinition` instances only; it does not load serialized specs or resource packs.
- Manifest generation embeds summaries only and does not persist or merge manifests.

**Not yet implemented**

- CLI/batch command-line host, serialized batch-run plan codec, resource-pack loading, filesystem output layout, result-file writes, manifest merge/update workflows, retention/naming policy, duration metrics, parallel execution, or editor integration.

**Requirements advanced:** REQ-RTEST, REQ-VALIDATE, REQ-RANDOM.

### Slice 31 - Serialized resource-test batch-run plans

**Status:** Verified on 2026-06-12 for the refined Slice 31 acceptance criteria. CLI commands, filesystem output layout, binary plan codecs, resource-pack loading, result persistence, manifest merge/update policy, and retention policy remain **In progress**.

**Objective:** Make Slice 30 batch runs designer/tool-authored by adding deterministic JSON batch-run plan resources that embed existing serialized resource-test specifications and materialize into executable batch requests.

**Deliverables**

- Add immutable resource-test batch-run specifications with run ID, result path, serialized-resource-test specifications, per-run options, optional label, optional completion timestamp, and tags.
- Add materialization from batch-run specifications to Slice 30 batch-run requests.
- Add deterministic JSON stream/text/buffer codecs for batch-run specifications.
- Embed each run's resource tests as structured `ResourceTestJsonCodec` JSON so fixture/step serialization remains owned by the existing resource-test codec.
- Serialize and deserialize per-run include tags, exclude tags, and maximum diagnostic severity.
- Add convenience execution of batch-run specifications through the existing batch runner.

**Acceptance criteria**

- Batch-run JSON round trips canonically.
- Deserialized batch-run specifications materialize into executable batch requests.
- Nested resource-test specifications preserve existing fixture/step behavior through `ResourceTestJsonCodec`.
- Per-run options, result paths, labels, tags, and UTC-normalized completion timestamps round trip.
- Unsupported batch envelope versions, invalid completion timestamps, invalid diagnostic severities, and missing nested resource-test payloads are rejected.
- No CLI behavior, filesystem writes, or storage policy is introduced.

**Tests**

- Batch-run JSON round trip with multiple runs and nested resource-test specifications.
- Materialized execution through `ResourceTestBatchRunner.RunSpecifications`.
- Per-run include-tag option preservation and filtered execution.
- UTC timestamp normalization.
- Rejection of unsupported envelope versions, invalid timestamps, invalid diagnostic severities, and missing nested resource-test payloads.
- Full build/test verification through `scripts/verify.sh`.

**Implemented**

- `ResourceTestBatchRunSpecification`.
- `ResourceTestBatchRunJsonCodec`.
- `ResourceTestBatchRunner.RunSpecifications`.
- Package-free implementation tests for serialized batch plans and materialized execution.

**Implementation simplification choices**

- Batch-run plans embed resource-test JSON directly instead of introducing external resource-pack references.
- The codec supports JSON only; preliminary binary wrapping is deferred.
- Completion timestamps remain caller-authored metadata and are not measured by the runner.
- Plans remain in-memory stream/buffer/text resources; no filesystem layout or save/load factory is added in this slice.
- The batch-runner remains sequential and deterministic.

**Not yet implemented**

- CLI/batch command-line host, binary batch-plan codec, JSON-file storage factory for plans, resource-pack references/loading, result-file writes, output directory conventions, manifest merge/update workflows, retention/naming policy, duration metrics, parallel execution, or editor integration.

**Requirements advanced:** REQ-RTEST, REQ-SERIAL, REQ-VALIDATE, REQ-RANDOM.

### Slice 32 - Batch-plan binary codec and JSON-file storage

**Status:** Verified on 2026-06-15 for the refined Slice 32 acceptance criteria. CLI commands, filesystem output layout, result persistence, manifest merge/update policy, and retention policy remain **In progress**.

**Objective:** Make serialized resource-test batch-run plans portable through the same preliminary binary-envelope and optional JSON-file storage boundaries already used by earlier resource-test resources.

**Deliverables**

- Add a preliminary binary codec for `ResourceTestBatchRunSpecification` collections.
- Reuse the canonical Slice 31 batch-plan JSON payload inside the binary envelope.
- Validate binary magic/header and truncated payload failures.
- Add an optional typed JSON-file storage factory for batch-run plans in `Genomancy.Storage.JsonFile`.
- Keep storage path selection caller-owned and avoid defining project layout, CLI behavior, result writes, or retention.

**Acceptance criteria**

- Batch-plan binary round trips preserve canonical JSON equality.
- Binary payloads materialize into executable batch specifications.
- Truncated and wrong-kind binary envelopes are rejected.
- Batch-plan JSON-file storage can save/load plans and the loaded plan can execute through the batch runner.
- Core still has no filesystem-storage dependency.

**Tests**

- Batch-plan binary round trip and materialized execution.
- Truncated binary rejection.
- Wrong binary magic/header rejection using another resource-test binary payload.
- JSON-file storage round trip for batch-run plans.
- Loaded batch plans execute through `ResourceTestBatchRunner.RunSpecifications`.
- Full build/test verification through `scripts/verify.sh`.

**Implemented**

- `ResourceTestBatchRunBinaryCodec`.
- `ResourceTestBatchRunJsonFileStore.Create`.
- Package-free implementation tests for batch-plan binary and JSON-file storage behavior.

**Implementation simplification choices**

- The binary format remains a preliminary JSON-wrapped binary envelope, consistent with earlier serialization slices.
- JSON-file storage persists a caller-selected plan file only; it does not define naming, directories, output routing, retention, or locking policy.
- Storage is optional and remains outside core.
- This slice does not persist run results or generated manifests.

**Not yet implemented**

- CLI/batch command-line host, project file layout, result-file writes, generated-manifest persistence, manifest merge/update workflows, retention/naming policy, compact binary batch-plan schema, resource-pack references/loading, duration metrics, parallel execution, or editor integration.

**Requirements advanced:** REQ-RTEST, REQ-SERIAL, REQ-STORAGE, REQ-VALIDATE.

### Slice 33 - Batch-plan Godot bridge and batch-run text reports

**Status:** Verified on 2026-06-15 for the refined Slice 33 acceptance criteria. CLI commands, filesystem output layout, result persistence, manifest merge/update policy, retention policy, and GodotSharp/editor integration remain **In progress**.

**Objective:** Extend the package-free tooling boundary around batch-run plans and results by making serialized batch plans available to Godot-facing callers and adding deterministic human-readable reporting for already-materialized batch-run results.

**Deliverables**

- Add a Godot-facing resource kind for serialized resource-test batch-run plans.
- Add package-free Godot bridge import/export methods for `ResourceTestBatchRunSpecification` collections.
- Preserve canonical Slice 31 batch-plan JSON through the Godot document payload.
- Derive Godot document metadata from embedded test system-definition versions and combined run/test tags.
- Add a deterministic batch-run text report formatter over `ResourceTestBatchRunResult`.
- Keep reporting in-memory only; do not define CLI output files, result writes, manifest persistence, or retention policy.

**Acceptance criteria**

- Godot batch-plan document round trips preserve canonical batch-plan JSON.
- Imported Godot batch-plan documents materialize into executable batch specifications.
- Godot batch-plan documents expose deterministic kind, system-definition-version metadata, and de-duplicated sorted tags.
- Kind mismatches return adapter diagnostics through the existing import-result surface.
- Batch-run text reports summarize aggregate status, runs, per-run summaries, tags, labels, reproducibility-packet counts, and manifest entries in deterministic order.
- Core remains independent of Godot and filesystem storage dependencies.

**Tests**

- Batch-run text report summary and manifest rendering test.
- Godot batch-plan document round trip and execution test.
- Godot batch-plan metadata/tag ordering test.
- Godot resource package lookup and kind-mismatch diagnostic test.
- Full build/test verification through `scripts/verify.sh`.

**Implemented**

- `ResourceTestBatchRunTextReportFormatter`.
- `GodotResourceKind.ResourceTestBatchRuns`.
- `GodotResourceBridge.ExportResourceTestBatchRuns`.
- `GodotResourceBridge.ImportResourceTestBatchRuns`.
- Package-free implementation tests for batch reports and Godot batch-plan bridge behavior.

**Implementation simplification choices**

- Godot batch-plan documents use the existing package-free `GodotResourceDocument` DTO and canonical JSON payload only.
- System-definition-version metadata is a comma-separated set derived from embedded resource-test specifications, matching existing multi-resource document behavior.
- Batch reports format already-materialized in-memory results only; they do not execute plans, write files, or dereference result paths.

**Not yet implemented**

- CLI/batch command-line host, project file layout, result-file writes, generated-manifest persistence, manifest merge/update workflows, retention/naming policy, compact binary batch-plan schema, resource-pack references/loading, duration metrics, parallel execution, GodotSharp resources, editor plugins, binary Godot import/export, or Godot-side result dereferencing.

**Requirements advanced:** REQ-RTEST, REQ-SERIAL, REQ-GODOT, REQ-VALIDATE.

### Later hardening and release work

- Performance profiling and bounded-allocation work for runtime hot paths.
- Backward/forward compatibility matrices and migration fixtures.
- Public API review, documentation, examples, and package publication.
- Security/resource-limit review for untrusted serialized resources and long-running simulations.
- Expanded resource packs and required baseline resource tests.

## Implementation accounting

### Implemented

- Initial persistent requirements/progress ledger.
- Requirements-family IDs and source-section traceability.
- Initial architecture constraints and incrementally refined implementation plan.
- Slice 0 project foundation:
  - `Genomancy.sln`
  - `src/Genomancy.Core`
  - `tests/Genomancy.Tests`
  - shared build configuration in `Directory.Build.props`
  - formatting defaults in `.editorconfig`
  - repo ignore rules in `.gitignore`
  - architecture note in `docs/architecture.md`
  - documented verification command in `README.md`
  - verification script in `scripts/verify.sh`
- Minimal engine-neutral core assembly marker.
- Package-free implementation smoke test harness with core dependency inspection.
- Repo-local ignored `.dotnet-work/` verification state for restricted Linux/Codex sandbox execution.
- Slice 1 definition kernel:
  - validated stable resource IDs
  - validated system-definition versions
  - fixed design/runtime system mode
  - mutable design definition builder
  - immutable frozen runtime definition snapshot
  - authored allele, gene, group, and body-plan definitions
  - policy reference identity and policy kind taxonomy
  - validation diagnostics and deterministic validation result ordering
  - duplicate-ID, missing-reference, and group-dependency-cycle validation
  - runtime startup with optional migration before freeze
  - typed exceptions for validation failure and post-freeze definition mutation
- Verification hardening for Codex Linux sandbox:
  - forced single-node/non-parallel MSBuild restore/build
  - switched test execution to `dotnet exec` on the built test DLL
- Slice 2 genome lifecycle and baseline serialization:
  - opaque external individual IDs
  - immutable genome versions and replacement-based current copies
  - ranked allele/group genome state with optional numeric allele values
  - deterministic JSON and preliminary binary codecs over streams/buffers/text
  - system-definition version compatibility hooks
- Slice 3 group completeness, body-plan activation, and basic expression:
  - required allele counts and initial expression strategy metadata
  - group completeness evaluation with structured diagnostics
  - body-plan availability and activation state separate from genome mutation
  - explicit body-plan/developmental-phase/group/external expression context
  - strict dominance, codominance, and numeric midpoint expression
- Slice 4 deterministic ordinary reproduction:
  - named deterministic random streams using FNV-1a derivation and SplitMix64 draws
  - parent roles, reproduction policies, transmission weights, requests, results, and diagnostics
  - ordinary offspring genome version creation from frozen definitions
  - equal and weighted allele transmission, explicit multi-parent contribution, and ambiguity rejection
  - sterile/incompatible compatibility-policy stub outcomes
- Slice 5 mutation, repair, and acquired heritable changes:
  - mutation targets, operations, policy, request/result, diagnostics, and source labels
  - allele replacement, numeric value update, copy-count adjustment, group add, and group remove
  - protected group/gene enforcement and allowed-allele validation
  - current-only mutation, committed mutation, base-version repair, and current-state reversion
  - external mutation source allow/deny boundary
- Slice 6 non-ploidal inheritance and traces:
  - typed non-ploidal object state and trace state
  - immutable genome-version integration for heritable object state
  - deterministic weighted non-ploidal object inheritance
  - trace activation, strength replacement, degradation, and reproduction inheritance
  - JSON and preliminary binary codec integration for non-ploidal objects and traces
- Slice 7 compatibility, development, and expanded reproduction:
  - compatibility evaluations/rules and hybrid morphology contribution metadata
  - inviable reproduction outcomes
  - clonal-copy reproduction mode
  - development stages, plans, gestation context, timelines, and maternal numeric effects
- Slice 8 advanced expression, generated complements, and runtime variants:
  - numeric sum and weighted-average expression
  - generated complement policy/result/service for missing group state
  - runtime body-plan variant IDs/state/evaluation
  - deterministic JSON codec for runtime body-plan variants
- Slice 9 mosaicism and chimerism:
  - mosaic region IDs, inheritance site IDs, and region assignments
  - mosaic genome state with primary and regional genome versions
  - region-scoped expression and inheritance-site source resolution
  - distinct chimeric material state
- Slice 10 population templates:
  - immutable population-template IDs, version IDs, and group/gene/allele frequency profiles
  - deterministic genome sampling and stable population generation
  - template blending and template-from-individual creation
  - deterministic JSON codec for population template versions
- Slice 11 template groups and nested populations:
  - immutable population-template-group IDs and version IDs
  - weighted direct templates and weighted child template groups
  - deterministic nested group sampling and stable generated population outputs
  - optional deterministic cross-template blending among direct templates
  - generated genome metadata preserving selected group path and source template IDs
- Slice 12 resource testing framework foundation:
  - stable resource-test IDs and in-memory resource-test definitions
  - deterministic resource-test runner with case/run results
  - validation/freeze steps and validation assertion steps
  - structured resource-test diagnostics
  - custom step extension point and fresh fixture isolation per run
- Slice 13 resource testing framework expansion:
  - typed serialized resource-test specifications for definition fixtures and supported steps
  - deterministic resource-test JSON codec over streams, buffers, and text
  - materialization from serialized specifications into executable resource-test definitions
  - tag include/exclude filtering for resource-test runs
- Slice 14 serialization hardening and optional JSON-file storage:
  - shared preliminary binary envelope framing
  - population-template and resource-test binary codecs
  - optional `Genomancy.Storage.JsonFile` assembly
  - generic atomic JSON-file save/load adapter
- Slice 15 Godot adapter and packaging:
  - optional `Genomancy.Godot` assembly
  - Godot-style resource paths, documents, and packages
  - import/export bridges for genomes, population templates, and resource tests
  - runtime startup diagnostics for Godot-facing callers
  - basic adapter package metadata
- Slice 16 statistical simulation and reproducibility hardening:
  - bounded deterministic population-template allele-frequency simulation
  - absolute statistical proportion tolerance evaluation
  - aggregate ranked-allele observation reporting
  - resource-test statistical assertion diagnostics
  - deterministic JSON reproducibility packets exposed on failed resource-test cases
- Slice 17 serialized statistical resource-test assertions:
  - serialized resource-test step kind for population-template frequency assertions
  - embedded population-template statistical assertion payloads in resource-test JSON
  - materialization of serialized statistical assertions into executable resource-test steps
  - resource-test binary compatibility through the existing JSON-wrapped binary codec
- Slice 18 resource-test result and failure-packet codecs:
  - deterministic JSON codec for resource-test run results
  - preliminary binary codec for resource-test run results
  - serialized diagnostics, tags, case statuses, and embedded reproducibility packets
  - aggregate result status consistency validation
- Slice 19 population template-group codecs:
  - deterministic JSON codec for nested population template-group versions
  - preliminary binary codec for population template-group versions
  - embedded direct template and child template-group serialization
  - cross-template blend policy and selection-weight preservation
- Slice 20 Godot adapter coverage for template groups and test results:
  - Godot-facing resource kinds for population template groups and resource-test results
  - package-free import/export bridge methods for population template groups
  - package-free import/export bridge methods for resource-test run results
  - adapter metadata for result tags and packet-derived system-definition versions
- Slice 21 runtime body-plan variant binary codec:
  - preliminary binary codec for runtime body-plan variants
  - binary round-trip preservation of variant IDs, body-plan references, group sets, and change summaries
  - binary envelope validation and system-definition version compatibility checks
- Slice 22 standalone mosaic genome codecs:
  - deterministic JSON codec for mosaic genome state
  - preliminary binary codec for mosaic genome state
  - standalone serialization of primary, regional, and chimeric genome versions
  - preservation of regional coverage and chimeric expressed-region metadata
- Slice 23 Godot adapter coverage for mosaic genome state:
  - Godot-facing resource kind for standalone mosaic genome state
  - package-free import/export bridge methods for mosaic genome state
  - adapter metadata derived from the primary genome system-definition version
- Slice 24 resource-test diagnostic severity filtering:
  - optional diagnostic severity threshold on resource-test run options
  - report-focused diagnostic filtering during result construction
  - pass/fail status preservation based on all generated diagnostics
- Slice 25 resource-test human-readable text reports:
  - deterministic text report formatter for run results
  - aggregate case, diagnostic, and reproducibility packet counts
  - per-case diagnostics and reproducibility packet references
- Slice 26 JSON-file storage for resource-test results:
  - optional typed JSON-file store factory for resource-test run results
  - result persistence through the existing deterministic result JSON codec
  - preservation of diagnostics and reproducibility packets across file save/load
- Slice 27 resource-test run summaries:
  - reusable derived run summary for status and counts
  - case, diagnostic severity, and reproducibility packet counts
  - text-report formatter backed by the shared summary surface
- Slice 28 resource-test result manifests:
  - immutable result manifest entries and manifest collection
  - deterministic result manifest JSON codec over streams, buffers, and text
  - embedded derived run summaries for stored result artifacts
  - optional typed JSON-file store factory for resource-test result manifests
- Slice 29 Godot bridge for resource-test result manifests:
  - Godot-facing resource kind for resource-test result manifests
  - package-free import/export bridge methods for result manifests
  - adapter metadata for manifest entry tags
  - package lookup coverage through the existing document/package model
- Slice 30 resource-test batch runs:
  - immutable batch run requests, records, and aggregate results
  - deterministic sequential batch execution through the existing resource-test runner
  - per-run resource-test options
  - generated result manifests from batch run outputs
  - duplicate run-ID rejection
- Slice 31 serialized resource-test batch-run plans:
  - immutable batch-run specifications over serialized resource-test specs
  - deterministic batch-run JSON codec over streams, buffers, and text
  - nested resource-test JSON payloads delegated to `ResourceTestJsonCodec`
  - per-run options, metadata, and materialization into executable batch requests
  - convenience batch execution from serialized specifications
- Slice 32 batch-plan binary codec and JSON-file storage:
  - preliminary binary codec for serialized resource-test batch-run plans
  - optional typed JSON-file store factory for batch-run plans
  - binary header/truncation validation
  - persisted batch-plan load and materialized execution coverage
- Slice 33 batch-plan Godot bridge and batch-run text reports:
  - package-free Godot resource kind and bridge methods for serialized batch-run plans
  - deterministic Godot document metadata from embedded system-definition versions and tags
  - deterministic text report formatter for already-materialized batch-run results
  - package lookup, kind mismatch, canonical JSON preservation, and materialized execution coverage

### Not yet implemented

- Nonstandard reproduction beyond clonal copy, full compatibility, gestational simulation, and advanced mosaic/chimera behavior.
- Regional geometry, genome-version-embedded mosaic/chimera persistence, overlapping mosaic expression, chimeric expression integration, and reproduction workflows from inheritance sites.
- Resource-authored generated complement policies, generated structures beyond group state, variant persistence in genome versions, and compact/final binary variant schemas.
- External template-group reference resolution, resource-authored template-group validation, pair-specific blend matrices, and structure-level statistical simulation reports.
- Full hybrid morphology construction, compatibility resource graphs, inviable embryo state, and germline/generation-site behavior.
- Authored non-ploidal/trace resource definitions, non-ploidal mutation operations, trace activation effects, trace loss policies, and trace statistical tests.
- Full mutation event history, serialized/resource-authored mutation policies, random mutation timing/target selection, and arbitrary historical repair.
- Resource-pack loading, serialized operation/assertion registries beyond validation/freeze/assertions and the Slice 17 population-template frequency assertion, snapshots, fuzz/matrix execution, broader statistical assertions, validation reachability/policy coverage assertions, runtime-safe subset handling, CLI host/commands, editor integration, manifest merge/update workflows, result file layout, result writes, compact/final batch-plan binary schemas, and result retention/naming policy.
- Compact final binary schemas, remaining model codecs, custom binary-file storage, SQLite storage/provider selection, schema migrations, resource-pack manifests, and storage concurrency controls.
- GodotSharp `Resource` subclasses, editor plugins, Godot addon layout, `.tres`/`.res` export, runtime node helpers, binary Godot import/export, manifest/result/batch-plan file dereferencing, persistence policy, and Godot engine-version matrices.
- Reproduction/transmission distribution simulation, template-group aggregate reports, multi-generation simulation, confidence/outlier statistical policies, and broader serialized statistical resource-test steps.

### Recorded simplifications

- Slice 3 starts with dominance hierarchy, codominance, and numeric midpoint expression; advanced expression is deferred to Slice 8.
- Slice 4 starts with ordinary deterministic reproduction and a compatibility-policy stub; nonstandard reproduction and full compatibility are deferred to Slice 7.
- Slice 5 starts with request-time mutation policy objects and current-copy operations; serialized policy resources, mutation history, random mutation timing, and arbitrary historical repair are deferred.
- Slice 6 stores non-ploidal/trace state directly on genome versions; authored non-ploidal definitions and trace effect policies are deferred.
- Slice 7 starts compatibility/development with in-memory rules and opaque gestation context; full authored resources and gestational simulation are deferred.
- Slice 8 starts generated complements and variants as request-time/runtime state; Slice 21 adds preliminary binary variant serialization while resource-authored policies, variant persistence, and compact/final binary schemas remain deferred.
- Slice 9 starts mosaicism with ID-based regional assignment only; Slice 22 adds standalone codecs while geometry, blending, genome-version embedding, and automatic chimeric expression remain deferred.
- Slice 10 starts templates with independent allele-rank sampling and JSON only; linkage/correlation, biased inheritance/mutation hooks, broader statistical reports beyond Slice 16 allele-frequency simulation, and binary template codecs are deferred.
- Slice 11 embeds child template-group versions directly and supports a single cross-template blend policy per group; Slice 19 adds codecs for the embedded model while reference registries, pair-specific blend matrices, and statistical reports remain deferred.
- Slice 12 starts resource testing with in-memory fixture factories and a small built-in operation/assertion set; serialized designer-authored resources, snapshots, fuzz/matrix execution, and statistical/reproducibility features beyond Slice 16's in-memory template-frequency step are deferred.
- Slice 13 serializes typed resource-test specifications for the current authored definition kernel only; binary codecs, result/failure packet serialization, broad operation registries, and resource-pack loading are deferred.
- Slice 14 retains JSON-wrapped preliminary binary envelopes and introduces only generic JSON-file storage; compact binary schemas, SQLite/custom-binary storage, migrations, and resource-pack layouts remain deferred.
- Slice 15 uses package-free Godot-facing DTOs and bridges; GodotSharp resource classes, editor plugins, addon layout, binary import/export, and engine-specific packaging are deferred.
- Slice 16 starts statistical coverage with sequential population-template allele-frequency sampling, absolute tolerances, sample-count limits, and JSON failure packets; broader simulation domains and confidence/outlier models are deferred.
- Slice 17 embeds population-template statistical assertion data directly in serialized resource-test steps; external template references and broader statistical registries are deferred.
- Slice 18 serializes resource-test result fields and embedded reproducibility packets only; result persistence, manifests, reporting metadata, and compact binary schemas are deferred.
- Slice 19 serializes template groups with embedded templates and child groups only; external references, manifests, and compact binary schemas remain deferred.
- Slice 20 extends only the package-free Godot document bridge; GodotSharp resources, editor tooling, binary resources, and persistence/export packaging remain deferred.
- Slice 24 filters diagnostics only in reported resource-test results; execution, status calculation, reproducibility packets, and test selection remain unchanged.
- Slice 25 formats only already-materialized resource-test results; test execution, machine-readable JSON reporting, storage, CLI exit codes, and editor integration remain unchanged.
- Slice 26 stores resource-test results only through a typed factory over the generic JSON-file store; it does not define project file layout, retention, manifests, or output routing.
- Slice 27 summarizes already-materialized resource-test results only; it does not serialize manifests, define CLI status output, track durations, or add storage catalogs.
- Slice 28 serializes caller-authored result manifests with opaque result paths and embedded summaries only; it does not define CLI/batch execution, retention, manifest merge/update policy, duration metrics, or project layout.
- Slice 29 exposes result manifests through the package-free Godot document bridge only; it does not add GodotSharp resources, editor tooling, binary import/export, result dereferencing, or manifest storage/update policy.
- Slice 30 runs in-memory resource-test definitions in deterministic sequential batches only; it does not load serialized specs, write result files, define CLI commands, merge manifests, measure durations, or run in parallel.
- Slice 31 serializes batch plans with embedded resource-test JSON payloads only; it does not add external resource-pack references, binary plan codecs, JSON-file plan storage, CLI execution, result writes, or manifest update policy.
- Slice 32 uses a preliminary JSON-wrapped binary envelope and a typed single-file JSON store for batch plans only; it does not define compact binary schemas, project layout, result writes, manifest writes, CLI execution, retention, or merge/update policy.
- Slice 33 exposes batch plans through the package-free Godot document bridge and formats already-materialized batch results only; it does not add GodotSharp resources, editor tooling, CLI output, result writes, manifest writes, result dereferencing, retention, or merge/update policy.
- Slice 4 weighted-selection coverage is deterministic boundary coverage; reproduction/transmission statistical tolerance coverage remains deferred after Slice 16's first template-simulation layer.
- Later slices are intentionally outcome-level under incremental refinement and cannot start until their deliverables, acceptance criteria, and tests are expanded.

## Test accounting

### Tests present

- Slice 0 package-free implementation smoke tests in `tests/Genomancy.Tests`:
  - core assembly exposes stable name
  - core assembly is marked Godot independent
  - core assembly has no forbidden Godot, SQLite, or test-framework dependencies
- Slice 1 package-free implementation tests in `tests/Genomancy.Tests`:
  - ID/version value validation and stable comparison
  - design-mode edits
  - minimal valid body-plan graph validation and freeze
  - runtime startup/freeze
  - invalid runtime startup rejection before runtime state
  - retained-source snapshot isolation
  - post-freeze builder mutation rejection
  - stable-ID reference resolution
  - duplicate, missing-reference, and dependency-cycle diagnostics
  - deterministic diagnostic ordering
  - migration hook execution before freeze
- Slice 2 package-free implementation tests in `tests/Genomancy.Tests`:
  - genome version collection-alias isolation
  - current-copy edit, discard, and commit lifecycle
  - no version spam for uncommitted current-copy changes
  - JSON and binary genome round trips
  - malformed and unknown-version serialization failures
  - caller-supplied stream/buffer serialization boundaries
- Slice 3 package-free implementation tests in `tests/Genomancy.Tests`:
  - complete, absent, partial, wrong-ploidy, and dependency-failed group cases
  - shared-group evaluation under distinct body-plan contexts
  - activation state changes without genome version creation
  - strict dominance, codominance, and numeric midpoint expression
  - explicit expression context and opaque external facts
  - active available body-plan validation
- Slice 4 package-free implementation tests in `tests/Genomancy.Tests`:
  - seeded diploid deterministic offspring serialization
  - multi-parent and variable-ploidy contribution matrix
  - ambiguous lower-arity rejection
  - same individual/version in multiple parent roles
  - zero-weight exclusion and invalid-weight rejection
  - named random-stream separation
  - sterile/incompatible compatibility outcomes and parent immutability
- Slice 5 package-free implementation tests in `tests/Genomancy.Tests`:
  - current-only mutation divergence without version creation
  - committed mutation child-version creation and parent linkage
  - protected-target and invalid-allele rejection
  - numeric update, copy-count adjustment, group add, and group remove
  - repair and revert lifecycle behavior
  - external source allow/deny policy boundary
- Slice 6 package-free implementation tests in `tests/Genomancy.Tests`:
  - weighted deterministic non-ploidal inheritance
  - inactive and zero-weight transmission boundaries
  - trace activation, replacement, and degradation
  - ordinary reproduction integration for non-ploidal objects and traces
  - JSON and binary serialization round trips for heritable object state
- Slice 7 package-free implementation tests in `tests/Genomancy.Tests`:
  - compatibility evaluation and hybrid morphology contribution metadata
  - inviable reproduction outcome
  - clonal-copy reproduction success and invalid multi-contributor rejection
  - development timeline ordering and missing gestation context
  - maternal numeric-effect immutability
- Slice 8 package-free implementation tests in `tests/Genomancy.Tests`:
  - numeric sum and weighted-average expression
  - generated complement success, idempotence, and invalid-reference rejection
  - runtime body-plan variant availability with generated required groups
  - runtime body-plan variant JSON round trip and version rejection
- Slice 21 package-free implementation tests in `tests/Genomancy.Tests`:
  - runtime body-plan variant binary round trip
  - canonical JSON equality after binary round trip
  - truncated binary rejection
  - incompatible expected system-definition version rejection
- Slice 9 package-free implementation tests in `tests/Genomancy.Tests`:
  - regional expression from assigned and fallback genome versions
  - inheritance-site source resolution
  - chimeric material distinction from integrated runtime variants
- Slice 22 package-free implementation tests in `tests/Genomancy.Tests`:
  - mosaic genome JSON and binary round trips
  - regional assignment and fallback resolution after deserialization
  - chimeric material state preservation
  - unknown envelope, incompatible system-version, and truncated binary rejection
- Slice 10 package-free implementation tests in `tests/Genomancy.Tests`:
  - deterministic template sampling and sampled genome serialization
  - blend weight combination
  - stable generated population version and individual IDs
  - template-from-individual creation
  - population template JSON round trip and version rejection
- Slice 11 package-free implementation tests in `tests/Genomancy.Tests`:
  - nested weighted template-group generation and group-path preservation
  - deterministic forced cross-template blending with structural output equality
  - template-group version compatibility rejection, zero-positive-weight generation rejection, and source-alias isolation
- Slice 12 package-free implementation tests in `tests/Genomancy.Tests`:
  - valid resource-test fixture with validation assertions and freeze
  - expected invalid fixture and unexpected-valid failure diagnostics with deterministic ordering
  - custom resource-test step extension and fresh fixture isolation across repeated runs
- Slice 13 package-free implementation tests in `tests/Genomancy.Tests`:
  - resource-test JSON round trip and materialized execution
  - unsupported serialized step and malformed JSON rejection
  - deterministic include/exclude tag filtering
- Slice 14 package-free implementation tests in `tests/Genomancy.Tests`:
  - population-template binary round trip, invalid/truncated header, and system-version rejection
  - resource-test binary round trip, executable materialization, and truncated header
  - optional JSON-file storage persistence and loaded-resource execution
- Slice 15 package-free implementation tests in `tests/Genomancy.Tests`:
  - Godot adapter genome/template document round trips
  - Godot adapter kind mismatch and import failure diagnostics
  - Godot adapter resource-test import/export, resource package behavior, and runtime startup diagnostics
- Slice 20 package-free implementation tests in `tests/Genomancy.Tests`:
  - Godot adapter template-group document round trip
  - Godot adapter resource-test result document round trip
  - Godot resource package lookup for template-group/result documents
  - Godot adapter kind-mismatch diagnostic for new resource kinds
- Slice 23 package-free implementation tests in `tests/Genomancy.Tests`:
  - Godot adapter mosaic genome document round trip
  - Godot mosaic document metadata verification
  - Godot resource package lookup for mosaic documents
  - Godot adapter kind-mismatch diagnostic for mosaic documents
- Slice 16 package-free implementation tests in `tests/Genomancy.Tests`:
  - deterministic population-template allele-frequency simulation and ranked-observation accounting
  - configured simulation maximum-sample rejection
  - resource-test statistical failure diagnostic and reproducibility packet creation
  - deterministic reproducibility packet JSON round trip
- Slice 17 package-free implementation tests in `tests/Genomancy.Tests`:
  - resource-test JSON round trip for serialized population-template frequency assertions
  - materialized serialized statistical assertion execution with expected diagnostic and reproducibility packet metadata
- Slice 18 package-free implementation tests in `tests/Genomancy.Tests`:
  - resource-test result JSON round trip for mixed passing/failing cases
  - statistical failure diagnostic and reproducibility packet preservation
  - resource-test result binary round trip
  - unknown JSON envelope version, inconsistent aggregate status, and truncated binary rejection
- Slice 19 package-free implementation tests in `tests/Genomancy.Tests`:
  - nested population template-group JSON and binary round trips
  - deterministic generated sample equality after JSON round trip
  - cross-template blend policy preservation
  - unknown envelope, incompatible system-version, and truncated binary rejection
- Slice 24 package-free implementation tests in `tests/Genomancy.Tests`:
  - resource-test runner severity filtering for error, warning, and info diagnostics
  - failure status preservation when lower-priority diagnostics are filtered
  - existing tag-filtering regression coverage
- Slice 25 package-free implementation tests in `tests/Genomancy.Tests`:
  - resource-test text report summary counts for mixed pass/fail results
  - rendered diagnostic details and explicit empty sections
  - reproducibility packet reference rendering
  - deterministic case ordering by resource-test id
- Slice 26 package-free implementation tests in `tests/Genomancy.Tests`:
  - JSON-file storage round trip for failed resource-test run results
  - diagnostic preservation across save/load
  - reproducibility packet preservation across save/load
  - canonical result JSON equality after storage round trip
- Slice 27 package-free implementation tests in `tests/Genomancy.Tests`:
  - empty resource-test run summary
  - mixed pass/fail case summary
  - diagnostic severity counts for error, warning, and info
  - reproducibility packet counting
  - existing text-report compatibility after summary refactor
- Slice 28 package-free implementation tests in `tests/Genomancy.Tests`:
  - resource-test result manifest summary derivation and deterministic ordering
  - manifest tag normalization, UTC timestamp normalization, and duplicate run-ID rejection
  - manifest JSON round trip and unsupported/invalid JSON rejection
  - JSON-file storage round trip for resource-test result manifests
- Slice 29 package-free implementation tests in `tests/Genomancy.Tests`:
  - Godot adapter resource-test result manifest document round trip
  - manifest document metadata verification for empty system-definition version and sorted tags
  - Godot resource package lookup for manifest documents
  - Godot adapter kind-mismatch diagnostic for manifest documents
- Slice 30 package-free implementation tests in `tests/Genomancy.Tests`:
  - resource-test batch runner executes multiple named runs and builds a manifest
  - aggregate batch status reflects failed contained runs
  - generated manifest entries include derived summaries and merged sorted tags
  - per-run include-tag options filter executed cases and manifest tags
  - duplicate batch run-ID rejection
- Slice 31 package-free implementation tests in `tests/Genomancy.Tests`:
  - resource-test batch JSON round trip with multiple runs and nested resource-test specifications
  - materialized serialized batch execution through the batch runner
  - per-run options and UTC timestamp normalization
  - nested resource-test JSON payload preservation
  - unsupported envelope, invalid timestamp, invalid diagnostic severity, and missing nested payload rejection
- Slice 32 package-free implementation tests in `tests/Genomancy.Tests`:
  - resource-test batch binary round trip and materialized execution
  - truncated batch binary rejection
  - wrong binary header rejection
  - JSON-file storage round trip for resource-test batch plans
  - loaded batch-plan execution through the batch runner
- Slice 33 package-free implementation tests in `tests/Genomancy.Tests`:
  - deterministic batch-run text report summary and manifest rendering
  - Godot adapter batch-plan document round trip
  - Godot batch-plan metadata and combined tag ordering
  - imported Godot batch-plan execution through the batch runner
  - Godot package lookup and kind-mismatch diagnostics for batch-plan documents
- Build verification through `scripts/verify.sh`.

### Requirements with tests

- Slice 0 project-foundation acceptance criteria are verified by `scripts/verify.sh`.
- Slice 1 acceptance criteria are verified by `scripts/verify.sh`.
- Slice 2 acceptance criteria are verified by `scripts/verify.sh`.
- Slice 3 acceptance criteria are verified by `scripts/verify.sh`.
- Slice 4 acceptance criteria are verified by `scripts/verify.sh`.
- Slice 5 acceptance criteria are verified by `scripts/verify.sh`.
- Slice 6 acceptance criteria are verified by `scripts/verify.sh`.
- Slice 7 acceptance criteria are verified by `scripts/verify.sh`.
- Slice 8 acceptance criteria are verified by `scripts/verify.sh`.
- Slice 9 acceptance criteria are verified by `scripts/verify.sh`.
- Slice 10 acceptance criteria are verified by `scripts/verify.sh`.
- Slice 11 acceptance criteria are verified by `scripts/verify.sh`.
- Slice 12 acceptance criteria are verified by `scripts/verify.sh`.
- Slice 13 acceptance criteria are verified by `scripts/verify.sh`.
- Slice 14 acceptance criteria are verified by `scripts/verify.sh`.
- Slice 15 acceptance criteria are verified by `scripts/verify.sh`.
- Slice 16 acceptance criteria are verified by `scripts/verify.sh`.
- Slice 17 acceptance criteria are verified by `scripts/verify.sh`.
- Slice 18 acceptance criteria are verified by `scripts/verify.sh`.
- Slice 19 acceptance criteria are verified by `scripts/verify.sh`.
- Slice 20 acceptance criteria are verified by `scripts/verify.sh`.
- Slice 21 acceptance criteria are verified by `scripts/verify.sh`.
- Slice 22 acceptance criteria are verified by `scripts/verify.sh`.
- Slice 23 acceptance criteria are verified by `scripts/verify.sh`.
- Slice 24 acceptance criteria are verified by `scripts/verify.sh`.
- Slice 25 acceptance criteria are verified by `scripts/verify.sh`.
- Slice 26 acceptance criteria are verified by `scripts/verify.sh`.
- Slice 27 acceptance criteria are verified by `scripts/verify.sh`.
- Slice 28 acceptance criteria are verified by `scripts/verify.sh`.
- Slice 29 acceptance criteria are verified by `scripts/verify.sh`.
- Slice 30 acceptance criteria are verified by `scripts/verify.sh`.
- Slice 31 acceptance criteria are verified by `scripts/verify.sh`.
- Slice 32 acceptance criteria are verified by `scripts/verify.sh`.
- Slice 33 acceptance criteria are verified by `scripts/verify.sh`.
- REQ-GODOT is partially covered for the core-boundary requirement and the package-free adapter assembly; GodotSharp resource subclasses/editor plugins remain unimplemented and untested.
- REQ-MODE, REQ-MODE-FREEZE, REQ-ID, REQ-MODEL, REQ-POLICY, REQ-VALIDATE, REQ-GENOME, REQ-GENE, REQ-GROUP, REQ-BODY, REQ-VARIANT, REQ-EXPR, REQ-EXTERNAL, REQ-PLOIDY, REQ-REPRO, REQ-RANDOM, REQ-MUTATION, REQ-VERSION, REQ-ACQUIRED, REQ-NONPLOID, REQ-TRACE, REQ-COMPAT, REQ-DEVELOP, REQ-MOSAIC, REQ-TEMPLATE, REQ-TGROUP, REQ-TFROMIND, REQ-RTEST, REQ-SERIAL, REQ-STORAGE, and REQ-GODOT have partial slice coverage only; each remains broader than the implemented slices and stays **In progress** where later slices add required behavior.

### Requirements without tests

- Requirement families not listed under partial coverage above remain without implementation tests.
- Serialized designer-authored resource-test files can now be represented as JSON buffers/text, including the Slice 17 population-template frequency assertion. Resource-test run results can now be represented as JSON/binary buffers, deterministic human-readable text reports, optional JSON files through `Genomancy.Storage.JsonFile`, derived in-memory summaries, serialized result manifests with opaque stored-result paths, deterministic in-memory batch-run outputs with generated manifests, deterministic batch-run text reports, and serialized JSON/binary batch-run plans with embedded resource-test specifications and optional JSON-file storage. Population template groups can now be represented as JSON/binary buffers with embedded templates/child groups. Runtime body-plan variants can now be represented as JSON/binary buffers. Standalone mosaic genome state can now be represented as JSON/binary buffers. The package-free Godot adapter can bridge genome, mosaic-genome, population-template, population-template-group, resource-test, resource-test-batch-plan, resource-test-result, and resource-test-result-manifest JSON documents. Repository-level resource-pack loading, result retention policy, manifest merge/update workflows, CLI host/commands, result file writes, compact/final batch-plan binary schemas, and project file layout do not exist yet.

### Test layers required by the project

- **Implementation tests:** Verify C# engine behavior, invariants, codecs, adapters, and storage modules.
- **Resource tests:** Verify designer-authored genetics resources and policies using the first-class framework required by Section 26.
- **Integration tests:** Verify startup/freeze ordering, cross-policy workflows, serialization compatibility, storage boundaries, and Godot adapter behavior.
- **Simulation/statistical tests:** Verify seeded reproducibility and distribution behavior within explicit tolerances.

## Open decisions and risks

| ID | Decision or risk | Needed by | Current handling |
|---|---|---|---|
| OPEN-001 | Supported Godot adapter version range beyond the initial local Godot 4.6.2 environment. | Later GodotSharp/editor plugin refinement | Slice 15 adapter is package-free and targets `net9.0`; GodotSharp resource/editor packaging compatibility remains open. |
| OPEN-002 | Binary format design and compatibility strategy. | Slice 2 | Use a versioned preliminary codec, then stabilize in Slice 14. |
| OPEN-003 | Definition immutability mechanism. | Slice 1 | Resolved for Slice 1 with immutable definition records, read-only copied collections, and a frozen snapshot created from the mutable builder; retained-reference mutation and snapshot-isolation tests pass. |
| OPEN-004 | Policy extensibility model and safe serialization of policy configuration. | Slice 1 | Separate policy identity/configuration from executable host implementation. |
| OPEN-005 | Numeric value representation and deterministic arithmetic guarantees. | Slice 2-3 | Decide before numeric expression becomes public format. |
| OPEN-006 | Random algorithm and stream-derivation contract. | Slice 4 | Resolved for implemented mechanics with FNV-1a stream-name derivation and SplitMix64 draws; Slice 16 adds first template-frequency tolerances and JSON reproducibility packets, while broader statistical/reproduction packet design remains under REQ-RANDOM later work. |
| OPEN-007 | Resource limits for graph depth, dependency traversal, and simulation workloads. | Slice 1 onward | Slice 16 adds a configurable sample-count limit for population-template simulations; graph depth, elapsed-time, allocation, and other simulation-domain limits remain open. |
| OPEN-008 | SQLite provider and native-binary implications for Godot export targets. | Later storage hardening / Slice 15 packaging review | Keep provider outside core; Slice 14 added only package-free JSON-file storage, so provider selection remains open. |

## Ledger update checklist

Every implementation change must update applicable portions of this ledger:

1. Record new or changed scope in **Scope injections and changes**.
2. Update requirement-family status and planned slice if affected.
3. Update the active slice deliverables and acceptance criteria when refinement reveals new detail.
4. Move completed work into **Implemented** and retain meaningful simplifications.
5. List incomplete or deferred behavior explicitly.
6. Update **Test accounting** with exact covered and uncovered requirements.
7. Record new architectural decisions, open decisions, and material risks.
8. Mark a requirement **Verified** only after its required tests pass.
