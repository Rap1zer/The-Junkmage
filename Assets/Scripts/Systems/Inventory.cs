using System;
using UnityEngine;

public class Inventory
{
    public static Inventory Instance { get; private set; }

    private int width;
    private int height;

    public IItem[,] Data { get; private set; }
    public event Action<IItem, Vector2Int> OnItemPlaced; // InventoryGrid.OccupyCells()
    public event Action<IItem> OnItemRemoved;

    public Inventory(int width, int height)
    {
        Data = new IItem[height, width]; // row-major: y first, x second
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
                if (IsCellOccupied(new Vector2Int(col, row))) 
                    return false;
            }
        }

        return true;
    }

    public bool PlaceItem(IItem item, Vector2Int anchorCell)
    {
        if (!CanPlaceItem(item.CurrentShape, anchorCell)) return false;

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
        
        OnItemPlaced?.Invoke(item, anchorCell);
        return true;
    }

    public void RemoveItem(IItem item)
    {
        if (!TryGetItemGridPos(item, out Vector2Int anchor)) return;

        for (int y = 0; y < item.CurrentShape.GetLength(0); y++)
        {
            for (int x = 0; x < item.CurrentShape.GetLength(1); x++)
            {
                if (!item.CurrentShape[y, x]) continue;

                int targetY = anchor.y + y;
                int targetX = anchor.x + x;

                // Bounds check
                if (targetY < 0 || targetY >= height || targetX < 0 || targetX >= width) 
                    continue;

                Data[targetY, targetX] = null;
            }
        }

        OnItemRemoved?.Invoke(item);
    }

    public bool IsCellOccupied(Vector2Int cell)
    {
        if (cell.x < 0 || cell.x >= width || cell.y < 0 || cell.y >= height) return false;
        return Data[cell.y, cell.x] != null;
    }

    public bool ChestItemEquipped(Chest chest)
    {
        // Fixed loop order: y = rows, x = columns
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (Data[y, x] != null && chest.chestItems.ContainsKey(Data[y, x].Id))
                    return true;
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
