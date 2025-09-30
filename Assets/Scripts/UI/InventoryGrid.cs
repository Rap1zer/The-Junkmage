using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InventoryGrid
{
    public GameObject[,] CellObjs { get; private set; }

    private readonly float cellSize;
    private readonly float margin;
    private readonly int rows;
    private readonly int cols;

    public InventoryGrid(int rows, int cols, float cellSize, float margin)
    {
        this.rows = rows;
        this.cols = cols;
        this.cellSize = cellSize;
        this.margin = margin;

        CellObjs = new GameObject[rows, cols];
    }

    // -----------------------
    // Grid math helpers
    // -----------------------

    private Vector2 CalculateGridOffset()
    {
        float offsetW = ((cellSize * cols) + (margin * (cols - 1))) / 2 - (cellSize / 2);
        float offsetH = ((cellSize * rows) + (margin * (rows - 1))) / 2 - (cellSize / 2);
        return new Vector2(offsetW, offsetH);
    }

    public Vector2 GridToLocal(CellPos pos)
    {
        Vector2 offset = CalculateGridOffset();
        return new Vector2(
            ((cellSize + margin) * pos.Col) - offset.x,
            ((cellSize + margin) * pos.Row) - offset.y
        );
    }

    public CellPos LocalToGrid(Vector2 local)
    {
        Vector2 offset = CalculateGridOffset();
        float localX = local.x + offset.x;
        float localY = local.y + offset.y;

        int col = Mathf.Clamp(Mathf.RoundToInt(localX / (cellSize + margin)), 0, cols - 1);
        int row = Mathf.Clamp(Mathf.RoundToInt(localY / (cellSize + margin)), 0, rows - 1);

        return new CellPos(row, col);
    }

    // -----------------------
    // UI: Grid rendering
    // -----------------------

    public void DrawGrid(Transform InvContainer, GameObject cellPrefab)
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                GameObject gridCell = Object.Instantiate(cellPrefab, InvContainer);
                CellObjs[row, col] = gridCell;

                RectTransform rt = gridCell.GetComponent<RectTransform>();
                rt.anchoredPosition = GridToLocal(new CellPos(row, col));
            }
        }
    }

    public CellPos GetNearestGridPosition(Vector2 localPosition)
    {
        Vector2 offset = CalculateGridOffset();

        float localX = localPosition.x + offset.x;
        float localY = localPosition.y + offset.y;

        int col = Mathf.Clamp(Mathf.RoundToInt(localX / (cellSize + margin)), 0, cols - 1);
        int row = Mathf.Clamp(Mathf.RoundToInt(localY / (cellSize + margin)), 0, rows - 1);

        return new CellPos(row, col);
    }

    // -----------------------
    // UI: Cell highlighting
    // -----------------------

    private void SetCellColor(CellPos pos, Color color)
    {
        if (pos.Row < 0 || pos.Row >= rows) return;
        if (pos.Col < 0 || pos.Col >= cols) return;

        Image image = CellObjs[pos.Row, pos.Col].GetComponent<Image>();
        image.color = color;
    }

    public void HighlightCells(CellPos anchorCell, IItem item, bool canPlace)
    {
        foreach (var pos in item.GetOccupiedCells(anchorCell))
        {
            SetCellColor(pos, canPlace ? Color.green : Color.red);
        }

        if (canPlace)
        {
            foreach (var pos in item.GetStarCells(anchorCell))
            {
                SetCellColor(pos, Color.yellow);
            }
        }
    }

    public void ClearHighlights()
    {
        foreach (var cell in CellObjs)
        {
            cell.GetComponent<Image>().color = Color.white;
        }
    }
}
