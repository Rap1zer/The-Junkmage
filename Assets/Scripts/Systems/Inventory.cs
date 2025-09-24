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

    public bool CanPlaceItem(bool[,] shape, Vector2Int nearestCell)
    {
        if (shape == null) return false;

        int rows = shape.GetLength(0);
        int cols = shape.GetLength(1);

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (!shape[y, x]) continue; // Skip empty cells in the shape

                int cellX = nearestCell.x + x;
                int cellY = nearestCell.y + y;

                // Check bounds
                if (cellX >= width || cellY >= height) 
                    return false;

                // Check if the cell is already occupied
                if (InventoryManager.Inventory.IsCellOccupied(new Vector2Int(cellX, cellY))) 
                    return false;
            }
        }

        return true;
    }

    public void PlaceItem(IItem item, Vector2Int startingCell)
    {
        if (!CanPlaceItem(item.CurrentShape, startingCell))
        {
            Debug.LogWarning("Model rejected item placement!");
            return;
        }

        for (int y = 0; y < item.CurrentShape.GetLength(0); y++)
        {
            for (int x = 0; x < item.CurrentShape.GetLength(1); x++)
            {
                if (item.CurrentShape[y, x]) Data[startingCell.x + x, startingCell.y + y] = item;
            }
        }
        InventoryGrid.OccupyCells(new Vector2Int(startingCell.x, startingCell.y), item.CurrentShape);
    }

    public void TryRemoveItem(IItem item)
    {
        if (!TryGetItemGridPos(item, out Vector2Int anchor)) return;

        for (int y = 0; y < item.CurrentShape.GetLength(0); y++)
        {
            for (int x = 0; x < item.CurrentShape.GetLength(1); x++)
            {
                if (item.CurrentShape[y, x]) Data[anchor.x + x, anchor.y + y] = null;
            }
        }
    }

    public bool IsCellOccupied(Vector2Int cell)
    {
        if (cell.x >= width || cell.y >= height) return false;
        return Data[cell.x, cell.y] != null;
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
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (Data[x, y] != null && Data[x, y].Id == guid)
                {
                    position = new Vector2Int(x, y);
                    return true;
                }
            }
        }

        position = default;
        return false;
    }
}
