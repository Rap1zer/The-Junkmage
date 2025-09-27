using UnityEngine;

[System.Serializable]
public class BoolRow
{
    public bool[] values;
}

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite icon;
    public GameObject prefab;

    [Header("Item Shape")]
    public BoolRow[] shape;

    [Header("Star Placement")]
    public int extraRowsHalf = 0;
    public int extraColsHalf = 0;
    public BoolRow[] stars;

    // Helper to convert to 2D array
    public bool[,] Get2DShape()
    {
        if (shape == null || shape.Length == 0) return new bool[0, 0];

        int rows = shape.Length;
        int cols = shape[0].values.Length;
        bool[,] result = new bool[rows, cols];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                result[r, c] = shape[r].values[c];
            }
        }

        return result;
    }

    private void OnValidate()
    {
        // If shape is null, nothing to enforce
        if (shape == null || shape.Length == 0) return;

        // If the first row's values are null, initialize to a default length (e.g., 1)
        if (shape[0].values == null) shape[0].values = new bool[1];

        int rows = shape.Length;
        int cols = shape[0].values.Length; // The length of the first column determines the length of the rest

        // Normalize shape rows to ensure no jaggedness
        for (int r = 0; r < rows; r++)
        {
            if (shape[r] == null) shape[r] = new BoolRow();

            if (shape[r].values == null) shape[r].values = new bool[cols];
            else if (shape[r].values.Length != cols)
            {
                System.Array.Resize(ref shape[r].values, cols);
            }
        }

        int starRows = extraRowsHalf > 0 || extraColsHalf > 0 ? rows + extraRowsHalf * 2 : 0;
        int starCols = extraRowsHalf > 0 || extraColsHalf > 0 ? cols + extraColsHalf * 2 : 0;

        // Initialize stars if null
        if (stars == null || stars.Length != starRows)
        {
            System.Array.Resize(ref stars, starRows);
        }

        // Ensure each row in stars matches the column length of the corresponding row in shape
        for (int r = 0; r < starRows; r++)
        {
            if (stars[r] == null) stars[r] = new BoolRow();

            if (stars[r].values == null || stars[r].values.Length != starCols)
            {
                System.Array.Resize(ref stars[r].values, starCols);
            }
        }

        // Ensure no star cells overlap item grid
        for (int r = 0; r < starRows; r++)
        {
            for (int c = 0; c < starCols; c++)
            {
                if (IsStarOverlappingItem(r, c))
                {
                    Debug.LogWarning($"[ItemData] Star cell at row {r}, column {c} overlaps the item grid. Automatically reset to false.");
                    stars[r].values[c] = false;
                }
            }
        }
    }

    // Returns the index in shape for a given overlay row, or -1 if outside
    private int MapStarRowToShape(int starRow)
    {
        // If shape is null, nothing to enforce
        if (shape == null || shape.Length == 0) return -1;

        int row = starRow - extraRowsHalf;
        return (row >= 0 && row < shape.Length) ? row : -1;
    }

    // Same for columns
    private int MapStarColToShape(int starCol)
    {
        // If shape is null, nothing to enforce
        if (shape == null || shape.Length == 0) return -1;

        // If the first row's values are null, initialize to a default length (e.g., 1)
        if (shape[0].values == null) return -1;

        int col = starCol - extraColsHalf;
        return (col >= 0 && col < shape[0].values.Length) ? col : -1;
    }
    
    private bool IsStarOverlappingItem(int r, int c)
    {
        int shapeR = MapStarRowToShape(r);
        int shapeC = MapStarColToShape(c);

        // Outside item grid?
        if (shapeR == -1 || shapeC == -1) return false;

        // Inside item grid: true if the shape cell is false
        return stars[r].values[c] && shape[shapeR].values[shapeC];
    }
}
