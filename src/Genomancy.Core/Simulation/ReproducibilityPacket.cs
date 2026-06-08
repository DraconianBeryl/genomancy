using Genomancy.Core.Definitions;

namespace Genomancy.Core.Simulation;

public sealed record ReproducibilityPacket(
    SystemDefinitionVersion ResourceSetVersion,
    string TestIdentifier,
    ulong Seed,
    string OperationPath,
    string InputState,
    string FailureAssertion,
    double ExpectedValue,
    double ActualValue,
    string Diagnostic);
