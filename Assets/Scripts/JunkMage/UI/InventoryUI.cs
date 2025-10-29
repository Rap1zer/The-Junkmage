using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryUI
{
    private Vector2 beginDragPos;

    private Canvas canvas;
    private GameObject cellPrefab;
    private Transform chestContainer;

    private readonly ChestUI chestUI;
    public InventoryGrid invGrid;

    private readonly float cellSize = 100f;
    private readonly float margin = 10f;

    public InventoryUI(Canvas canvas, GameObject cellPrefab, Transform chestContainer)
    {
        this.canvas = canvas;
        this.cellPrefab = cellPrefab;
        this.chestContainer = chestContainer;

        var chestSlots1 = new RectTransform[Chest.itemPoolCount];
        Transform itemDropsPos = canvas.transform.Find("Item Drops");
        for (int i = 0; i < chestSlots1.Length; i++)
        {
            if (itemDropsPos.GetChild(i) == null) continue;
            chestSlots1[i] = itemDropsPos.GetChild(i).GetComponent<RectTransform>();
        }

        chestUI = new ChestUI(chestSlots1, chestContainer.gameObject);
        invGrid = new InventoryGrid(InventoryManager.Height, InventoryManager.Width, cellSize, margin);
    }

    public void DrawGrid(Transform container)
    {
        invGrid.DrawGrid(container, cellPrefab);
    }

    public void BeginDrag(PointerEventData eventData)
    {
        beginDragPos = eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition;
    }

    public void Drag(PointerEventData eventData)
    {
        invGrid.ClearHighlights();
        
        (var anchorCell, bool canPlace) = InventoryManager.Instance.CanPlaceDraggedItem();
        invGrid.HighlightCells(anchorCell, InventoryManager.Instance.Current.Item, canPlace);
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
    public void PlaceItem(GameObject itemObj, CellPos startingCell)
    {
        Vector3 anchorWorldPos = ItemAnchorPos(itemObj.GetComponent<ItemBase>());
        Vector3 targetCellPos = invGrid.CellObjs[startingCell.Row, startingCell.Col].transform.position;

        // compute how far off the item is
        Vector3 offset = anchorWorldPos - itemObj.transform.position;

        // move item so pivot lands exactly on target cell
        itemObj.transform.position = targetCellPos - offset;
    }

    public Vector2 GetCurrentItemCanvasPos()
    {
        RectTransform canvasRT = canvas.GetComponent<RectTransform>();
        return canvasRT.InverseTransformPoint(ItemAnchorPos(InventoryManager.Instance.Current.Item));
    }

    public bool TryReturnItemToChest(ItemBase item)
    {
        return chestUI.SnapItemBackToChest(item);
    }

    public ItemBase[] HandleChestOpened(Chest chest)
    {
        chestContainer.gameObject.SetActive(true);
        return chestUI.RenderChestItems(chest.ItemsInChest);
    }

    public void HandleChestClosed()
    {
        chestUI.ClearChestItems();
        chestContainer.gameObject.SetActive(false);
    }
    
    private Vector2 ItemAnchorOffset(ItemBase item)
    {
        float cellStep = cellSize + margin;

        int itemWidth = item.CurrentShape.GetLength(1);
        int itemCol = item.CurrentShape.GetLength(0);

        // Offset by half cell for items with more than one cell
        float xCenterOffset = itemWidth > 1 ? cellStep / 2f : 0f;
        float yCenterOffset = itemCol > 1 ? cellStep / 2f : 0f;

        // Number of cells to shift from the origin to reach the center (not including first cell)
        float xCellOffset = Mathf.Ceil(itemWidth / 2f - 1);
        float yCellOffset = Mathf.Ceil(itemCol / 2f - 1);

        float xPos = xCenterOffset + xCellOffset * cellStep;
        float yPos = yCenterOffset + yCellOffset * cellStep;

        // Return negative because anchor moves left/up from origin
        return new Vector2(-xPos, -yPos);
    }


    private Vector3 ItemAnchorPos(ItemBase item) => item.transform.position + (Vector3)ItemAnchorOffset(item);

    /** Rotate local anchor into world space and add to item position. */
    private Vector3 CalcAnchorWorld(ItemBase item) =>
        item.transform.position + item.transform.rotation * (Vector3)ItemAnchorOffset(item);
}
