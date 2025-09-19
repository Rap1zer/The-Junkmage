using UnityEngine;
using UnityEngine.EventSystems;

public enum ItemUIType
{
    Chest,
    Inventory
}

public class InventoryUI
{
    private ItemUIType? currentType;

    private Canvas canvas;
    private GameObject cellPrefab;
    private Transform gridContainer;
    private Transform itemDropsContainer;

    private InventoryRenderer invRenderer;
    private RectTransform[] chestSlots;

    public InventoryUI(Canvas canvas, GameObject cellPrefab, Transform gridContainer, Transform itemDropsContainer)
    {
        this.canvas = canvas;
        this.cellPrefab = cellPrefab;
        this.gridContainer = gridContainer;
        this.itemDropsContainer = itemDropsContainer;

        chestSlots = new RectTransform[Chest.itemPoolCount];
        for (int i = 0; i < chestSlots.Length; i++)
        {
            chestSlots[i] = itemDropsContainer.GetChild(i).GetComponent<RectTransform>();
        }

        invRenderer = new InventoryRenderer(canvas, chestSlots);
    }

    public void DrawGrid()
    {
        InventoryGrid.DrawGrid(gridContainer, cellPrefab);
    }

    public void BeginDrag(PointerEventData eventData, ItemUIType type, Vector2Int index)
    {
        InventoryManager.Instance.CurrentItem = eventData.pointerDrag;
        InventoryManager.Instance.CurrentIndex = index;
        currentType = type;
    }

    public void Drag(PointerEventData eventData)
    {
        InventoryGrid.ClearHighlights();

        var (nearestCell, canPlace, itemSize) = InventoryManager.Instance.CalculateDragPlacement();
        InventoryGrid.HighlightCells(nearestCell, itemSize, canPlace);
    }

    public void EndDrag(PointerEventData eventData)
    {
        InventoryManager.Instance.CurrentItem = null;
        currentType = null;

        // TODO: place logic here (snap to grid or reset)
        bool placed = InventoryManager.Instance.TryPlaceItem(
            InventoryManager.Instance.CurrentIndex,
            InventoryManager.Instance.CurrentItem);

        if (!placed) return; // TODO: reset item position
    }

    // Snap item's position to grid
    public void PlaceItem(GameObject itemObj, Vector2Int startingCell)
    {
        itemObj.transform.Find("Pivot").position = InventoryGrid.CellObjs[startingCell.x, startingCell.y].transform.position;
    }

    public Vector2 GetCurrentItemPosition()
    {
        RectTransform canvasRT = canvas.GetComponent<RectTransform>();
        RectTransform itemRT = InventoryManager.Instance.CurrentItem.transform.Find("Pivot").GetComponent<RectTransform>();

        return canvasRT.InverseTransformPoint(itemRT.position);
    }

    public void HandleChestOpened(Chest chest)
    {
        if (chest == null || chest.ItemsInChest == null) return;

        invRenderer.RenderChestItems(chest.ItemsInChest);
        itemDropsContainer.gameObject.SetActive(true);
    }
}
