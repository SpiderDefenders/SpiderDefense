using UnityEngine;

public class PathTile : Tile, IPathTile
{
    [SerializeField] private GameObject waypoint;
    
    public Vector3 GetWaypointPosition()
    {
        return waypoint.transform.position;
    }
}