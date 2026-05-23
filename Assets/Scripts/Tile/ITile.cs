using UnityEngine;

public interface ITile
{
    Vector3 Position { get; }
    TileType Type { get; }
    bool IsOccupied { get; }
    bool CanPlace(IPlacable placable);
    bool Place(IPlacable placable);
    void Remove();
}