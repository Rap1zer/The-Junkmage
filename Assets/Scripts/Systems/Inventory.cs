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

    public void PlaceItem(IItem item, Vector2Int startingCell)
    {
        if (!InventoryGrid.CanPlaceItem(item.CurrentShape, startingCell))
        {
            Debug.LogWarning("Model rejected item placement!");
            return;
        }

        for (int y = 0; y < item.CurrentShape.y; y++)
        {
            for (int x = 0; x < item.CurrentShape.x; x++)
            {
                Data[startingCell.x + x, startingCell.y + y] = item;
                InventoryGrid.OccupyCells(new Vector2Int(startingCell.x + x, startingCell.y + y), item.CurrentShape);
            }
        }
    }

    public void TryRemoveItem(IItem item)
    {
        if (!TryGetItemGridPos(item, out Vector2Int anchor)) return;

        for (int x = 0; x < item.CurrentShape.x; x++)
        {
            for (int y = 0; y < item.CurrentShape.y; y++)
            {
                Data[anchor.x + x, anchor.y + y] = null;
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
