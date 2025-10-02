using UnityEngine;
using UnityEngine.UI;

public class DebugInventoryRenderer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform debugContainer;
    [SerializeField] private GameObject debugCellPrefab;

    private GameObject[,] debugCells;

    public void Init(int rows, int cols)
    {
        foreach (Transform child in debugContainer)
            Destroy(child.gameObject);

        debugCells = new GameObject[rows, cols];

        float cellSize = debugCellPrefab.GetComponent<RectTransform>().rect.width;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                GameObject cell = Instantiate(debugCellPrefab, debugContainer);
                cell.name = $"DebugCell_{row}_{col}";

                // Bottom-left origin
                cell.transform.localPosition = new Vector3(col * cellSize, row * cellSize, 0);

                debugCells[row, col] = cell; // match row,col indexing
            }
        }
    }

    public void Refresh(ItemBase[,] inventoryData)
    {
        int rows = inventoryData.GetLength(0);
        int cols = inventoryData.GetLength(1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Image renderer = debugCells[row, col].GetComponent<Image>();
                if (renderer == null) continue;

                var item = inventoryData[row, col];

                if (item == null)
                    renderer.color = Color.gray;
                else if (item.UIType == ItemUIType.Inventory)
                    renderer.color = Color.green;
                else if (item.UIType == ItemUIType.Chest)
                    renderer.color = Color.blue;
                else
                    renderer.color = Color.magenta;
            }
        }
    }
}

