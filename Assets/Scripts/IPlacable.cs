using UnityEngine;

public interface IPlacable
{
    void OnPlaced(ITile tile);
    void OnRemoved();
    Vector3 GetPlacingOffset();

    PlacableType Type { get; }
}