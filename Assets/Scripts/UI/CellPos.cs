using UnityEngine;

public readonly struct CellPos
{
    public int Row { get; }
    public int Col { get; }

    public CellPos(int row, int col)
    {
        Row = row;
        Col = col;
    }

    public override string ToString() => $"({Row}, {Col})";

    // Convert to Vector2Int 
    public Vector2Int ToVector2Int() => new Vector2Int(Col, Row);

    // Equality support
    public override bool Equals(object obj) => obj is CellPos other && Row == other.Row && Col == other.Col;
    public override int GetHashCode() => (Row, Col).GetHashCode();

    public static bool operator ==(CellPos a, CellPos b) => a.Row == b.Row && a.Col == b.Col;
    public static bool operator !=(CellPos a, CellPos b) => !(a == b);
}
