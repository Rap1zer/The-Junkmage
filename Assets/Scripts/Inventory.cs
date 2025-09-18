using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int width = 3;
    public int height = 3;
    private int[,] inventory;

    [Header("UI Settings")]
    public Canvas canvas;
    Transform gridContainer;
    public GameObject cell;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gridContainer = canvas.transform.Find("Inventory Grid");

        inventory = new int[width, height];
        DrawGrid();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V)) ToggleInventory();
    }

    private void ToggleInventory()
    {
        gridContainer.gameObject.SetActive(!gridContainer.gameObject.activeSelf);
    }

    public void OpenInventory()
    {
         gridContainer.gameObject.SetActive(true);
    }
    
    public void CloseInventory()
    {
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
