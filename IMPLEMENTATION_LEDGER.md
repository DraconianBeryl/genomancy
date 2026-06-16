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
| Last ledger update | 2026-06-16 |
| Current implementation slice | Slice 12 - Template-group serialization and generation manifests |

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
| 2026-06-06 | Refine Slice 8 to numeric sum/weighted-average expression, deterministic generated complements, and JSON-serializable runtime body-plan variants. | Slice 8 implementation | Advances advanced expression and variant state while deferring full expression policy language, generated complement resource graphs, and binary variant codecs. | Accepted |
| 2026-06-06 | Refine Slice 9 to regional genome assignments, region-scoped expression, inheritance-site source resolution, and distinct chimeric material state. | Slice 9 implementation | Advances mosaic/chimera modeling while deferring region geometry, chimeric serialization, and full inheritance workflows. | Accepted |
| 2026-06-06 | Refine Slice 10 to immutable statistical population template versions with deterministic sampling, blending, template-from-individual, population generation, and JSON codecs. | Slice 10 implementation | Advances template workflows while deferring biased inheritance/mutation hooks, full statistical tolerances, and binary template codecs. | Accepted |
| 2026-06-07 | Refine Slice 11 to immutable population template-group versions, weighted direct/nested selection, optional deterministic cross-template blending, and structure-preserving generated genome metadata. | Slice 11 implementation | Advances nested population simulation while deferring template-group serialization, authored resource validation, statistical tolerance reports, and biased inheritance/mutation hooks. | Accepted |
| 2026-06-16 | Defer direct resource-test framework implementation and use Slice 12 for template-group serialization plus deterministic generation manifests. | Project request | Advances testability and reproducibility support without adding designer-authored test resources, test operations, assertions, or runners. Resource testing shifts to Slice 13+. | Accepted |

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
| REQ-RTEST | First-class immutable-input resource test definitions, fixtures, operations, assertions, diagnostics, and runners. | 26, 27 | Planned | 13+ | Self-tests + integration |
| REQ-RANDOM | Deterministic execution, separated random streams, reproducibility packets, and statistical tolerances. | 26.13-26.14, 26.24, 26.26 | In progress | 4, 12+ | Determinism + statistical tests |
| REQ-VALIDATE | Resource graph, reachability, policy coverage, invariants, negative cases, and required baseline content tests. | 26.19-26.22, 26.39 | In progress | 1, 12-13 | Validation + resource tests |
| REQ-SERIAL | Stable JSON and binary formats at multiple granularities, including versions, variants, templates, tests, and failure packets. | 31.1-31.3 | In progress | 2 onward; finalized 14 | Round-trip + compatibility |
| REQ-STORAGE | Core has no permanent storage; optional JSON-file, binary-file, and SQLite modules depend on core. | 31.4-31.7 | In progress | 14 | Integration tests |
| REQ-GODOT | Optional Godot adapter consumes core APIs without redefining genetics behavior. | Project scope injection | Planned | 15 | Build + adapter tests |

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

**Status:** Verified on 2026-06-06 for the Slice 4 acceptance criteria. Broader requirement families remain **In progress** where later slices add nonstandard reproduction, full compatibility, mutation during inheritance, gestation, non-ploidal objects, traces, resource tests, and statistical tolerances.

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
- Weighted-selection distribution has deterministic boundary coverage, but no statistical tolerance framework exists yet.

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

**Status:** Verified on 2026-06-06 for the refined Slice 8 acceptance criteria. Broader requirement families remain **In progress** where later slices add full expression policy language, resource-authored generated complements, binary variant codecs, and resource tests.

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
- Runtime variant serialization is JSON-only in this slice; binary variant serialization remains deferred.
- Advanced expression adds two numeric strategies only; full expression policy language and contextual trace/non-ploidal effects remain deferred.

**Not yet implemented**

- Resource-authored generated complement policies, generated body structures beyond group state, runtime variant persistence in genome versions, binary variant codecs, variant migration, advanced expression policy language, trace/non-ploidal expression effects, and resource-test coverage.

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
- Mosaic state is not yet serialized as part of genome versions or a separate state resource.
- Chimeric material is explicit runtime state and may mark whether it is integrated, but it does not yet participate in expression automatically.
- Inheritance-site resolution returns a genome version only; it does not yet execute reproduction from region/site sources.

**Not yet implemented**

- Regional geometry, multi-region blending, overlapping mosaic expression, chimeric expression integration, absorbed-twin inheritance rules, mosaic/chimera serialization, mutation/repair over regions, reproduction workflows from inheritance sites, and resource-test coverage.

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

- JSON/binary template-group codecs, generation manifests, external template-group reference resolution, resource-authored template-group validation, pair-specific blend matrices, structure-level statistical reports, biased inheritance/mutation hooks, and resource-test coverage.

**Requirements advanced:** REQ-TGROUP, REQ-TEMPLATE, REQ-RANDOM.

### Slice 12 - Template-group serialization and generation manifests

**Status:** Verified on 2026-06-16 for the refined Slice 12 acceptance criteria. Broader requirement families remain **In progress** where later slices add binary codecs, final compatibility contracts, statistical tolerance reports, resource tests, and storage modules.

**Objective:** Add deterministic serialization and reproducibility support for template-group generation while deferring direct resource-test framework work.

**Deliverables**

- Serialize immutable population-template-group versions to deterministic JSON through stream, buffer, and text APIs.
- Preserve nested template groups, weighted direct templates, weighted child groups, cross-template blend policy, change summaries, and embedded population-template profiles.
- Reject incompatible system-definition versions when loading template groups.
- Represent deterministic generated-population manifests with root template-group identity, system-definition version, seed, requested count, ID prefixes, per-sample seeds, generated genome IDs, individual IDs, group paths, and selected primary/secondary template provenance.
- Serialize generated-population manifests to deterministic JSON buffers and text.
- Keep all APIs in `Genomancy.Core` stream/buffer/text boundaries with no filesystem, storage provider, or test-framework dependency.

**Acceptance criteria**

- Template-group JSON round trips preserve nested immutable profile state and produce stable canonical text after round trip.
- Template-group deserialization rejects mismatched system-definition versions.
- Generation manifests are deterministic for identical template group, seed, count, and ID prefixes.
- Manifest JSON round trips preserve sample seeds, generated IDs, group path, and blend provenance.
- Direct resource-test definitions, operations, assertions, diagnostics, and runners remain unimplemented in this slice.

**Tests**

- Nested template-group JSON round-trip, canonical text, blend-policy preservation, and version-rejection test.
- Template-group generated-population manifest determinism, JSON round-trip, sample-seed/ID/provenance preservation, and version-rejection test.

**Implemented**

- `PopulationTemplateGroupJsonCodec` for deterministic JSON stream, buffer, and text operations over recursively embedded template-group versions.
- `TemplatePopulationManifest` and `TemplatePopulationManifestEntry` for reproducibility-oriented generated-population provenance.
- `TemplatePopulationManifestJsonCodec` for deterministic manifest JSON buffers and text.
- `PopulationTemplateGroupService.GeneratePopulationManifest` and `CreatePopulationManifest` helpers.

**Implementation simplification choices**

- Template-group JSON embeds child template groups and direct population templates by value, matching the current in-memory model; external registry/reference resolution remains deferred.
- Manifest entries record deterministic selection/provenance metadata, not complete genome payloads or statistical aggregate reports.
- Manifest sample seeds are recorded as `rootSeed + index`, matching current generated-population sampling.
- Serialization remains JSON-only; binary template-group and manifest codecs remain deferred.

**Not yet implemented**

- Direct resource-test framework work: test resources, fixtures, operations, assertions, runners, failure packets, tags, severity, snapshots, fuzz/matrix execution, and runtime-safe resource-test subsets.
- Binary template-group or manifest codecs, external template/template-group registries, migration hooks, compatibility matrices, statistical tolerance reports, and storage modules.

**Requirements advanced:** REQ-TGROUP, REQ-TEMPLATE, REQ-RANDOM, REQ-SERIAL, REQ-STORAGE.

### Slice 13+ - Resource testing framework

Refine and likely subdivide before implementation. Build designer-authored test resources, fixtures, operations, assertions, custom extension points, validation/reachability/policy coverage, deterministic simulation and statistical tests, diagnostics/reproducibility packets, tags, severity, snapshots, fuzz/matrix execution, isolation, and runtime-safe subsets.

**Requirements targeted:** REQ-RTEST, REQ-VALIDATE, REQ-RANDOM, REQ-SERIAL.

### Slice 14 - Serialization hardening and optional storage modules

Refine before implementation. Finalize compatibility contracts and granular JSON/binary formats, then add non-core JSON-file, custom-binary-file, and SQLite storage modules with migrations and test-fixture support.

**Requirements targeted:** REQ-SERIAL, REQ-STORAGE, REQ-ID.

### Slice 15 - Godot adapter and packaging

Refine against the selected Godot/.NET versions. Add a thin adapter for Godot authoring/runtime workflows, package import/export, diagnostics, and engine-facing conversions while preserving a Godot-free core.

**Requirements targeted:** REQ-GODOT, REQ-MODE, REQ-SERIAL.

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
- Slice 12 template-group serialization and generation manifests:
  - deterministic JSON codec for recursively embedded population-template-group versions
  - JSON round-trip compatibility checks for template groups by system-definition version
  - deterministic generated-population manifests with root group identity, seed, count, ID prefixes, sample seeds, generated IDs, group paths, and template provenance
  - deterministic JSON codec for generated-population manifests

### Not yet implemented

- Nonstandard reproduction beyond clonal copy, full compatibility, gestational simulation, and advanced mosaic/chimera behavior.
- Regional geometry, mosaic/chimera serialization, overlapping mosaic expression, chimeric expression integration, and reproduction workflows from inheritance sites.
- Resource-authored generated complement policies, generated structures beyond group state, variant persistence in genome versions, and binary variant codecs.
- Template-group binary codecs, external template-group reference resolution, resource-authored template-group validation, pair-specific blend matrices, and structure-level statistical simulation reports.
- Full hybrid morphology construction, compatibility resource graphs, inviable embryo state, and germline/generation-site behavior.
- Authored non-ploidal/trace resource definitions, non-ploidal mutation operations, trace activation effects, trace loss policies, and trace statistical tests.
- Full mutation event history, serialized/resource-authored mutation policies, random mutation timing/target selection, and arbitrary historical repair.
- Final serialization/storage modules beyond Slice 2 preliminary core codecs.
- All Godot integration.
- All resource tests.
- Statistical simulation/tolerance tests and reproducibility packets.

### Recorded simplifications

- Slice 3 starts with dominance hierarchy, codominance, and numeric midpoint expression; advanced expression is deferred to Slice 8.
- Slice 4 starts with ordinary deterministic reproduction and a compatibility-policy stub; nonstandard reproduction and full compatibility are deferred to Slice 7.
- Slice 5 starts with request-time mutation policy objects and current-copy operations; serialized policy resources, mutation history, random mutation timing, and arbitrary historical repair are deferred.
- Slice 6 stores non-ploidal/trace state directly on genome versions; authored non-ploidal definitions and trace effect policies are deferred.
- Slice 7 starts compatibility/development with in-memory rules and opaque gestation context; full authored resources and gestational simulation are deferred.
- Slice 8 starts generated complements and variants as request-time/runtime state; resource-authored policies, variant persistence, and binary variant codecs are deferred.
- Slice 9 starts mosaicism with ID-based regional assignment only; geometry, blending, serialization, and automatic chimeric expression are deferred.
- Slice 10 starts templates with independent allele-rank sampling and JSON only; linkage/correlation, biased inheritance/mutation hooks, statistical tolerances, and binary template codecs are deferred.
- Slice 11 embeds child template-group versions directly and supports a single cross-template blend policy per group; reference registries, pair-specific blend matrices, codecs, and statistical reports are deferred.
- Slice 12 keeps template-group serialization by-value and manifest-only for generated population reproducibility; external registries, binary codecs, full generated genome packets, and statistical aggregate reports are deferred.
- Preliminary Slice 2 serialization covers only then-existing models; complete format stabilization is deferred to Slice 14.
- Slice 4 weighted-selection coverage is deterministic boundary coverage; statistical tolerances are deferred until the simulation/statistical test layer exists.
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
- Slice 9 package-free implementation tests in `tests/Genomancy.Tests`:
  - regional expression from assigned and fallback genome versions
  - inheritance-site source resolution
  - chimeric material distinction from integrated runtime variants
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
  - nested template-group JSON round trip, canonical text, blend-policy preservation, and version rejection
  - deterministic generated-population manifest creation, JSON round trip, sample-seed/ID/provenance preservation, and version rejection
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
- REQ-GODOT is partially covered only for the core-boundary requirement that `Genomancy.Core` has no Godot dependency. The actual Godot adapter remains unimplemented and untested.
- REQ-MODE, REQ-MODE-FREEZE, REQ-ID, REQ-MODEL, REQ-POLICY, REQ-VALIDATE, REQ-GENOME, REQ-GENE, REQ-GROUP, REQ-BODY, REQ-VARIANT, REQ-EXPR, REQ-EXTERNAL, REQ-PLOIDY, REQ-REPRO, REQ-RANDOM, REQ-MUTATION, REQ-VERSION, REQ-ACQUIRED, REQ-NONPLOID, REQ-TRACE, REQ-COMPAT, REQ-DEVELOP, REQ-MOSAIC, REQ-TEMPLATE, REQ-TGROUP, REQ-TFROMIND, REQ-SERIAL, and REQ-STORAGE have partial slice coverage only; each remains broader than the implemented slices and stays **In progress** where later slices add required behavior.

### Requirements without tests

- Requirement families not listed under partial coverage above remain without implementation tests.
- Designer-authored resource tests do not exist yet for any requirement family.

### Test layers required by the project

- **Implementation tests:** Verify C# engine behavior, invariants, codecs, adapters, and storage modules.
- **Resource tests:** Verify designer-authored genetics resources and policies using the first-class framework required by Section 26.
- **Integration tests:** Verify startup/freeze ordering, cross-policy workflows, serialization compatibility, storage boundaries, and Godot adapter behavior.
- **Simulation/statistical tests:** Verify seeded reproducibility and distribution behavior within explicit tolerances.

## Open decisions and risks

| ID | Decision or risk | Needed by | Current handling |
|---|---|---|---|
| OPEN-001 | Supported Godot adapter version range beyond the initial local Godot 4.6.2 environment. | Slice 15 | Initial core target is `net9.0`; adapter compatibility remains open until Godot adapter refinement. |
| OPEN-002 | Binary format design and compatibility strategy. | Slice 2 | Use a versioned preliminary codec, then stabilize in Slice 14. |
| OPEN-003 | Definition immutability mechanism. | Slice 1 | Resolved for Slice 1 with immutable definition records, read-only copied collections, and a frozen snapshot created from the mutable builder; retained-reference mutation and snapshot-isolation tests pass. |
| OPEN-004 | Policy extensibility model and safe serialization of policy configuration. | Slice 1 | Separate policy identity/configuration from executable host implementation. |
| OPEN-005 | Numeric value representation and deterministic arithmetic guarantees. | Slice 2-3 | Decide before numeric expression becomes public format. |
| OPEN-006 | Random algorithm and stream-derivation contract. | Slice 4 | Resolved for implemented mechanics with FNV-1a stream-name derivation and SplitMix64 draws; statistical tolerance and reproducibility packet design remains under REQ-RANDOM later work. |
| OPEN-007 | Resource limits for graph depth, dependency traversal, and simulation workloads. | Slice 1 onward | Add validation limits as affected features are refined. |
| OPEN-008 | SQLite provider and native-binary implications for Godot export targets. | Slice 14 | Keep provider outside core and evaluate platform support before selection. |

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
