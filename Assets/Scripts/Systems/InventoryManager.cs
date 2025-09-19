using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private Inventory inventory;
    public static Inventory Inventory => Instance.inventory;

    private InventoryUI ui;
    public static InventoryUI UI => Instance.ui;

    public static int Width { get; private set; } = 3;
    public static int Height { get; private set; } = 3;

    public Chest CurrentChest { get; set; }
    public GameObject CurrentItem { get; set; }
    public Vector2Int CurrentIndex { get; set; }

    [Header("UI Settings")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private Transform gridContainer;
    [SerializeField] private Transform itemDropsContainer;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Initialize logic + UI
        inventory = new Inventory(gridContainer, Width, Height);
        ui = new InventoryUI(canvas, cellPrefab, gridContainer, itemDropsContainer);
    }

    void Start()
    {
        UI.DrawGrid();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
            Inventory.ToggleInventory();
    }

    public bool TryPlaceItem(Vector2Int startingCell, GameObject itemObj)
    {
        Vector2Int itemSize = CurrentChest.ItemsInChest[CurrentIndex.y].size;
        bool canPlace = InventoryGrid.CanPlaceItem(itemSize, startingCell);

        if (canPlace)
        {
            PlaceItem(startingCell, itemSize, itemObj);
            return true;
        }

        return false;
    }

    private void PlaceItem(Vector2Int startingCell, Vector2Int itemSize, GameObject itemObj)
    {
        // Snap item's position to grid
        UI.PlaceItem(itemObj, startingCell);
        // Place item in inventory
        Inventory.PlaceItem(itemObj, startingCell, itemSize);
    }

    public (Vector2Int nearestCell, bool canPlace, Vector2Int itemSize) CalculateDragPlacement()
    {
        Vector2 itemPos = UI.GetCurrentItemPosition();
        Vector2Int nearestCell = InventoryGrid.GetNearestGridPosition(itemPos);

        Vector2Int itemSize = CurrentChest.ItemsInChest[CurrentIndex.y].size;
        bool canPlace = InventoryGrid.CanPlaceItem(itemSize, nearestCell);

        return (nearestCell, canPlace, itemSize);
    }

    public void RegisterChest(Chest chest)
    {
        chest.OnChestOpened += HandleChestOpened;
    }

    private void HandleChestOpened(Chest chest)
    {
        CurrentChest = chest;
        UI.HandleChestOpened(chest);
    }
}
