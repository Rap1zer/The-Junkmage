using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public Inventory Inventory { get; private set; }
    public Chest CurrentChest { get; private set; }

    public static int Width { get; private set; } = 3;
    public static int Height { get; private set; } = 3;

    [Header("UI Settings")]
    [SerializeField] private Transform gridContainer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Inventory = new Inventory(gridContainer, Width, Height);
    }

    void Start()
    {
        InventoryUI.Instance.DrawGrid();
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V)) Inventory.ToggleInventory();
    }
}
