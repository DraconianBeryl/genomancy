namespace Genomancy.Core.ResourceTesting;

public interface IResourceTestStep
{
    string Name { get; }

    void Execute(ResourceTestContext context);
}
