using System;
using Unity.VisualScripting;
using UnityEngine;

public abstract class ItemBase : MonoBehaviour, IItem
{
    public ItemData ItemData { get; private set; }
    public int RotationState { get; private set; } = 0;

    protected PlayerController Player { get; private set; }

    public Vector2Int CurrentShape { get; private set; }

    public virtual void Initialise(ItemData itemData)
    {
        ItemData = itemData;
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

public Vector2 AnchorLocalPos
{
    get
    {
        // Total size of one cell including margin
        float cellStep = InventoryGrid.CellSize + InventoryGrid.Margin;

        // Center offset for items larger than 1 cell
        float xCenterOffset = ItemData.shape.x > 1 ? cellStep / 2f : 0f;
        float yCenterOffset = ItemData.shape.y > 1 ? cellStep / 2f : 0f;

        // Number of cells to shift from the origin to reach the center
        float xCellOffset = Mathf.Ceil(ItemData.shape.x / 2f - 1);
        float yCellOffset = Mathf.Ceil(ItemData.shape.y / 2f - 1);

        // Convert cell offset to local position
        float xPos = xCenterOffset + xCellOffset * cellStep;
        float yPos = yCenterOffset + yCellOffset * cellStep;

        // Return negative because anchor moves left/up from origin
        return new Vector2(-xPos, -yPos);
    }
}


    public Vector3 AnchorWorldPos
    {
        get
        {
            return transform.position + transform.rotation * (Vector3)AnchorLocalPos;
        }
    }

    public Vector3 CalcAnchorWorld
    {
        get
        {
            // rotate local anchor into world space and add to item position
            return transform.position + transform.rotation * (Vector3)AnchorLocalPos;
        }
    }

    // Abstract methods that subclasses must implement if needed
    public virtual void OnHit() { }       // optional to override
    public virtual void OnMiss() { }      // optional to override
}
