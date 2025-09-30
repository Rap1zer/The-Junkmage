using System;
using UnityEngine;

public class Inventory
{
    public static Inventory Instance { get; private set; }

    public int width = 4;   // columns
    public int height = 3;  // rows

    // Data[row, col]
    public IItem[,] Data { get; private set; }
    public event Action<IItem, CellPos> OnItemPlaced; // InventoryGrid.OccupyCells()
    public event Action<IItem> OnItemRemoved;

    public Inventory(int width, int height)
    {
        // Data is allocated as [rows, cols] => [height, width]
        Data = new IItem[height, width];
        this.width = width;
        this.height = height;
    }

    public bool CanPlaceItem(IItem item, CellPos anchorCell)
    {
        foreach (var pos in item.GetOccupiedCells(anchorCell))
        {
            // Check bounds
            if (pos.Row < 0 || pos.Row >= height || pos.Col < 0 || pos.Col >= width)
                return false;

            // Check if the cell is already occupied
            if (IsCellOccupied(pos))
                return false;
        }

        return true;
    }


    public bool PlaceItem(IItem item, CellPos anchorCell)
    {
        if (!CanPlaceItem(item, anchorCell)) return false;

        item.AnchorGridPos = anchorCell;

        foreach (var pos in item.GetOccupiedCells(anchorCell))
        {
            Data[pos.Row, pos.Col] = item;
        }

        OnItemPlaced?.Invoke(item, anchorCell);
        return true;
    }

    public void RemoveItem(IItem item)
    {
        if (!TryGetItemGridPos(item, out CellPos anchor)) return;

        foreach (var pos in item.GetOccupiedCells(anchor))
        {
            if (pos.Row < 0 || pos.Row >= height || pos.Col < 0 || pos.Col >= width)
            {
                Debug.LogWarning($"Skipped out-of-bounds cell at Data[{pos.Row},{pos.Col}]");
                continue;
            }

            Data[pos.Row, pos.Col] = null;
        }

        OnItemRemoved?.Invoke(item);
    }

    // cell.x = row, cell.y = col
    public bool IsCellOccupied(CellPos cell)
    {
        if (cell.Row < 0 || cell.Row >= height || cell.Col < 0 || cell.Col >= width) return false;
        return Data[cell.Row, cell.Col] != null;
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

    public bool TryGetItemGridPos(IItem item, out CellPos position)
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
