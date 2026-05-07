using UnityEngine;

public class PathTile : Tile, IPathTile
{
    [SerializeField] private GameObject waypoint;
    [SerializeField] private PathTileType pathTileType;
    
    public Vector3 GetWaypointPosition()
    {
        return waypoint.transform.position;
    }

    public bool IsStraight()
    {
        return pathTileType == PathTileType.Straight;
    }
}