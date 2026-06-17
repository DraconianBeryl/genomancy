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
| Last ledger update | 2026-06-17 |
| Current implementation slice | Slice 0 - Project foundation (not started) |

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
- Complete-system export/import as core serialization over streams, buffers, byte arrays, or text data.
- In-memory stream, buffer, byte-array, and text serialization APIs in the core library.
- Optional non-core storage modules for JSON files, custom binary files, and SQLite.
- System-definition migration support for save compatibility through policy statements and optional host-provided callbacks.
- Resource metadata and tags required by Genomancy for identity, serialization, diagnostics, version compatibility, testing, policy selection, and resource selection.
- A first-class designer-authored resource testing framework, distinct from implementation tests.
- Deterministic behavior where seeded randomness is involved, including separated random streams where required.
- A thin, optional Godot runtime integration layer that adapts the engine-neutral core without moving domain rules into Godot types.
- A separate Godot editor integration plugin is planned but deferred.

### Explicitly out of scope

- Molecular biology simulation: DNA bases, RNA, transcription, translation, protein folding, cellular biochemistry, and full embryology.
- Identity, souls, reincarnation, possession identity, prophecy, fate, social caste, legal species identity, blood-purity ideology, political legitimacy, true names, and related metaphysics or social systems.
- Ecological selection, survival selection, mating behavior, geography, economy, and culture unless supplied as external context.
- A mandatory database, filesystem layout, cloud service, asset database, save repository, networking model, user interface, or game-specific entity model in the core library.
- Content management for external resource definitions, including authoritative corpus discovery, source-control status, content history, merge resolution, release membership, or completeness guarantees for project files, directories, repositories, editor workspaces, databases, or archives.
- Golden-sample approval workflows in the core library.
- Arbitrary database semantics, authoritative storage indexes, report-history storage, or source-control-aware changed-resource discovery as genetics-system behavior.
- Editor integration inside the core library.
- Godot-specific inheritance or serialization as a requirement of core domain objects.
- Unconstrained mutation or automatic invention of body structures outside authored policies.

### Scope injections and changes

| Date | Change | Source | Impact | Status |
|---|---|---|---|---|
| 2026-06-05 | Select C# as the implementation language. | Project request | Constrains solution structure and public API design. | Accepted |
| 2026-06-05 | Require Godot usability without exclusive Godot coupling. | Project request | Core assemblies must not reference Godot; integration belongs in an adapter assembly. | Accepted |
| 2026-06-05 | Require incremental-refinement planning. | Project request | Near-term slices have detailed deliverables/tests; distant slices remain outcome-level and must be refined before work starts. | Accepted |
| 2026-06-17 | Clarify that Genomancy storage modules are not content management systems for external resource definitions. | Project clarification | Storage modules must faithfully load supplied resources and may validate the loaded graph, but external systems own authoritative corpus completeness, source control, history, merge resolution, and release membership. | Accepted |
| 2026-06-17 | Clarify complete-system export/import as serialization, with storage modules owning stored-byte handling. | Project clarification | Core serializes/deserializes complete systems from streams/buffers; storage modules locate, read, write, and stream persisted serialized data. | Accepted |
| 2026-06-17 | Bound migrations to save compatibility across system-definition versions. | Project clarification | Migrations should normally be policy statements with optional host callbacks; they do not require saves to embed complete system definitions. | Accepted |
| 2026-06-17 | Bound storage transactions and indexing. | Project clarification | Transactions support atomic logical writes; indexes are efficient access aids using verified metadata/tags and are not authoritative catalogs. | Accepted |
| 2026-06-17 | Remove golden-sample approval workflows from core requirements. | Project clarification | Core may emit regenerated candidate output, but acceptance/approval workflows belong to external tools. | Accepted |
| 2026-06-17 | Scope affected/changed-resource test selection to loaded resources. | Project clarification | Test selection may use externally supplied selected/changed resource IDs and the currently loaded graph, not source-control or workspace discovery. | Accepted |
| 2026-06-17 | Separate Godot runtime support from deferred Godot editor integration. | Project clarification | Runtime adapter remains planned; editor plugin is a separate deferred integration. | Accepted |
| 2026-06-17 | Bound resource metadata and add resource tags. | Project clarification | Metadata is limited to Genomancy needs; tags may participate in policy, test, validation, reachability, and selection/matching behavior. | Accepted |
| 2026-06-17 | Replace audit terminology with runtime history diagnostics. | Project clarification | Genome history is for runtime genetic-state changes, including designer-authored past events in genome records, not design-change audit trails. | Accepted |
| 2026-06-17 | Clarify reports as transient outputs. | Project clarification | Reports are produced for callers/tools/CI/storage modules; the core runner does not store report history. | Accepted |

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
| ARC-010 | The exact target framework, binary encoding, and SQLite provider remain open until Slice 0/serialization refinement. | These choices require compatibility and maintenance evaluation. | Open |
| ARC-011 | Optional storage modules load and save supplied serialized resources but do not own external content-management responsibilities. | Keeps Genomancy focused on genetics resource loading/validation and leaves authoritative corpus management to version control, asset pipelines, editor databases, or game-specific systems. | Accepted |
| ARC-012 | Runtime package export/import is complete-system serialization in core; persisted-data handling belongs to storage modules or external tools. | Preserves stable core formats without making core responsible for files, databases, archives, repositories, or distribution packages. | Accepted |
| ARC-013 | Migrations are save-compatibility transforms between system-definition versions, preferably policy-defined and optionally host-callback-backed. | Supports game updates without requiring every save to embed a full genetics system definition. | Accepted |
| ARC-014 | Storage transactions provide atomic logical writes, and storage indexes are non-authoritative access aids. | Supports database-backed modules and efficient persistence without turning indexes into content catalogs. | Accepted |
| ARC-015 | Golden-sample approval, report-history storage, and source-control changed-resource discovery are external workflow concerns. | Keeps the core test runner focused on deterministic execution, candidate outputs, diagnostics, and transient reports. | Accepted |
| ARC-016 | Godot editor integration is a separate deferred plugin from Godot runtime support. | Keeps runtime adapter scope narrow and avoids moving editor workflows into core. | Accepted |
| ARC-017 | Resource tags are Genomancy metadata and may be used by tag-aware policies, tests, validation, reachability, and selection. | Provides structured selection/matching without broad asset metadata management. | Accepted |

## Requirements register

The source specification remains authoritative for detailed behavior. The IDs below provide implementation and test traceability; they do not replace the source text.

| ID | Requirement family | Source sections | Status | Planned slice | Verification |
|---|---|---:|---|---|---|
| REQ-MODE | Fixed design/runtime operating mode, startup ordering, validation, freeze, system-definition versions, and migration-before-freeze boundary. | 3, 27, 30 | Planned | 1 | Unit + integration |
| REQ-MODE-FREEZE | Runtime definitions become immutable after successful validation/migration and before ordinary runtime state loads/operations. | 3.3-3.6, 28.3 | Planned | 1 | Unit + negative tests |
| REQ-ID | Stable identifiers and compatibility references for authored resources and serialized state. | 3.6, 31.3 | Planned | 1 | Unit + serialization tests |
| REQ-MODEL | Core object hierarchy from allele/value through nested population structures. | 4, 24 | Planned | 1-2 | Unit tests |
| REQ-GENOME | Individuals, genomes, immutable permanent versions, and mutable/provisional current copies remain distinct. | 4.1-4.4, 16, 28.1 | Planned | 2 | Unit + invariant tests |
| REQ-GENE | Categorical and numeric genes, alleles, ranked sets, and opt-in expression patterns. | 4.5-4.7, 5 | Planned | 2-3 | Unit + resource tests |
| REQ-GROUP | Groups, subgroups, presence, completeness, sharing, dependencies, linkage, and generated complements. | 6 | Planned | 3, 8 | Unit + graph validation |
| REQ-BODY | Authored body plans, active/dormant/primary states, families, activation, and temporary shapeshifting. | 4.12, 7.1-7.7 | Planned | 3 | Unit + integration |
| REQ-VARIANT | Deterministic, serializable, policy-created runtime body-plan variants as instance state. | 4.13, 7.8, 28.4 | Planned | 8 | Unit + serialization + resource tests |
| REQ-DEVELOP | Developmental sequencing, gestation, post-birth requirements, and maternal/gestational effects. | 8 | Planned | 7 | Integration + resource tests |
| REQ-COMPAT | Explicit compatibility layers with fertile, sterile, inviable, and hybrid-morphology outcomes. | 9 | Planned | 7 | Unit + resource tests |
| REQ-PLOIDY | Default/variable ploidy, ranked interpretation, shared-group models, and multi-parent roles. | 10, 28.8 | Planned | 4 | Unit + matrix tests |
| REQ-REPRO | Policy-driven allele/group/object selection, ordinary and nonstandard reproduction, germline/generation sites. | 11 | Planned | 4, 7 | Unit + integration + simulation |
| REQ-MOSAIC | Mosaic/chimeric regions, expression, inheritance, and absorbed-twin genetic handling. | 12 | Planned | 9 | Unit + integration |
| REQ-NONPLOID | Flags, counters, accumulators, archives, markers, weights, activation, transmission, mutation, and decay. | 4.10, 13 | Planned | 6 | Unit + resource tests |
| REQ-TRACE | Trace structure, weighted transmission, activation, allele replacement effects, mutation, degradation, and loss. | 4.11, 14 | Planned | 6 | Unit + resource tests |
| REQ-MUTATION | Policy-controlled event source, timing, targets, scope, value selection, overwrite, numeric update, copy number, and structural changes. | 15, 28.9-28.10 | Planned | 5 | Unit + policy coverage |
| REQ-VERSION | Permanent version creation, commit policies, current-copy behavior, repair, reversion, and visibility. | 16 | Planned | 2, 5 | Unit + integration |
| REQ-ACQUIRED | External systems may request authored heritable changes through mutation interfaces without becoming core metaphysics. | 17, 20, 32 | Planned | 5 | Integration + negative tests |
| REQ-EXPR | Contextual expression and activation using body plan, phase, ploidy, dependencies, epigenetics, and external context. | 18, 28.5-28.7 | Planned | 3, 8 | Unit + resource tests |
| REQ-EXTERNAL | Container, genealogy, birth order, lineage facts, possession, and identity remain external inputs. | 19, 20, 28.7 | Planned | 3+ | Boundary tests |
| REQ-TEMPLATE | Immutable statistical templates, random generation, blending, biased inheritance, mutation, and simulation. | 21, 28.2, 28.11 | Planned | 10 | Unit + statistical tests |
| REQ-TGROUP | Nested template groups, weights, cross-template blending, generation simulation, and structure preservation. | 22 | Planned | 11 | Unit + simulation tests |
| REQ-TFROMIND | Create statistical templates from individuals without conflating templates and genomes. | 23 | Planned | 10 | Unit tests |
| REQ-POLICY | Explicit policy categories, granularity, metadata/tag-aware inputs where opted in, and outputs. | 25 | Planned | 1 onward | Unit + coverage tests |
| REQ-RTEST | First-class immutable-input resource test definitions, fixtures, operations, assertions, diagnostics, transient reports, and runners; core excludes golden-sample approval workflows. | 26, 27 | Planned | 12-13 | Self-tests + integration |
| REQ-RANDOM | Deterministic execution, separated random streams, reproducibility packets, and statistical tolerances. | 26.13-26.14, 26.24, 26.26 | Planned | 4, 12 | Determinism + statistical tests |
| REQ-VALIDATE | Resource graph, reachability, policy coverage, invariants, negative cases, and required baseline content tests. | 26.19-26.22, 26.39 | Planned | 1, 12-13 | Validation + resource tests |
| REQ-SERIAL | Stable JSON and binary formats at multiple granularities, including complete-system export/import, versions, variants, templates, tests, and failure packets. | 31.1-31.3 | Planned | 2 onward; finalized 14 | Round-trip + compatibility |
| REQ-STORAGE | Core has no permanent storage; optional JSON-file, binary-file, and SQLite modules depend on core, provide atomic logical writes/index aids where applicable, and are not content management systems for external resource definitions. | 31.4-31.8 | Planned | 14 | Integration tests |
| REQ-GODOT | Optional Godot runtime adapter consumes core APIs without redefining genetics behavior; Godot editor plugin is separate and deferred. | Project scope injection | Planned | 15 runtime; editor deferred | Build + adapter tests |

## Incremental implementation plan

The next five slices are deliberately detailed. Slices 5 and later are progressively less specific and must be refined before entering **In progress**. Refinement may split a slice, but it must preserve requirement IDs and record scope changes above.

### Slice 0 - Project foundation

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

**Objective:** Model an individual genome and its permanent/transient lifecycle without yet implementing full expression or reproduction.

**Deliverables**

- Implement ranked allele entries/sets and genome group state sufficient for categorical and numeric allele values.
- Implement individual identity references as opaque external IDs; do not encode personhood or genealogy.
- Implement immutable genome versions containing system-definition version, stable version ID, parent-version reference where applicable, and genome state.
- Implement a mutable or replacement-based current genome copy that is explicitly derived from a permanent version.
- Implement controlled commit behavior that creates a new immutable version; repeated temporary changes do not auto-commit.
- Implement historical comparison metadata sufficient for later repair and runtime history diagnostics without committing to full mutation history shape.
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

**Requirements advanced:** REQ-MODEL, REQ-GENOME, REQ-GENE, REQ-VERSION, REQ-SERIAL, REQ-STORAGE.

### Slice 3 - Group completeness, body-plan activation, and basic expression

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

**Requirements advanced:** REQ-GROUP, REQ-BODY, REQ-EXPR, REQ-EXTERNAL, REQ-GENE.

### Slice 4 - Deterministic inheritance and ordinary reproduction

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

**Requirements advanced:** REQ-PLOIDY, REQ-REPRO, REQ-RANDOM, REQ-SERIAL.

### Slice 5 - Mutation, commit policy, repair, and acquired heritable changes

Refine before implementation. Expected outcomes are policy-controlled mutation events across allele, numeric, copy-number, and structural targets; protected-target enforcement; explicit temporary/current/permanent commit behavior; immutable version creation; repair/reversion; and external-event entry points that do not import metaphysical logic into core.

**Requirements targeted:** REQ-MUTATION, REQ-VERSION, REQ-ACQUIRED, REQ-GROUP.

### Slice 6 - Non-ploidal inheritance and traces

Refine before implementation. Expected outcomes are typed non-ploidal objects, weighted transmission and update behavior, trace persistence/activation/replacement/degradation, and serialization/versioning integration.

**Requirements targeted:** REQ-NONPLOID, REQ-TRACE, REQ-REPRO, REQ-SERIAL.

### Slice 7 - Compatibility, development, and expanded reproduction

Refine before implementation. Expected outcomes are compatibility layers and fertile/sterile/inviable results, hybrid morphology inputs, developmental and gestational sequences, maternal effects, nonstandard reproduction modes, and germline/generation-site behavior.

**Requirements targeted:** REQ-COMPAT, REQ-DEVELOP, REQ-REPRO.

### Slice 8 - Advanced expression, generated complements, and runtime variants

Refine before implementation. Expected outcomes are the remaining expression patterns, policy-controlled generated genetic complements, and deterministic runtime-derived body-plan variants with validation, versioning, and serialization.

**Requirements targeted:** REQ-GENE, REQ-GROUP, REQ-BODY, REQ-VARIANT, REQ-EXPR.

### Slice 9 - Mosaicism and chimerism

Refine before implementation. Expected outcomes are regional genome assignment, contextual expression and inheritance from regions/sites, and explicit distinction between integrated body-plan variants and genetically distinct chimeric material.

**Requirements targeted:** REQ-MOSAIC, REQ-EXPR, REQ-REPRO.

### Slice 10 - Population templates

Refine before implementation. Implement immutable statistical profiles, deterministic genome sampling, blending, forward simulation, biased inheritance/mutation hooks, and template creation from individuals.

**Requirements targeted:** REQ-TEMPLATE, REQ-TFROMIND, REQ-RANDOM, REQ-SERIAL.

### Slice 11 - Template groups and nested populations

Refine before implementation. Implement nested template groups, group/template weights, cross-template blend rates, structure-preserving generation simulation, and immutable version outputs.

**Requirements targeted:** REQ-TGROUP, REQ-TEMPLATE.

### Slices 12-13 - Resource testing framework

Refine and likely subdivide before implementation. Build designer-authored test resources, fixtures, operations, assertions, custom extension points, validation/reachability/policy coverage, deterministic simulation and statistical tests, diagnostics/reproducibility packets, tags, severity, snapshots, fuzz/matrix execution, isolation, and runtime-safe subsets.

**Requirements targeted:** REQ-RTEST, REQ-VALIDATE, REQ-RANDOM, REQ-SERIAL.

### Slice 14 - Serialization hardening and optional storage modules

Refine before implementation. Finalize compatibility contracts and granular JSON/binary formats, including complete-system serialization, then add non-core JSON-file, custom-binary-file, and SQLite storage modules with migration orchestration, atomic logical writes, non-authoritative indexing, and test-fixture support.

**Requirements targeted:** REQ-SERIAL, REQ-STORAGE, REQ-ID.

### Slice 15 - Godot runtime adapter

Refine against the selected Godot/.NET versions. Add a thin runtime adapter for Godot runtime workflows, package import/export handoff, diagnostics, and engine-facing conversions while preserving a Godot-free core.

**Requirements targeted:** REQ-GODOT, REQ-MODE, REQ-SERIAL.

### Deferred - Godot editor plugin

A separate Godot editor plugin may later support authoring workflows, resource editing integration, editor-side test execution, diagnostics display, and golden-sample approval workflows. This is not part of core or the runtime adapter slice.

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

### Not yet implemented

- All C# projects and runtime/domain behavior.
- All core and optional serialization/storage modules.
- All Godot integration.
- All implementation and resource tests.

### Recorded simplifications

- Slice 3 starts with dominance hierarchy, codominance, and numeric midpoint expression; advanced expression is deferred to Slice 8.
- Slice 4 starts with ordinary deterministic reproduction and a compatibility-policy stub; nonstandard reproduction and full compatibility are deferred to Slice 7.
- Preliminary Slice 2 serialization covers only then-existing models; complete format stabilization is deferred to Slice 14.
- Later slices are intentionally outcome-level under incremental refinement and cannot start until their deliverables, acceptance criteria, and tests are expanded.

## Test accounting

### Tests present

- None. The repository had no implementation or test project at ledger creation.

### Requirements with tests

- None yet.

### Requirements without tests

- All requirement families in the requirements register.

### Test layers required by the project

- **Implementation tests:** Verify C# engine behavior, invariants, codecs, adapters, and storage modules.
- **Resource tests:** Verify designer-authored genetics resources and policies using the first-class framework required by Section 26.
- **Integration tests:** Verify startup/freeze ordering, cross-policy workflows, serialization compatibility, storage boundaries, and Godot adapter behavior.
- **Simulation/statistical tests:** Verify seeded reproducibility and distribution behavior within explicit tolerances.

## Open decisions and risks

| ID | Decision or risk | Needed by | Current handling |
|---|---|---|---|
| OPEN-001 | Exact .NET target framework and supported Godot version range. | Slice 0 | Evaluate current Godot C# compatibility before project creation. |
| OPEN-002 | Binary format design and compatibility strategy. | Slice 2 | Use a versioned preliminary codec, then stabilize in Slice 14. |
| OPEN-003 | Definition immutability mechanism: immutable records/collections, generated snapshots, or both. | Slice 1 | Prototype against retained-reference mutation tests. |
| OPEN-004 | Policy extensibility model and safe serialization of policy configuration. | Slice 1 | Separate policy identity/configuration from executable host implementation. |
| OPEN-005 | Numeric value representation and deterministic arithmetic guarantees. | Slice 2-3 | Decide before numeric expression becomes public format. |
| OPEN-006 | Random algorithm and stream-derivation contract. | Slice 4 | Select a specified cross-runtime deterministic algorithm; do not rely on `System.Random` behavior as a serialized contract. |
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
