using UnityEngine;

public class WoodenShield : MonoBehaviour, IOnHit, IItem
{
    public ItemData ItemData { get; private set; }
    private PlayerController Player { get; set; }

    public void Initialise(ItemData itemData, PlayerController player)
    {
        ItemData = itemData;
        Player = player;

    }
}
