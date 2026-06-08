using Genomancy.Core.Definitions;
using Genomancy.Core.Runtime;

namespace Genomancy.Godot;

public static class GodotRuntimeBridge
{
    public static GodotAdapterResult<GenomancySystem> StartRuntime(SystemDefinitionBuilder sourceDefinition)
    {
        ArgumentNullException.ThrowIfNull(sourceDefinition);

        try
        {
            return GodotAdapterResult<GenomancySystem>.Success(GenomancySystem.StartRuntime(sourceDefinition));
        }
        catch (SystemDefinitionValidationException exception)
        {
            var diagnostics = exception.ValidationResult.Diagnostics
                .Select(diagnostic => new GodotAdapterDiagnostic(
                    diagnostic.Code,
                    diagnostic.Path,
                    diagnostic.Message));

            return new GodotAdapterResult<GenomancySystem>(null, diagnostics);
        }
    }
}
