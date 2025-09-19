using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public static class InventoryGrid
{
    static public GameObject[,] CellObjs;

    static float cellSize = 100f;
    static float margin = 10f;

    public static void DrawGrid(Transform gridContainer, GameObject cellPrefab)
    {
        int width = InventoryManager.Width;
        int height = InventoryManager.Height;
        CellObjs = new GameObject[width, height];

        float offsetW = ((cellSize * width) + (margin * (width - 1))) / 2;
        float offsetH = ((cellSize * width) + (margin * (height - 1))) / 2;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject gridCell = Object.Instantiate(cellPrefab, gridContainer);
                CellObjs[x, y] = gridCell;

                RectTransform rt = gridCell.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector3(((cellSize + margin) * x) - offsetW,
                ((cellSize + margin) * y) - offsetH, 0);
            }
        }
    }

    public static Vector2Int GetNearestGridPosition(Vector2 mousePos)
    {
        int width = CellObjs.GetLength(0);
        int height = CellObjs.GetLength(1);

        // Calculate offset (same as in DrawGrid)
        float offsetW = ((cellSize * width) + (margin * (width - 1))) / 2;
        float offsetH = ((cellSize * height) + (margin * (height - 1))) / 2;

        float localX = mousePos.x + offsetW;
        float localY = mousePos.y + offsetH;

        int gridX = Mathf.Clamp(Mathf.RoundToInt(localX / (cellSize + margin)), 0, width - 1);
        int gridY = Mathf.Clamp(Mathf.RoundToInt(localY / (cellSize + margin)), 0, height - 1);

        return new Vector2Int(gridX, gridY);
    }

    public static bool CanPlaceItem(Vector2Int shape, Vector2Int nearestCell)
    {
        for (int y = 0; y < shape.y; y++)
        {
            for (int x = 0; x < shape.x; x++)
            {
                int cellX = nearestCell.x + x;
                int cellY = nearestCell.y + y;

                if (cellX >= CellObjs.GetLength(0) || cellY >= CellObjs.GetLength(1))
                    return false;

                // Optional: check if the cell is already occupied
                // if (inventory.IsCellOccupied(cellX, cellY)) return false;
            }
        }

        return true;
    }

    public static void HighlightCells(Vector2Int nearestCell, Vector2Int shape, bool canPlace)
    {
        int width = InventoryManager.Width;
        int height = InventoryManager.Height;
        for (int y = 0; y < shape.y; y++)
        {
            for (int x = 0; x < shape.x; x++)
            {
                Vector2Int cellPos = new Vector2Int(nearestCell.x + x, nearestCell.y + y);
                if (canPlace)
                    CellObjs[cellPos.x, cellPos.y].GetComponent<Image>().color = Color.green;
                else
                {
                    if (cellPos.x < width && cellPos.y < height)
                        CellObjs[cellPos.x, cellPos.y].GetComponent<Image>().color = Color.red;
                }

            }
        }
    }

    public static void ClearHighlights()
    {
        foreach (var cell in CellObjs)
        {
            cell.GetComponent<Image>().color = Color.white;
        }
    }
}
