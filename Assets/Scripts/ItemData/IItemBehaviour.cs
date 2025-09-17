using System;
using UnityEngine;

public interface IItemBehaviour
{
    public virtual void OnEquip(PlayerController player) { }
    public virtual void OnUnequip(PlayerController player) { }
}
