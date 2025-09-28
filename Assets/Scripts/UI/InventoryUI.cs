using System;
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
    private Transform chestContainer;

    private InventoryRenderer invRenderer;
    private RectTransform[] chestSlots;

    public InventoryUI(Canvas canvas, GameObject cellPrefab, Transform chestContainer)
    {
        this.canvas = canvas;
        this.cellPrefab = cellPrefab;
        this.chestContainer = chestContainer;

        chestSlots = new RectTransform[Chest.itemPoolCount];
        Transform itemDropsPos = canvas.transform.Find("Item Drops");
        for (int i = 0; i < chestSlots.Length; i++)
        {
            if (itemDropsPos.GetChild(i) == null) continue;
            chestSlots[i] = itemDropsPos.GetChild(i).GetComponent<RectTransform>();
        }

        invRenderer = new InventoryRenderer(canvas, chestSlots, chestContainer.gameObject);
    }

    public void DrawGrid(Transform container)
    {
        InventoryGrid.DrawGrid(container, cellPrefab);
    }

    public void BeginDrag(PointerEventData eventData, Vector2Int index)
    {
        beginDragPos = eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition;
    }

    public void Drag(PointerEventData eventData)
    {
        InventoryGrid.ClearHighlights();

        Vector2 anchorCanvasPos = GetCurrentItemCanvasPos();
        (var anchorCell, bool canPlace) = InventoryManager.Instance.CanPlaceDraggedItem(anchorCanvasPos, InventoryManager.Instance.Current.Item);
        InventoryGrid.HighlightCells(anchorCell, InventoryManager.Instance.Current.Item.CurrentShape, canPlace);
    }

    public void EndDrag(PointerEventData eventData)
    {
        beginDragPos = default;
        InventoryDragEvents.RaiseEndDrag(InventoryManager.Instance.Current.Obj, eventData);
    }

    public void UnDragCurrentItemPos()
    {
        RectTransform itemRT = InventoryManager.Instance.Current.Obj.transform as RectTransform;
        itemRT.anchoredPosition = beginDragPos;
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
        return canvasRT.InverseTransformPoint(InventoryManager.Instance.Current.Item.AnchorPos);
    }

    public IItem[] HandleChestOpened(Chest chest)
    {
        chestContainer.gameObject.SetActive(true);
        return invRenderer.RenderChestItems(chest.ItemsInChest);
    }

    public void HandleChestClosed()
    {
        invRenderer.ClearChestItems();
        chestContainer.gameObject.SetActive(false);
    }
}
