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
        int rows = InventoryManager.Height;
        int cols = InventoryManager.Width;
        CellObjs = new GameObject[rows, cols]; // [row, col]

        float offsetW = ((CellSize * cols) + (Margin * (cols - 1))) / 2 - (CellSize / 2);
        float offsetH = ((CellSize * rows) + (Margin * (rows - 1))) / 2 - (CellSize / 2);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                GameObject gridCell = Object.Instantiate(cellPrefab, InvContainer);
                CellObjs[row, col] = gridCell;

                RectTransform rt = gridCell.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector3(
                    ((CellSize + Margin) * col) - offsetW,
                    ((CellSize + Margin) * row) - offsetH,
                    0
                );
            }
        }
    }

    public static Vector2Int GetNearestGridPosition(Vector2 anchorCanvasPos)
    {
        int rows = CellObjs.GetLength(0);
        int cols = CellObjs.GetLength(1);

        float offsetW = ((CellSize * cols) + (Margin * (cols - 1))) / 2 - (CellSize / 2);
        float offsetH = ((CellSize * rows) + (Margin * (rows - 1))) / 2 - (CellSize / 2);

        float localX = anchorCanvasPos.x + offsetW;
        float localY = anchorCanvasPos.y + offsetH;

        int col = Mathf.Clamp(Mathf.RoundToInt(localX / (CellSize + Margin)), 0, cols - 1);
        int row = Mathf.Clamp(Mathf.RoundToInt(localY / (CellSize + Margin)), 0, rows - 1);

        // row first, col second
        return new Vector2Int(row, col);
    }

    public static void HighlightCells(Vector2Int nearestCell, bool[,] shape, bool canPlace)
    {
        int rows = InventoryManager.Height;
        int cols = InventoryManager.Width;

        for (int r = 0; r < shape.GetLength(0); r++)
        {
            for (int c = 0; c < shape.GetLength(1); c++)
            {
                if (!shape[r, c]) continue;

                int row = nearestCell.x + r;
                int col = nearestCell.y + c;

                if (row >= rows || col >= cols) continue;

                Image image = CellObjs[row, col].GetComponent<Image>();
                image.color = canPlace ? Color.green : Color.red;
            }
        }
    }

    public static void OccupyCells(Vector2Int cell, bool[,] shape)
    {
        int rows = InventoryManager.Height;
        int cols = InventoryManager.Width;

        for (int r = 0; r < shape.GetLength(0); r++)
        {
            for (int c = 0; c < shape.GetLength(1); c++)
            {
                if (!shape[r, c]) continue;

                int row = cell.x + r;
                int col = cell.y + c;

                if (row >= rows || col >= cols) continue;

                CellObjs[row, col].GetComponent<Image>().color = Color.yellow;
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
