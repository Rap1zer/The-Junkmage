using System;
using UnityEngine;

public class Inventory
{
    public static Inventory Instance { get; private set; }

    public int width = 3;
    public int height = 3;
    public bool isInventoryOpen = false;

    public IItem[,] Data { get; private set; }

    Transform gridContainer;


    public Inventory(Transform gridContainer, int width, int height)
    {
        this.gridContainer = gridContainer;
        Data = new IItem[width, height];
        this.width = width;
        this.height = height;
    }

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        gridContainer.gameObject.SetActive(!gridContainer.gameObject.activeSelf);
    }

    public void OpenInventory()
    {
        isInventoryOpen = true;
        gridContainer.gameObject.SetActive(true);
    }

    public void CloseInventory()
    {
        isInventoryOpen = false;
        gridContainer.gameObject.SetActive(false);
    }
    
    // Calculate if dragged item can be placed on grid in its current position
    public (Vector2Int nearestCell, bool canPlace, Vector2Int itemSize) CalculateDragPlacement(Vector2 itemPos, Vector2Int itemSize)
    {
        Vector2Int nearestCell = InventoryGrid.GetNearestGridPosition(itemPos);
        bool canPlace = InventoryGrid.CanPlaceItem(itemSize, nearestCell);

        return (nearestCell, canPlace, itemSize);
    }

    internal void PlaceItem(GameObject itemObj, Vector2Int startingCell, Vector2Int itemSize)
    {
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
