using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    public int width = 3;
    public int height = 3;
    public bool isInventoryOpen = false;

    public IItem[,] InventoryData { get; set; }
    public ItemData[] ChestItemsData { get; set; }

    [Header("UI Settings")]
    [SerializeField] private Canvas canvas;
    Transform gridContainer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        gridContainer = canvas.transform.Find("Inventory Grid");

        InventoryData = new IItem[width, height];
        ChestItemsData = new ItemData[3];
        InventoryUI.Instance.DrawGrid(gridContainer);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V)) ToggleInventory();
    }

    private void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        gridContainer.gameObject.SetActive(!gridContainer.gameObject.activeSelf);
    }

    public void OpenInventory()
    {
        isInventoryOpen = true;
         gridContainer.gameObject.SetActive(true);
    }

    public void CloseInventory()
    {
        isInventoryOpen = false;
        gridContainer.gameObject.SetActive(false);
    }
}
