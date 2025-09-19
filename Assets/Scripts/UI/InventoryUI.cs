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

    private InventoryGrid grid;
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
        grid = new InventoryGrid();
    }

    void Start()
    {
        grid.DrawGrid(gridContainer, cellPrefab);
    }

    public void BeginDrag(PointerEventData eventData, ItemUIType type, Vector2Int index)
    {
        currentItem = eventData.pointerDrag;
        currentIndex = index;
        currentType = type;
    }

    public void Drag(PointerEventData eventData)
    {
        grid.ClearHighlights();

        Vector2 itemPos = GetCurrentItemPosition();
        Vector2Int nearestCell = grid.GetNearestGridPosition(itemPos);

        // UPDATE THIS CLUNKY CODE
        Vector2Int itemSize = currentChest.ItemsInChest[currentIndex.y].size;

        bool canPlace = grid.CanPlaceItem(itemSize, nearestCell);
        grid.HighlightCells(nearestCell, itemSize, canPlace);
    }

    public void EndDrag(PointerEventData eventData)
    {
        currentItem = null;
        currentType = null;
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

