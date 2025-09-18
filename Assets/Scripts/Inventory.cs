using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    public int width = 3;
    public int height = 3;
    public bool isInventoryOpen = false;
    private int[,] data;

    [Header("UI Settings")]
    public Canvas canvas;
    Transform gridContainer;
    public GameObject cell;


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

        data = new int[width, height];
        DrawGrid();
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

    void DrawGrid()
    {
        float cellLength = 100;
        float margin = 20;
        float offsetW = ((cellLength * width) + (margin * (width - 1))) / 2;
        float offsetH = ((cellLength * width) + (margin * (height - 1))) / 2;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject gridCell = Instantiate(cell, gridContainer);

                RectTransform rt = gridCell.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector3(((cellLength + margin) * x) - offsetW,
                ((cellLength + margin) * y) - offsetH, 0);
            }
        }
    }
}
