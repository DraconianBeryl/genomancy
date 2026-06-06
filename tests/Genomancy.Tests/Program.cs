using System.Reflection;
using Genomancy.Core;
using Genomancy.Core.Definitions;
using Genomancy.Core.Genome;
using Genomancy.Core.Runtime;
using Genomancy.Core.Serialization;

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
