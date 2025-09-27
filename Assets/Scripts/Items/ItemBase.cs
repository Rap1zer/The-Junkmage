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

    public bool[,] CurrentShape { get; private set; }

    public virtual void Initialise(ItemData itemData)
    {
        ItemData = itemData;
        Id = Guid.NewGuid();
        Player = GameObject.Find("Player").GetComponent<PlayerController>();
        CurrentShape = itemData.Get2DShape();
    }

    public float Rotate()
    {
        RotationState = RotationState < 3 ? RotationState + 1 : 0;
        UpdateCurrentShape();
        return RotationState * 90f;
    }

    private void UpdateCurrentShape()
    {
        bool[,] original = ItemData.Get2DShape();

        switch (RotationState)
        {
            case 0:
                CurrentShape = original;
                break;
            case 1: // 90 degrees clockwise
                CurrentShape = Rotate90Clockwise(original);
                break;

            case 2: // 180 degrees clockwise
                CurrentShape = Rotate90Clockwise(Rotate90Clockwise(original));
                break;

            case 3: // 270 degrees clockwise / 90 counterclockwise
                CurrentShape = Rotate90Clockwise(Rotate90Clockwise(Rotate90Clockwise(original)));
                // Or write a Rotate90CounterClockwise method
                break;
        }
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

    private Vector2 AnchorOffset
    {
        get
        {
            float cellStep = InventoryGrid.CellSize + InventoryGrid.Margin;

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

    public Vector2Int AnchorGridPos { get; set; }

    // Abstract methods that subclasses must implement if needed
    public virtual void OnHit() { }       // optional to override
    public virtual void OnMiss() { }      // optional to override
}
