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
using Genomancy.Core.ResourceTesting;
using Genomancy.Core.Reproduction;
using Genomancy.Core.Runtime;
using Genomancy.Core.Serialization;
using Genomancy.Core.Simulation;
using Genomancy.Core.Templates;
using Genomancy.Core.Variants;
using Genomancy.Godot;
using Genomancy.Storage.JsonFile;

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
    ("Runtime body plan variant binary codec validates envelope", RuntimeBodyPlanVariantBinaryCodecValidatesEnvelope),
    ("Mosaic regional expression uses assigned genome version", MosaicRegionalExpressionUsesAssignedGenomeVersion),
    ("Mosaic inheritance sites resolve regional genome sources", MosaicInheritanceSitesResolveRegionalGenomeSources),
    ("Chimeric material remains distinct from integrated variants", ChimericMaterialRemainsDistinctFromIntegratedVariants),
    ("Population template sampling is deterministic and serializable", PopulationTemplateSamplingIsDeterministicAndSerializable),
    ("Population template blending combines allele weights", PopulationTemplateBlendingCombinesAlleleWeights),
    ("Population generation produces stable versioned genomes", PopulationGenerationProducesStableVersionedGenomes),
    ("Population template can be created from individual genome", PopulationTemplateCanBeCreatedFromIndividualGenome),
    ("Population template JSON round trip preserves immutable profile", PopulationTemplateJsonRoundTripPreservesImmutableProfile),
    ("Population template simulation is deterministic and bounded", PopulationTemplateSimulationIsDeterministicAndBounded),
    ("Nested template groups preserve generated population structure", NestedTemplateGroupsPreserveGeneratedPopulationStructure),
    ("Template groups apply deterministic cross template blending", TemplateGroupsApplyDeterministicCrossTemplateBlending),
    ("Template group versions validate compatibility and isolate aliases", TemplateGroupVersionsValidateCompatibilityAndIsolateAliases),
    ("Population template group codecs round trip nested groups", PopulationTemplateGroupCodecsRoundTripNestedGroups),
    ("Resource tests run validation fixtures and assertions", ResourceTestsRunValidationFixturesAndAssertions),
    ("Resource tests report deterministic failures", ResourceTestsReportDeterministicFailures),
    ("Resource tests support custom isolated steps", ResourceTestsSupportCustomIsolatedSteps),
    ("Resource statistical failures produce reproducibility packets", ResourceStatisticalFailuresProduceReproducibilityPackets),
    ("Resource test JSON serializes statistical assertions", ResourceTestJsonSerializesStatisticalAssertions),
    ("Resource test JSON round trip materializes executable definitions", ResourceTestJsonRoundTripMaterializesExecutableDefinitions),
    ("Resource test JSON rejects unsupported or malformed specs", ResourceTestJsonRejectsUnsupportedOrMalformedSpecs),
    ("Resource test runner filters by tags deterministically", ResourceTestRunnerFiltersByTagsDeterministically),
    ("Population template binary codec round trips and validates headers", PopulationTemplateBinaryCodecRoundTripsAndValidatesHeaders),
    ("Resource test binary codec round trips executable specs", ResourceTestBinaryCodecRoundTripsExecutableSpecs),
    ("Resource test result codecs preserve diagnostics and failure packets", ResourceTestResultCodecsPreserveDiagnosticsAndFailurePackets),
    ("JSON file storage persists resource test specs outside core", JsonFileStoragePersistsResourceTestSpecsOutsideCore),
    ("Godot adapter round trips genome and template documents", GodotAdapterRoundTripsGenomeAndTemplateDocuments),
    ("Godot adapter reports import diagnostics", GodotAdapterReportsImportDiagnostics),
    ("Godot adapter round trips template group and result documents", GodotAdapterRoundTripsTemplateGroupAndResultDocuments),
    ("Godot adapter bridges resource tests and runtime startup", GodotAdapterBridgesResourceTestsAndRuntimeStartup),
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

static void RuntimeBodyPlanVariantBinaryCodecValidatesEnvelope()
{
    var variant = new RuntimeBodyPlanVariant(
        BodyPlanVariantId.Parse("variant.binary"),
        TestVersion(),
        Id("body.human"),
        requiredGroupIds: [Id("group.common"), Id("group.wings")],
        optionalGroupIds: [Id("group.tail")],
        sharedGroupIds: [Id("group.shared")],
        changeSummary: "binary variant");
    var buffer = RuntimeBodyPlanVariantBinaryCodec.WriteToBuffer(variant);
    var roundTrip = RuntimeBodyPlanVariantBinaryCodec.ReadFromBuffer(buffer, TestVersion());

    AssertEqual(variant, roundTrip);
    AssertEqual(RuntimeBodyPlanVariantJsonCodec.WriteToText(variant), RuntimeBodyPlanVariantJsonCodec.WriteToText(roundTrip));
    AssertThrows<GenomeSerializationException>(() => RuntimeBodyPlanVariantBinaryCodec.ReadFromBuffer(buffer[..4], TestVersion()));
    AssertThrows<GenomeSerializationException>(
        () => RuntimeBodyPlanVariantBinaryCodec.ReadFromBuffer(buffer, SystemDefinitionVersion.Parse("other.1")));
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

static void PopulationTemplateSamplingIsDeterministicAndSerializable()
{
    var template = CreatePopulationTemplate("template.people", "template.people.v1", lightWeight: 9, darkWeight: 1);
    var first = PopulationTemplateService.SampleGenome(
        template,
        123,
        GenomeVersionId.Parse("sample.1"),
        ExternalIndividualId.Parse("external:sample"));
    var second = PopulationTemplateService.SampleGenome(
        template,
        123,
        GenomeVersionId.Parse("sample.1"),
        ExternalIndividualId.Parse("external:sample"));

    AssertEqual(GenomeJsonCodec.WriteVersionToText(first), GenomeJsonCodec.WriteVersionToText(second));
    AssertEqual(TestVersion(), first.SystemDefinitionVersion);
    AssertEqual(2, first.State.Groups.Single().GeneAlleles.Single().Entries.Count);
}

static void PopulationTemplateBlendingCombinesAlleleWeights()
{
    var first = CreatePopulationTemplate("template.first", "template.first.v1", lightWeight: 10, darkWeight: 0);
    var second = CreatePopulationTemplate("template.second", "template.second.v1", lightWeight: 0, darkWeight: 10);
    var blended = PopulationTemplateService.Blend(
        first,
        second,
        0.25,
        PopulationTemplateId.Parse("template.blended"),
        PopulationTemplateVersionId.Parse("template.blended.v1"),
        "blend");
    var alleleWeights = blended.GroupTemplates
        .Single(group => group.GroupId == Id("group.common"))
        .GeneTemplates
        .Single(gene => gene.GeneId == Id("gene.skin"))
        .AlleleFrequencies
        .ToDictionary(frequency => frequency.AlleleId, frequency => frequency.Weight);

    AssertEqual(7.5, alleleWeights[Id("allele.skin.light")]);
    AssertEqual(2.5, alleleWeights[Id("allele.skin.dark")]);
    AssertEqual("blend", blended.ChangeSummary);
}

static void PopulationGenerationProducesStableVersionedGenomes()
{
    var template = CreatePopulationTemplate("template.people", "template.people.v1", lightWeight: 1, darkWeight: 1);
    var generated = PopulationTemplateService.GeneratePopulation(template, 3, 50, "generated.", "external:generated.");

    AssertEqual(3, generated.Count);
    AssertEqual(GenomeVersionId.Parse("generated.0"), generated[0].Id);
    AssertEqual(ExternalIndividualId.Parse("external:generated.2"), generated[2].IndividualId);
    AssertEqual(TestVersion(), generated[1].SystemDefinitionVersion);
}

static void PopulationTemplateCanBeCreatedFromIndividualGenome()
{
    var genome = CreateMosaicGenomeVersion("genome.template-source", "allele.skin.dark");
    var template = PopulationTemplateService.FromGenome(
        genome,
        PopulationTemplateId.Parse("template.from-individual"),
        PopulationTemplateVersionId.Parse("template.from-individual.v1"),
        "from individual");
    var frequency = template.GroupTemplates
        .Single(group => group.GroupId == Id("group.common"))
        .GeneTemplates
        .Single(gene => gene.GeneId == Id("gene.skin"))
        .AlleleFrequencies
        .Single();

    AssertEqual(Id("allele.skin.dark"), frequency.AlleleId);
    AssertEqual(1.0, frequency.Weight);
    AssertEqual("from individual", template.ChangeSummary);
}

static void PopulationTemplateJsonRoundTripPreservesImmutableProfile()
{
    var template = CreatePopulationTemplate("template.people", "template.people.v1", lightWeight: 9, darkWeight: 1);
    var text = PopulationTemplateJsonCodec.WriteToText(template);
    var roundTrip = PopulationTemplateJsonCodec.ReadFromBuffer(
        PopulationTemplateJsonCodec.WriteToBuffer(template),
        TestVersion());

    AssertEqual(template, roundTrip);
    AssertEqual(text, PopulationTemplateJsonCodec.WriteToText(roundTrip));
    AssertThrows<GenomeSerializationException>(
        () => PopulationTemplateJsonCodec.ReadFromBuffer(
            PopulationTemplateJsonCodec.WriteToBuffer(template),
            SystemDefinitionVersion.Parse("other.1")));
}

static void PopulationTemplateSimulationIsDeterministicAndBounded()
{
    var template = CreatePopulationTemplate("template.simulation", "template.simulation.v1", lightWeight: 3, darkWeight: 1);
    var tolerance = new StatisticalTolerance(expectedProportion: 0.75, absoluteTolerance: 0.08);
    var first = PopulationTemplateSimulationService.MeasureAlleleFrequency(
        template,
        Id("group.common"),
        Id("gene.skin"),
        Id("allele.skin.light"),
        sampleCount: 1_000,
        seed: 8675309,
        tolerance);
    var second = PopulationTemplateSimulationService.MeasureAlleleFrequency(
        template,
        Id("group.common"),
        Id("gene.skin"),
        Id("allele.skin.light"),
        sampleCount: 1_000,
        seed: 8675309,
        tolerance);

    AssertTrue(first.IsWithinTolerance, first.FailurePacket?.Diagnostic ?? "Expected simulation to pass.");
    AssertEqual(first, second);
    AssertEqual(2_000, first.ObservationCount);
    AssertThrows<ArgumentOutOfRangeException>(() =>
        PopulationTemplateSimulationService.MeasureAlleleFrequency(
            template,
            Id("group.common"),
            Id("gene.skin"),
            Id("allele.skin.light"),
            sampleCount: 11,
            seed: 1,
            tolerance,
            new SimulationResourceLimits(maximumSamples: 10)));
}

static void NestedTemplateGroupsPreserveGeneratedPopulationStructure()
{
    var nestedTemplate = CreatePopulationTemplate("template.nested", "template.nested.v1", lightWeight: 10, darkWeight: 0);
    var child = new PopulationTemplateGroupVersion(
        PopulationTemplateGroupId.Parse("template-group.child"),
        PopulationTemplateGroupVersionId.Parse("template-group.child.v1"),
        TestVersion(),
        [new WeightedPopulationTemplate(nestedTemplate, 1)]);
    var root = new PopulationTemplateGroupVersion(
        PopulationTemplateGroupId.Parse("template-group.root"),
        PopulationTemplateGroupVersionId.Parse("template-group.root.v1"),
        TestVersion(),
        [new WeightedPopulationTemplate(CreatePopulationTemplate("template.excluded", "template.excluded.v1", 0, 10), 0)],
        [new WeightedPopulationTemplateGroup(child, 1)]);
    var generated = PopulationTemplateGroupService.GeneratePopulation(
        root,
        2,
        70,
        "nested.generated.",
        "external:nested.");

    AssertEqual(2, generated.Count);
    AssertEqual(GenomeVersionId.Parse("nested.generated.0"), generated[0].GenomeVersion.Id);
    AssertEqual(ExternalIndividualId.Parse("external:nested.1"), generated[1].GenomeVersion.IndividualId);
    AssertEqual(PopulationTemplateId.Parse("template.nested"), generated[0].PrimaryTemplateId);
    AssertEqual(2, generated[0].GroupPath.Count);
    AssertEqual(PopulationTemplateGroupId.Parse("template-group.root"), generated[0].GroupPath[0]);
    AssertEqual(PopulationTemplateGroupId.Parse("template-group.child"), generated[0].GroupPath[1]);
}

static void TemplateGroupsApplyDeterministicCrossTemplateBlending()
{
    var first = CreatePopulationTemplate("template.blend-first", "template.blend-first.v1", lightWeight: 10, darkWeight: 0);
    var second = CreatePopulationTemplate("template.blend-second", "template.blend-second.v1", lightWeight: 0, darkWeight: 10);
    var group = new PopulationTemplateGroupVersion(
        PopulationTemplateGroupId.Parse("template-group.blended"),
        PopulationTemplateGroupVersionId.Parse("template-group.blended.v1"),
        TestVersion(),
        [
            new WeightedPopulationTemplate(first, 1),
            new WeightedPopulationTemplate(second, 1),
        ],
        crossTemplateBlendPolicy: new CrossTemplateBlendPolicy(1, 0.25));
    var firstSample = PopulationTemplateGroupService.SampleGenome(
        group,
        1234,
        GenomeVersionId.Parse("template-group.sample"),
        ExternalIndividualId.Parse("external:template-group.sample"));
    var secondSample = PopulationTemplateGroupService.SampleGenome(
        group,
        1234,
        GenomeVersionId.Parse("template-group.sample"),
        ExternalIndividualId.Parse("external:template-group.sample"));

    AssertTrue(firstSample.WasBlended, "A blend rate of one must blend when another positive-weight template is available.");
    AssertTrue(firstSample.PrimaryTemplateId != firstSample.SecondaryTemplateId, "Cross-template blending must select a distinct secondary template.");
    AssertEqual(firstSample, secondSample);
    AssertTrue(
        firstSample.GenomeVersion.ChangeSummary.Contains("templateGroupBlend=template-group.blended", StringComparison.Ordinal),
        "Generated blended genome must retain its template-group blend provenance.");
}

static void TemplateGroupVersionsValidateCompatibilityAndIsolateAliases()
{
    var template = CreatePopulationTemplate("template.alias", "template.alias.v1", lightWeight: 1, darkWeight: 1);
    var entries = new List<WeightedPopulationTemplate>
    {
        new(template, 1),
    };
    var group = new PopulationTemplateGroupVersion(
        PopulationTemplateGroupId.Parse("template-group.alias"),
        PopulationTemplateGroupVersionId.Parse("template-group.alias.v1"),
        TestVersion(),
        entries);

    entries.Clear();

    AssertEqual(1, group.Templates.Count);
    AssertThrows<ArgumentException>(() => new PopulationTemplateGroupVersion(
        PopulationTemplateGroupId.Parse("template-group.incompatible"),
        PopulationTemplateGroupVersionId.Parse("template-group.incompatible.v1"),
        TestVersion(),
        [
            new WeightedPopulationTemplate(
                new PopulationTemplateVersion(
                    PopulationTemplateId.Parse("template.other-version"),
                    PopulationTemplateVersionId.Parse("template.other-version.v1"),
                    SystemDefinitionVersion.Parse("other.1"),
                    template.GroupTemplates),
                1),
        ]));
    AssertThrows<InvalidOperationException>(() => PopulationTemplateGroupService.SampleGenome(
        new PopulationTemplateGroupVersion(
            PopulationTemplateGroupId.Parse("template-group.zero"),
            PopulationTemplateGroupVersionId.Parse("template-group.zero.v1"),
            TestVersion(),
            [new WeightedPopulationTemplate(template, 0)]),
        1,
        GenomeVersionId.Parse("template-group.zero.sample"),
        ExternalIndividualId.Parse("external:template-group.zero.sample")));
}

static void PopulationTemplateGroupCodecsRoundTripNestedGroups()
{
    var light = CreatePopulationTemplate("template.group-codec.light", "template.group-codec.light.v1", lightWeight: 10, darkWeight: 0);
    var dark = CreatePopulationTemplate("template.group-codec.dark", "template.group-codec.dark.v1", lightWeight: 0, darkWeight: 10);
    var child = new PopulationTemplateGroupVersion(
        PopulationTemplateGroupId.Parse("template-group.codec.child"),
        PopulationTemplateGroupVersionId.Parse("template-group.codec.child.v1"),
        TestVersion(),
        [new WeightedPopulationTemplate(dark, 1)],
        changeSummary: "child codec fixture");
    var root = new PopulationTemplateGroupVersion(
        PopulationTemplateGroupId.Parse("template-group.codec.root"),
        PopulationTemplateGroupVersionId.Parse("template-group.codec.root.v1"),
        TestVersion(),
        [new WeightedPopulationTemplate(light, 2)],
        [new WeightedPopulationTemplateGroup(child, 1)],
        new CrossTemplateBlendPolicy(rate: 0.25, secondTemplateWeight: 0.4),
        "root codec fixture");
    var text = PopulationTemplateGroupJsonCodec.WriteToText(root);
    var jsonRoundTrip = PopulationTemplateGroupJsonCodec.ReadFromBuffer(
        PopulationTemplateGroupJsonCodec.WriteToBuffer(root),
        TestVersion());
    var binary = PopulationTemplateGroupBinaryCodec.WriteToBuffer(root);
    var binaryRoundTrip = PopulationTemplateGroupBinaryCodec.ReadFromBuffer(binary, TestVersion());
    var originalSample = PopulationTemplateGroupService.SampleGenome(
        root,
        12345,
        GenomeVersionId.Parse("template-group.codec.sample"),
        ExternalIndividualId.Parse("external:template-group.codec.sample"));
    var roundTripSample = PopulationTemplateGroupService.SampleGenome(
        jsonRoundTrip,
        12345,
        GenomeVersionId.Parse("template-group.codec.sample"),
        ExternalIndividualId.Parse("external:template-group.codec.sample"));

    AssertEqual(root, jsonRoundTrip);
    AssertEqual(root, binaryRoundTrip);
    AssertEqual(text, PopulationTemplateGroupJsonCodec.WriteToText(jsonRoundTrip));
    AssertEqual(text, PopulationTemplateGroupJsonCodec.WriteToText(binaryRoundTrip));
    AssertEqual(originalSample, roundTripSample);
    AssertEqual(0.25, jsonRoundTrip.CrossTemplateBlendPolicy.Rate);
    AssertEqual(0.4, jsonRoundTrip.CrossTemplateBlendPolicy.SecondTemplateWeight);
    AssertThrows<GenomeSerializationException>(
        () => PopulationTemplateGroupJsonCodec.ReadFromBuffer(
            PopulationTemplateGroupJsonCodec.WriteToBuffer(root),
            SystemDefinitionVersion.Parse("other.1")));
    AssertThrows<GenomeSerializationException>(
        () => PopulationTemplateGroupJsonCodec.ReadFromBuffer(
            """{"envelopeVersion":99,"id":"template-group.bad","versionId":"template-group.bad.v1","systemDefinitionVersion":"genomancy.test.1","templates":[]}"""u8,
            TestVersion()));
    AssertThrows<GenomeSerializationException>(() => PopulationTemplateGroupBinaryCodec.ReadFromBuffer(binary[..4], TestVersion()));
}

static void ResourceTestsRunValidationFixturesAndAssertions()
{
    var test = new ResourceTestDefinition(
        ResourceTestId.Parse("resource-test.valid-human"),
        CreateMinimalHumanBuilder,
        [
            new ValidateSystemDefinitionStep(),
            new ExpectValidationResultStep(ExpectedValid: true),
            new ExpectValidationDiagnosticStep("GENE_UNKNOWN_ALLELE", MustBePresent: false),
            new FreezeSystemDefinitionStep(),
        ],
        displayName: "Valid human baseline",
        tags: ["baseline", "validation"]);
    var result = ResourceTestRunner.Run([test]);

    AssertEqual(ResourceTestStatus.Passed, result.Status);
    AssertEqual(ResourceTestStatus.Passed, result.Cases.Single().Status);
    AssertEqual(ResourceTestId.Parse("resource-test.valid-human"), result.Cases.Single().TestId);
    AssertEqual("baseline", result.Cases.Single().Tags[0]);
    AssertEqual("validation", result.Cases.Single().Tags[1]);
}

static void ResourceTestsReportDeterministicFailures()
{
    var expectedInvalid = new ResourceTestDefinition(
        ResourceTestId.Parse("resource-test.invalid-expected"),
        () =>
        {
            var builder = new SystemDefinitionBuilder(TestVersion());
            builder.AddGene(new GeneDefinition(Id("gene.invalid"), [Id("allele.missing")]));
            return builder;
        },
        [
            new ValidateSystemDefinitionStep(),
            new ExpectValidationResultStep(ExpectedValid: false),
            new ExpectValidationDiagnosticStep("GENE_UNKNOWN_ALLELE"),
        ]);
    var unexpectedValid = new ResourceTestDefinition(
        ResourceTestId.Parse("resource-test.valid-expected-invalid"),
        CreateMinimalHumanBuilder,
        [
            new ValidateSystemDefinitionStep(),
            new ExpectValidationResultStep(ExpectedValid: false),
            new ExpectValidationDiagnosticStep("GENE_UNKNOWN_ALLELE"),
        ]);
    var result = ResourceTestRunner.Run([unexpectedValid, expectedInvalid]);
    var failing = result.Cases.Single(test => test.TestId == ResourceTestId.Parse("resource-test.valid-expected-invalid"));

    AssertEqual(ResourceTestStatus.Failed, result.Status);
    AssertEqual(ResourceTestId.Parse("resource-test.invalid-expected"), result.Cases[0].TestId);
    AssertEqual(ResourceTestId.Parse("resource-test.valid-expected-invalid"), result.Cases[1].TestId);
    AssertEqual(2, failing.Diagnostics.Count);
    AssertEqual("RESOURCE_TEST_VALIDATION_DIAGNOSTIC_MISMATCH", failing.Diagnostics[0].Code);
    AssertEqual("RESOURCE_TEST_VALIDATION_RESULT_MISMATCH", failing.Diagnostics[1].Code);
}

static void ResourceTestsSupportCustomIsolatedSteps()
{
    var observedAlleleCounts = new List<int>();
    var test = new ResourceTestDefinition(
        ResourceTestId.Parse("resource-test.custom-step"),
        CreateMinimalHumanBuilder,
        [
            new CountAllelesAndMutateStep(observedAlleleCounts),
        ]);
    var first = ResourceTestRunner.Run([test]);
    var second = ResourceTestRunner.Run([test]);

    AssertEqual(ResourceTestStatus.Passed, first.Status);
    AssertEqual(ResourceTestStatus.Passed, second.Status);
    AssertEqual(2, observedAlleleCounts.Count);
    AssertTrue(observedAlleleCounts.All(count => count == 1), "Each resource-test run must receive an isolated fresh fixture.");
}

static void ResourceStatisticalFailuresProduceReproducibilityPackets()
{
    var template = CreatePopulationTemplate("template.resource-statistical", "template.resource-statistical.v1", 1, 1);
    var test = new ResourceTestDefinition(
        ResourceTestId.Parse("resource-test.statistical-failure"),
        CreateMinimalHumanBuilder,
        [
            new AssertPopulationTemplateFrequencyStep(
                template,
                Id("group.common"),
                Id("gene.skin"),
                Id("allele.skin.light"),
                sampleCount: 100,
                seed: 42,
                new StatisticalTolerance(expectedProportion: 1, absoluteTolerance: 0)),
        ],
        tags: ["simulation", "statistical"]);
    var result = ResourceTestRunner.Run([test]);
    var testCase = result.Cases.Single();
    var packet = testCase.ReproducibilityPackets.Single();
    var roundTrip = ReproducibilityPacketJsonCodec.ReadFromBuffer(
        ReproducibilityPacketJsonCodec.WriteToBuffer(packet));

    AssertEqual(ResourceTestStatus.Failed, result.Status);
    AssertEqual("RESOURCE_TEST_STATISTICAL_TOLERANCE_FAILED", testCase.Diagnostics.Single().Code);
    AssertEqual(test.Id.Value, packet.TestIdentifier);
    AssertEqual(42UL, packet.Seed);
    AssertEqual(packet, roundTrip);
    AssertEqual(
        ReproducibilityPacketJsonCodec.WriteToText(packet),
        ReproducibilityPacketJsonCodec.WriteToText(roundTrip));
}

static void ResourceTestJsonSerializesStatisticalAssertions()
{
    var template = CreatePopulationTemplate("template.serialized-statistical", "template.serialized-statistical.v1", 1, 1);
    var spec = new ResourceTestSpecification(
        ResourceTestId.Parse("resource-test.serialized-statistical"),
        TestVersion(),
        CreateResourceTestFixture(invalidGene: false),
        [
            new ResourceTestStepSpecification(
                ResourceTestStepSpecification.AssertPopulationTemplateFrequencyKind,
                populationTemplateFrequencyAssertion: new PopulationTemplateFrequencyAssertionSpecification(
                    template,
                    Id("group.common"),
                    Id("gene.skin"),
                    Id("allele.skin.light"),
                    sampleCount: 100,
                    seed: 42,
                    new StatisticalTolerance(expectedProportion: 1, absoluteTolerance: 0),
                    new SimulationResourceLimits(maximumSamples: 100))),
        ],
        tags: ["serialized", "statistical"]);
    var text = ResourceTestJsonCodec.WriteToText([spec]);
    var roundTrip = ResourceTestJsonCodec.ReadFromBuffer(ResourceTestJsonCodec.WriteToBuffer([spec]));
    var result = ResourceTestRunner.Run(roundTrip.Select(test => test.ToDefinition()));
    var testCase = result.Cases.Single();
    var packet = testCase.ReproducibilityPackets.Single();

    AssertTrue(
        text.Contains("assertPopulationTemplateFrequency", StringComparison.Ordinal),
        "Serialized resource-test JSON must include the statistical step kind.");
    AssertTrue(
        text.Contains("populationTemplateFrequencyAssertion", StringComparison.Ordinal),
        "Serialized resource-test JSON must include the statistical assertion payload.");
    AssertEqual(text, ResourceTestJsonCodec.WriteToText(roundTrip));
    AssertEqual(ResourceTestStatus.Failed, result.Status);
    AssertEqual("RESOURCE_TEST_STATISTICAL_TOLERANCE_FAILED", testCase.Diagnostics.Single().Code);
    AssertEqual("resource-test.serialized-statistical", packet.TestIdentifier);
    AssertEqual(42UL, packet.Seed);
}

static void ResourceTestJsonRoundTripMaterializesExecutableDefinitions()
{
    var valid = CreateResourceTestSpecification("resource-test.serialized.valid", invalidGene: false, tags: ["baseline", "fast"]);
    var invalid = CreateResourceTestSpecification("resource-test.serialized.invalid", invalidGene: true, tags: ["negative"]);
    var text = ResourceTestJsonCodec.WriteToText([invalid, valid]);
    var roundTrip = ResourceTestJsonCodec.ReadFromBuffer(ResourceTestJsonCodec.WriteToBuffer([invalid, valid]));
    var definitions = roundTrip.Select(specification => specification.ToDefinition()).ToArray();
    var result = ResourceTestRunner.Run(definitions);

    AssertEqual(text, ResourceTestJsonCodec.WriteToText(roundTrip));
    AssertEqual(2, roundTrip.Count);
    AssertEqual(ResourceTestId.Parse("resource-test.serialized.invalid"), roundTrip[0].Id);
    AssertEqual(ResourceTestStatus.Passed, result.Status);
    AssertEqual(ResourceTestStatus.Passed, result.Cases[0].Status);
    AssertEqual(ResourceTestStatus.Passed, result.Cases[1].Status);
}

static void ResourceTestJsonRejectsUnsupportedOrMalformedSpecs()
{
    var unsupported = new ResourceTestSpecification(
        ResourceTestId.Parse("resource-test.serialized.unsupported"),
        TestVersion(),
        CreateResourceTestFixture(invalidGene: false),
        [new ResourceTestStepSpecification("unknownStep")]);
    var malformedJson = System.Text.Encoding.UTF8.GetBytes("""
        {"envelopeVersion":1,"tests":[{"id":"","systemDefinitionVersion":"test.1","fixture":{},"steps":[]}]}
        """);

    AssertThrows<GenomeSerializationException>(() => unsupported.ToDefinition());
    AssertThrows<GenomeSerializationException>(() => ResourceTestJsonCodec.ReadFromBuffer(malformedJson));
    AssertThrows<GenomeSerializationException>(() => ResourceTestJsonCodec.ReadFromBuffer("""{"envelopeVersion":99,"tests":[]}"""u8));
}

static void ResourceTestRunnerFiltersByTagsDeterministically()
{
    var fast = CreateResourceTestSpecification("resource-test.tags.fast", invalidGene: false, tags: ["fast"]).ToDefinition();
    var slow = CreateResourceTestSpecification("resource-test.tags.slow", invalidGene: false, tags: ["slow"]).ToDefinition();
    var excluded = CreateResourceTestSpecification("resource-test.tags.excluded", invalidGene: false, tags: ["fast", "skip"]).ToDefinition();
    var result = ResourceTestRunner.Run(
        [slow, excluded, fast],
        new ResourceTestRunOptions(includeTags: ["fast"], excludeTags: ["skip"]));

    AssertEqual(ResourceTestStatus.Passed, result.Status);
    AssertEqual(1, result.Cases.Count);
    AssertEqual(ResourceTestId.Parse("resource-test.tags.fast"), result.Cases[0].TestId);
}

static void PopulationTemplateBinaryCodecRoundTripsAndValidatesHeaders()
{
    var template = CreatePopulationTemplate("template.binary", "template.binary.v1", lightWeight: 2, darkWeight: 3);
    var buffer = PopulationTemplateBinaryCodec.WriteToBuffer(template);
    var roundTrip = PopulationTemplateBinaryCodec.ReadFromBuffer(buffer, TestVersion());

    AssertEqual(template, roundTrip);
    AssertThrows<GenomeSerializationException>(() => PopulationTemplateBinaryCodec.ReadFromBuffer(buffer[..4], TestVersion()));
    AssertThrows<GenomeSerializationException>(() => PopulationTemplateBinaryCodec.ReadFromBuffer(buffer, SystemDefinitionVersion.Parse("other.1")));
}

static void ResourceTestBinaryCodecRoundTripsExecutableSpecs()
{
    var specs = new[]
    {
        CreateResourceTestSpecification("resource-test.binary.valid", invalidGene: false, tags: ["binary"]),
        CreateResourceTestSpecification("resource-test.binary.invalid", invalidGene: true, tags: ["binary", "negative"]),
    };
    var buffer = ResourceTestBinaryCodec.WriteToBuffer(specs);
    var roundTrip = ResourceTestBinaryCodec.ReadFromBuffer(buffer);
    var result = ResourceTestRunner.Run(roundTrip.Select(specification => specification.ToDefinition()));

    AssertEqual(ResourceTestJsonCodec.WriteToText(specs), ResourceTestJsonCodec.WriteToText(roundTrip));
    AssertEqual(ResourceTestStatus.Passed, result.Status);
    AssertThrows<GenomeSerializationException>(() => ResourceTestBinaryCodec.ReadFromBuffer(buffer[..4]));
}

static void ResourceTestResultCodecsPreserveDiagnosticsAndFailurePackets()
{
    var template = CreatePopulationTemplate("template.result-codec", "template.result-codec.v1", 1, 1);
    var failing = new ResourceTestDefinition(
        ResourceTestId.Parse("resource-test.result-codec.failure"),
        CreateMinimalHumanBuilder,
        [
            new AssertPopulationTemplateFrequencyStep(
                template,
                Id("group.common"),
                Id("gene.skin"),
                Id("allele.skin.light"),
                sampleCount: 100,
                seed: 42,
                new StatisticalTolerance(expectedProportion: 1, absoluteTolerance: 0)),
        ],
        tags: ["result", "statistical"]);
    var passing = CreateResourceTestSpecification(
        "resource-test.result-codec.passing",
        invalidGene: false,
        tags: ["result"]).ToDefinition();
    var result = ResourceTestRunner.Run([passing, failing]);
    var text = ResourceTestResultJsonCodec.WriteToText(result);
    var jsonRoundTrip = ResourceTestResultJsonCodec.ReadFromBuffer(
        ResourceTestResultJsonCodec.WriteToBuffer(result));
    var binary = ResourceTestResultBinaryCodec.WriteToBuffer(result);
    var binaryRoundTrip = ResourceTestResultBinaryCodec.ReadFromBuffer(binary);
    var failure = jsonRoundTrip.Cases.Single(testCase => testCase.Status == ResourceTestStatus.Failed);

    AssertEqual(ResourceTestStatus.Failed, jsonRoundTrip.Status);
    AssertEqual(2, jsonRoundTrip.Cases.Count);
    AssertEqual("RESOURCE_TEST_STATISTICAL_TOLERANCE_FAILED", failure.Diagnostics.Single().Code);
    AssertEqual(42UL, failure.ReproducibilityPackets.Single().Seed);
    AssertEqual(text, ResourceTestResultJsonCodec.WriteToText(jsonRoundTrip));
    AssertEqual(text, ResourceTestResultJsonCodec.WriteToText(binaryRoundTrip));
    AssertThrows<GenomeSerializationException>(
        () => ResourceTestResultJsonCodec.ReadFromBuffer("""{"envelopeVersion":99,"status":"Passed","cases":[]}"""u8));
    AssertThrows<GenomeSerializationException>(
        () => ResourceTestResultJsonCodec.ReadFromBuffer(
            """
            {"envelopeVersion":1,"status":"Passed","cases":[{"testId":"resource-test.mismatch","status":"Failed","tags":[],"diagnostics":[],"reproducibilityPackets":[]}]}
            """u8));
    AssertThrows<GenomeSerializationException>(() => ResourceTestResultBinaryCodec.ReadFromBuffer(binary[..4]));
}

static void JsonFileStoragePersistsResourceTestSpecsOutsideCore()
{
    var path = Path.Combine(
        Path.GetTempPath(),
        "genomancy-json-file-storage-tests",
        $"{Guid.NewGuid():N}",
        "resource-tests.json");
    var store = new JsonFileStore<IReadOnlyList<ResourceTestSpecification>>(
        ResourceTestJsonCodec.Write,
        ResourceTestJsonCodec.Read);
    var specs = new[]
    {
        CreateResourceTestSpecification("resource-test.storage.valid", invalidGene: false, tags: ["storage"]),
    };

    try
    {
        store.Save(path, specs);
        var loaded = store.Load(path);
        var result = ResourceTestRunner.Run(loaded.Select(specification => specification.ToDefinition()));

        AssertEqual(ResourceTestJsonCodec.WriteToText(specs), ResourceTestJsonCodec.WriteToText(loaded));
        AssertEqual(ResourceTestStatus.Passed, result.Status);
    }
    finally
    {
        var root = Path.GetDirectoryName(path);

        if (!string.IsNullOrEmpty(root) && Directory.Exists(root))
        {
            Directory.Delete(root, recursive: true);
        }
    }
}

static void GodotAdapterRoundTripsGenomeAndTemplateDocuments()
{
    var genome = CreateGenomeVersion("genome.godot", null, "allele.skin.baseline");
    var genomeDocument = GodotResourceBridge.ExportGenomeVersion(
        GodotResourcePath.Parse("res://genomancy/genome.godot.json"),
        genome);
    var importedGenome = GodotResourceBridge.ImportGenomeVersion(genomeDocument, TestVersion());
    var template = CreatePopulationTemplate("template.godot", "template.godot.v1", lightWeight: 4, darkWeight: 1);
    var templateDocument = GodotResourceBridge.ExportPopulationTemplate(
        GodotResourcePath.Parse("user://genomancy/template.godot.json"),
        template);
    var importedTemplate = GodotResourceBridge.ImportPopulationTemplate(templateDocument, TestVersion());

    AssertEqual(GodotResourceKind.GenomeVersion, genomeDocument.Kind);
    AssertEqual(TestVersion().Value, genomeDocument.SystemDefinitionVersion);
    AssertTrue(importedGenome.IsSuccess, GodotDiagnosticsToString(importedGenome.Diagnostics));
    AssertGenomeVersionsEqual(genome, importedGenome.Value ?? throw new InvalidOperationException("Genome import failed."));
    AssertEqual(GodotResourceKind.PopulationTemplate, templateDocument.Kind);
    AssertTrue(importedTemplate.IsSuccess, GodotDiagnosticsToString(importedTemplate.Diagnostics));
    AssertEqual(template, importedTemplate.Value);
    AssertThrows<ArgumentException>(() => GodotResourcePath.Parse("genomancy/genome.json"));
}

static void GodotAdapterReportsImportDiagnostics()
{
    var templateDocument = GodotResourceBridge.ExportPopulationTemplate(
        GodotResourcePath.Parse("res://genomancy/template.godot.json"),
        CreatePopulationTemplate("template.godot-diagnostic", "template.godot-diagnostic.v1", lightWeight: 1, darkWeight: 1));
    var mismatchedKind = GodotResourceBridge.ImportGenomeVersion(templateDocument, TestVersion());
    var wrongVersionGenome = GodotResourceBridge.ExportGenomeVersion(
        GodotResourcePath.Parse("res://genomancy/genome.other-version.json"),
        CreateGenomeVersion("genome.other-version", null, "allele.skin.baseline", systemVersion: "other.1"));
    var failedImport = GodotResourceBridge.ImportGenomeVersion(wrongVersionGenome, TestVersion());

    AssertTrue(!mismatchedKind.IsSuccess, "Kind mismatch must return diagnostics.");
    AssertEqual("GODOT_RESOURCE_KIND_MISMATCH", mismatchedKind.Diagnostics.Single().Code);
    AssertTrue(!failedImport.IsSuccess, "Version mismatch must return diagnostics.");
    AssertEqual("GODOT_RESOURCE_IMPORT_FAILED", failedImport.Diagnostics.Single().Code);
}

static void GodotAdapterRoundTripsTemplateGroupAndResultDocuments()
{
    var light = CreatePopulationTemplate("template.godot-group.light", "template.godot-group.light.v1", lightWeight: 10, darkWeight: 0);
    var dark = CreatePopulationTemplate("template.godot-group.dark", "template.godot-group.dark.v1", lightWeight: 0, darkWeight: 10);
    var group = new PopulationTemplateGroupVersion(
        PopulationTemplateGroupId.Parse("template-group.godot"),
        PopulationTemplateGroupVersionId.Parse("template-group.godot.v1"),
        TestVersion(),
        [
            new WeightedPopulationTemplate(light, 1),
            new WeightedPopulationTemplate(dark, 1),
        ],
        crossTemplateBlendPolicy: new CrossTemplateBlendPolicy(rate: 0.5, secondTemplateWeight: 0.25));
    var groupDocument = GodotResourceBridge.ExportPopulationTemplateGroup(
        GodotResourcePath.Parse("res://genomancy/template-group.godot.json"),
        group);
    var importedGroup = GodotResourceBridge.ImportPopulationTemplateGroup(groupDocument, TestVersion());
    var failing = new ResourceTestDefinition(
        ResourceTestId.Parse("resource-test.godot-result.failure"),
        CreateMinimalHumanBuilder,
        [
            new AssertPopulationTemplateFrequencyStep(
                light,
                Id("group.common"),
                Id("gene.skin"),
                Id("allele.skin.light"),
                sampleCount: 20,
                seed: 99,
                new StatisticalTolerance(expectedProportion: 0, absoluteTolerance: 0)),
        ],
        tags: ["godot", "result"]);
    var result = ResourceTestRunner.Run([failing]);
    var resultDocument = GodotResourceBridge.ExportResourceTestResult(
        GodotResourcePath.Parse("user://genomancy/resource-test-result.godot.json"),
        result);
    var importedResult = GodotResourceBridge.ImportResourceTestResult(resultDocument);
    var package = new GodotResourcePackage("genomancy.godot-result-package", [resultDocument, groupDocument]);

    AssertEqual(GodotResourceKind.PopulationTemplateGroup, groupDocument.Kind);
    AssertEqual(TestVersion().Value, groupDocument.SystemDefinitionVersion);
    AssertTrue(importedGroup.IsSuccess, GodotDiagnosticsToString(importedGroup.Diagnostics));
    AssertEqual(group, importedGroup.Value);
    AssertEqual(GodotResourceKind.ResourceTestResult, resultDocument.Kind);
    AssertEqual(TestVersion().Value, resultDocument.SystemDefinitionVersion);
    AssertTrue(resultDocument.Tags.SequenceEqual(["godot", "result"]), "Result document tags must be sorted and preserved.");
    AssertTrue(importedResult.IsSuccess, GodotDiagnosticsToString(importedResult.Diagnostics));
    AssertEqual(
        ResourceTestResultJsonCodec.WriteToText(result),
        ResourceTestResultJsonCodec.WriteToText(importedResult.Value ?? throw new InvalidOperationException("Result import failed.")));
    AssertEqual(groupDocument, package.Get(GodotResourcePath.Parse("res://genomancy/template-group.godot.json")));
    AssertTrue(
        !GodotResourceBridge.ImportPopulationTemplateGroup(resultDocument, TestVersion()).IsSuccess,
        "Importing a result as a template group must report a kind mismatch.");
}

static void GodotAdapterBridgesResourceTestsAndRuntimeStartup()
{
    var specs = new[]
    {
        CreateResourceTestSpecification("resource-test.godot.valid", invalidGene: false, tags: ["godot"]),
    };
    var document = GodotResourceBridge.ExportResourceTests(
        GodotResourcePath.Parse("res://genomancy/resource-tests.json"),
        specs);
    var imported = GodotResourceBridge.ImportResourceTests(document);
    var package = new GodotResourcePackage(
        "genomancy.test-package",
        [
            document,
            GodotResourceBridge.ExportPopulationTemplate(
                GodotResourcePath.Parse("res://genomancy/template.package.json"),
                CreatePopulationTemplate("template.package", "template.package.v1", lightWeight: 1, darkWeight: 1)),
        ]);
    var resourceTestResult = ResourceTestRunner.Run((imported.Value ?? []).Select(specification => specification.ToDefinition()));
    var runtime = GodotRuntimeBridge.StartRuntime(CreateMinimalHumanBuilder());
    var invalidBuilder = new SystemDefinitionBuilder(TestVersion());
    invalidBuilder.AddGene(new GeneDefinition(Id("gene.invalid"), [Id("allele.missing")]));
    var invalidRuntime = GodotRuntimeBridge.StartRuntime(invalidBuilder);

    AssertEqual(GodotResourceKind.ResourceTests, document.Kind);
    AssertEqual("godot", document.Tags.Single());
    AssertEqual(2, package.Resources.Count);
    AssertEqual(document, package.Get(GodotResourcePath.Parse("res://genomancy/resource-tests.json")));
    AssertThrows<ArgumentException>(() => new GodotResourcePackage("duplicate", [document, document]));
    AssertTrue(imported.IsSuccess, GodotDiagnosticsToString(imported.Diagnostics));
    AssertEqual(ResourceTestStatus.Passed, resourceTestResult.Status);
    AssertTrue(runtime.IsSuccess, GodotDiagnosticsToString(runtime.Diagnostics));
    AssertEqual(GenomancyMode.Runtime, runtime.Value?.Mode);
    AssertTrue(!invalidRuntime.IsSuccess, "Invalid runtime startup must return diagnostics.");
    AssertEqual("GENE_UNKNOWN_ALLELE", invalidRuntime.Diagnostics.Single().Code);
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

static PopulationTemplateVersion CreatePopulationTemplate(
    string id,
    string versionId,
    double lightWeight,
    double darkWeight)
{
    return new PopulationTemplateVersion(
        PopulationTemplateId.Parse(id),
        PopulationTemplateVersionId.Parse(versionId),
        TestVersion(),
        [
            new GroupTemplate(
                Id("group.common"),
                [
                    new GeneTemplate(
                        Id("gene.skin"),
                        2,
                        [
                            new AlleleFrequency(Id("allele.skin.light"), lightWeight, 0.25),
                            new AlleleFrequency(Id("allele.skin.dark"), darkWeight, 0.75),
                        ]),
                ]),
        ]);
}

static ResourceTestSpecification CreateResourceTestSpecification(string id, bool invalidGene, IEnumerable<string> tags)
{
    var steps = invalidGene
        ? new[]
        {
            new ResourceTestStepSpecification(ResourceTestStepSpecification.ValidateSystemDefinitionKind),
            new ResourceTestStepSpecification(ResourceTestStepSpecification.ExpectValidationResultKind, expectedValid: false),
            new ResourceTestStepSpecification(ResourceTestStepSpecification.ExpectValidationDiagnosticKind, diagnosticCode: "GENE_UNKNOWN_ALLELE"),
        }
        : new[]
        {
            new ResourceTestStepSpecification(ResourceTestStepSpecification.ValidateSystemDefinitionKind),
            new ResourceTestStepSpecification(ResourceTestStepSpecification.ExpectValidationResultKind, expectedValid: true),
            new ResourceTestStepSpecification(ResourceTestStepSpecification.ExpectValidationDiagnosticKind, diagnosticCode: "GENE_UNKNOWN_ALLELE", mustBePresent: false),
            new ResourceTestStepSpecification(ResourceTestStepSpecification.FreezeSystemDefinitionKind),
        };

    return new ResourceTestSpecification(
        ResourceTestId.Parse(id),
        TestVersion(),
        CreateResourceTestFixture(invalidGene),
        steps,
        displayName: id,
        tags: tags);
}

static ResourceTestFixtureSpecification CreateResourceTestFixture(bool invalidGene)
{
    return new ResourceTestFixtureSpecification(
        alleles: invalidGene
            ? []
            : [new AlleleResourceSpecification(Id("allele.skin.baseline"), "Baseline skin")],
        genes:
        [
            new GeneResourceSpecification(
                Id("gene.skin"),
                [Id(invalidGene ? "allele.skin.missing" : "allele.skin.baseline")],
                "Skin"),
        ],
        groups:
        [
            new GroupResourceSpecification(
                Id("group.common"),
                geneIds: [Id("gene.skin")],
                displayName: "Common"),
        ],
        bodyPlans:
        [
            new BodyPlanResourceSpecification(
                Id("body.human"),
                requiredGroupIds: [Id("group.common")],
                displayName: "Human"),
        ]);
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

static string GodotDiagnosticsToString(IEnumerable<GodotAdapterDiagnostic> diagnostics)
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

sealed class CountAllelesAndMutateStep(List<int> observedAlleleCounts) : IResourceTestStep
{
    public string Name => "count-alleles-and-mutate";

    public void Execute(ResourceTestContext context)
    {
        observedAlleleCounts.Add(context.SystemDefinition.Alleles.Count);
        context.SystemDefinition.AddAllele(new AlleleDefinition(ResourceId.Parse($"allele.custom.{observedAlleleCounts.Count}")));
    }
}
