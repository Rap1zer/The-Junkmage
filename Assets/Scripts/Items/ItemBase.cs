using System;
using System.Collections.Generic;
using UnityEngine;

public enum StorageType
{
    Chest,
    Inventory
}

public abstract class ItemBase : MonoBehaviour
{
    public ItemData ItemData { get; private set; }
    protected PlayerController player;
    protected EntityEventDispatcher ownerDispatcher;

    private GameObject InvContainer;
    private StorageType _storageType = StorageType.Chest;
    public StorageType StorageType
    {
        get => _storageType;
        set
        {
            _storageType = value;
            if (value == StorageType.Inventory)
            {
                if (InvContainer == null) InvContainer = GameObject.Find("Inventory");
                transform.SetParent(InvContainer.transform, true);

                OnEquip();
            }
            else
            {
                OnUnequip();
            }
        }
    }
    public Guid Id { get; private set; }
    public int RotationState { get; private set; } = 0;

    public bool[,] CurrentShape { get; private set; }
    public bool[,] CurrentStars { get; private set; }
    private int currStarOffsetRow;
    private int currStarOffsetCol;

    private Vector2 AnchorOffset
    {
        get
        {
            float cellStep = InventoryManager.UI.CellSize + InventoryManager.UI.Margin;

            // Offset by half cell for items with more than one cell
            float xCenterOffset = CurrentShape.GetLength(1) > 1 ? cellStep / 2f : 0f;
            float yCenterOffset = CurrentShape.GetLength(0) > 1 ? cellStep / 2f : 0f;

            // Number of cells to shift from the origin to reach the center (not including first cell)
            float xCellOffset = Mathf.Ceil(CurrentShape.GetLength(1) / 2f - 1);
            float yCellOffset = Mathf.Ceil(CurrentShape.GetLength(0) / 2f - 1);

            float xPos = xCenterOffset + xCellOffset * cellStep;
            float yPos = yCenterOffset + yCellOffset * cellStep;

            // Return negative because anchor moves left/up from origin
            return new Vector2(-xPos, -yPos);
        }
    }


    public Vector3 AnchorPos
    {
        get
        {
            return transform.position + (Vector3)AnchorOffset;
        }
    }

    public Vector3 CalcAnchorWorld
    {
        get
        {
            // rotate local anchor into world space and add to item position
            return transform.position + transform.rotation * (Vector3)AnchorOffset;
        }
    }

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
        RotationState = RotationState < 3 ? RotationState + 1 : 0;
        CurrentShape = UpdateRotatedBoolArray(ItemData.Get2DBoolArray(ItemData.shape));
        CurrentStars = UpdateRotatedBoolArray(ItemData.Get2DBoolArray(ItemData.stars));
        UpdateStarOffset();
        return RotationState * 90f;
    }

    private void UpdateStarOffset()
    {
        switch (RotationState)
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

        switch (RotationState)
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
        player = GameObject.Find("Player").GetComponent<PlayerController>();

        // find the owning dispatcher on parent (adjust if your ownership model differs)
        ownerDispatcher = GetComponentInParent<EntityEventDispatcher>();
        if (ownerDispatcher != null)
            ownerDispatcher.RegisterItemHandlers(this);
    }

    // ensure we unregister when destroyed / unequipped
    protected virtual void OnDestroy()
    {
        if (ownerDispatcher != null)
        {
            ownerDispatcher.UnregisterItemHandlers(this);
            ownerDispatcher = null;
        }
    }
    
    // Called when Item is placed in the inventory
    protected virtual void OnEquip() { }
    
    // Called when Item is removed from the inventory
    protected virtual void OnUnequip() { }
    
    // Default hook implementations (override in subclasses as needed)
    public virtual float OnIncomingDamage(float damage, GameObject attacker = null) => damage;
}
