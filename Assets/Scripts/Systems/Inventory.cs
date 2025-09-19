using UnityEngine;

public class Inventory
{
    public static Inventory Instance { get; private set; }

    public int width = 3;
    public int height = 3;
    public bool isInventoryOpen = false;

    public IItem[,] InventoryData { get; private set; }

    Transform gridContainer;


    public Inventory(Transform gridContainer, int width, int height)
    {
        this.gridContainer = gridContainer;
        InventoryData = new IItem[width, height];
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

    public void PlaceItem(ItemData data, Vector2Int startingCell)
    {

    }
    
    // Calculate if dragged item can be placed on grid in its current position
    public (Vector2Int nearestCell, bool canPlace, Vector2Int itemSize) CalculateDragPlacement(Vector2 itemPos, Vector2Int itemSize)
    {
        Vector2Int nearestCell = InventoryGrid.GetNearestGridPosition(itemPos);
        bool canPlace = InventoryGrid.CanPlaceItem(itemSize, nearestCell);

        return (nearestCell, canPlace, itemSize);
    }
}
