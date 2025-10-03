public enum ModifierType
{
    Flat,       // +5
    PercentAdd, // +20%
    PercentMul  // x1.5
}

public class StatModifier
{
    public StatType Stat { get; }
    public float Amount { get; }
    public ModifierType Type { get; }

    public StatModifier(StatType stat, float amount, ModifierType type)
    {
        Stat = stat;
        Amount = amount;
        Type = type;
    }
}
