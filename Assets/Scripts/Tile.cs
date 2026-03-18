using UnityEngine;

public class Tile : MonoBehaviour, ITile
{
    [SerializeField] private TileType type;

    private IPlacable placedObject;

    public Vector3 Position => transform.position;

    public TileType Type => type;

    public bool IsOccupied => placedObject != null;

    public bool CanPlace(IPlacable placable)
    {
        if (IsOccupied)
            return false;

        switch (type)
        {
            case TileType.Buildable:
                return placable.Type == PlacableType.Defense;

            case TileType.Decoration:
                return placable.Type == PlacableType.Decoration;

            case TileType.Path:
                return false;
        }

        return false;
    }

    public bool Place(IPlacable placable)
    {
        placedObject = placable;

        MonoBehaviour mb = placable as MonoBehaviour;
        mb.transform.position = transform.position + placable.GetPlacingOffset();

        placable.OnPlaced(this);

        return true;
    }

    public void Remove()
    {
        if (placedObject == null)
            return;

        placedObject.OnRemoved();
        placedObject = null;
    }
}