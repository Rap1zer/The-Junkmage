using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public GameObject CurrentObj { get; set; }
    public Vector2Int? CurrentIndex { get; set; }
    public ItemUIType? CurrentType { get; set; }
    public IItem CurrentItem => CurrentObj != null ? CurrentObj.GetComponent<IItem>() : null;

    public bool isInventoryOpen { get; set; } = false;
    private bool isChestOpen { get; set; } = false;

    [Header("UI Settings")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private Transform InvContainer;
    [SerializeField] private Transform ChestContainer;
    [SerializeField] private Button continueBtn;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Initialize logic + UI
        inventory = new Inventory(Width, Height);
        ui = new InventoryUI(canvas, cellPrefab, InvContainer, ChestContainer);
    }

    void Start()
    {
        UI.DrawGrid();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V) && !isChestOpen) ToggleInventory();

        if (DraggableItem.IsDragging && Input.GetKeyDown(KeyCode.R))
            RotateItem();
    }

    private void RotateItem()
    {
        IItem item = CurrentObj.GetComponent<IItem>();
        RectTransform rt = CurrentObj.transform as RectTransform;

        rt.localEulerAngles = new Vector3(0f, 0f, item.Rotate());
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

    public bool TryPlaceItem(Vector2Int startingCell, GameObject itemObj)
    {
        IItem item = itemObj.GetComponent<IItem>();
        bool canPlace = InventoryGrid.CanPlaceItem(item.CurrentShape, startingCell);

        if (canPlace)
        {
            PlaceItem(item, startingCell, itemObj);
            return true;
        }

        return false;
    }

    private void PlaceItem(IItem item, Vector2Int startingCell, GameObject itemObj)
    {
        // Snap item's position to grid
        UI.PlaceItem(itemObj, startingCell);
        // Place item in inventory
        Inventory.PlaceItem(item, startingCell);

        CurrentChest.TakeItem(CurrentItem);
    }

    // Calculate if dragged item can be placed on grid in its current position
    public (Vector2Int nearestCell, bool canPlace, Vector2Int itemSize) CalculateDragPlacement()
    {
        Vector2 itemPos = UI.GetCurrentItemCanvasPos();
        Vector2Int nearestCell = InventoryGrid.GetNearestGridPosition(itemPos);

        Vector2Int itemSize = CurrentObj.GetComponent<IItem>().CurrentShape;
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
        IItem[] chestItems = UI.HandleChestOpened(chest);
        continueBtn.gameObject.SetActive(true);

        chest.SetItemIds(chestItems);
    }

    public void OnContinueClicked()
    {
        Debug.Log("On continue clicked");
        Debug.Log(CurrentChest.ItemsTaken);

        if (CurrentChest.ItemsTaken > 0)
        {
            CurrentChest.CloseChest();
            continueBtn.gameObject.SetActive(false);
            CloseInventory();
        }
    }

    public bool CanDrag(IItem item)
    {
        // Cannot drag another chest item if one is already equipped
        if (item.UIType == ItemUIType.Chest && CurrentChest.ItemsTaken >= 1) return false;
        return isInventoryOpen;
    }

    public void HandleChestClosed()
    {
       UI.HandleChestClosed();
    }
}
