using System;
using System.Collections.Generic;
using UnityEngine;

public interface IItem
{
    public ItemData ItemData { get; }
    public ItemUIType UIType { get; set; }

    public Guid Id { get; }

    public bool[,] CurrentShape { get; }
    public bool[,] CurrentStars { get; }

    IEnumerable<CellPos> GetOccupiedCells(CellPos anchor);
    IEnumerable<CellPos> GetStarCells(CellPos anchor);
    
    public CellPos AnchorGridPos { get; set; }
    public Vector3 AnchorPos { get; }

    public int RotationState { get; }
    public float Rotate();
    
}