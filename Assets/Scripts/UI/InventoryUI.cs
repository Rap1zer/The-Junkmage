using UnityEngine;
using UnityEngine.EventSystems;

public enum ItemUIType
{
    Chest,
    Inventory
}

public class InventoryUI
{
    private Vector2 beginDragPos;

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

    public void BeginDrag(PointerEventData eventData, Vector2Int index)
    {
        InventoryManager.Instance.CurrentObj = eventData.pointerDrag;
        InventoryManager.Instance.CurrentIndex = index;
        InventoryManager.Instance.CurrentType = InventoryManager.Instance.CurrentItem.UIType;
        
        beginDragPos = eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition;

        InventoryManager.Inventory.TryRemoveItem(InventoryManager.Instance.CurrentItem);
    }

    public void Drag(PointerEventData eventData)
    {
        InventoryGrid.ClearHighlights();

        var (nearestCell, canPlace, itemSize) = InventoryManager.Instance.CalculateDragPlacement();
        InventoryGrid.HighlightCells(nearestCell, itemSize, canPlace);
    }

    public void EndDrag(PointerEventData eventData)
    {
        Vector2Int nearestCell = InventoryGrid.GetNearestGridPosition(GetCurrentItemCanvasPos());
        bool placed = InventoryManager.Instance.TryPlaceItem(nearestCell, InventoryManager.Instance.CurrentObj);

        // Reset item position if cannot place
        if (!placed && InventoryManager.Instance.CurrentType == ItemUIType.Chest)
        {
            RectTransform itemRT = InventoryManager.Instance.CurrentObj.transform as RectTransform;
            itemRT.anchoredPosition = beginDragPos;
        }

        InventoryManager.Instance.CurrentObj = null;
        InventoryManager.Instance.CurrentType = null;
    }

    // Snap item's position to grid
    public void PlaceItem(GameObject itemObj, Vector2Int startingCell)
    {
        Vector3 anchorWorldPos = itemObj.GetComponent<IItem>().AnchorPos;
        Vector3 targetCellPos = InventoryGrid.CellObjs[startingCell.x, startingCell.y].transform.position;

        // compute how far off the item is
        Vector3 offset = anchorWorldPos - itemObj.transform.position;

        // move item so pivot lands exactly on target cell
        itemObj.transform.position = targetCellPos - offset;
    }

    public Vector2 GetCurrentItemCanvasPos()
    {
        RectTransform canvasRT = canvas.GetComponent<RectTransform>();
        return canvasRT.InverseTransformPoint(InventoryManager.Instance.CurrentItem.AnchorPos);
    }

    public IItem[] HandleChestOpened(Chest chest)
    {
        itemDropsContainer.gameObject.SetActive(true);
        return invRenderer.RenderChestItems(chest.ItemsInChest);
    }
}
