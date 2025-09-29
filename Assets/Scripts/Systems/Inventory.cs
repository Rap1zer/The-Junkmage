using System;
using UnityEngine;

public class Inventory
{
    public static Inventory Instance { get; private set; }

    public int width = 4;   // columns
    public int height = 3;  // rows

    // Data[row, col]
    public IItem[,] Data { get; private set; }
    public event Action<IItem, Vector2Int> OnItemPlaced; // InventoryGrid.OccupyCells()
    public event Action<IItem> OnItemRemoved;

    public Inventory(int width, int height)
    {
        // Data is allocated as [rows, cols] => [height, width]
        Data = new IItem[height, width];
        this.width = width;
        this.height = height;
    }

    // NOTE: anchorCell.x == row, anchorCell.y == col
    public bool CanPlaceItem(bool[,] shape, Vector2Int anchorCell)
    {
        if (shape == null) return false;

        int shapeRows = shape.GetLength(0);
        int shapeCols = shape.GetLength(1);

        for (int r = 0; r < shapeRows; r++)
        {
            for (int c = 0; c < shapeCols; c++)
            {
                if (!shape[r, c]) continue; // Skip empty cells in the shape

                int row = anchorCell.x + r;
                int col = anchorCell.y + c;

                // Check bounds
                if (row < 0 || row >= height || col < 0 || col >= width)
                    return false;

                // Check if the cell is already occupied
                if (IsCellOccupied(new Vector2Int(row, col)))
                    return false;
            }
        }

        return true;
    }

    public bool PlaceItem(IItem item, Vector2Int anchorCell)
    {
        if (!CanPlaceItem(item.CurrentShape, anchorCell)) return false;

        item.AnchorGridPos = anchorCell; // stored as (row, col)

        for (int r = 0; r < item.CurrentShape.GetLength(0); r++)
        {
            for (int c = 0; c < item.CurrentShape.GetLength(1); c++)
            {
                if (item.CurrentShape[r, c])
                {
                    int row = anchorCell.x + r;
                    int col = anchorCell.y + c;
                    Data[row, col] = item;
                }
            }
        }

        OnItemPlaced?.Invoke(item, anchorCell);
        return true;
    }

    public void RemoveItem(IItem item)
    {
        if (!TryGetItemGridPos(item, out Vector2Int anchor)) return;

        for (int r = 0; r < item.CurrentShape.GetLength(0); r++)
        {
            for (int c = 0; c < item.CurrentShape.GetLength(1); c++)
            {
                if (!item.CurrentShape[r, c]) continue;

                int targetRow = anchor.x + r;
                int targetCol = anchor.y + c;

                // Bounds check (defensive)
                if (targetRow < 0 || targetRow >= height || targetCol < 0 || targetCol >= width)
                {
                    Debug.LogWarning($"Skipped out-of-bounds cell at Data[{targetRow},{targetCol}]");
                    continue;
                }

                Data[targetRow, targetCol] = null;
            }
        }

        OnItemRemoved?.Invoke(item);
    }

    // cell.x = row, cell.y = col
    public bool IsCellOccupied(Vector2Int cell)
    {
        if (cell.x < 0 || cell.x >= height || cell.y < 0 || cell.y >= width) return false;
        return Data[cell.x, cell.y] != null;
    }

    public bool ChestItemEquipped(Chest chest)
    {
        // iterate rows then cols
        for (int r = 0; r < height; r++)
        {
            for (int c = 0; c < width; c++)
            {
                var it = Data[r, c];
                if (it != null && chest.chestItems.ContainsKey(it.Id)) return true;
            }
        }

        return false;
    }

    public bool TryGetItemGridPos(IItem item, out Vector2Int position)
    {
        Guid guid = item.Id;
        for (int r = 0; r < height; r++) // rows first
        {
            for (int c = 0; c < width; c++) // cols second
            {
                if (Data[r, c] != null && Data[r, c].Id == guid)
                {
                    position = Data[r, c].AnchorGridPos; // AnchorGridPos should also be (row,col)
                    return true;
                }
            }
        }

        position = default;
        return false;
    }
}
