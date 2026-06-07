using System.Reflection;
using Genomancy.Core.Compatibility;
using Genomancy.Core;
using Genomancy.Core.Definitions;
using Genomancy.Core.Development;
using Genomancy.Core.Expression;
using Genomancy.Core.Genome;
using Genomancy.Core.Inheritance;
using Genomancy.Core.Mosaicism;
using Genomancy.Core.Mutation;
using Genomancy.Core.Reproduction;
using Genomancy.Core.Runtime;
using Genomancy.Core.Serialization;
using Genomancy.Core.Variants;

var tests = new (string Name, Action Test)[]
{
    ("Core assembly exposes stable name", CoreAssemblyExposesStableName),
    ("Core assembly is Godot independent", CoreAssemblyIsGodotIndependent),
    ("Core assembly has no forbidden dependencies", CoreAssemblyHasNoForbiddenDependencies),
    ("Resource ids validate and compare by stable value", ResourceIdsValidateAndCompareByStableValue),
    ("System definition versions validate and compare by stable value", SystemDefinitionVersionsValidateAndCompareByStableValue),
    ("Design mode allows authored definition edits", DesignModeAllowsAuthoredDefinitionEdits),
    ("Minimal human body-plan graph validates and freezes", MinimalHumanBodyPlanGraphValidatesAndFreezes),
    ("Runtime startup freezes a validated definition", RuntimeStartupFreezesAValidatedDefinition),
    ("Runtime startup rejects invalid definitions before runtime state", RuntimeStartupRejectsInvalidDefinitions),
    ("Frozen definitions reject retained builder mutation", FrozenDefinitionsRejectRetainedBuilderMutation),
    ("Frozen snapshot is isolated from retained design references", FrozenSnapshotIsIsolatedFromRetainedDesignReferences),
    ("All authored references resolve by stable id", AuthoredReferencesResolveByStableId),
    ("Validation reports duplicate ids and missing references", ValidationReportsDuplicateIdsAndMissingReferences),
    ("Validation reports group dependency cycles", ValidationReportsGroupDependencyCycles),
    ("Validation diagnostics are deterministic", ValidationDiagnosticsAreDeterministic),
    ("Runtime startup applies migration before freeze", RuntimeStartupAppliesMigrationBeforeFreeze),
    ("Genome versions isolate collection aliases", GenomeVersionsIsolateCollectionAliases),
    ("Current genome copy edits discard and commit explicitly", CurrentGenomeCopyEditsDiscardAndCommitExplicitly),
    ("Uncommitted current genome changes do not create version spam", UncommittedCurrentGenomeChangesDoNotCreateVersionSpam),
    ("Genome JSON round trip preserves version state and ancestry", GenomeJsonRoundTripPreservesVersionStateAndAncestry),
    ("Genome binary round trip preserves version state and ancestry", GenomeBinaryRoundTripPreservesVersionStateAndAncestry),
    ("Genome serializers reject malformed and unknown versions", GenomeSerializersRejectMalformedAndUnknownVersions),
    ("Genome serialization uses caller supplied streams and buffers", GenomeSerializationUsesCallerSuppliedStreamsAndBuffers),
    ("Group completeness reports complete absent partial wrong ploidy and dependency failures", GroupCompletenessReportsExpectedStates),
    ("Shared group evaluates under distinct body plan contexts", SharedGroupEvaluatesUnderDistinctBodyPlanContexts),
    ("Body plan activation does not create genome versions", BodyPlanActivationDoesNotCreateGenomeVersions),
    ("Expression strategies are deterministic and distinct", ExpressionStrategiesAreDeterministicAndDistinct),
    ("Expression requires explicit body plan context and opaque external facts", ExpressionRequiresExplicitContextAndOpaqueExternalFacts),
    ("Individual expression state requires at least one active available body plan", IndividualExpressionStateRequiresActiveAvailableBodyPlan),
    ("Seeded diploid reproduction produces deterministic offspring serialization", SeededDiploidReproductionProducesDeterministicOffspringSerialization),
    ("Multi parent and variable ploidy reproduction uses explicit contributors", MultiParentAndVariablePloidyReproductionUsesExplicitContributors),
    ("Ambiguous lower arity reproduction is rejected", AmbiguousLowerArityReproductionIsRejected),
    ("Same individual can fill multiple parent roles", SameIndividualCanFillMultipleParentRoles),
    ("Weighted transmission excludes zero weights and validates finite weights", WeightedTransmissionExcludesZeroWeightsAndValidatesFiniteWeights),
    ("Named random streams prevent unrelated gene perturbation", NamedRandomStreamsPreventUnrelatedGenePerturbation),
    ("Reproduction reports compatibility outcomes and preserves parents", ReproductionReportsCompatibilityOutcomesAndPreservesParents),
    ("Current only mutation diverges without committing a version", CurrentOnlyMutationDivergesWithoutCommittingVersion),
    ("Committed mutation creates immutable child version", CommittedMutationCreatesImmutableChildVersion),
    ("Mutation policy rejects protected targets and invalid alleles", MutationPolicyRejectsProtectedTargetsAndInvalidAlleles),
    ("Numeric copy count and structural mutations update current state", NumericCopyCountAndStructuralMutationsUpdateCurrentState),
    ("Repair and revert restore current genome state", RepairAndRevertRestoreCurrentGenomeState),
    ("External mutation source is explicit and policy controlled", ExternalMutationSourceIsExplicitAndPolicyControlled),
    ("Non ploidal object inheritance is weighted and deterministic", NonPloidalObjectInheritanceIsWeightedAndDeterministic),
    ("Non ploidal inheritance respects inactive and zero weight objects", NonPloidalInheritanceRespectsInactiveAndZeroWeightObjects),
    ("Trace state supports activation replacement and degradation", TraceStateSupportsActivationReplacementAndDegradation),
    ("Reproduction can inherit non ploidal objects and traces", ReproductionCanInheritNonPloidalObjectsAndTraces),
    ("Genome serialization preserves non ploidal objects and traces", GenomeSerializationPreservesNonPloidalObjectsAndTraces),
    ("Compatibility service reports layered outcomes and hybrid morphology", CompatibilityServiceReportsLayeredOutcomesAndHybridMorphology),
    ("Reproduction reports inviable compatibility outcome", ReproductionReportsInviableCompatibilityOutcome),
    ("Clonal copy reproduction creates offspring without allele recombination", ClonalCopyReproductionCreatesOffspringWithoutAlleleRecombination),
    ("Development timeline enforces gestation ordering requirements", DevelopmentTimelineEnforcesGestationOrderingRequirements),
    ("Maternal effects update numeric genome state without mutating source", MaternalEffectsUpdateNumericGenomeStateWithoutMutatingSource),
    ("Advanced numeric expression strategies are deterministic", AdvancedNumericExpressionStrategiesAreDeterministic),
    ("Generated complements add missing groups and validate definitions", GeneratedComplementsAddMissingGroupsAndValidateDefinitions),
    ("Runtime body plan variants evaluate required generated groups", RuntimeBodyPlanVariantsEvaluateRequiredGeneratedGroups),
    ("Runtime body plan variant serialization preserves versioned state", RuntimeBodyPlanVariantSerializationPreservesVersionedState),
    ("Mosaic regional expression uses assigned genome version", MosaicRegionalExpressionUsesAssignedGenomeVersion),
    ("Mosaic inheritance sites resolve regional genome sources", MosaicInheritanceSitesResolveRegionalGenomeSources),
    ("Chimeric material remains distinct from integrated variants", ChimericMaterialRemainsDistinctFromIntegratedVariants),
};

var failures = new List<string>();

foreach (var (name, test) in tests)
{
    try
    {
        test();
        Console.WriteLine($"PASS {name}");
    }
    catch (Exception exception)
    {
        failures.Add($"{name}: {exception.Message}");
        Console.Error.WriteLine($"FAIL {name}: {exception.Message}");
    }
}

if (failures.Count > 0)
{
    Console.Error.WriteLine();
    Console.Error.WriteLine("Implementation test failures:");

    foreach (var failure in failures)
    {
        Console.Error.WriteLine($"- {failure}");
    }

    return 1;
}

Console.WriteLine("All implementation tests passed.");
return 0;

static void CoreAssemblyExposesStableName()
{
    AssertEqual("Genomancy.Core", GenomancyCoreAssembly.Name);
}

static void CoreAssemblyIsGodotIndependent()
{
    AssertTrue(GenomancyCoreAssembly.IsGodotIndependent, "Core assembly must be marked Godot independent.");
}

static void CoreAssemblyHasNoForbiddenDependencies()
{
    var forbiddenPrefixes = new[]
    {
        "Godot",
        "Microsoft.Data.Sqlite",
        "System.Data.SQLite",
        "SQLite",
        "xunit",
        "NUnit",
        "MSTest",
    };

    var references = typeof(GenomancyCoreAssembly)
        .Assembly
        .GetReferencedAssemblies()
        .Select(reference => reference.Name ?? string.Empty)
        .ToArray();

    foreach (var reference in references)
    {
        if (forbiddenPrefixes.Any(prefix => reference.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Forbidden core dependency found: {reference}");
        }
    }
}

static void ResourceIdsValidateAndCompareByStableValue()
{
    var id = ResourceId.Parse("common.human:skin-tone");

    AssertEqual("common.human:skin-tone", id.Value);
    AssertEqual(id, ResourceId.Parse(" common.human:skin-tone "));
    AssertThrows<ArgumentException>(() => ResourceId.Parse(""));
    AssertThrows<ArgumentException>(() => ResourceId.Parse("bad id"));
}

static void SystemDefinitionVersionsValidateAndCompareByStableValue()
{
    var version = SystemDefinitionVersion.Parse("genomancy.0.1.0+slice1");

    AssertEqual("genomancy.0.1.0+slice1", version.Value);
    AssertEqual(version, SystemDefinitionVersion.Parse(" genomancy.0.1.0+slice1 "));
    AssertThrows<ArgumentException>(() => SystemDefinitionVersion.Parse(""));
    AssertThrows<ArgumentException>(() => SystemDefinitionVersion.Parse("bad version"));
}

static void DesignModeAllowsAuthoredDefinitionEdits()
{
    var system = GenomancySystem.CreateDesign(TestVersion());

    AssertEqual(GenomancyMode.Design, system.Mode);
    AssertTrue(system.DesignDefinition is not null, "Design system must expose a design definition builder.");
    AssertTrue(system.RuntimeDefinition is null, "Design system must not expose a runtime definition.");

    var designDefinition = system.DesignDefinition ?? throw new InvalidOperationException("Design definition was null.");
    designDefinition.AddAllele(new AlleleDefinition(Id("human.baseline")));

    AssertEqual(1, designDefinition.Alleles.Count);
}

static void MinimalHumanBodyPlanGraphValidatesAndFreezes()
{
    var builder = CreateMinimalHumanBuilder();
    var result = builder.Validate();

    AssertTrue(result.IsValid, DiagnosticsToString(result.Diagnostics));

    var frozen = builder.Freeze();

    AssertEqual(TestVersion(), frozen.Version);
    AssertEqual(1, frozen.Alleles.Count);
    AssertEqual(1, frozen.Genes.Count);
    AssertEqual(1, frozen.Groups.Count);
    AssertEqual(1, frozen.BodyPlans.Count);
}

static void RuntimeStartupFreezesAValidatedDefinition()
{
    var builder = CreateMinimalHumanBuilder();
    var runtime = GenomancySystem.StartRuntime(builder);

    AssertEqual(GenomancyMode.Runtime, runtime.Mode);
    AssertTrue(runtime.IsRuntimeFrozen, "Runtime system must expose a frozen definition.");
    AssertTrue(runtime.DesignDefinition is null, "Runtime system must not expose an editable design definition.");
    AssertTrue(runtime.RuntimeDefinition is not null, "Runtime definition must be available.");
}

static void RuntimeStartupRejectsInvalidDefinitions()
{
    var invalid = new SystemDefinitionBuilder(TestVersion());
    invalid.AddGene(new GeneDefinition(Id("gene.skin"), [Id("missing.allele")]));

    var exception = AssertThrows<SystemDefinitionValidationException>(() => GenomancySystem.StartRuntime(invalid));

    AssertTrue(!exception.ValidationResult.IsValid, "Invalid runtime startup must carry failed validation result.");
    AssertContainsDiagnostic(exception.ValidationResult, "GENE_UNKNOWN_ALLELE");
}

static void FrozenDefinitionsRejectRetainedBuilderMutation()
{
    var builder = CreateMinimalHumanBuilder();

    _ = GenomancySystem.StartRuntime(builder);

    builder.AddAllele(new AlleleDefinition(Id("extra.allowed.before-source-freeze")));

    var frozen = builder.Freeze();

    AssertEqual(2, frozen.Alleles.Count);
    AssertThrows<DefinitionMutationException>(() => builder.AddAllele(new AlleleDefinition(Id("extra.after-freeze"))));
}

static void FrozenSnapshotIsIsolatedFromRetainedDesignReferences()
{
    var builder = CreateMinimalHumanBuilder();
    var runtime = GenomancySystem.StartRuntime(builder);

    builder.AddAllele(new AlleleDefinition(Id("extra.design-only")));

    AssertTrue(runtime.RuntimeDefinition is not null, "Runtime definition must exist.");
    var runtimeDefinition = runtime.RuntimeDefinition ?? throw new InvalidOperationException("Runtime definition was null.");
    AssertEqual(1, runtimeDefinition.Alleles.Count);
}

static void AuthoredReferencesResolveByStableId()
{
    var allele = new AlleleDefinition(Id("allele.skin.baseline"));
    var gene = new GeneDefinition(Id("gene.skin"), [ResourceId.Parse("allele.skin.baseline")]);
    var builder = new SystemDefinitionBuilder(TestVersion());

    builder.AddAllele(allele);
    builder.AddGene(gene);
    builder.AddGroup(new GroupDefinition(Id("group.common"), geneIds: [Id("gene.skin")]));
    builder.AddBodyPlan(new BodyPlanDefinition(Id("body.human"), requiredGroupIds: [Id("group.common")]));

    AssertTrue(builder.Validate().IsValid, "Equivalent stable ids must resolve even when objects are distinct instances.");
}

static void ValidationReportsDuplicateIdsAndMissingReferences()
{
    var builder = new SystemDefinitionBuilder(TestVersion());
    builder.AddAllele(new AlleleDefinition(Id("duplicate")));
    builder.AddAllele(new AlleleDefinition(Id("duplicate")));
    builder.AddGene(new GeneDefinition(Id("gene.skin"), [Id("missing.allele")]));
    builder.AddGroup(new GroupDefinition(Id("group.common"), geneIds: [Id("missing.gene")]));
    builder.AddBodyPlan(new BodyPlanDefinition(Id("body.human"), requiredGroupIds: [Id("missing.group")]));

    var result = builder.Validate();

    AssertTrue(!result.IsValid, "Invalid graph must fail validation.");
    AssertContainsDiagnostic(result, "DUPLICATE_ID");
    AssertContainsDiagnostic(result, "GENE_UNKNOWN_ALLELE");
    AssertContainsDiagnostic(result, "GROUP_UNKNOWN_GENE");
    AssertContainsDiagnostic(result, "BODY_PLAN_UNKNOWN_REQUIRED_GROUP");
}

static void ValidationReportsGroupDependencyCycles()
{
    var builder = new SystemDefinitionBuilder(TestVersion());
    builder.AddGroup(new GroupDefinition(Id("group.a"), dependencyGroupIds: [Id("group.b")]));
    builder.AddGroup(new GroupDefinition(Id("group.b"), dependencyGroupIds: [Id("group.a")]));

    var result = builder.Validate();

    AssertTrue(!result.IsValid, "Dependency cycle must fail validation.");
    AssertContainsDiagnostic(result, "GROUP_DEPENDENCY_CYCLE");
}

static void ValidationDiagnosticsAreDeterministic()
{
    static string Fingerprint(ValidationResult result)
    {
        return string.Join(
            "\n",
            result.Diagnostics.Select(diagnostic => $"{diagnostic.Severity}:{diagnostic.Code}:{diagnostic.Path}:{diagnostic.ResourceId}"));
    }

    var first = CreateNoisyInvalidBuilder().Validate();
    var second = CreateNoisyInvalidBuilder().Validate();

    AssertEqual(Fingerprint(first), Fingerprint(second));
}

static void RuntimeStartupAppliesMigrationBeforeFreeze()
{
    var builder = new SystemDefinitionBuilder(TestVersion());
    builder.AddAllele(new AlleleDefinition(Id("allele.skin.baseline")));
    builder.AddGene(new GeneDefinition(Id("gene.skin"), [Id("allele.skin.baseline")]));
    builder.AddGroup(new GroupDefinition(Id("group.common"), geneIds: [Id("gene.skin")]));

    var runtime = GenomancySystem.StartRuntime(
        builder,
        migration => migration.AddBodyPlan(new BodyPlanDefinition(Id("body.human"), requiredGroupIds: [Id("group.common")])));

    AssertTrue(runtime.RuntimeDefinition is not null, "Runtime definition must exist.");
    var runtimeDefinition = runtime.RuntimeDefinition ?? throw new InvalidOperationException("Runtime definition was null.");
    AssertEqual(1, runtimeDefinition.BodyPlans.Count);
    AssertEqual(Id("body.human"), runtimeDefinition.BodyPlans[0].Id);
}

static void GenomeVersionsIsolateCollectionAliases()
{
    var groups = new List<GenomeGroupState>
    {
        CreateCommonGroupState("allele.skin.baseline"),
    };

    var version = new GenomeVersion(
        GenomeVersionId.Parse("genome.v1"),
        TestVersion(),
        ExternalIndividualId.Parse("external:person-1"),
        new GenomeState(groups));

    groups.Add(CreateCommonGroupState("allele.skin.changed"));

    AssertEqual(1, version.State.Groups.Count);
    AssertEqual(Id("allele.skin.baseline"), version.State.Groups[0].GeneAlleles[0].Entries[0].AlleleId);
}

static void CurrentGenomeCopyEditsDiscardAndCommitExplicitly()
{
    var permanent = CreateGenomeVersion("genome.v1", null, "allele.skin.baseline");
    var current = new CurrentGenomeCopy(permanent);

    current.ReplaceGroup(CreateCommonGroupState("allele.skin.changed"));

    AssertTrue(current.HasUncommittedChanges, "Current copy must track divergence from the base version.");
    AssertEqual(Id("allele.skin.baseline"), permanent.State.Groups[0].GeneAlleles[0].Entries[0].AlleleId);
    AssertEqual(Id("allele.skin.changed"), current.CurrentState.Groups[0].GeneAlleles[0].Entries[0].AlleleId);

    current.DiscardChanges();

    AssertTrue(!current.HasUncommittedChanges, "Discard must restore the latest permanent version state.");
    AssertEqual(Id("allele.skin.baseline"), current.CurrentState.Groups[0].GeneAlleles[0].Entries[0].AlleleId);

    current.ReplaceGroup(CreateCommonGroupState("allele.skin.changed"));
    var committed = current.Commit(GenomeVersionId.Parse("genome.v2"), "manual change");

    AssertEqual(GenomeVersionId.Parse("genome.v1"), committed.ParentVersionId);
    AssertEqual(GenomeVersionId.Parse("genome.v2"), current.BaseVersion.Id);
    AssertTrue(!current.HasUncommittedChanges, "Commit must make the new permanent version current.");
    AssertEqual(Id("allele.skin.changed"), committed.State.Groups[0].GeneAlleles[0].Entries[0].AlleleId);
}

static void UncommittedCurrentGenomeChangesDoNotCreateVersionSpam()
{
    var permanent = CreateGenomeVersion("genome.v1", null, "allele.skin.baseline");
    var current = new CurrentGenomeCopy(permanent);

    current.ReplaceGroup(CreateCommonGroupState("allele.skin.changed-a"));
    current.ReplaceGroup(CreateCommonGroupState("allele.skin.changed-b"));

    AssertEqual(GenomeVersionId.Parse("genome.v1"), current.BaseVersion.Id);
    AssertEqual(null, current.BaseVersion.ParentVersionId);

    var committed = current.Commit(GenomeVersionId.Parse("genome.v2"));

    AssertEqual(GenomeVersionId.Parse("genome.v1"), committed.ParentVersionId);
    AssertEqual(Id("allele.skin.changed-b"), committed.State.Groups[0].GeneAlleles[0].Entries[0].AlleleId);
}

static void GenomeJsonRoundTripPreservesVersionStateAndAncestry()
{
    var version = CreateGenomeVersion("genome.v2", "genome.v1", "allele.skin.changed", numericValue: 0.75);
    var text = GenomeJsonCodec.WriteVersionToText(version);
    var roundTripped = GenomeJsonCodec.ReadVersionFromBuffer(System.Text.Encoding.UTF8.GetBytes(text), TestVersion());

    AssertGenomeVersionsEqual(version, roundTripped);
    AssertEqual(text, GenomeJsonCodec.WriteVersionToText(version));
}

static void GenomeBinaryRoundTripPreservesVersionStateAndAncestry()
{
    var version = CreateGenomeVersion("genome.v2", "genome.v1", "allele.skin.changed", numericValue: 0.75);
    var buffer = GenomeBinaryCodec.WriteVersionToBuffer(version);
    var roundTripped = GenomeBinaryCodec.ReadVersionFromBuffer(buffer, TestVersion());

    AssertGenomeVersionsEqual(version, roundTripped);
}

static void GenomeSerializersRejectMalformedAndUnknownVersions()
{
    AssertThrows<GenomeSerializationException>(
        () => GenomeJsonCodec.ReadVersionFromBuffer("{malformed"u8, TestVersion()));

    var futureVersion = CreateGenomeVersion("genome.v1", null, "allele.skin.baseline", systemVersion: "future.1");

    AssertThrows<GenomeSerializationException>(
        () => GenomeJsonCodec.ReadVersionFromBuffer(GenomeJsonCodec.WriteVersionToBuffer(futureVersion), TestVersion()));

    var accepted = GenomeJsonCodec.ReadVersionFromBuffer(
        GenomeJsonCodec.WriteVersionToBuffer(futureVersion),
        TestVersion(),
        static (_, _) => GenomeCompatibilityResult.Compatible());

    AssertEqual(SystemDefinitionVersion.Parse("future.1"), accepted.SystemDefinitionVersion);

    var binary = GenomeBinaryCodec.WriteVersionToBuffer(futureVersion);
    AssertThrows<GenomeSerializationException>(() => GenomeBinaryCodec.ReadVersionFromBuffer(binary[..4], TestVersion()));
}

static void GenomeSerializationUsesCallerSuppliedStreamsAndBuffers()
{
    var version = CreateGenomeVersion("genome.v1", null, "allele.skin.baseline");

    using var jsonStream = new MemoryStream();
    GenomeJsonCodec.WriteVersion(jsonStream, version);
    jsonStream.Position = 0;
    AssertGenomeVersionsEqual(version, GenomeJsonCodec.ReadVersion(jsonStream, TestVersion()));

    using var binaryStream = new MemoryStream();
    GenomeBinaryCodec.WriteVersion(binaryStream, version);
    binaryStream.Position = 0;
    AssertGenomeVersionsEqual(version, GenomeBinaryCodec.ReadVersion(binaryStream, TestVersion()));
}

static void GroupCompletenessReportsExpectedStates()
{
    var definition = CreateExpressionDefinition().Freeze();
    var complete = CreateExpressionGenomeState();

    AssertEqual(
        GroupCompletenessStatus.Complete,
        GroupCompletenessEvaluator.Evaluate(definition, complete, Id("group.common")).Status);

    AssertEqual(
        GroupCompletenessStatus.Missing,
        GroupCompletenessEvaluator.Evaluate(definition, new GenomeState([]), Id("group.common")).Status);

    var partial = new GenomeState([new GenomeGroupState(Id("group.common"), [])]);
    var partialResult = GroupCompletenessEvaluator.Evaluate(definition, partial, Id("group.common"));

    AssertEqual(GroupCompletenessStatus.Incomplete, partialResult.Status);
    AssertContainsExpressionDiagnostic(partialResult.Diagnostics, "GROUP_REQUIRED_GENE_MISSING");

    var wrongPloidy = new GenomeState(
        [
            new GenomeGroupState(
                Id("group.common"),
                [
                    new RankedAlleleSet(
                        Id("gene.skin"),
                        [
                            new RankedAlleleEntry(Id("allele.skin.light"), 0),
                        ]),
                ]),
        ]);
    var wrongPloidyResult = GroupCompletenessEvaluator.Evaluate(definition, wrongPloidy, Id("group.common"));

    AssertEqual(GroupCompletenessStatus.WrongPloidy, wrongPloidyResult.Status);
    AssertContainsExpressionDiagnostic(wrongPloidyResult.Diagnostics, "GROUP_GENE_WRONG_PLOIDY");

    var dependencyFailure = GroupCompletenessEvaluator.Evaluate(
        definition,
        new GenomeState(
            [
                new GenomeGroupState(
                    Id("group.dependent"),
                    [
                        new RankedAlleleSet(
                            Id("gene.fur"),
                            [
                                new RankedAlleleEntry(Id("allele.fur.short"), 0),
                            ]),
                    ]),
            ]),
        Id("group.dependent"));

    AssertEqual(GroupCompletenessStatus.DependencyFailed, dependencyFailure.Status);
    AssertContainsExpressionDiagnostic(dependencyFailure.Diagnostics, "GROUP_DEPENDENCY_FAILED");
}

static void SharedGroupEvaluatesUnderDistinctBodyPlanContexts()
{
    var definition = CreateExpressionDefinition().Freeze();
    var genomeState = CreateExpressionGenomeState();
    var expressionState = new BodyPlanExpressionState([Id("body.human"), Id("body.wolf")]);
    var phase = DevelopmentalPhaseId.Parse("phase.adult");
    var externalContext = new ExpressionExternalContext();

    var human = BodyPlanAvailabilityEvaluator.Evaluate(
        definition,
        genomeState,
        expressionState,
        Id("body.human"),
        phase,
        externalContext);
    var wolf = BodyPlanAvailabilityEvaluator.Evaluate(
        definition,
        genomeState,
        expressionState,
        Id("body.wolf"),
        phase,
        externalContext);

    AssertEqual(BodyPlanAvailabilityStatus.Active, human.Status);
    AssertEqual(BodyPlanAvailabilityStatus.Active, wolf.Status);
    AssertTrue(human.GroupResults.Any(result => result.GroupId == Id("group.common")), "Human context must include shared common group.");
    AssertTrue(wolf.GroupResults.Any(result => result.GroupId == Id("group.common")), "Wolf context must include shared common group.");
}

static void BodyPlanActivationDoesNotCreateGenomeVersions()
{
    var version = CreateGenomeVersion("genome.v1", null, "allele.skin.baseline");
    var current = new CurrentGenomeCopy(version);
    var state = new BodyPlanExpressionState();

    state = state.Activate(Id("body.human"));
    state = state.Deactivate(Id("body.human"));

    AssertEqual(GenomeVersionId.Parse("genome.v1"), current.BaseVersion.Id);
    AssertTrue(!current.HasUncommittedChanges, "Activation state changes must not mutate genome current copies.");
}

static void ExpressionStrategiesAreDeterministicAndDistinct()
{
    var context = new ExpressionEvaluationContext(
        Id("body.human"),
        DevelopmentalPhaseId.Parse("phase.adult"),
        null,
        new ExpressionExternalContext());
    var alleleSet = new RankedAlleleSet(
        Id("gene.skin"),
        [
            new RankedAlleleEntry(Id("allele.skin.dark"), 1, 0.9),
            new RankedAlleleEntry(Id("allele.skin.light"), 0, 0.1),
        ]);

    var dominance = GeneExpressionEvaluator.Evaluate(
        context,
        new GeneDefinition(
            Id("gene.skin"),
            [Id("allele.skin.dark"), Id("allele.skin.light")],
            requiredAlleleCount: 2,
            expressionStrategy: GeneExpressionStrategy.StrictDominance),
        alleleSet);
    var codominance = GeneExpressionEvaluator.Evaluate(
        context,
        new GeneDefinition(
            Id("gene.skin"),
            [Id("allele.skin.dark"), Id("allele.skin.light")],
            requiredAlleleCount: 2,
            expressionStrategy: GeneExpressionStrategy.Codominance),
        alleleSet);
    var midpoint = GeneExpressionEvaluator.Evaluate(
        context,
        new GeneDefinition(
            Id("gene.skin"),
            [Id("allele.skin.dark"), Id("allele.skin.light")],
            requiredAlleleCount: 2,
            expressionStrategy: GeneExpressionStrategy.NumericMidpoint),
        alleleSet);

    AssertEqual(1, dominance.ExpressedAlleleIds.Count);
    AssertEqual(Id("allele.skin.light"), dominance.ExpressedAlleleIds[0]);
    AssertEqual(2, codominance.ExpressedAlleleIds.Count);
    AssertEqual(0.5, midpoint.NumericValue);
    AssertEqual(GeneExpressionEvaluator.Evaluate(
        context,
        new GeneDefinition(
            Id("gene.skin"),
            [Id("allele.skin.dark"), Id("allele.skin.light")],
            requiredAlleleCount: 2,
            expressionStrategy: GeneExpressionStrategy.NumericMidpoint),
        alleleSet), midpoint);
}

static void ExpressionRequiresExplicitContextAndOpaqueExternalFacts()
{
    var facts = new List<KeyValuePair<string, string>>
    {
        new("moon", "full"),
    };
    var externalContext = new ExpressionExternalContext(facts);
    facts.Add(new KeyValuePair<string, string>("mutation-after-context", "ignored"));

    var context = new ExpressionEvaluationContext(
        Id("body.human"),
        DevelopmentalPhaseId.Parse("phase.adult"),
        CreateExpressionCommonGroupState(),
        externalContext);
    var gene = new GeneDefinition(
        Id("gene.skin"),
        [Id("allele.skin.light"), Id("allele.skin.dark")],
        requiredAlleleCount: 2);
    var alleleSet = context.GroupState?.GeneAlleles.Single(set => set.GeneId == Id("gene.skin"))
        ?? throw new InvalidOperationException("Expected group state.");

    AssertEqual(1, externalContext.Facts.Count);
    AssertEqual(Id("body.human"), context.BodyPlanId);
    AssertEqual(DevelopmentalPhaseId.Parse("phase.adult"), context.DevelopmentalPhaseId);
    AssertThrows<ArgumentNullException>(() => GeneExpressionEvaluator.Evaluate(null!, gene, alleleSet));
}

static void IndividualExpressionStateRequiresActiveAvailableBodyPlan()
{
    var definition = CreateExpressionDefinition().Freeze();
    var genomeState = CreateExpressionGenomeState();
    var phase = DevelopmentalPhaseId.Parse("phase.adult");
    var externalContext = new ExpressionExternalContext();

    AssertTrue(
        !IndividualExpressionStateValidator.HasAtLeastOneActiveAvailableBodyPlan(
            definition,
            genomeState,
            new BodyPlanExpressionState(),
            phase,
            externalContext),
        "Dormant body plans must not satisfy active individual runtime state.");

    AssertTrue(
        IndividualExpressionStateValidator.HasAtLeastOneActiveAvailableBodyPlan(
            definition,
            genomeState,
            new BodyPlanExpressionState([Id("body.human")]),
            phase,
            externalContext),
        "At least one active complete body plan must satisfy individual runtime state.");
}

static void SeededDiploidReproductionProducesDeterministicOffspringSerialization()
{
    var definition = CreateReproductionDefinition(requiredSkinAlleleCount: 2).Freeze();
    var mother = CreateParentGenome("parent.mother", "external:mother", "allele.skin.light", "allele.skin.dark");
    var father = CreateParentGenome("parent.father", "external:father", "allele.skin.dark", "allele.skin.light");

    var first = Reproduce(
        definition,
        42,
        [new ReproductionParentRole("mother", mother), new ReproductionParentRole("father", father)]);
    var second = Reproduce(
        definition,
        42,
        [new ReproductionParentRole("mother", mother), new ReproductionParentRole("father", father)]);

    AssertEqual(ReproductionResultStatus.Success, first.Status);
    AssertTrue(first.OffspringVersion is not null, "Successful reproduction must create offspring.");
    AssertTrue(second.OffspringVersion is not null, "Successful reproduction must create offspring.");
    var firstOffspring = first.OffspringVersion ?? throw new InvalidOperationException("Expected offspring.");
    var secondOffspring = second.OffspringVersion ?? throw new InvalidOperationException("Expected offspring.");
    AssertEqual(
        GenomeJsonCodec.WriteVersionToText(firstOffspring),
        GenomeJsonCodec.WriteVersionToText(secondOffspring));
    AssertEqual(definition.Version, firstOffspring.SystemDefinitionVersion);
    AssertEqual(2, OffspringAlleles(first, Id("group.common"), Id("gene.skin")).Count);
}

static void MultiParentAndVariablePloidyReproductionUsesExplicitContributors()
{
    var definition = CreateReproductionDefinition(requiredSkinAlleleCount: 3).Freeze();
    var firstParent = CreateParentGenome("parent.one", "external:one", "allele.skin.light");
    var secondParent = CreateParentGenome("parent.two", "external:two", "allele.skin.dark");
    var thirdParent = CreateParentGenome("parent.three", "external:three", "allele.skin.gold");

    var result = Reproduce(
        definition,
        7,
        [
            new ReproductionParentRole("one", firstParent),
            new ReproductionParentRole("two", secondParent),
            new ReproductionParentRole("three", thirdParent),
        ]);

    AssertEqual(ReproductionResultStatus.Success, result.Status);
    AssertEqual(3, OffspringAlleles(result, Id("group.common"), Id("gene.skin")).Count);

    var diploidDefinition = CreateReproductionDefinition(requiredSkinAlleleCount: 2).Freeze();
    var lowerArity = Reproduce(
        diploidDefinition,
        7,
        [
            new ReproductionParentRole("one", firstParent),
            new ReproductionParentRole("two", secondParent),
            new ReproductionParentRole("three", thirdParent),
        ],
        new ReproductionPolicy(contributingRoleNames: ["one", "three"]));

    AssertEqual(ReproductionResultStatus.Success, lowerArity.Status);
    var alleleIds = OffspringAlleles(lowerArity, Id("group.common"), Id("gene.skin"))
        .Select(entry => entry.AlleleId)
        .ToArray();
    AssertEqual(Id("allele.skin.light"), alleleIds[0]);
    AssertEqual(Id("allele.skin.gold"), alleleIds[1]);
}

static void AmbiguousLowerArityReproductionIsRejected()
{
    var definition = CreateReproductionDefinition(requiredSkinAlleleCount: 2).Freeze();
    var result = Reproduce(
        definition,
        7,
        [
            new ReproductionParentRole("one", CreateParentGenome("parent.one", "external:one", "allele.skin.light")),
            new ReproductionParentRole("two", CreateParentGenome("parent.two", "external:two", "allele.skin.dark")),
            new ReproductionParentRole("three", CreateParentGenome("parent.three", "external:three", "allele.skin.gold")),
        ]);

    AssertEqual(ReproductionResultStatus.InvalidRequest, result.Status);
    AssertContainsReproductionDiagnostic(result.Diagnostics, "REPRODUCTION_AMBIGUOUS_PLOIDY");
}

static void SameIndividualCanFillMultipleParentRoles()
{
    var definition = CreateReproductionDefinition(requiredSkinAlleleCount: 2).Freeze();
    var parent = CreateParentGenome("parent.same", "external:same", "allele.skin.light");
    var result = Reproduce(
        definition,
        13,
        [new ReproductionParentRole("self-a", parent), new ReproductionParentRole("self-b", parent)]);

    AssertEqual(ReproductionResultStatus.Success, result.Status);
    AssertEqual(2, OffspringAlleles(result, Id("group.common"), Id("gene.skin")).Count);
}

static void WeightedTransmissionExcludesZeroWeightsAndValidatesFiniteWeights()
{
    var definition = CreateReproductionDefinition(requiredSkinAlleleCount: 1).Freeze();
    var parent = CreateParentGenome("parent.weighted", "external:weighted", "allele.skin.light", "allele.skin.dark");
    var result = Reproduce(
        definition,
        99,
        [new ReproductionParentRole("source", parent)],
        new ReproductionPolicy(
            transmissionWeights:
            [
                new TransmissionWeight("source", Id("group.common"), Id("gene.skin"), Id("allele.skin.light"), 0),
                new TransmissionWeight("source", Id("group.common"), Id("gene.skin"), Id("allele.skin.dark"), 1),
            ]));

    AssertEqual(ReproductionResultStatus.Success, result.Status);
    AssertEqual(Id("allele.skin.dark"), OffspringAlleles(result, Id("group.common"), Id("gene.skin"))[0].AlleleId);
    AssertThrows<ArgumentOutOfRangeException>(
        () => new TransmissionWeight("source", Id("group.common"), Id("gene.skin"), Id("allele.skin.light"), double.PositiveInfinity));

    var lightWins = 0;
    var darkWins = 0;

    for (ulong seed = 0; seed < 128; seed++)
    {
        var weightedResult = Reproduce(
            definition,
            seed,
            [new ReproductionParentRole("source", parent)],
            new ReproductionPolicy(
                transmissionWeights:
                [
                    new TransmissionWeight("source", Id("group.common"), Id("gene.skin"), Id("allele.skin.light"), 9),
                    new TransmissionWeight("source", Id("group.common"), Id("gene.skin"), Id("allele.skin.dark"), 1),
                ]));
        var selectedAllele = OffspringAlleles(weightedResult, Id("group.common"), Id("gene.skin"))[0].AlleleId;

        if (selectedAllele == Id("allele.skin.light"))
        {
            lightWins++;
        }
        else if (selectedAllele == Id("allele.skin.dark"))
        {
            darkWins++;
        }
    }

    AssertTrue(lightWins > darkWins, "Higher weighted allele should win more often across a deterministic seed sweep.");
}

static void NamedRandomStreamsPreventUnrelatedGenePerturbation()
{
    var baseDefinition = CreateReproductionDefinition(requiredSkinAlleleCount: 2).Freeze();
    var expandedDefinition = CreateReproductionDefinition(requiredSkinAlleleCount: 2, includeFur: true).Freeze();
    var mother = CreateParentGenome("parent.mother", "external:mother", "allele.skin.light", "allele.skin.dark", includeFur: true);
    var father = CreateParentGenome("parent.father", "external:father", "allele.skin.dark", "allele.skin.light", includeFur: true);

    var baseResult = Reproduce(
        baseDefinition,
        123,
        [new ReproductionParentRole("mother", mother), new ReproductionParentRole("father", father)]);
    var expandedResult = Reproduce(
        expandedDefinition,
        123,
        [new ReproductionParentRole("mother", mother), new ReproductionParentRole("father", father)]);

    AssertEqual(
        OffspringAlleles(baseResult, Id("group.common"), Id("gene.skin"))[0].AlleleId,
        OffspringAlleles(expandedResult, Id("group.common"), Id("gene.skin"))[0].AlleleId);
    AssertEqual(
        OffspringAlleles(baseResult, Id("group.common"), Id("gene.skin"))[1].AlleleId,
        OffspringAlleles(expandedResult, Id("group.common"), Id("gene.skin"))[1].AlleleId);
}

static void ReproductionReportsCompatibilityOutcomesAndPreservesParents()
{
    var definition = CreateReproductionDefinition(requiredSkinAlleleCount: 2).Freeze();
    var mother = CreateParentGenome("parent.mother", "external:mother", "allele.skin.light");
    var father = CreateParentGenome("parent.father", "external:father", "allele.skin.dark");
    var motherBefore = GenomeJsonCodec.WriteVersionToText(mother);
    var fatherBefore = GenomeJsonCodec.WriteVersionToText(father);

    var sterile = Reproduce(
        definition,
        55,
        [new ReproductionParentRole("mother", mother), new ReproductionParentRole("father", father)],
        new ReproductionPolicy(ReproductionCompatibility.Sterile));
    var incompatible = Reproduce(
        definition,
        55,
        [new ReproductionParentRole("mother", mother), new ReproductionParentRole("father", father)],
        new ReproductionPolicy(ReproductionCompatibility.Incompatible));

    AssertEqual(ReproductionResultStatus.Sterile, sterile.Status);
    AssertEqual(ReproductionResultStatus.Incompatible, incompatible.Status);
    AssertEqual(motherBefore, GenomeJsonCodec.WriteVersionToText(mother));
    AssertEqual(fatherBefore, GenomeJsonCodec.WriteVersionToText(father));
}

static void CurrentOnlyMutationDivergesWithoutCommittingVersion()
{
    var definition = CreateMutationDefinition().Freeze();
    var current = new CurrentGenomeCopy(CreateMutationGenomeVersion());

    var result = GenomeMutationService.Apply(
        definition,
        current,
        new GenomeMutationPolicy(),
        new GenomeMutationRequest(
            MutationSourceKind.InternalPolicy,
            "policy.test",
            MutationApplicationMode.CurrentOnly,
            [GenomeMutationOperation.ReplaceAllele(Id("group.common"), Id("gene.skin"), 0, Id("allele.skin.dark"))]));

    AssertEqual(GenomeMutationResultStatus.AppliedToCurrent, result.Status);
    AssertEqual(null, result.CommittedVersion);
    AssertTrue(current.HasUncommittedChanges, "Current-only mutation must not commit automatically.");
    AssertEqual(GenomeVersionId.Parse("genome.mutation.v1"), current.BaseVersion.Id);
    AssertEqual(Id("allele.skin.dark"), CurrentAlleles(current, Id("group.common"), Id("gene.skin"))[0].AlleleId);
    AssertEqual(Id("allele.skin.light"), current.BaseVersion.State.Groups[0].GeneAlleles[0].Entries[0].AlleleId);
}

static void CommittedMutationCreatesImmutableChildVersion()
{
    var definition = CreateMutationDefinition().Freeze();
    var current = new CurrentGenomeCopy(CreateMutationGenomeVersion());

    var result = GenomeMutationService.Apply(
        definition,
        current,
        new GenomeMutationPolicy(),
        new GenomeMutationRequest(
            MutationSourceKind.InternalPolicy,
            "policy.test",
            MutationApplicationMode.Commit,
            [GenomeMutationOperation.ReplaceAllele(Id("group.common"), Id("gene.skin"), 0, Id("allele.skin.dark"))],
            GenomeVersionId.Parse("genome.mutation.v2"),
            "skin mutation"));

    AssertEqual(GenomeMutationResultStatus.Committed, result.Status);
    AssertTrue(result.CommittedVersion is not null, "Committed mutation must return a new version.");
    var committed = result.CommittedVersion ?? throw new InvalidOperationException("Expected committed version.");

    AssertEqual(GenomeVersionId.Parse("genome.mutation.v2"), committed.Id);
    AssertEqual(GenomeVersionId.Parse("genome.mutation.v1"), committed.ParentVersionId);
    AssertEqual("skin mutation", committed.ChangeSummary);
    AssertTrue(!current.HasUncommittedChanges, "Committed mutation becomes the new base current state.");
    AssertThrows<ArgumentException>(() => current.Commit(GenomeVersionId.Parse("genome.mutation.v2")));
}

static void MutationPolicyRejectsProtectedTargetsAndInvalidAlleles()
{
    var definition = CreateMutationDefinition().Freeze();
    var protectedCurrent = new CurrentGenomeCopy(CreateMutationGenomeVersion());
    var protectedResult = GenomeMutationService.Apply(
        definition,
        protectedCurrent,
        new GenomeMutationPolicy(protectedGeneIds: [Id("gene.skin")]),
        new GenomeMutationRequest(
            MutationSourceKind.InternalPolicy,
            "policy.test",
            MutationApplicationMode.CurrentOnly,
            [GenomeMutationOperation.ReplaceAllele(Id("group.common"), Id("gene.skin"), 0, Id("allele.skin.dark"))]));

    AssertEqual(GenomeMutationResultStatus.Rejected, protectedResult.Status);
    AssertContainsMutationDiagnostic(protectedResult.Diagnostics, "MUTATION_TARGET_PROTECTED");
    AssertTrue(!protectedCurrent.HasUncommittedChanges, "Rejected mutation must leave current state unchanged.");

    var invalidAlleleCurrent = new CurrentGenomeCopy(CreateMutationGenomeVersion());
    var invalidAlleleResult = GenomeMutationService.Apply(
        definition,
        invalidAlleleCurrent,
        new GenomeMutationPolicy(),
        new GenomeMutationRequest(
            MutationSourceKind.InternalPolicy,
            "policy.test",
            MutationApplicationMode.CurrentOnly,
            [GenomeMutationOperation.ReplaceAllele(Id("group.common"), Id("gene.skin"), 0, Id("allele.fur.short"))]));

    AssertEqual(GenomeMutationResultStatus.Rejected, invalidAlleleResult.Status);
    AssertContainsMutationDiagnostic(invalidAlleleResult.Diagnostics, "MUTATION_ALLELE_NOT_ALLOWED");
}

static void NumericCopyCountAndStructuralMutationsUpdateCurrentState()
{
    var definition = CreateMutationDefinition().Freeze();
    var current = new CurrentGenomeCopy(CreateMutationGenomeVersion());

    var result = GenomeMutationService.Apply(
        definition,
        current,
        new GenomeMutationPolicy(),
        new GenomeMutationRequest(
            MutationSourceKind.InternalPolicy,
            "policy.test",
            MutationApplicationMode.CurrentOnly,
            [
                GenomeMutationOperation.UpdateNumericValue(Id("group.common"), Id("gene.skin"), 0, 0.75),
                GenomeMutationOperation.SetCopyCount(Id("group.common"), Id("gene.skin"), 2),
                GenomeMutationOperation.AddGroup(new GenomeGroupState(
                    Id("group.fur"),
                    [
                        new RankedAlleleSet(
                            Id("gene.fur"),
                            [
                                new RankedAlleleEntry(Id("allele.fur.short"), 0),
                            ]),
                    ])),
            ]));

    AssertEqual(GenomeMutationResultStatus.AppliedToCurrent, result.Status);
    AssertEqual(2, CurrentAlleles(current, Id("group.common"), Id("gene.skin")).Count);
    AssertEqual(0.75, CurrentAlleles(current, Id("group.common"), Id("gene.skin"))[0].NumericValue);
    AssertTrue(current.CurrentState.Groups.Any(group => group.GroupId == Id("group.fur")), "Structural add-group mutation must add group state.");

    var removeResult = GenomeMutationService.Apply(
        definition,
        current,
        new GenomeMutationPolicy(),
        new GenomeMutationRequest(
            MutationSourceKind.InternalPolicy,
            "policy.test",
            MutationApplicationMode.CurrentOnly,
            [GenomeMutationOperation.RemoveGroup(Id("group.fur"))]));

    AssertEqual(GenomeMutationResultStatus.AppliedToCurrent, removeResult.Status);
    AssertTrue(!current.CurrentState.Groups.Any(group => group.GroupId == Id("group.fur")), "Structural remove-group mutation must remove group state.");
}

static void RepairAndRevertRestoreCurrentGenomeState()
{
    var definition = CreateMutationDefinition().Freeze();
    var current = new CurrentGenomeCopy(CreateMutationGenomeVersion());

    _ = GenomeMutationService.Apply(
        definition,
        current,
        new GenomeMutationPolicy(),
        new GenomeMutationRequest(
            MutationSourceKind.InternalPolicy,
            "policy.test",
            MutationApplicationMode.CurrentOnly,
            [
                GenomeMutationOperation.ReplaceAllele(Id("group.common"), Id("gene.skin"), 0, Id("allele.skin.dark")),
                GenomeMutationOperation.AddGroup(new GenomeGroupState(
                    Id("group.fur"),
                    [
                        new RankedAlleleSet(
                            Id("gene.fur"),
                            [
                                new RankedAlleleEntry(Id("allele.fur.short"), 0),
                            ]),
                    ])),
            ]));

    var repairGene = GenomeMutationService.RepairFromBase(current, Id("group.common"), Id("gene.skin"));

    AssertEqual(GenomeMutationResultStatus.Repaired, repairGene.Status);
    AssertEqual(Id("allele.skin.light"), CurrentAlleles(current, Id("group.common"), Id("gene.skin"))[0].AlleleId);
    AssertTrue(current.CurrentState.Groups.Any(group => group.GroupId == Id("group.fur")), "Gene repair should not remove unrelated groups.");

    var revert = GenomeMutationService.RevertCurrent(current);

    AssertEqual(GenomeMutationResultStatus.Reverted, revert.Status);
    AssertTrue(!current.HasUncommittedChanges, "Revert must discard all current-only changes.");
    AssertTrue(!current.CurrentState.Groups.Any(group => group.GroupId == Id("group.fur")), "Revert must restore base group set.");
}

static void ExternalMutationSourceIsExplicitAndPolicyControlled()
{
    var definition = CreateMutationDefinition().Freeze();
    var blockedCurrent = new CurrentGenomeCopy(CreateMutationGenomeVersion());
    var blocked = GenomeMutationService.Apply(
        definition,
        blockedCurrent,
        new GenomeMutationPolicy(allowExternalRequests: false),
        new GenomeMutationRequest(
            MutationSourceKind.ExternalRequest,
            "spell.polymorph",
            MutationApplicationMode.CurrentOnly,
            [GenomeMutationOperation.ReplaceAllele(Id("group.common"), Id("gene.skin"), 0, Id("allele.skin.dark"))]));

    AssertEqual(GenomeMutationResultStatus.Rejected, blocked.Status);
    AssertContainsMutationDiagnostic(blocked.Diagnostics, "MUTATION_EXTERNAL_SOURCE_BLOCKED");

    var allowedCurrent = new CurrentGenomeCopy(CreateMutationGenomeVersion());
    var allowed = GenomeMutationService.Apply(
        definition,
        allowedCurrent,
        new GenomeMutationPolicy(allowExternalRequests: true),
        new GenomeMutationRequest(
            MutationSourceKind.ExternalRequest,
            "spell.polymorph",
            MutationApplicationMode.CurrentOnly,
            [GenomeMutationOperation.ReplaceAllele(Id("group.common"), Id("gene.skin"), 0, Id("allele.skin.dark"))]));

    AssertEqual(GenomeMutationResultStatus.AppliedToCurrent, allowed.Status);
    AssertEqual(Id("allele.skin.dark"), CurrentAlleles(allowedCurrent, Id("group.common"), Id("gene.skin"))[0].AlleleId);
}

static void NonPloidalObjectInheritanceIsWeightedAndDeterministic()
{
    var firstParent = CreateHeritableParent(
        "parent.first",
        "external:first",
        new HeritableObjectState(
            [
                new NonPloidalObjectState(
                    Id("marker.blessing"),
                    NonPloidalObjectKind.Marker,
                    textValue: "first",
                    transmissionWeight: 9),
            ]));
    var secondParent = CreateHeritableParent(
        "parent.second",
        "external:second",
        new HeritableObjectState(
            [
                new NonPloidalObjectState(
                    Id("marker.blessing"),
                    NonPloidalObjectKind.Marker,
                    textValue: "second",
                    transmissionWeight: 1),
            ]));

    var first = NonPloidalInheritanceService.Inherit(
        [new ReproductionParentRole("first", firstParent), new ReproductionParentRole("second", secondParent)],
        42);
    var second = NonPloidalInheritanceService.Inherit(
        [new ReproductionParentRole("first", firstParent), new ReproductionParentRole("second", secondParent)],
        42);

    AssertEqual(first.State, second.State);
    AssertEqual(1, first.State.NonPloidalObjects.Count);
    AssertEqual(Id("marker.blessing"), first.State.NonPloidalObjects[0].Id);

    var firstWins = 0;
    var secondWins = 0;

    for (ulong seed = 0; seed < 128; seed++)
    {
        var result = NonPloidalInheritanceService.Inherit(
            [new ReproductionParentRole("first", firstParent), new ReproductionParentRole("second", secondParent)],
            seed);
        var selected = result.State.NonPloidalObjects[0].TextValue;

        if (selected == "first")
        {
            firstWins++;
        }
        else if (selected == "second")
        {
            secondWins++;
        }
    }

    AssertTrue(firstWins > secondWins, "Higher weighted non-ploidal object should win more often across a deterministic seed sweep.");
}

static void NonPloidalInheritanceRespectsInactiveAndZeroWeightObjects()
{
    var parent = CreateHeritableParent(
        "parent.state",
        "external:state",
        new HeritableObjectState(
            [
                new NonPloidalObjectState(Id("flag.inactive"), NonPloidalObjectKind.Flag, isActive: false),
                new NonPloidalObjectState(Id("flag.zero"), NonPloidalObjectKind.Flag, transmissionWeight: 0),
                new NonPloidalObjectState(Id("flag.active"), NonPloidalObjectKind.Flag),
            ]));

    var activeOnly = NonPloidalInheritanceService.Inherit([new ReproductionParentRole("source", parent)], 12);
    var includeInactive = NonPloidalInheritanceService.Inherit(
        [new ReproductionParentRole("source", parent)],
        12,
        includeInactiveObjects: true);

    AssertTrue(!activeOnly.State.NonPloidalObjects.Any(value => value.Id == Id("flag.inactive")), "Inactive objects should not transmit by default.");
    AssertTrue(!includeInactive.State.NonPloidalObjects.Any(value => value.Id == Id("flag.zero")), "Zero-weight objects should not transmit.");
    AssertTrue(includeInactive.State.NonPloidalObjects.Any(value => value.Id == Id("flag.inactive")), "Explicit include-inactive should allow active-state-independent transmission.");
}

static void TraceStateSupportsActivationReplacementAndDegradation()
{
    var state = new HeritableObjectState(
        traces:
        [
            new TraceState(Id("trace.dragon"), Id("source.dragon"), 1.0, degradationPerStep: 0.25),
        ]);

    state = TraceUpdateService.ActivateTrace(state, Id("trace.dragon"));
    state = TraceUpdateService.ReplaceTraceStrength(state, Id("trace.dragon"), 0.8);
    state = TraceUpdateService.DegradeTraces(state, steps: 2);

    var trace = state.Traces.Single(value => value.Id == Id("trace.dragon"));

    AssertTrue(trace.IsActive, "Trace activation must persist through replacement and degradation.");
    AssertEqual(0.3, Math.Round(trace.Strength, 10));
    AssertEqual(2, trace.Age);
}

static void ReproductionCanInheritNonPloidalObjectsAndTraces()
{
    var definition = CreateReproductionDefinition(requiredSkinAlleleCount: 2).Freeze();
    var mother = CreateHeritableParent(
        "parent.mother",
        "external:mother",
        new HeritableObjectState(
            [
                new NonPloidalObjectState(Id("archive.maternal"), NonPloidalObjectKind.Archive, textValue: "memory"),
            ],
            [
                new TraceState(Id("trace.maternal"), Id("source.maternal"), 0.8, isActive: true, degradationPerStep: 0.1),
            ]),
        "allele.skin.light");
    var father = CreateHeritableParent(
        "parent.father",
        "external:father",
        new HeritableObjectState(),
        "allele.skin.dark");

    var result = Reproduce(
        definition,
        8,
        [new ReproductionParentRole("mother", mother), new ReproductionParentRole("father", father)],
        new ReproductionPolicy(inheritNonPloidalObjects: true, inheritedTraceDegradationSteps: 1));

    AssertEqual(ReproductionResultStatus.Success, result.Status);
    var offspring = result.OffspringVersion ?? throw new InvalidOperationException("Expected offspring.");

    AssertTrue(offspring.HeritableObjects.NonPloidalObjects.Any(value => value.Id == Id("archive.maternal")), "Reproduction should carry inherited non-ploidal objects when requested.");
    AssertEqual(0.7, Math.Round(offspring.HeritableObjects.Traces.Single(value => value.Id == Id("trace.maternal")).Strength, 10));
}

static void GenomeSerializationPreservesNonPloidalObjectsAndTraces()
{
    var version = CreateHeritableParent(
        "parent.serial",
        "external:serial",
        new HeritableObjectState(
            [
                new NonPloidalObjectState(
                    Id("counter.oath"),
                    NonPloidalObjectKind.Counter,
                    numericValue: 3,
                    textValue: "kept",
                    transmissionWeight: 0.5),
            ],
            [
                new TraceState(Id("trace.oath"), Id("source.oath"), 0.6, isActive: true, age: 2, degradationPerStep: 0.1),
            ]));

    var jsonRoundTrip = GenomeJsonCodec.ReadVersionFromBuffer(GenomeJsonCodec.WriteVersionToBuffer(version), TestVersion());
    var binaryRoundTrip = GenomeBinaryCodec.ReadVersionFromBuffer(GenomeBinaryCodec.WriteVersionToBuffer(version), TestVersion());

    AssertEqual(version.HeritableObjects, jsonRoundTrip.HeritableObjects);
    AssertEqual(version.HeritableObjects, binaryRoundTrip.HeritableObjects);
}

static void CompatibilityServiceReportsLayeredOutcomesAndHybridMorphology()
{
    var roles = new[]
    {
        new ReproductionParentRole("mother", CreateParentGenome("parent.mother", "external:mother", "allele.skin.light")),
        new ReproductionParentRole("father", CreateParentGenome("parent.father", "external:father", "allele.skin.dark")),
    };
    var evaluation = CompatibilityService.Evaluate(
        roles,
        [
            new CompatibilityRule(
                ReproductionCompatibility.Fertile,
                requiredRoleNames: ["mother", "father"],
                hybridMorphologyContributions:
                [
                    new HybridMorphologyContribution(Id("body.human"), 0.5),
                    new HybridMorphologyContribution(Id("body.wolf"), 0.5),
                ],
                "hybrid morphology"),
        ]);

    AssertEqual(ReproductionCompatibility.Fertile, evaluation.Compatibility);
    AssertEqual(2, evaluation.HybridMorphologyContributions.Count);
    AssertEqual("hybrid morphology", evaluation.Reason);
}

static void ReproductionReportsInviableCompatibilityOutcome()
{
    var definition = CreateReproductionDefinition(requiredSkinAlleleCount: 2).Freeze();
    var result = Reproduce(
        definition,
        44,
        [
            new ReproductionParentRole("mother", CreateParentGenome("parent.mother", "external:mother", "allele.skin.light")),
            new ReproductionParentRole("father", CreateParentGenome("parent.father", "external:father", "allele.skin.dark")),
        ],
        new ReproductionPolicy(ReproductionCompatibility.Inviable));

    AssertEqual(ReproductionResultStatus.Inviable, result.Status);
    AssertContainsReproductionDiagnostic(result.Diagnostics, "REPRODUCTION_INVIABLE");
}

static void ClonalCopyReproductionCreatesOffspringWithoutAlleleRecombination()
{
    var definition = CreateReproductionDefinition(requiredSkinAlleleCount: 2).Freeze();
    var source = CreateParentGenome("parent.clone", "external:clone-source", "allele.skin.light", "allele.skin.dark", includeFur: false);
    var result = Reproduce(
        definition,
        78,
        [new ReproductionParentRole("source", source)],
        new ReproductionPolicy(
            mode: ReproductionMode.ClonalCopy,
            contributingRoleNames: ["source"]));

    AssertEqual(ReproductionResultStatus.Success, result.Status);
    var offspring = result.OffspringVersion ?? throw new InvalidOperationException("Expected offspring.");
    AssertEqual(source.State, offspring.State);
    AssertEqual(TestVersion(), offspring.SystemDefinitionVersion);

    var invalid = Reproduce(
        definition,
        78,
        [
            new ReproductionParentRole("one", source),
            new ReproductionParentRole("two", source),
        ],
        new ReproductionPolicy(mode: ReproductionMode.ClonalCopy));

    AssertEqual(ReproductionResultStatus.InvalidRequest, invalid.Status);
    AssertContainsReproductionDiagnostic(invalid.Diagnostics, "REPRODUCTION_CLONE_REQUIRES_ONE_CONTRIBUTOR");
}

static void DevelopmentTimelineEnforcesGestationOrderingRequirements()
{
    var plan = new DevelopmentPlan(
        [
            new DevelopmentStageDefinition(Id("stage.birth"), 2),
            new DevelopmentStageDefinition(Id("stage.gestation"), 1, requiresGestation: true),
            new DevelopmentStageDefinition(Id("stage.adult"), 3),
        ]);
    var missingGestation = DevelopmentService.CreateTimeline(plan);
    var withGestation = DevelopmentService.CreateTimeline(plan, new GestationContext("container.mother", elapsedSteps: 4));

    AssertTrue(!missingGestation.IsValid, "Timeline without required gestation context must be invalid.");
    AssertTrue(missingGestation.Diagnostics.Contains("DEVELOPMENT_GESTATION_REQUIRED"), "Missing gestation diagnostic expected.");
    AssertTrue(withGestation.IsValid, "Timeline with gestation context must be valid.");
    AssertEqual(Id("stage.gestation"), withGestation.Stages[0].Id);
    AssertEqual("container.mother", withGestation.GestationContext?.ContainerId);
}

static void MaternalEffectsUpdateNumericGenomeStateWithoutMutatingSource()
{
    var source = CreateMutationGenomeVersion();
    var affected = DevelopmentService.ApplyMaternalEffects(
        source.State,
        [new MaternalEffect(Id("group.common"), Id("gene.skin"), 0, 0.5)]);
    var affectedValue = affected.Groups
        .Single(group => group.GroupId == Id("group.common"))
        .GeneAlleles
        .Single(set => set.GeneId == Id("gene.skin"))
        .Entries[0]
        .NumericValue;

    AssertEqual(0.75, affectedValue);
    AssertEqual(0.25, source.State.Groups[0].GeneAlleles[0].Entries[0].NumericValue);
}

static void AdvancedNumericExpressionStrategiesAreDeterministic()
{
    var context = new ExpressionEvaluationContext(
        Id("body.human"),
        DevelopmentalPhaseId.Parse("phase.adult"),
        null,
        new ExpressionExternalContext());
    var alleleSet = new RankedAlleleSet(
        Id("gene.size"),
        [
            new RankedAlleleEntry(Id("allele.size.small"), 0, 2),
            new RankedAlleleEntry(Id("allele.size.large"), 1, 8),
        ]);
    var sum = GeneExpressionEvaluator.Evaluate(
        context,
        new GeneDefinition(
            Id("gene.size"),
            [Id("allele.size.small"), Id("allele.size.large")],
            requiredAlleleCount: 2,
            expressionStrategy: GeneExpressionStrategy.NumericSum),
        alleleSet);
    var weighted = GeneExpressionEvaluator.Evaluate(
        context,
        new GeneDefinition(
            Id("gene.size"),
            [Id("allele.size.small"), Id("allele.size.large")],
            requiredAlleleCount: 2,
            expressionStrategy: GeneExpressionStrategy.NumericWeightedAverage),
        alleleSet);

    AssertEqual(10.0, sum.NumericValue);
    AssertEqual(4.0, weighted.NumericValue);
}

static void GeneratedComplementsAddMissingGroupsAndValidateDefinitions()
{
    var definition = CreateVariantDefinition().Freeze();
    var initial = new GenomeState([CreateExpressionCommonGroupState()]);
    var policy = new GeneratedComplementPolicy(
        Id("group.wings"),
        [
            new RankedAlleleSet(
                Id("gene.wing"),
                [
                    new RankedAlleleEntry(Id("allele.wing.feathered"), 0),
                ]),
        ]);
    var result = GeneratedComplementService.ApplyMissingComplement(definition, initial, policy);

    AssertTrue(result.WasGenerated, "Missing complement group should be generated.");
    AssertTrue(result.State.Groups.Any(group => group.GroupId == Id("group.wings")), "Generated group must be present.");

    var second = GeneratedComplementService.ApplyMissingComplement(definition, result.State, policy);

    AssertTrue(!second.WasGenerated, "Existing complement group should not be regenerated.");

    var invalid = GeneratedComplementService.ApplyMissingComplement(
        definition,
        initial,
        new GeneratedComplementPolicy(
            Id("group.wings"),
            [
                new RankedAlleleSet(
                    Id("gene.wing"),
                    [
                        new RankedAlleleEntry(Id("allele.skin.light"), 0),
                    ]),
            ]));

    AssertTrue(!invalid.WasGenerated, "Invalid generated allele should be rejected.");
    AssertContainsExpressionDiagnostic(invalid.Diagnostics, "GENERATED_COMPLEMENT_UNKNOWN_ALLELE");
}

static void RuntimeBodyPlanVariantsEvaluateRequiredGeneratedGroups()
{
    var definition = CreateVariantDefinition().Freeze();
    var variant = new RuntimeBodyPlanVariant(
        BodyPlanVariantId.Parse("variant.human.winged"),
        TestVersion(),
        Id("body.human"),
        requiredGroupIds: [Id("group.wings")],
        changeSummary: "temporary wings");
    var expressionState = new BodyPlanExpressionState([Id("body.human")]);
    var withoutComplement = RuntimeBodyPlanVariantService.EvaluateAvailability(
        definition,
        new GenomeState([CreateExpressionCommonGroupState()]),
        expressionState,
        variant);

    AssertEqual(BodyPlanAvailabilityStatus.Incomplete, withoutComplement.Status);

    var generated = GeneratedComplementService.ApplyMissingComplement(
        definition,
        new GenomeState([CreateExpressionCommonGroupState()]),
        new GeneratedComplementPolicy(
            Id("group.wings"),
            [
                new RankedAlleleSet(
                    Id("gene.wing"),
                    [
                        new RankedAlleleEntry(Id("allele.wing.feathered"), 0),
                    ]),
            ]));
    var available = RuntimeBodyPlanVariantService.EvaluateAvailability(
        definition,
        generated.State,
        expressionState,
        variant);

    AssertEqual(BodyPlanAvailabilityStatus.Active, available.Status);
}

static void RuntimeBodyPlanVariantSerializationPreservesVersionedState()
{
    var variant = new RuntimeBodyPlanVariant(
        BodyPlanVariantId.Parse("variant.human.winged"),
        TestVersion(),
        Id("body.human"),
        requiredGroupIds: [Id("group.wings")],
        optionalGroupIds: [Id("group.fur")],
        sharedGroupIds: [Id("group.common")],
        "temporary wings");
    var text = RuntimeBodyPlanVariantJsonCodec.WriteToText(variant);
    var roundTrip = RuntimeBodyPlanVariantJsonCodec.ReadFromBuffer(
        RuntimeBodyPlanVariantJsonCodec.WriteToBuffer(variant),
        TestVersion());

    AssertEqual(text, RuntimeBodyPlanVariantJsonCodec.WriteToText(roundTrip));
    AssertEqual(variant, roundTrip);
    AssertThrows<GenomeSerializationException>(
        () => RuntimeBodyPlanVariantJsonCodec.ReadFromBuffer(
            RuntimeBodyPlanVariantJsonCodec.WriteToBuffer(variant),
            SystemDefinitionVersion.Parse("other.1")));
}

static void MosaicRegionalExpressionUsesAssignedGenomeVersion()
{
    var definition = CreateExpressionDefinition().Freeze();
    var primary = CreateGenomeVersion("genome.primary", null, "allele.skin.baseline");
    var regional = CreateMosaicGenomeVersion("genome.regional", "allele.skin.dark");
    var mosaic = new MosaicGenomeState(
        primary,
        [new MosaicRegionAssignment(MosaicRegionId.Parse("region.left-arm"), regional)]);
    var result = MosaicExpressionService.EvaluateGeneInRegion(
        mosaic,
        definition,
        MosaicRegionId.Parse("region.left-arm"),
        Id("group.common"),
        Id("gene.skin"),
        Id("body.human"),
        DevelopmentalPhaseId.Parse("phase.adult"),
        new ExpressionExternalContext());

    AssertEqual(GenomeVersionId.Parse("genome.regional"), result.GenomeVersion.Id);
    AssertEqual(Id("allele.skin.dark"), result.Expression.ExpressedAlleleIds[0]);

    var fallback = MosaicExpressionService.EvaluateGeneInRegion(
        mosaic,
        definition,
        MosaicRegionId.Parse("region.unassigned"),
        Id("group.common"),
        Id("gene.skin"),
        Id("body.human"),
        DevelopmentalPhaseId.Parse("phase.adult"),
        new ExpressionExternalContext());

    AssertEqual(GenomeVersionId.Parse("genome.primary"), fallback.GenomeVersion.Id);
}

static void MosaicInheritanceSitesResolveRegionalGenomeSources()
{
    var primary = CreateMosaicGenomeVersion("genome.primary", "allele.skin.light");
    var germline = CreateMosaicGenomeVersion("genome.germline", "allele.skin.dark");
    var mosaic = new MosaicGenomeState(
        primary,
        [new MosaicRegionAssignment(MosaicRegionId.Parse("region.germline"), germline)]);
    var resolved = MosaicInheritanceService.ResolveGenomeForInheritanceSite(
        mosaic,
        InheritanceSiteId.Parse("site.gamete"),
        [new InheritanceSiteAssignment(InheritanceSiteId.Parse("site.gamete"), MosaicRegionId.Parse("region.germline"))]);
    var fallback = MosaicInheritanceService.ResolveGenomeForInheritanceSite(
        mosaic,
        InheritanceSiteId.Parse("site.unknown"),
        []);

    AssertEqual(GenomeVersionId.Parse("genome.germline"), resolved.Id);
    AssertEqual(GenomeVersionId.Parse("genome.primary"), fallback.Id);
}

static void ChimericMaterialRemainsDistinctFromIntegratedVariants()
{
    var primary = CreateMosaicGenomeVersion("genome.primary", "allele.skin.light");
    var chimera = CreateMosaicGenomeVersion("genome.chimera", "allele.skin.dark");
    var material = new ChimericMaterialState(
        Id("chimera.absorbed-twin"),
        chimera,
        [MosaicRegionId.Parse("region.torso")],
        isIntegratedBodyPlanVariant: false);
    var variant = new RuntimeBodyPlanVariant(
        BodyPlanVariantId.Parse("variant.integrated"),
        TestVersion(),
        Id("body.human"),
        requiredGroupIds: [Id("group.common")]);
    var mosaic = new MosaicGenomeState(primary, chimericMaterials: [material]);

    AssertEqual(1, mosaic.ChimericMaterials.Count);
    AssertTrue(!mosaic.ChimericMaterials[0].IsIntegratedBodyPlanVariant, "Chimeric material must remain distinct from integrated body-plan variants.");
    AssertEqual(Id("chimera.absorbed-twin"), mosaic.ChimericMaterials[0].Id);
    AssertEqual(Id("body.human"), variant.BaseBodyPlanId);
}

static SystemDefinitionBuilder CreateMinimalHumanBuilder()
{
    var builder = new SystemDefinitionBuilder(TestVersion());
    builder.AddAllele(new AlleleDefinition(Id("allele.skin.baseline"), "Baseline skin"));
    builder.AddGene(new GeneDefinition(Id("gene.skin"), [Id("allele.skin.baseline")], displayName: "Skin"));
    builder.AddGroup(new GroupDefinition(Id("group.common"), geneIds: [Id("gene.skin")], displayName: "Common"));
    builder.AddBodyPlan(new BodyPlanDefinition(Id("body.human"), requiredGroupIds: [Id("group.common")], displayName: "Human"));
    return builder;
}

static SystemDefinitionBuilder CreateNoisyInvalidBuilder()
{
    var builder = new SystemDefinitionBuilder(TestVersion());
    builder.AddBodyPlan(new BodyPlanDefinition(Id("body.z"), requiredGroupIds: [Id("missing.z")]));
    builder.AddAllele(new AlleleDefinition(Id("duplicate")));
    builder.AddGroup(new GroupDefinition(Id("group.b"), dependencyGroupIds: [Id("group.a")]));
    builder.AddAllele(new AlleleDefinition(Id("duplicate")));
    builder.AddGroup(new GroupDefinition(Id("group.a"), dependencyGroupIds: [Id("group.b")], geneIds: [Id("missing.gene")]));
    builder.AddGene(new GeneDefinition(Id("gene.skin"), [Id("missing.allele")]));
    return builder;
}

static GenomeVersion CreateGenomeVersion(
    string id,
    string? parentId,
    string alleleId,
    double? numericValue = null,
    string systemVersion = "test.1")
{
    return new GenomeVersion(
        GenomeVersionId.Parse(id),
        SystemDefinitionVersion.Parse(systemVersion),
        ExternalIndividualId.Parse("external:person-1"),
        new GenomeState([CreateCommonGroupState(alleleId, numericValue)]),
        parentId is null ? null : GenomeVersionId.Parse(parentId),
        "test change");
}

static GenomeGroupState CreateCommonGroupState(string alleleId, double? numericValue = null)
{
    return new GenomeGroupState(
        Id("group.common"),
        [
            new RankedAlleleSet(
                Id("gene.skin"),
                [
                    new RankedAlleleEntry(Id(alleleId), 0, numericValue),
                ]),
        ]);
}

static SystemDefinitionBuilder CreateExpressionDefinition()
{
    var builder = new SystemDefinitionBuilder(TestVersion());
    builder.AddAllele(new AlleleDefinition(Id("allele.skin.light")));
    builder.AddAllele(new AlleleDefinition(Id("allele.skin.dark")));
    builder.AddAllele(new AlleleDefinition(Id("allele.fur.short")));
    builder.AddGene(new GeneDefinition(
        Id("gene.skin"),
        [Id("allele.skin.light"), Id("allele.skin.dark")],
        requiredAlleleCount: 2,
        expressionStrategy: GeneExpressionStrategy.StrictDominance));
    builder.AddGene(new GeneDefinition(
        Id("gene.fur"),
        [Id("allele.fur.short")],
        requiredAlleleCount: 1,
        expressionStrategy: GeneExpressionStrategy.Codominance));
    builder.AddGroup(new GroupDefinition(Id("group.common"), geneIds: [Id("gene.skin")]));
    builder.AddGroup(new GroupDefinition(Id("group.fur"), geneIds: [Id("gene.fur")]));
    builder.AddGroup(new GroupDefinition(Id("group.dependent"), geneIds: [Id("gene.fur")], dependencyGroupIds: [Id("group.fur")]));
    builder.AddBodyPlan(new BodyPlanDefinition(
        Id("body.human"),
        requiredGroupIds: [Id("group.common")],
        optionalGroupIds: [Id("group.fur")]));
    builder.AddBodyPlan(new BodyPlanDefinition(
        Id("body.wolf"),
        requiredGroupIds: [Id("group.fur")],
        sharedGroupIds: [Id("group.common")]));
    return builder;
}

static GenomeState CreateExpressionGenomeState()
{
    return new GenomeState(
        [
            CreateExpressionCommonGroupState(),
            new GenomeGroupState(
                Id("group.fur"),
                [
                    new RankedAlleleSet(
                        Id("gene.fur"),
                        [
                            new RankedAlleleEntry(Id("allele.fur.short"), 0),
                        ]),
                ]),
            new GenomeGroupState(
                Id("group.dependent"),
                [
                    new RankedAlleleSet(
                        Id("gene.fur"),
                        [
                            new RankedAlleleEntry(Id("allele.fur.short"), 0),
                        ]),
                ]),
        ]);
}

static GenomeGroupState CreateExpressionCommonGroupState()
{
    return new GenomeGroupState(
        Id("group.common"),
        [
            new RankedAlleleSet(
                Id("gene.skin"),
                [
                    new RankedAlleleEntry(Id("allele.skin.light"), 0, 0.1),
                    new RankedAlleleEntry(Id("allele.skin.dark"), 1, 0.9),
                ]),
        ]);
}

static SystemDefinitionBuilder CreateReproductionDefinition(int requiredSkinAlleleCount, bool includeFur = false)
{
    var builder = new SystemDefinitionBuilder(TestVersion());
    builder.AddAllele(new AlleleDefinition(Id("allele.skin.light")));
    builder.AddAllele(new AlleleDefinition(Id("allele.skin.dark")));
    builder.AddAllele(new AlleleDefinition(Id("allele.skin.gold")));
    builder.AddGene(new GeneDefinition(
        Id("gene.skin"),
        [Id("allele.skin.light"), Id("allele.skin.dark"), Id("allele.skin.gold")],
        requiredAlleleCount: requiredSkinAlleleCount));
    builder.AddGroup(new GroupDefinition(Id("group.common"), geneIds: [Id("gene.skin")]));

    if (includeFur)
    {
        builder.AddAllele(new AlleleDefinition(Id("allele.fur.short")));
        builder.AddAllele(new AlleleDefinition(Id("allele.fur.long")));
        builder.AddGene(new GeneDefinition(
            Id("gene.fur"),
            [Id("allele.fur.short"), Id("allele.fur.long")],
            requiredAlleleCount: requiredSkinAlleleCount));
        builder.AddGroup(new GroupDefinition(Id("group.fur"), geneIds: [Id("gene.fur")]));
    }

    builder.AddBodyPlan(new BodyPlanDefinition(
        Id("body.reproduction-test"),
        requiredGroupIds: includeFur ? [Id("group.common"), Id("group.fur")] : [Id("group.common")]));

    return builder;
}

static GenomeVersion CreateParentGenome(
    string versionId,
    string individualId,
    string firstSkinAllele,
    string? secondSkinAllele = null,
    bool includeFur = false)
{
    var groups = new List<GenomeGroupState>
    {
        new(
            Id("group.common"),
            [
                new RankedAlleleSet(
                    Id("gene.skin"),
                    (secondSkinAllele is null
                        ? [new RankedAlleleEntry(Id(firstSkinAllele), 0)]
                        : new[]
                        {
                            new RankedAlleleEntry(Id(firstSkinAllele), 0),
                            new RankedAlleleEntry(Id(secondSkinAllele), 1),
                        })),
            ]),
    };

    if (includeFur)
    {
        groups.Add(new GenomeGroupState(
            Id("group.fur"),
            [
                new RankedAlleleSet(
                    Id("gene.fur"),
                    (secondSkinAllele is null
                        ? [new RankedAlleleEntry(Id("allele.fur.short"), 0)]
                        : new[]
                        {
                            new RankedAlleleEntry(Id("allele.fur.short"), 0),
                            new RankedAlleleEntry(Id("allele.fur.long"), 1),
                        })),
            ]));
    }

    return new GenomeVersion(
        GenomeVersionId.Parse(versionId),
        TestVersion(),
        ExternalIndividualId.Parse(individualId),
        new GenomeState(groups));
}

static GenomeVersion CreateHeritableParent(
    string versionId,
    string individualId,
    HeritableObjectState heritableObjectState,
    string skinAllele = "allele.skin.light")
{
    return new GenomeVersion(
        GenomeVersionId.Parse(versionId),
        TestVersion(),
        ExternalIndividualId.Parse(individualId),
        new GenomeState(
            [
                new GenomeGroupState(
                    Id("group.common"),
                    [
                        new RankedAlleleSet(
                            Id("gene.skin"),
                            [
                                new RankedAlleleEntry(Id(skinAllele), 0),
                            ]),
                    ]),
            ]),
        heritableObjects: heritableObjectState);
}

static GenomeVersion CreateMosaicGenomeVersion(string versionId, string skinAllele)
{
    return new GenomeVersion(
        GenomeVersionId.Parse(versionId),
        TestVersion(),
        ExternalIndividualId.Parse($"external:{versionId}"),
        new GenomeState(
            [
                new GenomeGroupState(
                    Id("group.common"),
                    [
                        new RankedAlleleSet(
                            Id("gene.skin"),
                            [
                                new RankedAlleleEntry(Id(skinAllele), 0),
                            ]),
                    ]),
            ]));
}

static ReproductionResult Reproduce(
    FrozenSystemDefinition definition,
    ulong seed,
    IEnumerable<ReproductionParentRole> parentRoles,
    ReproductionPolicy? policy = null)
{
    return OrdinaryReproductionService.Reproduce(new ReproductionRequest(
        definition,
        parentRoles,
        policy ?? new ReproductionPolicy(),
        seed,
        GenomeVersionId.Parse("offspring.v1"),
        ExternalIndividualId.Parse("external:offspring")));
}

static SystemDefinitionBuilder CreateMutationDefinition()
{
    var builder = new SystemDefinitionBuilder(TestVersion());
    builder.AddAllele(new AlleleDefinition(Id("allele.skin.light")));
    builder.AddAllele(new AlleleDefinition(Id("allele.skin.dark")));
    builder.AddAllele(new AlleleDefinition(Id("allele.fur.short")));
    builder.AddGene(new GeneDefinition(
        Id("gene.skin"),
        [Id("allele.skin.light"), Id("allele.skin.dark")],
        requiredAlleleCount: 1));
    builder.AddGene(new GeneDefinition(
        Id("gene.fur"),
        [Id("allele.fur.short")],
        requiredAlleleCount: 1));
    builder.AddGroup(new GroupDefinition(Id("group.common"), geneIds: [Id("gene.skin")]));
    builder.AddGroup(new GroupDefinition(Id("group.fur"), geneIds: [Id("gene.fur")]));
    builder.AddBodyPlan(new BodyPlanDefinition(Id("body.mutation-test"), requiredGroupIds: [Id("group.common")], optionalGroupIds: [Id("group.fur")]));
    return builder;
}

static SystemDefinitionBuilder CreateVariantDefinition()
{
    var builder = CreateExpressionDefinition();
    builder.AddAllele(new AlleleDefinition(Id("allele.wing.feathered")));
    builder.AddGene(new GeneDefinition(
        Id("gene.wing"),
        [Id("allele.wing.feathered")],
        requiredAlleleCount: 1));
    builder.AddGroup(new GroupDefinition(Id("group.wings"), geneIds: [Id("gene.wing")]));
    return builder;
}

static GenomeVersion CreateMutationGenomeVersion()
{
    return new GenomeVersion(
        GenomeVersionId.Parse("genome.mutation.v1"),
        TestVersion(),
        ExternalIndividualId.Parse("external:mutation-target"),
        new GenomeState(
            [
                new GenomeGroupState(
                    Id("group.common"),
                    [
                        new RankedAlleleSet(
                            Id("gene.skin"),
                            [
                                new RankedAlleleEntry(Id("allele.skin.light"), 0, 0.25),
                            ]),
                    ]),
            ]));
}

static IReadOnlyList<RankedAlleleEntry> CurrentAlleles(
    CurrentGenomeCopy current,
    ResourceId groupId,
    ResourceId geneId)
{
    return current.CurrentState.Groups
        .Single(group => group.GroupId == groupId)
        .GeneAlleles
        .Single(set => set.GeneId == geneId)
        .Entries;
}

static IReadOnlyList<RankedAlleleEntry> OffspringAlleles(
    ReproductionResult result,
    ResourceId groupId,
    ResourceId geneId)
{
    if (result.OffspringVersion is null)
    {
        throw new InvalidOperationException("Expected offspring version.");
    }

    return result.OffspringVersion.State.Groups
        .Single(group => group.GroupId == groupId)
        .GeneAlleles
        .Single(set => set.GeneId == geneId)
        .Entries;
}

static void AssertGenomeVersionsEqual(GenomeVersion expected, GenomeVersion actual)
{
    AssertEqual(expected.Id, actual.Id);
    AssertEqual(expected.SystemDefinitionVersion, actual.SystemDefinitionVersion);
    AssertEqual(expected.IndividualId, actual.IndividualId);
    AssertEqual(expected.ParentVersionId, actual.ParentVersionId);
    AssertEqual(expected.ChangeSummary, actual.ChangeSummary);
    AssertEqual(expected.State.Groups.Count, actual.State.Groups.Count);

    for (var groupIndex = 0; groupIndex < expected.State.Groups.Count; groupIndex++)
    {
        var expectedGroup = expected.State.Groups[groupIndex];
        var actualGroup = actual.State.Groups[groupIndex];

        AssertEqual(expectedGroup.GroupId, actualGroup.GroupId);
        AssertEqual(expectedGroup.GeneAlleles.Count, actualGroup.GeneAlleles.Count);

        for (var geneIndex = 0; geneIndex < expectedGroup.GeneAlleles.Count; geneIndex++)
        {
            var expectedSet = expectedGroup.GeneAlleles[geneIndex];
            var actualSet = actualGroup.GeneAlleles[geneIndex];

            AssertEqual(expectedSet.GeneId, actualSet.GeneId);
            AssertEqual(expectedSet.Entries.Count, actualSet.Entries.Count);

            for (var entryIndex = 0; entryIndex < expectedSet.Entries.Count; entryIndex++)
            {
                var expectedEntry = expectedSet.Entries[entryIndex];
                var actualEntry = actualSet.Entries[entryIndex];

                AssertEqual(expectedEntry.AlleleId, actualEntry.AlleleId);
                AssertEqual(expectedEntry.Rank, actualEntry.Rank);
                AssertEqual(expectedEntry.NumericValue, actualEntry.NumericValue);
            }
        }
    }
}

static SystemDefinitionVersion TestVersion()
{
    return SystemDefinitionVersion.Parse("test.1");
}

static ResourceId Id(string value)
{
    return ResourceId.Parse(value);
}

static void AssertContainsDiagnostic(ValidationResult result, string code)
{
    if (!result.Diagnostics.Any(diagnostic => diagnostic.Code == code))
    {
        throw new InvalidOperationException(
            $"Expected diagnostic '{code}'. Actual diagnostics: {DiagnosticsToString(result.Diagnostics)}");
    }
}

static void AssertContainsExpressionDiagnostic(IEnumerable<ExpressionDiagnostic> diagnostics, string code)
{
    if (!diagnostics.Any(diagnostic => diagnostic.Code == code))
    {
        throw new InvalidOperationException(
            $"Expected expression diagnostic '{code}'. Actual diagnostics: {string.Join(", ", diagnostics.Select(diagnostic => $"{diagnostic.Code}:{diagnostic.Path}"))}");
    }
}

static void AssertContainsReproductionDiagnostic(IEnumerable<ReproductionDiagnostic> diagnostics, string code)
{
    if (!diagnostics.Any(diagnostic => diagnostic.Code == code))
    {
        throw new InvalidOperationException(
            $"Expected reproduction diagnostic '{code}'. Actual diagnostics: {string.Join(", ", diagnostics.Select(diagnostic => $"{diagnostic.Code}:{diagnostic.Path}"))}");
    }
}

static void AssertContainsMutationDiagnostic(IEnumerable<GenomeMutationDiagnostic> diagnostics, string code)
{
    if (!diagnostics.Any(diagnostic => diagnostic.Code == code))
    {
        throw new InvalidOperationException(
            $"Expected mutation diagnostic '{code}'. Actual diagnostics: {string.Join(", ", diagnostics.Select(diagnostic => $"{diagnostic.Code}:{diagnostic.Path}"))}");
    }
}

static string DiagnosticsToString(IEnumerable<ValidationDiagnostic> diagnostics)
{
    return string.Join(", ", diagnostics.Select(diagnostic => $"{diagnostic.Code}:{diagnostic.Path}"));
}

static TException AssertThrows<TException>(Action action)
    where TException : Exception
{
    try
    {
        action();
    }
    catch (TException exception)
    {
        return exception;
    }

    throw new InvalidOperationException($"Expected exception of type {typeof(TException).Name}.");
}

static void AssertTrue(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}

static void AssertEqual<T>(T expected, T actual)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
    {
        throw new InvalidOperationException($"Expected '{expected}', got '{actual}'.");
    }
}
