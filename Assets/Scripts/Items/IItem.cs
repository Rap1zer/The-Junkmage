using UnityEngine;

public interface IItem
{
    public ItemData ItemData { get; }

    public Vector2Int CurrentShape { get; }
    public Vector3 AnchorPos { get; }

    public int RotationState { get; }
    public float Rotate();
    
}