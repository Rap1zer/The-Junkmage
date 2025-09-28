using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public static class InventoryGrid
{
    public static GameObject[,] CellObjs;

    public static float CellSize = 100f;
    public static float Margin = 10f;

    public static void DrawGrid(Transform InvContainer, GameObject cellPrefab)
    {
        int width = InventoryManager.Width;
        int height = InventoryManager.Height;
        CellObjs = new GameObject[width, height]; // x = columns, y = rows

        float offsetW = ((CellSize * width) + (Margin * (width - 1))) / 2 - (CellSize / 2);
        float offsetH = ((CellSize * height) + (Margin * (height - 1))) / 2 - (CellSize / 2);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject gridCell = Object.Instantiate(cellPrefab, InvContainer);
                CellObjs[x, y] = gridCell;

                RectTransform rt = gridCell.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector3(
                    ((CellSize + Margin) * x) - offsetW,
                    ((CellSize + Margin) * y) - offsetH,
                    0
                );
            }
        }
    }

    public static Vector2Int GetNearestGridPosition(Vector2 anchorCanvasPos)
    {
        int width = CellObjs.GetLength(0);
        int height = CellObjs.GetLength(1);

        float offsetW = ((CellSize * width) + (Margin * (width - 1))) / 2 - (CellSize / 2);
        float offsetH = ((CellSize * height) + (Margin * (height - 1))) / 2 - (CellSize / 2);

        float localX = anchorCanvasPos.x + offsetW;
        float localY = anchorCanvasPos.y + offsetH;

        int gridX = Mathf.Clamp(Mathf.RoundToInt(localX / (CellSize + Margin)), 0, width - 1);
        int gridY = Mathf.Clamp(Mathf.RoundToInt(localY / (CellSize + Margin)), 0, height - 1);

        return new Vector2Int(gridX, gridY);
    }

    public static void HighlightCells(Vector2Int nearestCell, bool[,] shape, bool canPlace)
    {
        int width = InventoryManager.Width;
        int height = InventoryManager.Height;

        for (int y = 0; y < shape.GetLength(0); y++)
        {
            for (int x = 0; x < shape.GetLength(1); x++)
            {
                if (!shape[y, x]) continue;

                int cellX = nearestCell.x + x;
                int cellY = nearestCell.y + y;

                if (cellX >= width || cellY >= height) continue; // Fixed bounds check

                Image image = CellObjs[cellX, cellY].GetComponent<Image>();
                image.color = canPlace ? Color.green : Color.red;
            }
        }
    }

    public static void OccupyCells(Vector2Int cell, bool[,] shape)
    {
        int width = InventoryManager.Width;
        int height = InventoryManager.Height;

        for (int y = 0; y < shape.GetLength(0); y++)
        {
            for (int x = 0; x < shape.GetLength(1); x++)
            {
                if (!shape[y, x]) continue;

                int cellX = cell.x + x;
                int cellY = cell.y + y;

                if (cellX >= width || cellY >= height) continue;

                CellObjs[cellX, cellY].GetComponent<Image>().color = Color.yellow;
            }
        }
    }

    public static void ClearHighlights()
    {
        if (CellObjs == null) return;

        foreach (var cell in CellObjs)
        {
            Image image = cell.GetComponent<Image>();
            if (image.color != Color.yellow) image.color = Color.white;
        }
    }
}
