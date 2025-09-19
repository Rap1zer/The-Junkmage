using UnityEngine;

public interface IItem
{
    public ItemData ItemData { get; }

    public Vector2Int CurrentShape { get; }
    public Vector2 AnchorLocalPos { get; }
    public Vector3 AnchorWorldPos { get; }

    public int RotationState { get; }
    public float Rotate();
    
}