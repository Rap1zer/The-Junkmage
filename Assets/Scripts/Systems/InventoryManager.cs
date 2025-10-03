using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    // --- Singleton ---
    public static InventoryManager Instance { get; private set; }

    // --- Inventory State ---
    private Inventory inventory;
    public static Inventory Inventory => Instance.inventory;

    private InventoryUI ui;
    public static InventoryUI UI => Instance.ui;

    public static int Width { get; private set; } = 4;
    public static int Height { get; private set; } = 3;

    public Chest CurrentChest { get; set; }
    public SelectedItem Current { get; private set; } = new SelectedItem();

    public bool isInventoryOpen { get; set; } = false;

    [SerializeField] private DebugInventoryRenderer debugRenderer;

    // --- UI References ---
    [Header("UI Settings")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private Transform InvContainer;
    [SerializeField] private Transform ChestContainer;
    [SerializeField] private Button continueBtn;

    // ----------------- UNITY CALLBACKS -----------------

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Initialize logic + UI
        inventory = new Inventory(Width, Height);
        ui = new InventoryUI(canvas, cellPrefab, ChestContainer);

        if (debugRenderer != null)
            debugRenderer.Init(Height, Width);
    }

    private void Start()
    {
        UI.DrawGrid(InvContainer.transform.Find("Inventory Grid"));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
            ToggleInventory();

        if (DraggableItem.IsDragging && Current.Item != null && Input.GetKeyDown(KeyCode.R))
            RotateItem();

        if (debugRenderer != null)
            debugRenderer.Refresh(Inventory.Data);
    }

    private void OnEnable()
    {
        InventoryDragEvents.OnBeginDrag += HandleBeginDrag;
        InventoryDragEvents.OnDrag += HandleDrag;
        InventoryDragEvents.OnEndDrag += HandleEndDrag;
    }

    private void OnDisable()
    {
        InventoryDragEvents.OnBeginDrag -= HandleBeginDrag;
        InventoryDragEvents.OnDrag -= HandleDrag;
        InventoryDragEvents.OnEndDrag -= HandleEndDrag;
    }

    // ----------------- INVENTORY OPERATIONS -----------------

    private void RotateItem()
    {
        if (Current.Item == null) return;

        RectTransform rt = Current.Obj.transform as RectTransform;
        rt.localEulerAngles = new Vector3(0f, 0f, Current.Item.Rotate());
    }

    private void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        InvContainer.gameObject.SetActive(!InvContainer.gameObject.activeSelf);
    }

    public void OpenInventory()
    {
        isInventoryOpen = true;
        InvContainer.gameObject.SetActive(true);
    }

    public void CloseInventory()
    {
        isInventoryOpen = false;
        CurrentChest = null;
        InvContainer.gameObject.SetActive(false);
    }

    public bool TryPlaceItem(CellPos anchorCell, GameObject itemObj)
    {
        ItemBase item = itemObj.GetComponent<ItemBase>();
        bool canPlace = Inventory.CanPlaceItem(item, anchorCell);

        if (canPlace)
        {
            PlaceItem(item, anchorCell, itemObj);
            return true;
        }

        return false;
    }

    private void PlaceItem(ItemBase item, CellPos anchorCell, GameObject itemObj)
    {
        UI.PlaceItem(itemObj, anchorCell);      // Snap item to grid
        Inventory.PlaceItem(item, anchorCell);  // Place in inventory
        CurrentChest?.TakeItem(item);           // Remove from chest if applicable
        GameObject.Find("Player").GetComponent<EntityEventDispatcher>().RegisterItemHandlers(item);
        Debug.Log($"Registered item handlers for {item.name}");
    }

    public (CellPos anchorCell, bool canPlace) CanPlaceDraggedItem(Vector2 anchorCanvasPos, ItemBase item)
    {
        CellPos anchorCell = ui.invGrid.GetNearestGridPosition(anchorCanvasPos);
        bool canPlace = Inventory.CanPlaceItem(item, anchorCell);
        return (anchorCell, canPlace);
    }

    private void RemoveItem(ItemBase item)
    {
        GameObject.Find("Player").GetComponent<EntityEventDispatcher>().UnregisterItemHandlers(item);
        inventory.RemoveItem(item);
    }

    // ----------------- CHEST OPERATIONS -----------------

    public void RegisterChest(Chest chest)
    {
        chest.OnChestOpened += HandleChestOpened;
        chest.OnChestClosed += HandleChestClosed;
    }

    private void HandleChestOpened(Chest chest)
    {
        CurrentChest = chest;
        ItemBase[] chestItems = UI.HandleChestOpened(chest);
        continueBtn.gameObject.SetActive(true);

        chest.SetItemIds(chestItems);

        OpenInventory();
    }

    public void HandleChestClosed()
    {
        UI.HandleChestClosed();
        CloseInventory();
    }

    public void OnContinueClicked()
    {
        if (inventory.ChestItemEquipped(CurrentChest))
        {
            CurrentChest.CloseChest();
            continueBtn.gameObject.SetActive(false);
            CloseInventory();
        }
    }

    // ----------------- DRAG & DROP HANDLERS -----------------

    public bool CanDrag(ItemBase item)
    {
        if (item.UIType == ItemUIType.Chest && CurrentChest.ItemsTaken >= 1) return false;
        return isInventoryOpen;
    }

    private void HandleBeginDrag(GameObject itemObj, PointerEventData data)
    {
        ItemBase item = itemObj.GetComponent<ItemBase>();
        if (!CanDrag(item)) return;

        Current.Obj = itemObj;

        UI.BeginDrag(data);
        RemoveItem(item);
    }

    private void HandleDrag(GameObject itemObj, PointerEventData data)
    {
        if (Current.Item == null || !CanDrag(Current.Item)) return;
        UI.Drag(data);
    }

    public void HandleEndDrag(GameObject itemObj, PointerEventData eventData)
    {
        if (Current.Item == null || !CanDrag(Current.Item)) return;

        Vector2 anchorCanvasPos = UI.GetCurrentItemCanvasPos();
        CellPos anchorCell = ui.invGrid.GetNearestGridPosition(anchorCanvasPos);

        bool placed = TryPlaceItem(anchorCell, itemObj);

        if (!placed && Current.Item.UIType == ItemUIType.Chest)
            UI.UnDragCurrentItemPos();

        if (placed)
            Current.Item.UIType = ItemUIType.Inventory;

        Current.Clear();
    }
}
