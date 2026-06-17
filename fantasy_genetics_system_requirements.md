# Fantasy Genetics System Requirements Specification

## 0. Document status

This document consolidates the design requirements for a fantasy genetics system intended for use in games, simulations, and fantasy-world authoring tools. It is implementation-language agnostic.

The system is designed as a **hard fantasy genetics framework**: it uses real genetics as a conceptual baseline, but abstracts away molecular biology and adds constrained support for fantasy morphologies, lineages, bloodlines, shapeshifting, hybridization, mutation, traces, population profiles, authoring workflows, and runtime-safe execution.

This document describes system requirements, concepts, data structures, behavior, invariants, operating modes, serialization requirements, storage boundaries, and resource testing requirements. It does not prescribe implementation language, UI, networking model, database design, or engine integration. The core library defines stable serialization formats but does not directly manage permanent storage.

---

# 1. Purpose and scope

## 1.1 Purpose

The system models heritable fantasy physiology, morphology, lineage traits, and population-level genetic profiles using real genetics as a conceptual baseline while allowing controlled departures for fantasy settings.

The system is meant to support fantasy tropes such as:

- dormant dragon bloodlines in otherwise normal-looking individuals
- shapeshifter ancestry
- alternate inherited body plans
- viable, sterile, or inviable hybrids
- magical gene splicing
- inherited curses or blessings when a setting treats them genetically
- mythic throwback descendants
- trace inheritance from historically significant ancestors
- inherited physiology not visible in ordinary morphology
- fantasy species with nonstandard reproduction
- population-level genetic simulation without explicit genealogy

The system should enable a designer to define a fantasy genetic system that is consistent, testable, tunable, and capable of producing emergent consequences.

## 1.2 Non-goals

The system is not intended to simulate molecular biology.

It does not directly model:

- DNA base pairs
- RNA
- mRNA
- transcription
- translation
- protein folding
- protein misfolding
- molecular mutation mechanisms
- cellular biochemistry
- full embryological simulation
- metabolic cost accounting
- ecological selection unless supplied externally

Instead, it models a higher-level inheritance and expression layer that should feel as though it could sit on top of a deeper biological simulation.

## 1.3 Out-of-scope metaphysics

The system does not define:

- identity
- soul mechanics
- reincarnation mechanics
- spirit identity
- mantle inheritance
- divine legitimacy
- prophecy
- fate
- social caste
- legal species identity
- blood purity ideology
- political legitimacy
- true names
- social inheritance
- family-name metaphysics

Such systems may interact with the genetics system by supplying external context, activation triggers, mutation events, or authored genetic changes. They are not part of the genetics system itself.

## 1.4 Core distinction

The system distinguishes between:

- **genome**: what inheritable material exists
- **expression**: what is currently active
- **body plan**: how genetic material becomes a body
- **mutation**: how the genome changes
- **versioning**: how permanent changes are recorded
- **template**: how populations are statistically represented
- **external systems**: why special events happen

---

# 2. Core design principles

## 2.1 Genetics is a property of an individual

A genome belongs to an individual, but the genome does not define the individual’s identity.

Identity, personhood, spirit, soul, reincarnation status, possession identity, absorbed-twin identity, legal identity, and social recognition are external to the genetics system.

## 2.2 Expression is distinct from inheritance

An individual may carry genetic information that is not currently expressed.

An individual’s visible body is not necessarily the full extent of their genetic inheritance.

For example, an individual may appear human while carrying dormant genetic groups that represent partial or complete dragon morphology.

## 2.3 Body plans are developmental interpretations

A body plan defines how genetic groups are interpreted into morphology, physiology, developmental sequence, reproduction, compatibility, and expression.

The same genome may support multiple possible body plans.

Authored body-plan definitions provide stable interpretation rules. Runtime-created body-plan variants may also exist as instance state when produced by frozen mutation, transformation, construction, or expression policies. This prevents designers from needing to pre-author every possible combination of modular features such as horns, wings, fins, gills, tails, extra limbs, or altered hindquarters.

A body plan may be:

- active
- dormant
- incomplete
- temporarily expressed
- permanently made primary
- partially expressed
- unavailable due to missing groups
- unavailable due to insufficient ploidy
- unavailable due to failed compatibility or activation requirements

## 2.4 Groups are structural inheritance units

Genes are organized into groups. Groups are the primary unit for determining whether an individual has or lacks the genetic material required for a body plan, physiology, developmental path, or inheritance subsystem.

Groups may be shared by multiple body plans.

Example: a full-sized dragon body and a shoulder-dragon body may share many of the same groups while differing in scale, developmental sequence, reproductive mode, or expression rules.

## 2.5 System mechanics are opt-in

All special mechanics are design-time opt-in.

The system may support a wide range of inheritance, mutation, activation, linkage, expression, and population behaviors, but no gene, group, body plan, template, or setting is required to participate in every subsystem.

Participation is usually declared at gene or group granularity. Specific values or outcomes are usually allele-level.

## 2.6 External causes may write into the system

Curses, blessings, divine marks, lycanthropy, vampirism, magical infection, possession effects, rituals, transformations, alchemical alterations, and other setting-specific causes may alter genetics if the setting defines them as doing so.

When such effects permanently alter genetic state, they use the mutation and versioning system.

The genetics system does not decide whether such effects should be genetic. It only provides mechanics for representing them if the setting defines them as genetic.

## 2.7 Designer authority

The system should protect its own invariants, but it should not second-guess the setting designer’s authored design choices. The designer may deliberately allow unusual, dangerous, unstable, or bizarre genetic effects if those effects are valid under declared policies.

---

# 3. Operating modes

## 3.1 Requirement

The core system must have two distinct operating modes:

1. **Design mode**
2. **Runtime mode**

The operating mode is chosen at startup and remains fixed for the lifetime of the core system instance.

A running instance must not switch modes. To change modes, a new system instance must be created.

## 3.2 Design mode

Design mode is used to create, edit, validate, test, mutate, simulate, migrate, and export a genetics system definition.

Design mode is intended for:

- editor tools
- authoring tools
- balancing tools
- simulation tools
- resource migration
- test harnesses
- content validation
- population template generation
- developer experimentation
- system construction

In design mode, the **system definition** is mutable under controlled rules.

Design mode may allow creation, modification, replacement, or removal of:

- genes
- alleles
- dominance hierarchies
- groups
- group dependencies
- linkage definitions
- body plans
- developmental phases
- expression policies
- inheritance policies
- mutation policies
- versioning policies
- repair policies
- trace definitions
- non-ploidal inheritance object definitions
- compatibility rules
- reproductive policies
- population templates
- template groups
- test resources
- validation rules
- resource metadata
- migrations

Design mode may also allow **system-definition mutation**: changes to the authored genetics system itself.

## 3.3 Runtime mode

Runtime mode is used by the actual game or simulation after the genetics system has been authored and loaded.

Runtime mode prioritizes:

- determinism
- safety
- stable interpretation
- reproducibility
- performance
- predictable resource identity
- compatibility with saved genomes
- prevention of accidental authoring changes

In runtime mode, the genetics system definition is frozen immediately after load and validation.

After freeze, the system definition is immutable.

Frozen resources include:

- gene definitions
- allele definitions
- group definitions
- authored body-plan definitions
- developmental phase definitions
- expression rules
- inheritance rules
- mutation policies
- versioning policies
- repair policies
- compatibility rules
- reproductive policies
- trace definitions
- non-ploidal inheritance object definitions
- population template schemas
- template group schemas
- resource identifiers
- linkage definitions
- dependency definitions
- body-plan variant construction rules

Runtime freeze applies to authored definitions and interpretation rules, not to every possible expressed or mutated body configuration. Runtime may create derived body-plan variants as instance state if frozen policies define how those variants are constructed, interpreted, versioned, serialized, and validated.

Runtime may still create and modify **instance state** governed by the frozen definition.

Runtime may allow, if frozen policies permit:

- individual reproduction
- offspring genome creation
- genome mutation
- current genome copy creation
- permanent genome version creation
- trace inheritance
- trace activation
- body-plan activation
- runtime body-plan variant creation by frozen policy
- generated complement creation
- repair and reversion
- population template sampling
- runtime population simulation outputs

Runtime must not allow:

- adding a new gene definition
- adding a new allele definition
- changing authored body-plan definitions
- changing body-plan variant construction rules
- changing policy definitions
- changing linkage definitions
- changing group dependencies
- changing mutation rules
- changing expression rules
- altering the meaning of existing resource identifiers
- changing how existing genomes are interpreted

Runtime-created body-plan variants must be interpreted using frozen definitions and policies. They are instance data, not edits to the authored system definition.

The core rule is:

> Runtime may change genetic state and derived body-plan state inside individuals. Runtime may not change the rules that interpret genetic state.

## 3.4 Startup sequence: design mode

A design-mode startup sequence should generally be:

1. Select design mode.
2. Load existing system definition, if any.
3. Load editable resources.
4. Apply design migrations, if needed.
5. Validate resource graph.
6. Enable authoring, mutation, simulation, and testing tools.
7. Allow save/export of revised system definition.
8. Optionally export a validated runtime package.

## 3.5 Startup sequence: runtime mode

A runtime-mode startup sequence should generally be:

1. Select runtime mode.
2. Load runtime package/system definition.
3. Validate package and resource graph.
4. Apply explicitly packaged runtime migrations, if any.
5. Freeze system definition immediately after successful load.
6. Load saved game, genome, individual, and template state.
7. Run gameplay or simulation using frozen definitions.

Runtime freeze occurs before normal runtime operations.

## 3.6 System-definition versions

System-definition versions and genome versions are distinct.

A system-definition version describes the rules and resources used to interpret genomes.

A genome version describes an individual’s permanent genetic state under a system definition.

Saved genomes should record the system-definition version they were created under.

If a saved genome is loaded under a different system-definition version, migration or compatibility validation may be required.

---

# 4. Core terminology

## 4.1 Individual

A single organism, person, creature, or other genetically modeled entity.

An individual may have:

- one or more genome versions
- zero or more current/transient genome states
- one or more possible body plans
- one or more active expression states
- optional mosaic/chimeric regions
- optional external state not modeled by genetics

The individual is not identical to their genome.

## 4.2 Genome

A structured collection of heritable information belonging to an individual.

A genome may contain:

- gene groups
- genes
- allele sets
- non-ploidal inheritance objects
- body-plan-relevant groups
- compatibility-relevant genes
- mutation-relevant genes
- expression-control genes
- optional trace objects
- optional ranked allele sets
- optional mosaic/chimeric region assignments

A genome may be immutable as a permanent version or mutable as a current working copy.

## 4.3 Genome version

An immutable snapshot of an individual’s genome at a point of permanent change.

Genome versions are used for:

- repair
- reversion
- historical comparison
- determining original state
- distinguishing temporary expression from permanent mutation
- allowing policy-specific restoration
- audit and diagnostics

A permanent genome mutation creates a new genome version unless policy states otherwise.

## 4.4 Current genome copy

A transient or provisional genome state that may differ from the most recent permanent genome version.

A current genome copy may be created by temporary or provisional effects.

It may or may not later be committed as a new genome version, depending on mutation and versioning policy.

## 4.5 Gene

A functional hereditary unit.

The system uses “gene” rather than “locus” because the system models functional inheritance units, not physical genomic positions.

A gene may represent:

- visible trait
- physiological trait
- control mechanism
- compatibility value
- expression switch
- developmental instruction
- mutation policy input
- group dependency value
- ploidy-related value
- numeric inherited value
- flag-like inherited state, where appropriate

Genes are not necessarily molecular genes.

## 4.6 Allele

A possible value or variant of a gene.

An allele may represent:

- categorical trait value
- numeric value
- null/damaged/deleted state
- control state
- reference to another group
- dominance value
- linkage value
- compatibility value
- mutation-policy modifier
- developmental instruction variant

The term is an abstraction. Some system alleles may be more complex than biological alleles.

## 4.7 Allele set

A ranked collection of alleles for a gene or group, contributed through one or more genetic parent roles.

For ordinary diploid inheritance, a gene normally has two allele entries.

For polyploid body plans, a gene may require three or more allele entries.

For shared groups used by multiple ploidy models, allele sets are ranked so that expression under lower ploidy arity remains unambiguous.

## 4.8 Group

A named collection of genes and/or inheritance objects.

Groups define functional units of body-plan, physiology, development, compatibility, or inheritance structure.

Groups may be:

- required by body plans
- shared across body plans
- dormant
- active
- partially present
- complete
- linked to other groups
- dependent on other groups
- mutable or protected by policy
- ploidal or non-ploidal

## 4.9 Common group

A group shared across multiple body plans.

The common group may include baseline traits such as:

- pigmentation
- sex-determining systems
- broad physiological defaults
- other traits interpreted by multiple morphologies

A common group may be accessed by body plans with different ploidy requirements.

## 4.10 Non-ploidal inheritance object

A heritable object not interpreted through paired or polyploid allele-set logic.

Non-ploidal objects may include:

- flags
- counters
- accumulators
- traces
- inherited memory archives
- lineage markers
- externally inserted curse/blessing markers
- weighted tags
- non-paired magical inheritance structures

Non-ploidal objects may have:

- presence/absence
- values
- weights
- transmission rules
- activation rules
- mutation rules
- decay/update rules
- expression targets

## 4.11 Trace

A special kind of non-ploidal inheritance object representing a set of alleles or inheritance information associated with a historically significant ancestor.

A trace:

- does not participate in ordinary allele recombination
- may persist through a lineage
- may have weighted heritability
- may activate under external or policy-defined conditions
- may replace allele sets inherited from actual parents
- may interact with body plans, compatibility, and mutation
- may be created, modified, degraded, or lost by policy

Traces support dramatic throwback descendants and mythic recurrence without requiring ordinary genetics to preserve exact ancestral genomes over long timescales.

## 4.12 Body plan

A named developmental and physiological interpretation of genetic groups.

A body plan may be an authored definition or a runtime-derived variant constructed under frozen policies.

A body plan may define:

- required groups
- optional groups
- shared groups
- ploidy requirements
- developmental stages
- gestational requirements
- reproductive mode
- morphology
- physiology
- sex-determination interpretation
- compatibility requirements
- expression rules
- activation requirements
- dormancy rules
- mutation participation
- allowed generated-complement behavior

## 4.13 Runtime-derived body-plan variant

A runtime-derived body-plan variant is an instance-level body-plan configuration created during runtime by frozen policies.

It is used for bodies that are valid under system rules but were not exhaustively authored as named body-plan definitions.

A runtime-derived variant may represent modular or policy-created changes such as:

- added wings
- added horns
- added fins or gills
- altered tails
- additional limbs
- replacement hindquarters
- generated complement morphology
- mutation-created morphology packages

A runtime-derived variant is not a system-definition edit. It must reference frozen definitions, groups, genes, policies, and construction rules.

Runtime-derived variants may be stored in genome versions, current genome copies, individuals, templates, or save data as permitted by policy.

## 4.14 Developmental phase

A stage in a body plan’s development.

A body plan may define a sequence of developmental phases such as:

- embryo
- larva
- egg stage
- pouch stage
- neonate
- juvenile
- adolescent
- adult
- metamorphic stage
- post-metamorphic form

The exact granularity is setting-defined.

## 4.15 Mutation

A genome modification event.

In this system, mutation is a broad category and may include:

- ordinary random mutation
- inheritance transcription error
- developmental mutation
- magical alteration
- curse insertion
- blessing insertion
- shapeshifting fill-in
- cross-body-plan copying
- value drift
- conversion or overwrite
- structural group change
- repair or reversion
- externally authored genome rewrite

Temporary expression changes are not mutations unless policy commits them as genome modifications.

## 4.16 Mutation policy

A policy defining how mutation events target, select, modify, version, and commit genome changes.

Mutation policies may refer to:

- genes
- groups
- allele values
- body plans
- current expression state
- genome versions
- non-ploidal objects
- external event type
- timing/origin
- scope
- filters
- value-selection modes
- repair targets
- versioning rules

## 4.17 Population genetic profile template

A named immutable statistical profile used to generate random genomes without explicit ancestry.

A template contains ranked allele rates, group rates, non-ploidal object rates, optional linkages, and optional policy references.

Templates can be blended, versioned, simulated forward, nested into groups, and created from individual genomes.

---

# 5. Gene and allele behavior

## 5.1 Supported expression patterns

A gene may support one or more expression patterns, including:

- dominant/recessive
- dominance hierarchy
- codominance
- incomplete dominance
- blending
- continuum midpoint
- polygenic contribution
- epistasis
- pleiotropy
- penetrance
- variable expressivity
- sex-linked expression
- sex-limited expression
- parent-of-origin expression
- timing-dependent expression
- epigenetically modified expression
- mosaic/chimeric regional expression

A gene is not required to support all patterns.

## 5.2 Dominance hierarchy

A gene may define a ranked hierarchy of alleles.

Higher-ranked alleles may mask lower-ranked alleles.

Example pattern:

- metallic silver masks bright orange
- bright orange masks brown
- brown masks blue

The system must allow dominance to be more than binary dominant/recessive behavior.

## 5.3 Codominance

A gene may allow multiple alleles to express simultaneously.

Codominance does not blend values. It expresses multiple values together.

## 5.4 Blending and continuum expression

A gene may define expression as a calculated value between allele values.

A simple model may use the midpoint between two alleles.

More complex models may use weighted averages, clamped ranges, nonlinear interpolation, or policy-defined functions.

## 5.5 Polygenic traits

A trait may be affected by multiple genes.

Polygenic traits may produce:

- distributions
- thresholds
- summed values
- averaged values
- weighted contribution
- categorical outcomes from quantitative inputs

Some incomplete dominance behavior may be represented through polygenic or continuum mechanics.

## 5.6 Epistasis

A gene may affect the expression, availability, interpretation, or activation of another gene.

Epistasis may include:

- suppressing expression
- enabling expression
- changing dominance behavior
- changing allele interpretation
- redirecting group reference
- modifying penetrance
- modifying expressivity
- changing developmental timing
- changing mutation policy parameters

Epistasis is a gene-level interaction and may use allele-level values.

## 5.7 Pleiotropy

A gene may affect multiple traits or systems.

For example, a baseline color gene in a shared common group may affect coloration across multiple body plans.

Pleiotropy allows a single gene to produce multiple downstream effects without modeling each as a separate gene.

## 5.8 Penetrance

A gene may be present but not express in all individuals carrying the relevant genotype.

Penetrance may be:

- deterministic
- probabilistic
- condition-dependent
- epigenetically modified
- externally activated
- developmental-phase-dependent

Penetrance differs from epistasis in that penetrance describes whether a genotype manifests, while epistasis describes gene-to-gene interaction.

## 5.9 Variable expressivity

A gene may express with variable intensity, form, degree, or configuration even when the relevant genotype is present.

Variable expressivity may depend on:

- allele values
- modifier genes
- epigenetic state
- developmental phase
- external conditions
- body plan
- mutation policy
- random expression roll
- trace activation

## 5.10 Linkage

Genes or inheritance objects may be linked so that they tend to be inherited together.

Linkage may occur:

- within a group
- between a deciding gene and a group dependent on that gene

Arbitrary cross-genome linkage is not assumed.

Variable linkage should normally be represented by system-wide allele values interpreted by group definitions or body-plan definitions, rather than by bespoke linkage metadata inside individual genomes.

## 5.11 Weighted heritability

Alleles and non-ploidal inheritance objects may have transmission weights.

A weighted inheritance selection replaces ordinary equal-probability selection.

This supports:

- ordinary 50/50 inheritance
- biased inheritance
- meiotic drive-like effects
- segregation distortion-like effects
- fragile alleles
- strongly persistent alleles
- trace persistence
- mutation-sensitive transmission

## 5.12 Numeric genes

A gene may hold numeric values.

Numeric values may be used for:

- compatibility distance
- bloodline strength
- accumulation
- decay
- mutation rates
- expression thresholds
- body-plan scaling
- trace strength
- inherited memory magnitude
- developmental timing

Numeric genes may be modified by mutation value-selection modes or inheritance update functions.

---

# 6. Groups

## 6.1 Group structure

A group may contain:

- genes
- subgroups
- non-ploidal objects
- linkage definitions
- dependency definitions
- body-plan references
- expression policies
- mutation participation flags
- inheritance participation flags
- ploidy requirements
- ranked allele-set rules

A group should be treated as a coherent functional unit for body-plan compatibility and inheritance generation.

## 6.2 Group presence

An individual may have:

- no copy of a group
- partial group information
- complete group information
- dormant group information
- active group information
- generated complement group information
- mosaic/chimeric group presence by region

A body plan may require a group to be complete before full expression is possible.

## 6.3 Group sharing

Multiple body plans may refer to the same group.

A shared group may express differently depending on active body plan.

For example, the same pigmentation group may be interpreted differently in a human body, dragon body, and shoulder-dragon body.

## 6.4 Group dependency

A group may depend on another group or on a deciding gene within another group.

Dependency may determine:

- whether a group is active
- whether a group is inherited
- whether a group is interpreted
- which body plans may use it
- what ploidy model applies
- what linkage applies
- whether mutation policies target it
- whether it is dormant or expressed

Dependency chains are allowed.

## 6.5 Group completeness

A group may be considered complete if it has the required allele sets, ploidy, and dependency conditions for a given body plan.

Completeness is context-dependent.

A group may be complete for one body plan and incomplete for another.

## 6.6 Generated complement groups

When an individual expresses or shapeshifts into a body plan for which they have only partial genetic information, the system may generate missing complementary genetic material if policy allows.

Generated complement material:

- completes missing allele sets or groups
- persists for that individual if policy says so
- may be current-copy-only or permanently versioned
- may or may not be heritable
- is normally heritable only when reproduction occurs through a body using that generated material
- may be treated as a mutation event

The same mechanism may apply when an individual with no prior genetic information for a body plan shapeshifts into that body.

---

# 7. Body plans

## 7.1 Body-plan definition

A body plan defines biological and developmental interpretation of genetic material.

A body plan may specify:

- required groups
- optional groups
- shared common groups
- ploidy model
- developmental phase sequence
- reproductive mode
- gestational requirements
- compatibility prerequisites
- expression rules
- activation rules
- dormancy rules
- mutation participation
- generated-complement rules
- morphology
- physiology
- sex-determination interpretation
- scaling rules
- linked body plans

## 7.2 Active body plan

An individual has at least one currently active body plan.

The active body plan determines current morphology and physiology.

Changing active body plan is not itself a mutation.

## 7.3 Dormant body plan

An individual may carry genetic groups sufficient for a body plan that is not currently active.

Dormant body plans may be:

- activatable by magic
- inaccessible
- incomplete
- partially expressible
- inheritable
- targetable by mutation policy
- protected by mutation policy

## 7.4 Primary morphology

An individual may have a primary body plan.

A permanent change of primary morphology may be represented as:

- activation state change, if genome is unchanged
- mutation, if genome is permanently altered
- external identity/metaphysics event, if outside the genetic system
- a combination of these

## 7.5 Temporary shapeshifting

Temporary shapeshifting changes expression state but does not create new genome versions unless policy says otherwise.

Repeated temporary transformations must not automatically produce genome version spam.

## 7.6 Innate shapeshifting

Innate shapeshifting may be modeled as genes or groups that allow controlled activation of multiple body plans.

The ability itself is genetic if the setting defines it as genetic.

## 7.7 Body-plan family

Multiple body plans may share many groups and differ mainly in:

- scale
- developmental sequence
- reproductive mode
- activation rules
- expression control
- ploidy interpretation
- physiology details

For example, a full dragon and shoulder dragon may be separate body plans with shared groups.

## 7.8 Runtime-derived body-plan variants

The runtime system must support mutation or construction of body-plan variants as instance state when frozen policies allow it.

This requirement exists because fully freezing body-plan definitions without any runtime variant mechanism would force designers to pre-author every possible combination of modular morphology features. Features such as horn number and placement, wing count and type, fins, gills, extra limbs, tails, altered hindquarters, and similar morphology packages can create combinatorial explosion if every combination must be declared as a separate authored body plan.

A runtime-derived body-plan variant may be created by:

- mutation policy
- generated complement policy
- shapeshifting completion policy
- magical gene splicing policy
- acquired heritable state policy
- external event using frozen mutation/construction rules
- template or genome generation policy, if permitted

A runtime-derived body-plan variant may add, remove, replace, or reinterpret morphology packages, provided that the operation is valid under frozen definitions and policies.

Examples of valid runtime-derived body-plan variants include:

- a ferret gaining two pairs of bat wings
- a ferret gaining a unicorn horn
- a ferret gaining the back end of a seal
- a body gaining fins or gills
- a body gaining additional arms or legs
- a body gaining altered tail morphology

A runtime-derived body-plan variant does not automatically make the individual a chimera. Chimerism requires multiple genetically distinct material lines or regionally distinct genetic material. A body-plan variant may be a single integrated morphology interpreted from one genome state.

Runtime-derived body-plan variants must have:

- stable identity within the owning genome or individual state
- deterministic construction from frozen policies and input data
- serializable structure
- validation against frozen body-plan construction rules
- explicit versioning behavior when permanent
- clear distinction from temporary expression state
- clear distinction from authored body-plan definitions

A runtime-derived body-plan variant may be temporary, current-copy-only, or permanent depending on mutation and versioning policy.

If permanent, the variant must be recorded as part of the relevant genome version or individual genetic state.

If temporary, it must not create a genome version unless policy commits it.

---

# 8. Development and gestation

## 8.1 Developmental sequencing

A body plan may define a sequence of developmental phases.

Each phase may have:

- required groups
- active genes
- inactive genes
- morphology
- physiology
- environmental requirements
- transition conditions
- compatibility checks
- mutation susceptibility
- expression rules

This supports creatures whose juvenile, larval, neonatal, or metamorphic forms differ strongly from adult form.

## 8.2 Gestational compatibility

Gestational and early developmental compatibility are prerequisites for general reproductive compatibility.

If an embryo cannot be gestated, hatched, nourished, carried, or survive required early phases, later fertility compatibility is irrelevant.

## 8.3 Post-birth developmental requirements

The system may model post-birth developmental dependencies when relevant.

Examples include:

- marsupial pouch stages
- helpless neonates
- kittens opening eyes days after birth
- larval feeding requirements
- external metamorphosis
- egg incubation conditions

The system does not require fine-grained separation between late gestation and early post-birth development unless the setting needs it.

## 8.4 Maternal and gestational effects

Maternal developmental influences are resolved at creation in this system.

If a gestational parent’s state affects the child, the effect is applied as part of child creation or as a separate child-affecting epigenetic condition.

The genetics system does not model ongoing direct interaction between parental genomes during development.

---

# 9. Compatibility

## 9.1 Compatibility layers

Reproductive success may require compatibility at multiple layers:

1. mating compatibility
2. gamete/contribution compatibility
3. gestational compatibility
4. developmental compatibility
5. general genetic compatibility
6. fertility of offspring
7. reproductive maturity compatibility

A setting may compress or expand these layers as needed.

## 9.2 Compatibility genes

Compatibility may be represented by genes.

A compatibility gene may hold a numeric or categorical value.

One suggested model uses bit-distance between inherited compatibility allele values:

- below threshold A: viable and fertile
- between threshold A and B: viable but sterile
- above threshold B: inviable

The system does not require chromosome-count mismatch to explain sterile hybrids.

## 9.3 Fertile hybrids

A hybrid may be fertile if compatibility thresholds and developmental requirements are satisfied.

## 9.4 Sterile hybrids

A hybrid may be viable but sterile if compatibility values fall within a defined sterile range.

This supports mule-like results without modeling literal chromosome mismatch.

## 9.5 Inviable offspring

A potential offspring may be inviable if compatibility values exceed viability thresholds or gestational/developmental prerequisites fail.

## 9.6 Hybrid morphology

Hybrid morphology is not assumed to be a simple visual midpoint.

A hybrid may express:

- one parent’s body plan
- a blended body plan
- a distinct predesigned hybrid body plan
- partial groups from multiple body plans
- dormant body-plan information
- generated complement material
- designer-defined chimeric or spliced structure

The system does not assume aesthetic blending.

---

# 10. Ploidy and ranked allele sets

## 10.1 Default ploidy

Diploid inheritance is the default.

Most ordinary genes require two allele entries unless otherwise specified.

## 10.2 Variable ploidy by group or body plan

Ploidy may vary by group or body plan.

A body plan may require:

- haploid-like interpretation
- diploid interpretation
- triploid interpretation
- tetraploid interpretation
- higher ploidy
- mixed ploidy by group

## 10.3 Shared groups with multiple ploidy models

If a group is accessed by body plans with different ploidy arities, full expression of the highest-arity body plan requires the highest number of allele sets.

Lower-arity body plans use ranked subsets of the allele sets.

## 10.4 Ranked allele-set interpretation

When a group contains more allele sets than a body plan requires, the body plan uses the highest-ranked required subset unless policy defines another unambiguous selection rule.

Ranking may be determined by:

- parental role
- conception policy
- random assignment
- body-plan-specific ranking
- inherited ranking
- setting-defined role hierarchy

The system requires ranking to be unambiguous at expression time.

## 10.5 Multi-parent contribution

A body plan may define more than two genetic parent roles.

Different groups may require different numbers of contributing roles.

For example, a mostly diploid organism may have a triploid group where the gestational parent contributes a third allele set.

## 10.6 Same individual in multiple parent roles

In parthenogenesis or self-reproduction, the same individual may fill multiple parent roles.

Each role performs independent allele selection unless the reproductive mode specifies direct copying.

---

# 11. Reproduction

## 11.1 Reproduction inputs

A reproduction event receives genetic contribution inputs from one or more parent roles.

A parent role may be:

- genetic parent
- gestational parent
- self-parent role
- donor role
- magical contributor
- generated-material contributor
- template-generated contributor
- external source

Roles are defined by the body plan or reproductive policy.

## 11.2 Parent genomes do not directly interact

During reproduction, parents contribute allele sets or inheritance objects as inputs.

Parent genomes do not directly negotiate or interact during development.

All resulting interactions occur inside the offspring creation process.

## 11.3 Allele selection

For each gene requiring contribution, the system selects allele values according to:

- ploidy
- parent role
- allele-set ranking
- heritability weights
- linkage
- group dependency
- mutation policy
- trace activation
- external reproductive policy

## 11.4 Group selection

Groups may be inherited according to:

- group presence in parents
- body-plan requirements
- group rates
- linkage
- dependency chains
- parent role
- heritability weights
- mutation
- generated complement rules

A group may be inherited complete, partial, dormant, active, or absent.

## 11.5 Non-ploidal inheritance selection

Non-ploidal objects are selected using their own weighted heritability rules.

They do not require paired allele sets.

## 11.6 Parthenogenesis

A parthenogenetic reproductive mode may be modeled as either:

1. **copy-style reproduction**  
   The offspring receives a copy of the parent’s relevant genome state.

2. **self-parenting reproduction**  
   The same individual fills multiple genetic parent roles, with independent allele selection for each role.

The reproductive mode defines which applies.

## 11.7 Budding, fission, spawning, and vegetative reproduction

Nonsexual reproduction may be modeled by defining the generation site and sampling rules.

The offspring may inherit from:

- the parent’s whole genome
- a local body region
- a defined germline organ
- a budding site
- a distributed tissue
- a magical sampling process
- a current active body plan
- a permanent genome version

The reproductive mode defines whether selection is copy-style, role-based, or mutation-mediated.

## 11.8 Germline and generation sites

A reproductive event may use a defined generation site.

Generation sites may include:

- reproductive organs
- germline tissue
- budding organs
- body surface regions
- magical growth sites
- fragments
- distributed tissue systems

For mosaic or chimeric individuals, the genetic composition of the generation site determines what genetic material is available unless policy samples more broadly.

---

# 12. Mosaicism and chimerism

## 12.1 Definition

Mosaicism and genetic chimerism are treated as a spectrum of one individual containing multiple genetically distinct material lines.

The cause may differ, but the system behavior is similar.

Possible causes include:

- absorbed twin
- developmental mutation
- magical splicing
- transformation error
- grafting
- elemental inheritance
- regional mutation
- engineered construction
- body-plan fusion

The system does not require separate mechanics for each cause.

## 12.2 Granularity

The system may support any granularity the setting designer is willing to define.

Recommended default granularity is organ-level.

Exceptions may be made for distributed organs or systems such as:

- skin
- blood
- immune system
- nerves
- connective tissue
- magical/aura-like tissue, if modeled

## 12.3 Regional genome assignment

A mosaic/chimeric individual may assign different genome material to different regions.

Regions may include:

- organs
- limbs
- skin patches
- reproductive organs
- blood
- nervous system
- elementalized tissue
- grafted structures
- transformed regions

## 12.4 Expression

A region expresses traits according to the genetic material assigned to it and the active body plan.

Mosaicism may affect more than surface appearance.

## 12.5 Inheritance from mosaic/chimeric individuals

Inheritance depends on the generation site.

If the reproductive contribution originates from a region containing multiple genetic material lines, the reproductive policy defines whether one, several, or all are sampled.

Mosaic/chimeric genetic material may be inherited.

If the other parent does not contribute matching material for an extra set, the unmatched expression may be forced dormant.

## 12.6 Absorbed twins

A setting may treat an absorbed twin as one individual with chimeric material or as two individuals in one body.

The latter is outside the genetics system and belongs to identity or metaphysics rules.

---

# 13. Non-ploidal inheritance objects

## 13.1 General behavior

Non-ploidal inheritance objects are inherited outside ploidy-based allele-set logic.

They may be generated, inherited, mutated, activated, suppressed, degraded, or removed by policy.

## 13.2 Object properties

A non-ploidal object may define:

- name
- type
- value
- presence
- heritability weight
- activation conditions
- expression targets
- mutation participation
- decay/update rules
- linkage to groups or genes
- trace status
- historical source
- repair behavior
- versioning behavior

## 13.3 Flags

A flag is a binary or categorical non-ploidal object.

Flags may mark:

- lineage state
- curse presence
- blessing presence
- inherited membership
- trace carrier state
- activation eligibility
- acquired genetic change
- archive presence

## 13.4 Counters and accumulators

A counter or accumulator stores numeric state.

It may change by:

- inheritance
- mutation value update
- decay
- expansion
- activation
- external event
- epigenetic condition
- threshold crossing

Inherited memory systems may be represented as accumulator-like or archive-like non-ploidal objects.

## 13.5 Inherited memory archives

If inherited memory exists as part of the genetics system, it likely requires non-standard inheritance.

An inherited memory archive may need to pass all copies from each parent or behave as a non-recombining archive object.

The system may represent it as a non-ploidal inheritance object rather than ordinary alleles.

---

# 14. Traces

## 14.1 Trace purpose

Traces provide a mechanism for dramatic throwback descendants, historical recurrence, and mythic bloodline persistence.

They allow a history simulation to preserve ancestor-linked potential without tracking every ancestor explicitly.

## 14.2 Trace structure

A trace may contain:

- reference to historical ancestor or source profile
- allele sets
- group references
- body-plan-relevant information
- non-ploidal values
- activation policy
- transmission weight
- degradation/update policy
- replacement behavior
- repair behavior
- versioning behavior

## 14.3 Trace inheritance

Trace transmission uses ordinary non-ploidal weighted heritability unless policy defines otherwise.

Trace transmission is not necessarily 50%, though 50% may be sufficient in some settings.

Transmission weights may vary by trace.

## 14.4 Trace activation

Trace activation is usually external or policy-defined.

Activation may occur:

- at conception
- during development
- during mutation
- during shapeshifting
- under epigenetic conditions
- due to prophecy or external history
- under stress
- by ritual
- by body-plan activation
- by mutation policy

The genetics object itself does not track ancestry, birth order, destiny, or historical timing.

External systems provide activation triggers.

## 14.5 Trace effects

A trace may:

- replace allele sets inherited from actual parents
- restore ancestral values
- activate dormant body-plan groups
- add non-ploidal objects
- modify compatibility
- influence mutation policy
- act as a repair template
- provide generated body-plan information
- alter expression or penetrance

## 14.6 Trace degradation

Trace values, weights, or activation probability may degrade over time or generations if policy defines an update rule.

Degradation is treated as a value-selection or value-update mutation/inheritance rule, not as a separate universal mechanic.

---

# 15. Mutation system

## 15.1 Mutation as general genome modification

Mutation is the general mechanism for permanent or potentially permanent genome modification.

Mutation includes both ordinary biological-style mutation and fantasy-style genome alteration.

## 15.2 Temporary expression is not mutation

The following are not mutations unless policy commits genome changes:

- temporary group activation
- temporary body-plan activation
- temporary shapeshifting
- temporary wildshape-like transformation
- temporary expression suppression
- temporary current-form state

## 15.3 Mutation event

A mutation event defines or receives:

- event source
- timing
- origin site
- minimum scope
- target filters
- actual scope
- affected genome copy
- affected genome version
- value-selection mode
- mutation policy
- versioning policy
- repair/reversion behavior
- inheritance behavior

## 15.4 Event source

Mutation event sources may include:

- external force
- inheritance error
- early-development error
- random mutation
- magical effect
- curse
- blessing
- ritual
- shapeshifting fill-in
- gene splicing
- body-plan copying
- trace activation
- repair effect
- reversion effect
- template generation
- cross-species transfer

The event source does not determine behavior by itself. Policy does.

## 15.5 Timing and origin

Timing may include:

- before contribution
- during contribution selection
- conception
- zygotic combination
- early development
- later development
- adult life
- transformation
- gestation
- reproduction-site formation
- current-copy modification
- version commit

Timing supplies an implicit minimum scope but does not force maximum scope.

For example, a local event may affect the entire body if policy permits.

## 15.6 Target filters

Mutation policy may filter targets by:

- gene
- group
- body plan
- active/dormant status
- developmental phase
- region
- reproductive site
- non-ploidal object type
- trace status
- current expression
- allele value
- compatibility role
- mutation participation flag
- external event type

All genome parts are targetable unless policy filters them out.

## 15.7 Scope

Mutation scope may be:

- single gene
- allele set
- group
- body-plan group
- non-ploidal object
- active groups only
- dormant groups only
- common group
- local region
- organ
- distributed organ
- reproductive tissue
- whole body
- whole genome
- current copy only
- permanent version
- mosaic/chimeric region

## 15.8 Value-selection modes

Mutation policy may choose new values using one or more modes:

- select existing allele
- select from allowed allele list
- create new allele
- create new gene
- create new group
- null/delete allele
- damage allele
- copy from paired allele
- copy from another allele set
- copy from another group
- copy from active body plan
- copy from dormant body plan
- copy from trace
- copy from template
- convert/overwrite
- apply numeric update
- apply decay
- apply expansion
- apply threshold jump
- random allowed value
- external specified value
- repair to reference
- revert to prior version

Not all modes are available to all genes or policies.

## 15.9 Conversion and overwrite

Conversion/overwrite is a value-selection mode where one allele, allele set, group, or inheritance object replaces another.

It may be used for:

- repair
- corruption
- purification
- trace replacement
- allele normalization
- collapse of unstable hybrid values
- magical rewriting

## 15.10 Numeric value update

Numeric value update is a value-selection mode.

It may represent:

- decay
- strengthening
- weakening
- expansion
- contraction
- accumulated corruption
- bloodline dilution
- bloodline strengthening
- inherited memory accumulation
- mutation instability
- threshold-triggered change

## 15.11 Copy-number change

The system may represent gene or group duplication, but spontaneous unconstrained copy-number mutation should occur only through external or constrained policies.

Copy-number changes are distinct from ploidy.

Ploidy defines how many allele sets are interpreted for one gene/group.

Copy number defines how many instances of the gene/group exist.

Copy-number changes may be used for:

- duplicated structures
- extra organs
- redundant systems
- diverging duplicated genes
- multiple activators
- inherited archives
- repeated body-plan modules

## 15.12 Structural mutation

Structural mutation changes relationships between genes or groups.

Examples include:

- changing group membership
- changing group dependency
- changing linkage
- changing group reference
- changing expression dependency
- changing body-plan interpretation
- changing deciding-gene behavior

Such changes should normally be mediated through system-wide allele values or template definitions, rather than arbitrary bespoke per-genome relationship metadata.

## 15.13 Mutation policies may read genes

Mutation policies may refer to gene values, groups, and non-ploidal objects.

This allows genes to modify mutation behavior without creating a separate mutation-rate subsystem.

Genes may affect:

- mutation chance
- target eligibility
- value-selection mode
- repair chance
- trace stability
- numeric update rate
- copy-number permission
- dormant-group mutability
- transformation fill-in behavior

## 15.14 Mutation and generated complements

Generated complement material created during shapeshifting or body-plan completion may be treated as mutation if it permanently alters the genome.

Policy determines whether generated material:

- is temporary
- affects current copy only
- creates a permanent version
- is heritable
- is repairable
- is removable
- is linked to the active body plan
- is stored as generated complement data

---

# 16. Genome versioning and repair

## 16.1 Immutable versions

Permanent genome versions are immutable.

A new permanent mutation normally creates a new immutable genome version.

Historical versions remain available for repair, reversion, comparison, and audit.

## 16.2 Version creation

A new version may be created when:

- mutation policy commits a change
- generated complement material becomes permanent
- acquired state becomes heritable
- trace activation permanently rewrites alleles
- repair creates a new stable state
- body-plan change becomes genomic
- curse/blessing writes into genome
- external effect commits genome change

Policy determines exact version creation rules.

## 16.3 Current copy

A current genome copy may differ from the latest permanent version.

Current copies may represent:

- temporary alterations
- provisional mutations
- unstable transformations
- reversible effects
- experimental magic
- current-form generated complements
- pending repair
- non-committed shapeshift consequences

## 16.4 Commit policy

Commit policy determines whether a current copy becomes a new permanent version.

Commit triggers may include:

- time elapsed
- reproduction in altered body
- ritual stabilization
- developmental milestone
- mutation threshold
- external decision
- failed repair window
- body-plan integration
- policy-defined event

## 16.5 Repair and reversion

Repair/reversion requires a reference state.

Reference states may include:

- birth genome
- previous permanent version
- most recent stable version
- pre-curse version
- species template
- body-plan template
- paired allele
- trace
- parent contribution
- externally supplied template

Repair is not automatically the inverse of mutation. Policy determines what can be repaired and to what.

## 16.6 Version visibility

Some mutations may be intentionally versioned to allow easy repair.

Others may be committed in ways that make repair difficult or impossible except by external intervention.

Policy defines which changes are visible, repairable, revertible, or permanent.

---

# 17. Acquired heritable states

## 17.1 External decision

Acquired states become heritable only when an external system or policy decides they do.

The external decider specifies:

- whether the acquired state is genetic
- where it is stored
- how it manifests
- how it is inherited
- whether it is ploidal or non-ploidal
- whether it creates a new genome version
- whether it is repairable
- whether it is mutable
- whether it affects body plans

## 17.2 Examples of externally authored heritable states

The system may represent the following as genetic if the setting defines them that way:

- curses
- blessings
- lycanthropy
- vampirism
- magical infection
- divine marks
- possession scars
- ritual transformations
- alchemical modifications
- magical breeding edits
- inherited titles with biological keys
- bloodline powers

The system does not require them to be genetic.

## 17.3 Interface with mutation

When an acquired state becomes a permanent genome modification, it uses mutation and versioning mechanics.

---

# 18. Expression and activation

## 18.1 Expression state

Expression state determines which genetic material currently affects the body.

Expression state may include:

- active body plan
- active developmental phase
- active groups
- suppressed groups
- active non-ploidal objects
- epigenetic conditions
- penetrance outcomes
- expressivity outcomes
- current form
- mosaic/chimeric regional expression

## 18.2 Activation

Activation changes expression state.

Activation may be:

- temporary
- developmental
- magical
- genetic
- trace-driven
- epigenetic
- external
- policy-defined

Activation is not mutation unless it creates or commits genome modification.

## 18.3 Epigenetic conditions

Epigenetic conditions are expression-modifying states.

They may affect:

- body-plan selection
- penetrance
- expressivity
- mutation policy
- developmental path
- trace activation
- non-ploidal objects
- inherited state

Standard real-world epigenetics may be below modeling threshold, but magical epigenetics may be important.

## 18.4 External context

External facts such as ancestry, birth order, prophecy, social status, mantle eligibility, possession method, or moon phase are not stored inside genes unless a policy writes them there.

External systems may provide context for activation or mutation.

---

# 19. Ancestor tracking, birth order, and external facts

## 19.1 Genetics objects do not track containers

Genes and inheritance objects do not know:

- family tree
- birth order
- number of siblings
- historical significance
- social role
- legal identity
- prophecy state
- mantle status
- reincarnation status
- ancestral records
- regional population statistics

These are external.

## 19.2 External activation

External systems may use such facts to trigger:

- trace activation
- mutation
- non-ploidal flag creation
- epigenetic condition
- body-plan activation
- inheritance object insertion
- repair eligibility
- expression modifier

## 19.3 Birth-order traits

Birth-order or nth-child traits are handled externally as activation or mutation triggers.

The genetics system receives the result rather than calculating birth order internally.

This applies to patterns such as:

- seventh son
- firstborn inheritance
- youngest child
- twin priority
- maternal-line condition
- paternal-line condition

## 19.4 Maternal-line and paternal-line patterns

Line-direction patterns may be handled through:

- parent role
- genomic imprinting-like expression
- sex-linked inheritance
- non-ploidal inheritance
- external activation
- birth-order/ancestor tracking
- mutation policy

The system does not require one universal implementation.

---

# 20. Possession and metaphysical interaction

## 20.1 Possession is external

Possession behavior is setting-dependent.

The genetics system does not define a default.

## 20.2 Allowed possession interfaces

A possession effect may use any existing behavior, including:

- no genetic effect
- temporary expression change
- epigenetic condition
- mutation
- non-ploidal object insertion
- trace-like object insertion
- additional parent role
- curse/blessing inheritance
- body-plan activation
- body-plan rewrite
- generated complement creation

The possession method determines behavior.

## 20.3 Spirit and identity

Spirit identity, possessing identity, absorbed-twin identity, or reincarnated identity are outside the genetics system.

If a spiritual event reintroduces physical genetics, the genetics system treats the resulting genetic material normally.

---

# 21. Population genetic profile templates

## 21.1 Purpose

Population genetic profile templates generate random genomes without explicit ancestry and support population-level simulation.

They provide statistical genetic profiles for populations, subpopulations, species complexes, regions, clans, castes, founder groups, hybrid zones, or historical eras.

## 21.2 Template structure

A population template may contain:

- name
- immutable version identifier
- ranked allele rates
- group presence rates
- non-ploidal object rates
- trace rates
- ploidy-related rates
- compatibility value distributions
- linkage definitions
- group dependency assumptions
- mutation policies
- inheritance policies
- generation policies
- body-plan frequencies
- optional metadata
- historical notes, if desired externally

## 21.3 Template immutability

Templates are versioned and immutable.

Simulating a new generation produces a new template version rather than mutating the existing template.

## 21.4 Random genome generation

A template can generate a random genome by sampling:

- groups
- genes
- allele values
- ranked allele sets
- non-ploidal objects
- traces
- compatibility values
- body-plan-relevant groups
- mutation-relevant values

Generation should respect:

- group rates
- allele rates
- linkages
- dependency chains
- ploidy requirements
- body-plan constraints
- mutation policy, if generation includes mutation
- ranking policy

## 21.5 Template blending

Templates can be blended with weights.

Blending may combine:

- allele rates
- group rates
- non-ploidal object rates
- trace rates
- linkage tendencies
- compatibility distributions
- body-plan group rates
- mutation-policy-relevant values

Blending produces a new immutable template.

## 21.6 Blending modes

Template blending should distinguish operation modes where relevant:

1. **pre-generation blending**  
   A blended profile is created first, then genomes are generated from it.

2. **parental-template reproduction**  
   Parent templates are sampled, genetic contributions are generated from each, and offspring are produced through ordinary reproduction mechanics.

3. **population-level approximation**  
   Rates are adjusted directly without generating individuals.

The operation mode should be explicit.

## 21.7 Template generation simulation

A template may simulate a new generation by adjusting rates according to:

- ordinary inheritance
- weighted heritability
- mutation policies
- linkage
- group dependencies
- non-ploidal inheritance
- trace inheritance
- ploidy rules
- compatibility filters
- external survival/reproduction weights, if supplied

This produces a new immutable template version.

## 21.8 Biased inheritance in templates

If an allele or object has biased heritability, template generation simulation should reflect that bias in changed frequency over generations.

This allows minority alleles with transmission bias to increase over time.

## 21.9 Mutation in templates

Mutation policies may operate at template level by adjusting rates or generating new values.

Template-level mutation may introduce:

- new alleles
- new group rates
- changed non-ploidal object frequencies
- changed trace rates
- changed numeric distributions
- changed linkage frequencies
- changed body-plan group availability

The simulation may approximate individual mutation behavior statistically.

---

# 22. Template groups

## 22.1 Template group definition

A template group is a weighted collection of population templates and/or other template groups.

Template groups represent structured populations.

## 22.2 Nested groups

Template groups may be nested.

This supports population structure at multiple scales, such as:

- household
- clan
- village
- caste
- region
- nation
- species complex
- continent

## 22.3 Template-level weights

Each template or subgroup in a template group has a weight representing its share of the population group.

Weights may change over simulated generations.

## 22.4 Cross-template blend rates

Template groups may define cross-template blend rates.

These rates describe how often individuals from different templates produce offspring or blended population profiles.

Cross-template blend rates may represent external factors such as geography, social structure, mating preference, compatibility, caste restriction, migration, or species barriers.

The genetics system does not need to know why the rate exists.

## 22.5 Group generation simulation

A template group can simulate a new generation by:

- updating each template
- updating template weights
- applying cross-template blend rates
- creating new blended templates when needed
- reducing or removing negligible templates, if policy allows
- preserving distinct substructure where blend rates are low
- recursively simulating nested groups

The output is a new immutable template group version.

## 22.6 Template group preservation of structure

Template groups prevent all population mixing from immediately collapsing into a single averaged template.

Distinct subpopulations may remain separate within the group while still interacting statistically.

---

# 23. Creating templates from individuals

## 23.1 Individual-derived template

A population template may be created from an individual genome.

This allows an individual’s genetic profile to be blended into a population.

## 23.2 Purpose

Individual-derived templates support founder effects, prolific ancestors, legendary progenitors, and large-scale demographic influence.

This feature can represent cases where one individual contributes disproportionately to future populations.

## 23.3 Behavior

An individual-derived template is not the individual.

It is a statistical profile derived from the individual’s genome.

Policy determines:

- which genome version is used
- whether current copy changes are included
- which body plan is used
- which groups are included
- whether traces are included
- how allele rates are initialized
- how reproductive role assumptions are modeled
- how much weight the template has when blended

---

# 24. System object hierarchy

The system can be understood as a hierarchy:

1. allele/value
2. gene
3. group
4. body plan
5. genome
6. genome version
7. current genome copy
8. individual
9. population template
10. template group
11. nested population structure

Non-ploidal inheritance objects may sit alongside genes or inside groups depending on definition.

External systems may interact at any level through policy-defined events.

---

# 25. Policy system

## 25.1 Policy role

Policies define how mechanics are applied.

The system should be permissive in representation but controlled by explicit policies.

## 25.2 Policy types

Useful policy categories include:

- inheritance policy
- expression policy
- body-plan activation policy
- group dependency policy
- mutation policy
- versioning policy
- repair policy
- trace activation policy
- non-ploidal inheritance policy
- reproductive policy
- gestational compatibility policy
- template generation policy
- template blending policy
- population simulation policy

## 25.3 Policy granularity

Policy participation should generally be declared at gene or group level.

Specific changes or values are usually allele-level.

## 25.4 Policy inputs

Policies may read:

- genes
- allele values
- groups
- body plan
- active expression state
- current genome copy
- genome version history
- non-ploidal objects
- mutation source
- timing
- scope
- external event context
- parent role
- template identity
- region/site
- developmental phase

## 25.5 Policy outputs

Policies may produce:

- selected alleles
- selected groups
- activation states
- expression results
- mutation events
- current-copy changes
- new genome versions
- repair results
- generated complement material
- trace activation
- non-ploidal object changes
- template rate changes
- new blended templates

---

# 26. Resource testing framework

## 26.1 Purpose

The genetics system must provide first-class support for game developers and setting designers to write custom tests covering the resources they create for the system.

These tests are not primarily for validating the genetics engine implementation itself. They are for validating authored content, such as:

- genes
- alleles
- groups
- body plans
- developmental phases
- compatibility rules
- mutation policies
- trace definitions
- expression rules
- reproductive policies
- population templates
- template groups
- generated test organisms
- setting-specific extensions

The testing framework should help designers confirm that authored resources behave as intended, remain stable across changes, and do not accidentally produce invalid or surprising outcomes.

## 26.2 Testing as a first-class feature

Testing support must be part of the system’s public design rather than an external afterthought.

A conforming implementation should provide a way to define, run, and report custom tests against system resources.

The test framework should be:

- deterministic when given a seed
- language-agnostic in concept
- suitable for editor integration
- suitable for command-line or automated execution
- able to test single resources in isolation
- able to test interactions between resources
- able to run simulations over many generated individuals
- able to produce human-readable diagnostics
- able to identify which authored resource caused or contributed to failure

## 26.3 Testable resources

Any authored system resource may be testable.

At minimum, the framework should support tests for:

- gene definitions
- allele definitions
- dominance hierarchies
- expression rules
- group definitions
- group dependency chains
- linkage rules
- body-plan requirements
- body-plan expression
- developmental phase sequences
- reproductive policies
- compatibility policies
- mutation policies
- versioning policies
- repair policies
- trace objects
- non-ploidal inheritance objects
- population templates
- template groups
- template blending rules
- template simulation rules
- generated individual genomes

## 26.4 Test categories

The testing framework should support:

- validation tests
- unit tests
- integration tests
- simulation tests
- regression tests
- negative tests
- invariant tests
- policy coverage tests
- reachability tests
- matrix tests
- fuzz tests
- golden sample tests

## 26.5 Validation tests

Validation tests confirm that authored resources are structurally valid.

Examples:

- every referenced gene exists
- every referenced group exists
- every body plan references valid groups
- every allele belongs to the gene that uses it
- every dominance hierarchy is complete or intentionally partial
- every dependency chain resolves
- no forbidden circular dependency exists
- every mutation policy references valid targets
- every population template references known genes/groups/objects
- every ploidy requirement has defined ranking behavior
- every required expression rule has a resolution policy

Validation tests may be run automatically whenever resources are loaded.

## 26.6 Unit tests

Unit tests check one resource or one narrow interaction.

Examples:

- a color gene resolves dominance correctly
- a compatibility gene calculates distance correctly
- a trace transmits with expected weighted behavior
- a body plan rejects missing required groups
- a mutation policy can only target allowed genes
- a non-ploidal flag inherits according to its weight
- a specific allele changes expression under a modifier gene

Unit tests should be small, deterministic, and easy to diagnose.

## 26.7 Integration tests

Integration tests check interactions between multiple resources.

Examples:

- a human-dragon hybrid body plan can be generated
- a shoulder-dragon and full-dragon share common groups correctly
- a sterile hybrid is viable but cannot reproduce
- shapeshifting fill-in creates generated complement material
- generated complement material becomes heritable only through the appropriate reproductive body
- a curse mutation creates a new genome version and can be repaired only by allowed policy
- a trace activation replaces the intended allele sets

## 26.8 Simulation tests

Simulation tests run many generated cases and compare aggregate outcomes against expected bounds.

Examples:

- a population template produces allele rates within tolerance
- a rare dormant body-plan group appears at expected frequency
- a trace persists across 100 simulated generations within expected range
- biased heritability causes an allele to increase over generations
- a template blend produces expected group-rate distributions
- sterile hybrids occur within a specified frequency band
- mutation policy generates new alleles only within allowed limits

Simulation tests must use deterministic random seeds.

## 26.9 Regression tests

Regression tests preserve previously accepted behavior.

A regression test may capture:

- specific generated genomes
- specific expression outputs
- specific inheritance outcomes
- specific population-rate results
- specific mutation/version histories
- specific repair outcomes

Regression tests are especially important when changing shared genes, body plans, mutation policies, or templates that affect many downstream resources.

## 26.10 Negative tests

Negative tests confirm that invalid or forbidden configurations fail correctly.

Examples:

- an incomplete body plan does not express fully
- incompatible parents cannot produce viable offspring
- sterile hybrids cannot reproduce
- a mutation policy cannot affect protected groups
- a template cannot generate forbidden allele combinations
- a trace cannot activate without required external trigger
- a body plan cannot use a group without required ploidy
- temporary shapeshifting does not create a permanent genome version

Negative tests should verify both failure and diagnostic quality.

## 26.11 Test resources

A test should be a resource or structured definition in its own right.

A test definition may contain:

- test name
- test identifier
- test category
- resource references
- fixture definitions
- setup operations
- input genomes or templates
- random seed
- operation under test
- expected result
- assertions
- tolerances
- tags
- expected failure state, if any
- diagnostic expectations
- author notes

Tests should be versionable and should be able to live alongside the resources they validate.

## 26.12 Test fixtures

A fixture is a reusable test setup.

Fixtures may define:

- test genes
- test alleles
- test groups
- test body plans
- test genomes
- test individuals
- test parent roles
- test population templates
- test mutation policies
- test traces
- test external context
- test random seeds
- test developmental phases
- test active body states

Fixtures should allow developers to avoid repeating large setup definitions.

## 26.13 Deterministic execution

All tests involving random behavior must support deterministic execution.

A test may specify:

- fixed random seed
- named random stream
- deterministic sampling mode
- sample count
- tolerance range
- expected exact output, when appropriate

The same test with the same resources, same engine version, same policy set, and same seed should produce the same result.

## 26.14 Random stream separation

The framework should support separate random streams for different operation categories where practical.

Examples:

- inheritance selection
- mutation selection
- expression randomness
- template generation
- population simulation
- trace transmission
- trace activation
- value update functions

This allows tests to remain stable when unrelated random operations are added elsewhere.

## 26.15 Assertions

Tests should support assertions over system state and operation results.

Assertion types should include:

- existence assertions
- equality assertions
- set and membership assertions
- absence assertions
- range assertions
- probability assertions
- versioning assertions
- diagnostic assertions

Examples:

- gene exists
- allele value equals expected value
- group contains gene
- body plan does not activate
- numeric gene value is within range
- trace carrier rate falls within expected bounds
- operation creates no new permanent version
- failure reports insufficient ploidy

## 26.16 Test operations

The framework should expose testable operations corresponding to core system behaviors.

Examples:

- validate resource
- validate resource graph
- generate genome from template
- blend templates
- simulate template generation
- simulate template group generation
- create offspring
- evaluate body-plan expression
- activate body plan
- apply mutation
- apply repair
- apply trace activation
- calculate compatibility
- evaluate developmental viability
- evaluate reproductive fertility
- inspect genome version history
- inspect current copy
- inspect generated complement material
- sample inheritance distribution
- sample mutation distribution

## 26.17 Custom test operations

Game developers should be able to define custom test operations that call into system mechanics.

Custom operations may test setting-specific resources or game-specific wrappers.

Examples:

- generate a noble house heir
- apply moon-phase birth rules
- resolve werewolf infection
- generate a village population
- simulate dragon-blooded ancestry for 30 generations
- create offspring from three parent roles
- attempt repair using temple blessing policy

Custom operations should report results through the standard assertion and diagnostic system.

## 26.18 External context injection

Tests must be able to provide external context because many system mechanics intentionally depend on external facts.

A test may inject context such as:

- birth order
- ancestor tracking result
- trace activation permission
- prophecy flag
- moon phase
- curse source
- blessing source
- possession method
- parent role metadata
- gestational role
- social lineage marker
- template blend rate
- environmental condition
- epigenetic condition

The genetics objects do not compute these facts internally, but tests must be able to supply them.

## 26.19 Resource graph validation

The framework should be able to validate the full graph of authored resources.

Validation operates on the resource graph supplied to the system. It does not make the genetics system responsible for discovering, inventorying, or managing every external resource definition that may exist outside that supplied graph.

Graph validation should detect:

- missing references
- unreachable resources, if considered suspicious
- circular dependencies, if not allowed
- circular dependencies lacking resolution policy
- invalid body-plan group references
- invalid mutation target references
- invalid template references
- invalid parent role references
- incompatible ploidy definitions
- ambiguous ranked allele-set interpretation
- undefined generated-complement behavior
- undefined repair reference
- undefined activation policy
- invalid non-ploidal object type
- conflicting policy definitions

Graph validation should distinguish fatal errors from warnings or informational diagnostics where the implementation supports severity levels.

## 26.20 Policy coverage tests

Because mechanics are design-time opt-in, the framework should support coverage-style checks over policies.

A policy coverage test may ask:

- which genes participate in this mutation policy?
- which groups are protected?
- which body plans can activate this group?
- which alleles are reachable through this template?
- which mutation value-selection modes can affect this gene?
- which traces can activate under this context?
- which repair policies apply to this mutation type?
- which groups are never generated?
- which alleles are defined but unreachable?

Coverage tests help detect authored content that exists but can never occur.

## 26.21 Reachability analysis

The framework may support reachability analysis.

Reachability analysis asks whether a resource or state can be produced through declared mechanics.

Examples:

- can allele X appear in generated individuals?
- can body plan Y ever fully express?
- can trace Z ever activate?
- can mutation policy M ever target group G?
- can template T ever generate a fertile individual?
- can this hybrid combination ever survive gestation?
- can generated complement material ever become heritable?

Reachability analysis may be exact for simple cases and approximate or simulation-based for complex cases.

## 26.22 Invariant tests

Developers should be able to define invariants that must always hold for a resource set.

Examples:

- temporary shapeshifting never creates permanent genome versions
- protected common group is never mutated by random mutation
- all full-dragon bodies require complete dragon respiratory group
- all shoulder-dragon bodies share the same pigmentation group as full dragons
- sterile hybrids never appear as valid genetic parents
- population template X never generates body plan Y
- all viable offspring must satisfy gestational compatibility
- repair policy R never creates new alleles

Invariant tests may run across many generated cases.

## 26.23 Golden sample tests

A golden sample test compares current output to a saved expected output.

Golden samples may include:

- genome generation results
- expression result summaries
- mutation histories
- repair histories
- template rate tables
- population simulation summaries
- compatibility reports

Golden sample tests are useful for regression testing but should be used carefully when output is expected to evolve.

Implementations should provide a way to regenerate or approve updated golden samples.

## 26.24 Statistical test tolerances

Simulation tests should avoid expecting exact percentages unless the operation is deterministic and exhaustive.

A statistical test should define:

- seed
- sample size
- expected range
- acceptable tolerance
- optional confidence interpretation
- maximum allowed outliers
- whether failure is fatal or advisory

## 26.25 Test diagnostics

When a test fails, diagnostics should identify:

- test name
- failed assertion
- expected result
- actual result
- involved resources
- operation under test
- random seed
- relevant policy
- relevant genome version
- relevant body plan
- relevant group/gene/allele
- relevant external context
- trace or mutation event involved, if any

For generated individuals, diagnostics should allow the failing genome or relevant subset to be inspected.

## 26.26 Minimal reproducibility packet

For any failed randomized test, the framework should be able to produce a minimal reproducibility packet containing:

- resource set version
- test identifier
- seed
- operation path
- input fixtures
- external context
- generated genome or template state
- failure assertion
- relevant diagnostics

This packet should allow the failure to be reproduced exactly.

## 26.27 Editor integration

A game/editor implementation should ideally allow developers to run tests from resource editing workflows.

Useful editor features include:

- run tests for selected resource
- run tests affected by selected resource
- show dependent tests
- show failing assertions inline
- inspect generated genomes
- inspect expression results
- inspect mutation/version history
- inspect population template samples
- generate example organisms from template
- compare expected and actual outputs
- approve updated golden samples

## 26.28 Continuous integration support

The testing framework should support automated execution outside the editor.

A command-line or batch mode should allow:

- run all tests
- run tests by tag
- run tests for changed resources
- run validation only
- run statistical tests
- run regression tests
- export reports
- fail with machine-readable status
- produce reproducibility packets

Reports should be available in both human-readable and machine-readable forms where practical.

## 26.29 Test tags

Tests may have tags for filtering.

Examples:

- validation
- unit
- integration
- simulation
- regression
- slow
- statistical
- template
- mutation
- trace
- body-plan
- compatibility
- reproduction
- shapeshifting
- repair
- population
- editor-only
- ci

## 26.30 Fast and slow tests

The framework should distinguish fast tests from slow tests.

Fast tests should be suitable for frequent editor execution.

Slow tests may include:

- large population simulations
- many-generation template simulations
- high-sample statistical tests
- exhaustive compatibility sweeps
- deep reachability analysis

Developers should be able to exclude slow tests during ordinary editing and include them during full validation.

## 26.31 Exhaustive matrix tests

The framework should support matrix-style tests over combinations of resources.

Examples:

- all alleles of gene X against all alleles of gene Y
- all parent body-plan combinations in a species group
- all mutation policies against all protected groups
- all traces against all activation contexts
- all population templates against all required body plans
- all compatible species pairs for fertility outcomes

Matrix tests help catch interaction failures in large resource sets.

## 26.32 Fuzz tests

The framework may support fuzz testing within resource constraints.

A fuzz test generates many random but valid inputs and checks invariants.

Examples:

- generate random genomes from all templates and verify expression never crashes
- apply allowed mutation policies repeatedly and verify version history remains valid
- generate hybrid offspring from random compatible parent templates
- randomize trace activation contexts and verify only allowed traces activate
- generate random mosaic/chimeric region assignments and verify reproduction policies resolve

Fuzz tests should be seedable and reproducible.

## 26.33 Snapshot and diff support

For complex results, tests may compare structured snapshots.

Snapshot outputs may include:

- genome summary
- active expression summary
- body-plan viability report
- mutation history
- version tree
- template rates
- group presence table
- trace carrier table
- compatibility matrix

When snapshots differ, the framework should produce a structured diff rather than only a pass/fail result.

## 26.34 Test isolation

Tests should not mutate shared authored resources.

Resources used in tests should be treated as immutable inputs.

Operations that create versions, generated genomes, mutated copies, or simulated templates should do so in isolated test state.

This prevents one test from affecting another.

## 26.35 Test inheritance and composition

Tests may be composed from smaller fixtures or inherited from base test definitions.

Examples:

- every dragon-derived body plan must pass a shared dragon body viability test
- every population template must pass a shared template-can-generate-valid-genome test
- every mutation policy must pass a shared protected-groups-respected test
- every reproductive policy must pass a shared deterministic-offspring-under-seed test

This helps designers enforce common rules across many resources.

## 26.36 User-authored custom assertions

Game developers should be able to write custom assertions.

Custom assertions should receive structured operation results and return:

- pass/fail
- diagnostic message
- involved resources
- optional expected/actual values
- optional severity

Custom assertions allow project-specific validation without modifying the core engine.

## 26.37 Test severity

Tests and diagnostics may support severity levels.

Suggested levels:

- fatal error
- error
- warning
- informational

A project may configure which severities fail automated builds.

For example, unreachable alleles may be an error in one project and a warning in another.

## 26.38 Version compatibility

Tests should record or infer the system/resource version they target.

A test definition may include:

- minimum system version
- expected resource schema version
- expected deterministic behavior version
- migration status
- deprecated-resource expectations

## 26.39 Required baseline tests

A mature resource set should normally include tests covering:

- every body plan can validate
- every population template can generate at least one valid genome
- every mutation policy respects protected targets
- every reproductive policy can produce expected success/failure cases
- every compatibility policy has fertile, sterile, and inviable examples where applicable
- every trace has at least one transmission test and one activation/non-activation test
- every generated-complement rule has temporary and permanent behavior tests if both are allowed
- every repair policy has at least one successful and one forbidden repair case
- every template group can simulate at least one generation without invalid output

These are recommended coverage expectations, not universal engine requirements.

## 26.40 Relationship to implementation tests

This resource testing framework is distinct from implementation unit tests.

Implementation tests verify that the engine code behaves correctly.

Resource tests verify that designer-authored genetic content behaves as intended.

A complete project should have both, but this section concerns resource tests.

---

# 27. Design/runtime interaction with tests

## 27.1 Design mode

Design mode should support the full resource testing framework.

This includes:

- authoring tests
- editing fixtures
- running validation tests
- running unit tests
- running integration tests
- running simulation tests
- approving golden samples
- performing reachability analysis
- running policy coverage checks

## 27.2 Runtime mode

Runtime mode may include limited tests or validation checks, but tests must not mutate frozen system definitions.

Runtime test execution should be optional and should normally be restricted to:

- integrity checks
- deterministic sanity checks
- save compatibility checks
- invariant checks

## 27.3 Test fixtures in runtime

Test fixtures are usually design resources and may be omitted from runtime packages.

If included, they should be frozen data.

---

# 28. Invariants and constraints

## 28.1 Genome versions are immutable

Permanent genome versions must not be modified after creation.

Any permanent change creates a new version unless policy explicitly uses a different archival model.

## 28.2 Template versions are immutable

Population template versions and template group versions should be immutable.

Simulation creates new versions.

## 28.3 Runtime definitions are immutable

In runtime mode, the system definition is frozen after load and validation.

Runtime may create genetic state and runtime-derived body-plan variants, but may not redefine authored genetic rules, authored body-plan definitions, or body-plan variant construction rules.

## 28.4 Runtime-derived body-plan variants are instance state

Runtime-derived body-plan variants are not edits to the system definition.

They must be validated and interpreted using frozen definitions and policies.

## 28.5 Temporary expression is not permanent mutation

Temporary body-plan activation, shapeshifting, or group activation must not automatically create genome versions.

## 28.6 Expression requires interpretation context

Genes and groups do not express in isolation.

Expression depends on body plan, developmental phase, group dependencies, ploidy, and policies.

## 28.7 External facts remain external

Genes do not internally know social, historical, prophetic, or genealogical context.

External systems may provide triggers or write results into the genome.

## 28.8 Ploidy interpretation must be unambiguous

When a body plan uses fewer allele sets than are present, ranking or selection policy must provide an unambiguous result.

## 28.9 Mutation requires policy

Mutation behavior must be defined by policy.

The system should not freely invent unconstrained mutation effects.

## 28.10 Copy-number changes require constraint

Copy-number changes may be represented, but spontaneous unconstrained generation of duplicate organs, limbs, heads, or systems should require external or constrained policies.

## 28.11 Population templates are not individuals

A template is a statistical distribution, not a genome.

A template-derived individual must be generated before individual-level mechanics apply.

## 28.12 Tests must not mutate shared authored resources

Tests should run against immutable resource inputs and produce isolated outputs.

---

# 29. Common operation sketches

## 29.1 Ordinary reproduction

1. Identify reproductive body plan and parent roles.
2. Check gestational/developmental prerequisites.
3. For each required group, determine parental availability.
4. Select allele sets using ploidy, ranking, linkage, and weights.
5. Select non-ploidal objects using weighted heritability.
6. Apply trace activation if external conditions trigger it.
7. Apply inheritance-time mutation policy.
8. Build offspring genome version.
9. Determine initial body plan and developmental phase.
10. Apply epigenetic or maternal/developmental effects resolved at creation.

## 29.2 Shapeshifting into partially known body plan

1. Identify target body plan.
2. Identify required groups.
3. Determine which groups are present, incomplete, or absent.
4. If policy allows, generate complement material.
5. Decide whether generated material affects current copy only or creates mutation.
6. Activate target body plan temporarily or permanently according to expression policy.
7. If permanent mutation occurs, create genome version according to versioning policy.

## 29.3 Runtime body-plan variant creation

1. Identify base body plan.
2. Identify mutation, construction, or external event source.
3. Select frozen body-plan variant construction policy.
4. Select eligible morphology packages, group changes, or generated complement changes.
5. Validate compatibility with base body plan, genome state, ploidy, and group dependencies.
6. Construct derived body-plan variant as instance data.
7. Decide whether variant is temporary, current-copy-only, or permanent.
8. If permanent, create or update the appropriate genome version according to versioning policy.
9. Serialize variant data as part of the owning individual/genome/template state if required.

## 29.4 Trace activation at conception

1. External system determines trace activation condition is met.
2. Trace object is selected from inherited non-ploidal objects.
3. Trace effect policy determines target allele sets or groups.
4. Trace replaces, modifies, or supplements inherited values.
5. Mutation/versioning policy determines whether this creates a permanent genome version.
6. Offspring genome is finalized.

## 29.5 Repair

1. Identify damaged or altered current genome state.
2. Select repair policy.
3. Select repair reference:
   - previous version
   - birth version
   - species/body-plan template
   - trace
   - paired allele
   - external template
4. Apply value-selection/overwrite mode.
5. Decide whether repair affects current copy or creates new permanent version.

## 29.6 Population generation

1. Select population template or template group.
2. If group, select template according to weights or blending mode.
3. Sample groups and ranked allele rates.
4. Apply linkages and dependencies.
5. Sample non-ploidal objects.
6. Apply template-level mutation if policy includes it.
7. Construct individual genome.
8. Assign initial body plan and expression state.

## 29.7 Population generation simulation

1. Start from immutable template version.
2. Apply inheritance approximation.
3. Apply weighted heritability changes.
4. Apply mutation policies.
5. Apply linkage/group dependency effects.
6. Update allele/group/non-ploidal rates.
7. Create new immutable template version.

## 29.8 Template group simulation

1. Simulate each contained template or subgroup.
2. Apply cross-template blend rates.
3. Generate new blended templates as necessary.
4. Adjust template weights.
5. Preserve nested structure unless policy merges it.
6. Create new immutable template group version.

## 29.9 Runtime load and freeze

1. Select runtime mode.
2. Load runtime package/system definition.
3. Validate resources and schema compatibility.
4. Apply explicitly packaged runtime migrations, if any.
5. Freeze system definition.
6. Load saved instance state.
7. Run genetic operations using frozen policies.

## 29.10 Design export

1. Select design mode.
2. Load editable resource graph.
3. Apply design migrations if needed.
4. Validate resources.
5. Run required resource tests.
6. Package validated definitions.
7. Emit immutable runtime package with version identifier.

---

# 30. Definition mutability matrix

| Resource or state | Design mode | Runtime mode |
|---|---:|---:|
| Gene definitions | Mutable | Frozen |
| Allele definitions | Mutable | Frozen |
| Group definitions | Mutable | Frozen |
| Authored body-plan definitions | Mutable | Frozen |
| Runtime-derived body-plan variants | Creatable/testable | Creatable by frozen policy as instance state |
| Developmental phase definitions | Mutable | Frozen |
| Expression policies | Mutable | Frozen |
| Mutation policies | Mutable | Frozen |
| Repair/versioning policies | Mutable | Frozen |
| Reproductive policies | Mutable | Frozen |
| Compatibility policies | Mutable | Frozen |
| Trace definitions | Mutable | Frozen |
| Non-ploidal object definitions | Mutable | Frozen |
| Population template definitions | Mutable | Frozen after load |
| Template group definitions | Mutable | Frozen after load |
| Test definitions | Mutable | Usually frozen or omitted |
| Individual genomes | Test/editable | Runtime mutable by policy |
| Genome versions | Creatable; immutable once made | Creatable; immutable once made |
| Current genome copies | Mutable | Mutable by policy |
| Runtime offspring | Creatable | Creatable |
| Runtime mutation events | Creatable | Creatable by frozen policy |
| Runtime population samples | Creatable | Creatable |
| System-definition migrations | Allowed | Only explicit packaged migrations before freeze |

---

# 31. Serialization and storage boundary

## 31.1 Core serialization requirement

The core library must define stable serialization formats at multiple granularity layers.

The required core serialization formats are:

- JSON
- binary

Serialization must be stable enough to support:

- saved individual genomes
- genome versions
- current genome copies, when persisted
- runtime-derived body-plan variants
- non-ploidal inheritance objects
- traces
- mutation histories, when persisted
- population templates
- template groups
- resource test fixtures and outputs
- runtime packages
- reproducible test failure packets

## 31.2 Serialization granularity

Serialization should be supported at multiple layers rather than only at the whole-system level.

Serializable layers may include:

- individual gene or allele definitions
- groups
- authored body-plan definitions
- runtime-derived body-plan variants
- policies
- genome versions
- current genome copies
- individuals
- population templates
- template groups
- complete system definitions
- runtime packages
- test definitions and fixtures
- simulation outputs

## 31.3 Stable identifiers and compatibility

Serialized data must use stable resource identifiers for authored definitions.

Serialized runtime-derived body-plan variants must reference frozen definitions and policies rather than embedding mutable authoring definitions.

Serialized genomes and templates should record the system-definition version they depend on.

If serialized data is loaded under a different system-definition version, migration or compatibility validation may be required.

## 31.4 Core storage boundary

The core library must not interact directly with permanent storage.

The core library may read from and write to streams, buffers, byte arrays, text data, or equivalent abstraction layers, but it must not require or own a database, filesystem layout, save-game repository, cloud service, asset database, or other persistence mechanism.

Permanent storage belongs outside the core library.

## 31.5 Optional standard storage modules

The project may provide optional standard non-core modules that add permanent storage support.

These modules must depend on the core library rather than being required by it.

The initial implementation should include optional modules for:

- SQLite storage
- JSON file storage
- custom binary file storage

These modules are especially useful for building tests, running test suites, storing fixtures, saving generated genomes, and comparing serialized outputs.

## 31.6 Storage module responsibilities

A storage module may provide:

- loading and saving serialized system definitions
- loading and saving runtime packages
- loading and saving genomes and genome versions
- loading and saving population templates and template groups
- loading and saving resource tests and fixtures
- indexing saved resources
- transaction support, if applicable
- test fixture setup and teardown helpers
- migration orchestration around serialized data

A storage module must not redefine core genetics behavior.

## 31.7 Test usage

Tests may use optional storage modules to verify serialization, persistence boundaries, migration behavior, and fixture loading.

Core resource tests should still be able to run without permanent storage by using in-memory serialized data.

## 31.8 Content management boundary

Genomancy is not a content management system for external resource definitions, even when optional storage modules are used.

The project expects some other system, such as a version control system, asset pipeline, editor database, repository manager, or game-specific content system, to own the authoritative set of external resource definition files and their history.

Genomancy is responsible for faithfully loading, decoding, and reporting on the serialized resources it is given. It may validate the loaded resource graph according to genetics-system rules, such as identifier resolution, schema compatibility, dependency validity, and policy consistency.

Genomancy is not responsible for proving that the supplied resource set is complete relative to an external project, repository, editor workspace, asset database, or setting canon.

Storage modules must not assume responsibility for:

- discovering every resource definition that ought to exist
- determining whether external files are missing from a project
- enforcing source-control status
- tracking authoritative content history
- resolving merge conflicts
- deciding which external resources belong to a release
- guaranteeing that a directory, database, archive, or workspace is a complete content corpus

Such responsibilities belong to the external content management, source control, asset pipeline, build, or editor systems that provide resources to Genomancy.

---

# 32. Interfaceable out-of-scope systems

The following are not defined by this genetics system but may interact with it:

- soul identity
- reincarnation
- possession identity
- mantles
- divine choice
- prophecy
- social species classification
- caste
- nobility
- legitimacy
- legal inheritance
- blood purity ideology
- curses/blessings as metaphysics
- spirit lineage
- true names
- fate
- cultural taxonomy
- survival selection
- mating behavior
- geography
- economy
- social taboo
- historical records

These systems may provide inputs, triggers, rates, or externally authored mutation events.

---

# 33. Consolidated architecture summary

The system is a generalized fantasy inheritance and expression framework where:

- ordinary genetics is the default conceptual baseline
- body plans are modular developmental interpretations of grouped genetic material
- individuals may carry unexpressed alternate body-plan information
- genetic groups support radically different morphologies and gestations
- expression, activation, and inheritance are separate
- compatibility is modeled through explicit genes and policies rather than literal chromosome counts
- ploidy can vary by body plan or group
- non-ploidal objects carry legacy-like inheritance outside allele-pair logic
- traces preserve mythic ancestry and allow dramatic throwback descendants
- mutation is the general policy-controlled mechanism for permanent genome modification
- immutable genome versions support repair, reversion, audit, and hard-system consistency
- population templates provide statistical genome generation and population simulation without explicit ancestry
- template groups support nested population structure and cross-template blending
- the core library defines stable JSON and binary serialization while leaving permanent storage to non-core modules
- resource testing is a first-class feature for validating authored content
- design mode allows system creation and mutation
- runtime mode freezes authored system definitions immediately after load
- runtime may create derived body-plan variants under frozen construction and mutation policies

The central hard-system boundary is:

> The genetics system defines how heritable material is represented, expressed, modified, inherited, tested, and simulated. External systems define why special metaphysical, social, prophetic, or narrative events occur.
