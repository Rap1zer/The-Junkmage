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

    internal void PlaceItem(GameObject itemObj, Vector2Int startingCell, Vector2Int itemSize)
    {
        if (!InventoryGrid.CanPlaceItem(startingCell, itemSize))
        {
            Debug.LogWarning("Model rejected item placement!");
            return;
        }

        IItem item = itemObj.GetComponent<IItem>();
        for (int y = 0; y < itemSize.y; y++)
        {
            for (int x = 0; x < itemSize.x; x++)
            {
                Data[startingCell.x + x, startingCell.y + y] = item;
            }
        }
    }
}
