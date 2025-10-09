using UnityEngine;

public interface IMovementController
{
    void MoveTo(Vector2 target, float arriveThreshold = 1f);
    void StrafeAround(Vector2 center);
    void Retreat(Vector2 away);
    void StopImmediate();

    // Optional extended AI hooks (for compatibility)
    void RequestStrafe(Vector2 toPlayer);
    void ImmediateStrafe(Vector2 toPlayer);
    void UpdateStrafe(Vector2 toPlayer);
    void SetHoldSuppressed(bool suppressed);
    bool IsAtPosition(Vector2 pos, float threshold);
    bool IsHolding();
}