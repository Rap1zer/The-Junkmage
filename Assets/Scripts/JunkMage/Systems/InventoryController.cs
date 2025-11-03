using UnityEngine;

namespace JunkMage.Systems
{
    public sealed class InventoryController : MonoBehaviour
    {
        [SerializeField] private DebugInventoryRenderer debugRenderer;
        private InventoryPresenter presenter;

        void Awake()
        {
            presenter = GetComponent<InventoryPresenter>();
            debugRenderer.Init(presenter.Rows, presenter.Cols);
        }
        
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.V))
                presenter.ToggleInventory();

            if (DraggableItem.IsDragging && Input.GetKeyDown(KeyCode.R))
                presenter.RotateItem();

            if (debugRenderer != null && presenter?.GridData != null)
                debugRenderer.Refresh(presenter.GridData); // presenter exposes model.Data
        }

        public void RegisterChest(Chest chest) => presenter.RegisterChest(chest);
    }
}