using UnityEngine;
using UnityEngine.EventSystems;

public enum ItemUIType
{
    Chest,
    Inventory
}


public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    private GameObject currentItem;
    private Vector2Int currentIndex;
    private ItemUIType? currentType;
    private Chest currentChest;

    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private Transform gridContainer;
    [SerializeField] private Transform itemDropsContainer;

    private InventoryRenderer invRenderer;

    private RectTransform[] chestSlots;

    

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);


        chestSlots = new RectTransform[Chest.itemPoolCount];
        for (int i = 0; i < chestSlots.Length; i++)
        {
            chestSlots[i] = itemDropsContainer.transform.GetChild(i).GetComponent<RectTransform>();
        }

        invRenderer = new InventoryRenderer(canvas, chestSlots);
    }

    public void DrawGrid()
    {
        InventoryGrid.DrawGrid(gridContainer, cellPrefab);
    }

    public void BeginDrag(PointerEventData eventData, ItemUIType type, Vector2Int index)
    {
        currentItem = eventData.pointerDrag;
        currentIndex = index;
        currentType = type;
    }

    public void Drag(PointerEventData eventData)
    {
        InventoryGrid.ClearHighlights();

        var (nearestCell, canPlace, itemSize) = CalculateDragPlacement();
        InventoryGrid.HighlightCells(nearestCell, itemSize, canPlace);
    }

    public void EndDrag(PointerEventData eventData)
    {
        currentItem = null;
        currentType = null;
        // IF CAN PLACE
        // Inventory must update internal state to store object
        // Item must snap to grid

        // IF CAN'T PLACE
        // Item snap back to original position

        // var (nearestCell, canPlace, itemSize) = CalculateDragPlacement();
        // if (canPlace)
        // {

        //     Inventory.Instance.InventoryData[nearestCell.x, nearestCell.y] = currentChest.ItemsInChest[currentIndex.y];
        // }
    }

    // Calculate if dragged item can be placed on grid in its current position
    private (Vector2Int nearestCell, bool canPlace, Vector2Int itemSize) CalculateDragPlacement()
    {
        Vector2 itemPos = GetCurrentItemPosition();
        Vector2Int nearestCell = InventoryGrid.GetNearestGridPosition(itemPos);

        Vector2Int itemSize = currentChest.ItemsInChest[currentIndex.y].size;
        bool canPlace = InventoryGrid.CanPlaceItem(itemSize, nearestCell);

        return (nearestCell, canPlace, itemSize);
    }


    private Vector2 GetCurrentItemPosition()
    {
        RectTransform canvasRT = canvas.GetComponent<RectTransform>();
        RectTransform itemRT = currentItem.transform.Find("Pivot").GetComponent<RectTransform>();

        return canvasRT.InverseTransformPoint(itemRT.position);
    }

    public void RegisterChest(Chest chest)
    {
        chest.OnChestOpened += HandleChestOpened;
    }

    private void HandleChestOpened(Chest chest)
    {
        if (chest == null || chest.ItemsInChest == null) return;

        currentChest = chest;
        invRenderer.RenderChestItems(chest.ItemsInChest);
        itemDropsContainer.gameObject.SetActive(true);
    }
}

