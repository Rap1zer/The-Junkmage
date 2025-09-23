using System;
using UnityEngine;

public interface IItem
{
    public ItemData ItemData { get; }
    public ItemUIType UIType { get; set; }

    public Guid Id { get; }

    public bool[,] CurrentShape { get; }

    public Vector2Int AnchorGridPos { get; }
    public Vector3 AnchorPos { get; }

    public int RotationState { get; }
    public float Rotate();
    
}