using UnityEngine;
using UnityEngine.UI;

public class InventoryGrid
{
    private GameObject[,] cells;

    float cellSize = 100f;
    float margin = 10f;

    Inventory inventory = Inventory.Instance;

    public void DrawGrid(Transform gridContainer, GameObject cellPrefab)
    {
        cells = new GameObject[inventory.width, inventory.height];

        float offsetW = ((cellSize * inventory.width) + (margin * (inventory.width - 1))) / 2;
        float offsetH = ((cellSize * inventory.width) + (margin * (inventory.height - 1))) / 2;

        for (int y = 0; y < inventory.height; y++)
        {
            for (int x = 0; x < inventory.width; x++)
            {
                GameObject gridCell = Object.Instantiate(cellPrefab, gridContainer);
                cells[x, y] = gridCell;

                RectTransform rt = gridCell.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector3(((cellSize + margin) * x) - offsetW,
                ((cellSize + margin) * y) - offsetH, 0);
            }
        }
    }

    public Vector2Int GetNearestGridPosition(Vector2 mousePos)
    {
        int width = cells.GetLength(0);
        int height = cells.GetLength(1);

        // Calculate offset (same as in DrawGrid)
        float offsetW = ((cellSize * width) + (margin * (width - 1))) / 2;
        float offsetH = ((cellSize * height) + (margin * (height - 1))) / 2;

        float localX = mousePos.x + offsetW;
        float localY = mousePos.y + offsetH;

        int gridX = Mathf.Clamp(Mathf.RoundToInt(localX / (cellSize + margin)), 0, width - 1);
        int gridY = Mathf.Clamp(Mathf.RoundToInt(localY / (cellSize + margin)), 0, height - 1);

        return new Vector2Int(gridX, gridY);
    }

    public bool CanPlaceItem(Vector2Int shape, Vector2Int nearestCell)
    {
        for (int y = 0; y < shape.y; y++)
        {
            for (int x = 0; x < shape.x; x++)
            {
                int cellX = nearestCell.x + x;
                int cellY = nearestCell.y + y;

                if (cellX >= cells.GetLength(0) || cellY >= cells.GetLength(1))
                    return false;

                // Optional: check if the cell is already occupied
                // if (inventory.IsCellOccupied(cellX, cellY)) return false;
            }
        }

        return true;
    }

    public void HighlightCells(Vector2Int nearestCell, Vector2Int shape, bool canPlace)
    {
        for (int y = 0; y < shape.y; y++)
        {
            for (int x = 0; x < shape.x; x++)
            {
                Vector2Int cellPos = new Vector2Int(nearestCell.x + x, nearestCell.y + y);
                if (canPlace)
                    cells[cellPos.x, cellPos.y].GetComponent<Image>().color = Color.green;
                else
                {
                    if (cellPos.x < inventory.width && cellPos.y < inventory.height)
                        cells[cellPos.x, cellPos.y].GetComponent<Image>().color = Color.red;
                }

            }
        }
    }

    public void ClearHighlights()
    {
        foreach (var cell in cells)
        {
            cell.GetComponent<Image>().color = Color.white;
        }
    }
}
