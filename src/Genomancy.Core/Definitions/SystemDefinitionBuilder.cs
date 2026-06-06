namespace Genomancy.Core.Definitions;

public sealed class SystemDefinitionBuilder
{
    private readonly List<AlleleDefinition> _alleles = [];
    private readonly List<GeneDefinition> _genes = [];
    private readonly List<GroupDefinition> _groups = [];
    private readonly List<BodyPlanDefinition> _bodyPlans = [];
    private bool _isFrozen;

    public SystemDefinitionBuilder(SystemDefinitionVersion version)
    {
        Version = version;
    }

    public SystemDefinitionVersion Version { get; }

    public IReadOnlyList<AlleleDefinition> Alleles => _alleles;

    public IReadOnlyList<GeneDefinition> Genes => _genes;

    public IReadOnlyList<GroupDefinition> Groups => _groups;

    public IReadOnlyList<BodyPlanDefinition> BodyPlans => _bodyPlans;

    public bool IsFrozen => _isFrozen;

    public void AddAllele(AlleleDefinition definition)
    {
        EnsureMutable();
        _alleles.Add(definition);
    }

    public void AddGene(GeneDefinition definition)
    {
        EnsureMutable();
        _genes.Add(definition);
    }

    public void AddGroup(GroupDefinition definition)
    {
        EnsureMutable();
        _groups.Add(definition);
    }

    public void AddBodyPlan(BodyPlanDefinition definition)
    {
        EnsureMutable();
        _bodyPlans.Add(definition);
    }

    public ValidationResult Validate()
    {
        return SystemDefinitionValidator.Validate(this);
    }

    public FrozenSystemDefinition Freeze()
    {
        var validationResult = Validate();

        if (!validationResult.IsValid)
        {
            throw new SystemDefinitionValidationException(validationResult);
        }

        _isFrozen = true;

        return new FrozenSystemDefinition(
            Version,
            _alleles,
            _genes,
            _groups,
            _bodyPlans);
    }

    private void EnsureMutable()
    {
        if (_isFrozen)
        {
            throw new DefinitionMutationException("System definition builder is frozen.");
        }
    }
}
