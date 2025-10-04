using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryUI
{
    private Vector2 beginDragPos;

    private Canvas canvas;
    private GameObject cellPrefab;
    private Transform chestContainer;

    public ChestUI ChestUI {get; private set;}
    public InventoryGrid invGrid;

    public float CellSize { get; private set; } = 100f;
    public float Margin { get; private set; } = 10f;

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

        ChestUI = new ChestUI(chestSlots, chestContainer.gameObject);
        invGrid = new InventoryGrid(InventoryManager.Height, InventoryManager.Width, CellSize, Margin);
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
        Vector3 anchorWorldPos = itemObj.GetComponent<ItemBase>().AnchorPos;
        Vector3 targetCellPos = invGrid.CellObjs[startingCell.Row, startingCell.Col].transform.position;

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

    public ItemBase[] HandleChestOpened(Chest chest)
    {
        chestContainer.gameObject.SetActive(true);
        return ChestUI.RenderChestItems(chest.ItemsInChest);
    }

    public void HandleChestClosed()
    {
        ChestUI.ClearChestItems();
        chestContainer.gameObject.SetActive(false);
    }
}
