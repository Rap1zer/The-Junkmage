using System;
using System.Collections.Generic;
using JunkMage.Stats;
using UnityEngine;

public enum StorageType
{
    Chest,
    Inventory
}

public abstract class ItemBase : MonoBehaviour
{
    private ItemData ItemData { get; set; }
    
    private GameObject player;
    protected PlayerStats playerStats;
    private EntityEventDispatcher playerDispatcher;

    private GameObject inventoryContainer;
    private StorageType storageType = StorageType.Chest;
    public StorageType StorageType
    {
        get => storageType;
        set
        {
            storageType = value;
            if (value == StorageType.Inventory)
            {
                if (inventoryContainer == null) inventoryContainer = GameObject.Find("Inventory");
                transform.SetParent(inventoryContainer.transform, true);

                if (playerDispatcher != null)
                {
                    playerDispatcher.RegisterItemHandlers(this);
                }
                
                OnEquip();
            }
            else
            {
                if (playerDispatcher != null)
                {
                    playerDispatcher.UnregisterItemHandlers(this);
                }
                OnUnequip();
            }
        }
    }
    
    public Guid Id { get; private set; }
    private int rotationState;

    public bool[,] CurrentShape { get; private set; }
    
    public bool[,] CurrentStars { get; private set; }
    private int currStarOffsetRow;
    private int currStarOffsetCol;

    public CellPos AnchorGridPos { get; set; }

    public virtual void Initialise(ItemData itemData)
    {
        ItemData = itemData;
        CurrentShape = itemData.Get2DBoolArray(ItemData.shape);
        CurrentStars = ItemData.Get2DBoolArray(ItemData.stars);
        currStarOffsetRow = ItemData.extraRowsHalf;
        currStarOffsetCol = ItemData.extraColsHalf;
    }

    public float Rotate()
    {
        rotationState = rotationState < 3 ? rotationState + 1 : 0;
        CurrentShape = UpdateRotatedBoolArray(ItemData.Get2DBoolArray(ItemData.shape));
        CurrentStars = UpdateRotatedBoolArray(ItemData.Get2DBoolArray(ItemData.stars));
        UpdateStarOffset();
        return rotationState * 90f;
    }

    private void UpdateStarOffset()
    {
        switch (rotationState)
        {
            case 0: // 0 degrees
            case 2: // 180 degrees clockwise
                currStarOffsetRow = ItemData.extraRowsHalf;
                currStarOffsetCol = ItemData.extraColsHalf;
                break;
            case 1: // 90 degrees clockwise
            case 3: // 270 degrees clockwise
                currStarOffsetRow = ItemData.extraColsHalf;
                currStarOffsetCol = ItemData.extraRowsHalf;
                break;
        }
    }

    private bool[,] UpdateRotatedBoolArray(bool[,] original)
    {
        bool[,] rotatedArray = new bool[original.GetLength(0), original.GetLength(1)];

        switch (rotationState)
        {
            case 0:
                rotatedArray = original;
                break;
            case 1: // 90 degrees clockwise
                rotatedArray = Rotate90Clockwise(original);
                break;

            case 2: // 180 degrees clockwise
                rotatedArray = Rotate90Clockwise(Rotate90Clockwise(original));
                break;

            case 3: // 270 degrees clockwise / 90 counterclockwise
                rotatedArray = Rotate90Clockwise(Rotate90Clockwise(Rotate90Clockwise(original)));
                // Or write a Rotate90CounterClockwise method
                break;
        }

        return rotatedArray;
    }

    // Rotates a 2D bool array 90° clockwise
    private bool[,] Rotate90Clockwise(bool[,] array)
    {
        int rows = array.GetLength(0);
        int cols = array.GetLength(1);
        bool[,] rotated = new bool[cols, rows];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                rotated[c, rows - 1 - r] = array[r, c];
            }
        }

        return rotated;
    }

    public IEnumerable<CellPos> GetOccupiedCells(CellPos anchor)
    {
        for (int r = 0; r < CurrentShape.GetLength(0); r++)
        {
            for (int c = 0; c < CurrentShape.GetLength(1); c++)
            {
                if (!CurrentShape[r, c]) continue;

                yield return new CellPos(anchor.Row + r, anchor.Col + c);
            }
        }
    }

    public IEnumerable<CellPos> GetStarCells(CellPos anchor)
    {
        for (int r = 0; r < CurrentStars.GetLength(0); r++)
        {
            for (int c = 0; c < CurrentStars.GetLength(1); c++)
            {
                if (!CurrentStars[r, c]) continue;

                yield return new CellPos(
                    anchor.Row + r - currStarOffsetRow,
                    anchor.Col + c - currStarOffsetCol
                );
            }
        }
    }

    protected virtual void Awake()
    {
        Id = Guid.NewGuid();
        player = GameObject.Find("Player");
        playerStats = player.GetComponent<PlayerStats>();

        // find the owning dispatcher on parent (adjust if your ownership model differs)
        playerDispatcher = player.GetComponent<EntityEventDispatcher>();
    }

    // ensure we unregister when destroyed / unequipped
    protected virtual void OnDestroy()
    {
        if (playerDispatcher != null)
        {
            playerDispatcher.UnregisterItemHandlers(this);
            playerDispatcher = null;
        }
    }
    
    // Called when Item is placed in the inventory
    protected virtual void OnEquip() { }
    
    // Called when Item is removed from the inventory
    protected virtual void OnUnequip() { Debug.Log("OnUnequip"); }
}
