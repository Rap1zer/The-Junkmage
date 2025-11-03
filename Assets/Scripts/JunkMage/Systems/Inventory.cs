using System;
using UnityEngine;

public class Inventory
{
    private readonly int cols;   // columns
    private readonly int rows;  // rows
    
    public ItemBase[,] Data { get; }
    public event Action<ItemBase, CellPos> OnItemPlaced; // InventoryGrid.OccupyCells()
    public event Action<ItemBase> OnItemRemoved;

    public Inventory(int cols, int rows)
    {
        // Data is allocated as [rows, cols]
        Data = new ItemBase[rows, cols];
        this.cols = cols;
        this.rows = rows;
    }

    public bool CanPlaceItem(ItemBase item, CellPos anchorCell)
    {
        foreach (var pos in item.GetOccupiedCells(anchorCell))
        {
            // Check bounds
            if (pos.Row < 0 || pos.Row >= rows || pos.Col < 0 || pos.Col >= cols)
                return false;

            // Check if the cell is already occupied
            if (IsCellOccupied(pos))
                return false;
        }

        return true;
    }


    public bool TryPlaceItem(ItemBase item, CellPos anchorCell)
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

    public void TryRemoveItem(ItemBase item)
    {
        if (!TryGetItemGridPos(item, out CellPos anchor)) return;

        foreach (var pos in item.GetOccupiedCells(anchor))
        {
            if (pos.Row < 0 || pos.Row >= rows || pos.Col < 0 || pos.Col >= cols)
            {
                UnityEngine.Debug.LogWarning($"Skipped out-of-bounds cell at Data[{pos.Row},{pos.Col}]");
                continue;
            }

            Data[pos.Row, pos.Col] = null;
        }

        OnItemRemoved?.Invoke(item);
    }

    // cell.x = row, cell.y = col
    private bool IsCellOccupied(CellPos cell)
    {
        if (cell.Row < 0 || cell.Row >= rows || cell.Col < 0 || cell.Col >= cols) return false;
        return Data[cell.Row, cell.Col] != null;
    }

    public bool IsChestItemEquipped(Chest chest)
    {
        if (chest == null) return false;
        if (chest.ChestItems == null) return false;
        
        // iterate rows then cols
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                ItemBase item = Data[r, c];
                if (item != null && chest.ChestItems.ContainsKey(item.Id)) return true;
            }
        }

        return false;
    }

    private bool TryGetItemGridPos(ItemBase item, out CellPos position)
    {
        Guid guid = item.Id;
        for (int r = 0; r < rows; r++) // rows first
        {
            for (int c = 0; c < cols; c++) // cols second
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
