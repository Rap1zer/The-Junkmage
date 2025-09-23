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
    public Sprite icon;
    public BoolRow[] shape; // Replaces bool[,]
    public string description;
    public GameObject prefab;

    // Helper to convert to 2D array
    public bool[,] Get2DShape()
    {
        if (shape == null || shape.Length == 0) return new bool[0, 0];
        int rows = shape.Length;
        int cols = shape[0].values.Length;
        bool[,] result = new bool[rows, cols];
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                result[r, c] = shape[r].values[c];
        return result;
    }
}
