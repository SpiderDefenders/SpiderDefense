using UnityEngine;

public class Tile : MonoBehaviour, ITile
{
    [SerializeField] private TileType type;
    
    public Tile North;
    public Tile East;
    public Tile South;
    public Tile West;

    private IPlacable placedObject;

    public Vector3 Position => transform.position;

    public TileType Type => type;

    public bool IsOccupied => placedObject != null;

    public bool CanPlace(IPlacable placable)
    {
        if (IsOccupied) return false;

        return type switch
        {
            TileType.Buildable => placable.Type == PlacableType.Defense,
            TileType.Decoration => placable.Type == PlacableType.Decoration,
            _ => false
        };
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
        //if (placedObject == null)
        //    return;
        //placedObject.OnRemoved();

        placedObject = null;
    }
    
    public Tile GetNeighbor(Direction dir)
    {
        return dir switch
        {
            Direction.N => North,
            Direction.E => East,
            Direction.S => South,
            Direction.W => West,
            _ => null
        };
    }
}