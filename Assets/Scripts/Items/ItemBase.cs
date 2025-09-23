using System;
using Unity.VisualScripting;
using UnityEngine;

public abstract class ItemBase : MonoBehaviour, IItem
{
    public ItemData ItemData { get; private set; }

    private GameObject InvContainer;
    private ItemUIType _uiType = ItemUIType.Chest;
    public ItemUIType UIType
    {
        get => _uiType;
        set
        {
            _uiType = value;
            if (value == ItemUIType.Inventory)
            {
                if (InvContainer == null) InvContainer = GameObject.Find("Inventory");
                transform.SetParent(InvContainer.transform, true);
            }
        }
    }

    public Guid Id { get; private set; }
    public int RotationState { get; private set; } = 0;

    protected PlayerController Player { get; private set; }

    public Vector2Int CurrentShape { get; private set; }

    public virtual void Initialise(ItemData itemData)
    {
        ItemData = itemData;
        Id = Guid.NewGuid();
        Player = GameObject.Find("Player").GetComponent<PlayerController>();
        CurrentShape = itemData.shape;
    }

    public float Rotate()
    {
        RotationState = RotationState < 3 ? RotationState + 1 : 0;
        UpdateCurrentShape();
        return RotationState * 90f;
    }

    private void UpdateCurrentShape()
    {
        if (RotationState == 0 || RotationState == 2) CurrentShape = ItemData.shape;
        else CurrentShape = new Vector2Int(ItemData.shape.y, ItemData.shape.x);
    }

    private Vector2 AnchorOffset
    {
        get
        {
            float cellStep = InventoryGrid.CellSize + InventoryGrid.Margin;

            // Offset by half cell for items with more than one cell
            float xCenterOffset = CurrentShape.x > 1 ? cellStep / 2f : 0f;
            float yCenterOffset = CurrentShape.y > 1 ? cellStep / 2f : 0f;

            // Number of cells to shift from the origin to reach the center (not including first cell)
            float xCellOffset = Mathf.Ceil(CurrentShape.x / 2f - 1);
            float yCellOffset = Mathf.Ceil(CurrentShape.y / 2f - 1);

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

    public Vector2Int AnchorGridPos
    {
        get
        {
            if (InventoryManager.Inventory.TryGetItemGridPos(this, out var pos))
                return pos;

            // If not found, decide how you want to handle it:
            // Option A: throw (explicit failure)
            throw new InvalidOperationException("Item not found in inventory.");

            // Option B: return a sentinel value
            // return new Vector2Int(-1, -1);
        }
    }

    // Abstract methods that subclasses must implement if needed
    public virtual void OnHit() { }       // optional to override
    public virtual void OnMiss() { }      // optional to override
}
