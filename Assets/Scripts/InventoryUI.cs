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
    private RectTransform currentRT;
    private ItemUIType? currentType;

    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private Transform gridContainer;
    [SerializeField] private Transform itemDropsContainer;

    private InventoryGrid grid;
    private InventoryRenderer invRenderer;

    private RectTransform[] chestSlots;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

        grid = new InventoryGrid();
        grid.DrawGrid(gridContainer, cellPrefab);
    }

    public void BeginDrag(PointerEventData eventData, ItemUIType type)
    {
        currentItem = eventData.pointerDrag;
        currentType = type;
        currentRT = currentItem.GetComponent<RectTransform>();
    }

    public void Drag(PointerEventData eventData)
    {
        grid.ClearHighlights();

        Vector2 itemPos = GetCurrentItemPosition();
        Vector2Int nearestCell = grid.GetNearestGridPosition(itemPos);

        Vector2Int itemSize = Inventory.Instance.ChestItemsData[currentItem.GetComponent<DraggableItem>().Index.y].size;

        bool canPlace = grid.CanPlaceItem(itemSize, nearestCell);
        grid.HighlightCells(nearestCell, itemSize, canPlace);
    }

    public void EndDrag(PointerEventData eventData)
    {
        currentItem = null;
        currentType = null;
        currentRT = null;
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

    private void HandleChestOpened(ItemData[] items)
    {
        if (items == null) return;

        invRenderer.RenderChestItems(items);
        itemDropsContainer.gameObject.SetActive(true);
    }
}

