using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JunkMage.Systems
{
    public class InventoryPresenter : MonoBehaviour
    { 
        // ---- Config / Scene refs ----
        [Header("UI Settings")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private Transform inventoryContainer;
        [SerializeField] private Transform chestContainer;
        [SerializeField] private Button continueBtn;

        // ---- State ----
        private Inventory model;
        private InventoryUI ui;

        private Chest currentChest;
        private SelectedItem Current { get; } = new();

        public bool IsInventoryOpen { get; private set; }
    
        public int Rows { get; } = 3;
        public int Cols { get; } = 4;

        // Optional: expose data for external debug renderers
        public ItemBase[,] GridData => model?.Data;
        
        private void Awake()
        {
            model = new Inventory(Cols, Rows);
            ui = new InventoryUI(canvas, cellPrefab, chestContainer, Rows, Cols);

            Transform gridContainer = inventoryContainer.Find("Inventory Grid");
            ui.DrawGrid(gridContainer, Rows, Cols);
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

        public void RotateItem()
        {
            if (Current.Item == null) return;

            RectTransform rt = Current.Item.transform as RectTransform;
            if (rt) rt.localEulerAngles = new Vector3(0f, 0f, Current.Item.Rotate());
        }

        public void ToggleInventory()
        {
            IsInventoryOpen = !IsInventoryOpen;
            inventoryContainer.gameObject.SetActive(IsInventoryOpen);
        }

        private void OpenInventory()
        {
            IsInventoryOpen = true;
            inventoryContainer.gameObject.SetActive(true);
        }

        private void CloseInventory()
        {
            IsInventoryOpen = false;
            currentChest = null;
            inventoryContainer.gameObject.SetActive(false);
        }

        private bool TryPlaceDraggedItem()
        {
            (CellPos anchorCell, bool canPlace) = CanPlaceDraggedItem();

            if (canPlace)
            {
                PlaceItem(Current.Item, anchorCell);
                return true;
            }

            return false;
        }

        private void PlaceItem(ItemBase item, CellPos anchorCell)
        {
            ui.PlaceItem(item, anchorCell);      // Snap item to grid
            model.TryPlaceItem(item, anchorCell);  // Place in model
            currentChest?.TakeItem(item);           // Remove from chest if applicable
            Current.Item.StorageType = StorageType.Inventory;
        }

        private (CellPos anchorCell, bool canPlace) CanPlaceDraggedItem()
        {
            Vector2 anchorCanvasPos = ui.GetCurrentItemCanvasPos(Current.Item);
            CellPos anchorCell = ui.invGrid.GetUnboundedCellPosition(anchorCanvasPos);
            bool canPlace = model.CanPlaceItem(Current.Item, anchorCell);
        
            return (anchorCell, canPlace);
        }

        private void TryRemoveItem(ItemBase item)
        {
            model.TryRemoveItem(item);
        }

        // ----------------- CHEST OPERATIONS -----------------

        public void RegisterChest(Chest chest)
        {
            chest.OnChestOpened += HandleChestOpened;
            chest.OnChestClosed += HandleChestClosed;
        }

        private void HandleChestOpened(Chest chest)
        {
            currentChest = chest;
            ItemBase[] chestItems = ui.HandleChestOpened(chest);
            continueBtn.gameObject.SetActive(true);

            chest.SetItemIds(chestItems);

            OpenInventory();
        }

        public void HandleChestClosed()
        {
            ui.HandleChestClosed();
            CloseInventory();
        }

        public void OnContinueClicked()
        {
            if (model.IsChestItemEquipped(currentChest))
            {
                currentChest.CloseChest();
                continueBtn.gameObject.SetActive(false);
                CloseInventory();
            }
        }
    
        private bool TryReturnItemToChest()
        {
            if (currentChest == null) return false;
            if (Current.Item == null) return false;

            bool snapped = ui.TryReturnItemToChest(Current.Item);
            if (snapped)
            {
                currentChest.UndoTakeItem(Current.Item);
                Current.Item.StorageType = StorageType.Chest;
                return true;
            }

            return false;
        }


        // ----------------- DRAG & DROP HANDLERS -----------------
        CellPos startCellPos;
    
        public bool CanDrag(ItemBase item)
        {
            if (item.StorageType == StorageType.Chest && currentChest != null && currentChest.ItemsTaken >= 1) return false;
            return IsInventoryOpen;
        }

        private void HandleBeginDrag(GameObject itemObj, PointerEventData data)
        {
            ItemBase item = itemObj.GetComponent<ItemBase>();
            if (!CanDrag(item)) return;

            Current.Item = item;
            ui.BeginDrag(data);
            startCellPos = item.AnchorGridPos;
        
            TryRemoveItem(item);
        }

        private void HandleDrag(GameObject itemObj, PointerEventData data)
        {
            if (Current.Item == null || !CanDrag(Current.Item)) return;
            (CellPos anchorCell, bool canPlace) = CanPlaceDraggedItem();
            ui.Drag(Current.Item, anchorCell, canPlace);
        }

        public void HandleEndDrag(GameObject itemObj, PointerEventData eventData)
        {
            if (Current.Item == null || !CanDrag(Current.Item)) return;
            
            ui.EndDrag();

            bool itemPlaced = TryPlaceDraggedItem();
            if (!itemPlaced)
            {
                bool returnedToChest = TryReturnItemToChest();

                if (!returnedToChest)
                {
                    ui.UnDragCurrentItemPos(Current.Item);
                    if (Current.Item.StorageType == StorageType.Inventory) model.TryPlaceItem(Current.Item, startCellPos);
                }
            }

            Current.Clear();
        }
    }
}
