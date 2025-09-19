using System;
using UnityEngine;

public abstract class ItemBase : MonoBehaviour, IItem
{
    public ItemData ItemData { get; private set; }
    public int RotationState { get; private set; } = 0;

    protected PlayerController Player { get; private set; }

    public virtual void Initialise(ItemData itemData, PlayerController player)
    {
        ItemData = itemData;
        Player = player;
    }
    
    public void UpdateRotationState() => RotationState = RotationState < 3 ? RotationState + 1 : 0;

    // Abstract methods that subclasses must implement if needed
    public virtual void OnHit() { }       // optional to override
    public virtual void OnMiss() { }      // optional to override
}
