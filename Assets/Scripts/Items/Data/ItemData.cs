using UnityEngine;
using System;

[System.Serializable]
public class BoolRow
{
    public bool[] values;
}

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    // ==============================
    // Basic Item Info
    // ==============================
    [Header("General Info")]
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
    public GameObject prefab;

    // ==============================
    // Item Shape
    // ==============================
    [Header("Item Shape")]
    public BoolRow[] shape;

    // ==============================
    // Stars (Overlay System)
    // ==============================
    [Header("Star Placement")]
    public int extraRowsHalf = 0;
    public int extraColsHalf = 0;
    public BoolRow[] stars;

    // ==============================
    // Unique Identifier
    // ==============================
    [SerializeField, HideInInspector] private string guid;
    public string Guid => guid;

    // ==============================
    // Unity Lifecycle
    // ==============================
    private void OnValidate()
    {
        EnsureGuid();
        NormalizeShape();
        NormalizeStars();
        EnforceStarRules();
    }

    // ==============================
    // GUID Handling
    // ==============================
    private void EnsureGuid()
    {
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(guid))
        {
            guid = System.Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }

    // ==============================
    // Shape Helpers
    // ==============================
    private void NormalizeShape()
    {
        if (shape == null || shape.Length == 0) return;

        // Ensure first row is valid
        if (shape[0].values == null)
            shape[0].values = new bool[1];

        int rows = shape.Length;
        int cols = shape[0].values.Length;

        // Normalize rows
        for (int r = 0; r < rows; r++)
        {
            if (shape[r] == null)
                shape[r] = new BoolRow();

            if (shape[r].values == null)
                shape[r].values = new bool[cols];
            else if (shape[r].values.Length != cols)
                Array.Resize(ref shape[r].values, cols);
        }
    }

    // ==============================
    // Stars Helpers
    // ==============================
    private void NormalizeStars()
    {
        int rows = shape != null ? shape.Length : 0;
        int cols = (rows > 0 && shape[0] != null && shape[0].values != null)
            ? shape[0].values.Length
            : 0;

        int starRows = extraRowsHalf > 0 || extraColsHalf > 0 ? rows + extraRowsHalf * 2 : 0;
        int starCols = extraRowsHalf > 0 || extraColsHalf > 0 ? cols + extraColsHalf * 2 : 0;

        // Resize stars array
        if (stars == null || stars.Length != starRows)
            Array.Resize(ref stars, starRows);

        // Normalize each row
        for (int r = 0; r < starRows; r++)
        {
            if (stars[r] == null)
                stars[r] = new BoolRow();

            if (stars[r].values == null || stars[r].values.Length != starCols)
                Array.Resize(ref stars[r].values, starCols);
        }
    }

    private void EnforceStarRules()
    {
        if (stars == null) return;

        int starRows = stars.Length;
        int starCols = starRows > 0 && stars[0] != null ? stars[0].values.Length : 0;

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

    // ==============================
    // Public API
    // ==============================
    public bool[,] Get2DBoolArray(BoolRow[] array)
    {
        if (array == null || array.Length == 0) return new bool[0, 0];

        int rows = array.Length;
        int cols = array[0].values.Length;
        bool[,] result = new bool[rows, cols];

        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                result[r, c] = array[r].values[c];

        return result;
    }

    // ==============================
    // Internal Helpers
    // ==============================
    private int MapStarRowToShape(int starRow)
    {
        if (shape == null || shape.Length == 0) return -1;

        int row = starRow - extraRowsHalf;
        return (row >= 0 && row < shape.Length) ? row : -1;
    }

    private int MapStarColToShape(int starCol)
    {
        if (shape == null || shape.Length == 0) return -1;
        if (shape[0].values == null) return -1;

        int col = starCol - extraColsHalf;
        return (col >= 0 && col < shape[0].values.Length) ? col : -1;
    }

    private bool IsStarOverlappingItem(int r, int c)
    {
        int shapeR = MapStarRowToShape(r);
        int shapeC = MapStarColToShape(c);

        if (shapeR == -1 || shapeC == -1) return false; // outside
        return stars[r].values[c] && shape[shapeR].values[shapeC];
    }
}
