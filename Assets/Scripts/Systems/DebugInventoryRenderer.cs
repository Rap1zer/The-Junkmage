using UnityEngine;
using UnityEngine.UI;

public class DebugInventoryRenderer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform debugContainer;   // Assign your "Debug Inventory" GameObject
    [SerializeField] private GameObject debugCellPrefab; // A simple cube or UI Image

    private GameObject[,] debugCells;

    public void Init(int width, int height)
    {
        // Clear previous grid if re-initialized
        foreach (Transform child in debugContainer)
            Destroy(child.gameObject);

        debugCells = new GameObject[width, height];

        float cellSize = debugCellPrefab.GetComponent<RectTransform>().rect.width;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject cell = Instantiate(debugCellPrefab, debugContainer);
                cell.name = $"DebugCell_{x}_{y}";

                // Bottom-up layout: (0,0) at bottom-left
                cell.transform.localPosition = new Vector3(x * cellSize, y * cellSize, 0);

                debugCells[x, y] = cell;
            }
        }
    }

    public void Refresh(IItem[,] inventoryData)
    {
        int width = inventoryData.GetLength(0);
        int height = inventoryData.GetLength(1);

        for (int y = 0; y < height; y++) // rows first
        {
            for (int x = 0; x < width; x++) // columns second
            {
                Image renderer = debugCells[x, y].GetComponent<Image>();
                if (renderer == null) continue;

                if (inventoryData[y, x] == null) // note the swapped indices
                    renderer.color = Color.gray; // Empty
                else if (inventoryData[y, x].UIType == ItemUIType.Inventory)
                    renderer.color = Color.green; // Inventory item
                else if (inventoryData[y, x].UIType == ItemUIType.Chest)
                    renderer.color = Color.blue; // Chest item
                else
                    renderer.color = Color.magenta; // Unknown type
            }
        }
    }
}
