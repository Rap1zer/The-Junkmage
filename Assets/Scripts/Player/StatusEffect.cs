public abstract class StatusEffect
{
    public float duration;
    protected float timeLeft;

    public StatusEffect(float duration)
    {
        this.duration = duration;
        timeLeft = duration;
    }

    public abstract void Apply(PlayerStats stats);
    public abstract void Remove(PlayerStats stats);

    public bool Tick(PlayerStats stats, float deltaTime)
    {
        timeLeft -= deltaTime;
        if (timeLeft <= 0)
        {
            Remove(stats);
            return true; // expired
        }
        return false;
    }
}
