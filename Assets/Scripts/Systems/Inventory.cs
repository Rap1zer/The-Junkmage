using System;
using UnityEngine;

public class Inventory
{
    public static Inventory Instance { get; private set; }

    public int width = 3;
    public int height = 3;

    public IItem[,] Data { get; private set; }

    public Inventory(int width, int height)
    {
        Data = new IItem[width, height];
        this.width = width;
        this.height = height;
    }

    public bool CanPlaceItem(bool[,] shape, Vector2Int anchorCell)
    {
        if (shape == null) return false;

        int rows = shape.GetLength(0);
        int cols = shape.GetLength(1);

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (!shape[y, x]) continue; // Skip empty cells in the shape

                int col = anchorCell.x + x;
                int row = anchorCell.y + y;

                // Check bounds
                if (col >= width || row >= height) 
                    return false;

                // Check if the cell is already occupied
                if (InventoryManager.Inventory.IsCellOccupied(new Vector2Int(row, col))) 
                    return false;
            }
        }

        return true;
    }

    public void PlaceItem(IItem item, Vector2Int anchorCell)
    {
        if (!CanPlaceItem(item.CurrentShape, anchorCell))
        {
            Debug.LogWarning("Model rejected item placement!");
            return;
        }

        item.AnchorGridPos = anchorCell;

        for (int y = 0; y < item.CurrentShape.GetLength(0); y++)
        {
            for (int x = 0; x < item.CurrentShape.GetLength(1); x++)
            {
                if (item.CurrentShape[y, x])
                {
                    Data[anchorCell.y + y, anchorCell.x + x] = item;
                }
            }
        }
        InventoryGrid.OccupyCells(new Vector2Int(anchorCell.x, anchorCell.y), item.CurrentShape);
    }

    public void TryRemoveItem(IItem item)
    {
        Debug.Log("try to remove item " + item.ItemData.name);
        if (!TryGetItemGridPos(item, out Vector2Int anchor)) return;
        Debug.Log(anchor);

        for (int y = 0; y < item.CurrentShape.GetLength(0); y++)
        {
            for (int x = 0; x < item.CurrentShape.GetLength(1); x++)
            {
                bool shapeCell = item.CurrentShape[y, x];
                int targetY = anchor.y + y;
                int targetX = anchor.x + x;

                Debug.Log($"Checking shape cell [{y},{x}] = {shapeCell} -> Data[{targetY},{targetX}]");

                // Optional: bounds check
                if (targetY < 0 || targetY >= Data.GetLength(0) || targetX < 0 || targetX >= Data.GetLength(1))
                {
                    Debug.LogWarning($"Skipped out-of-bounds cell at Data[{targetY},{targetX}]");
                    continue;
                }

                if (shapeCell)
                {
                    Debug.Log($"Setting Data[{targetY},{targetX}] = null");
                    Data[targetY, targetX] = null;
                }
            }
        }

    }

    public bool IsCellOccupied(Vector2Int cell)
    {
        if (cell.x >= width || cell.y >= height) return false;
        return Data[cell.y, cell.x] != null;
    }

    public bool ChestItemEquipped()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (Data[x, y]?.UIType == ItemUIType.Chest) return true;
            }
        }

        return false;
    }

    public bool TryGetItemGridPos(IItem item, out Vector2Int position)
    {
        Guid guid = item.Id;
        for (int y = 0; y < height; y++) // rows first
        {
            for (int x = 0; x < width; x++) // columns second
            {
                if (Data[y, x] != null && Data[y, x].Id == guid)
                {
                    position = Data[y, x].AnchorGridPos;
                    return true;
                }
            }
        }

        position = default;
        return false;
    }
}
