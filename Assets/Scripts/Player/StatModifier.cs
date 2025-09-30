public enum ModifierType
{
    Flat,       // +5
    PercentAdd, // +20%
    PercentMul  // x1.5
}

public class StatModifier
{
    public StatType Stat { get; }
    public float Value { get; }
    public ModifierType Type { get; }

    public StatModifier(StatType stat, float value, ModifierType type)
    {
        Stat = stat;
        Value = value;
        Type = type;
    }
}
