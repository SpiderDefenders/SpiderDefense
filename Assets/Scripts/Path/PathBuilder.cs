using System;
using System.Collections.Generic;
using UnityEngine;

public class PathBuilder : MonoBehaviour
{
    [SerializeField] private GameObject straightPrefab;
    [SerializeField] private GameObject turnPrefab;
    [SerializeField] private GameObject startPrefab;
    [SerializeField] private GameObject finishPrefab;
    [SerializeField] private float tileSize = 1f;
    
    private GameManager gameManager;
    
    private List<PathTile> pathTiles = new List<PathTile>();

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    public void BuildPath()
    {
        List<Direction> path = gameManager.GetPath();
        Vector2Int startPos = gameManager.GetStartPosition();
        Vector3 currentPosition = new Vector3(startPos.x, 0f, startPos.y);

        for (int i = 0; i < path.Count; i++)
        {
            Direction currentDir = path[i];
            Direction? previousDir = i > 0 ? path[i - 1] : (Direction?)null;

            GameObject prefabToUse;
            Quaternion rotation;

            if (i == 0)
            {
                prefabToUse = startPrefab;
                rotation = GetRotationForStraight(currentDir);
            }
            else if (previousDir == null || previousDir == currentDir)
            {
                // Straight tile
                prefabToUse = straightPrefab;
                rotation = GetRotationForStraight(currentDir);
            }
            else
            {
                // Turn tile
                prefabToUse = turnPrefab;
                rotation = GetRotationForTurn(previousDir.Value, currentDir);
            }

            GameObject go = Instantiate(prefabToUse, currentPosition, rotation, transform);
            PathTile tile = go.GetComponent<PathTile>();
            gameManager.ReplaceTileFromGrid((int)currentPosition.x, (int)currentPosition.z, tile);
            pathTiles.Add(tile);
            // Move to next position
            currentPosition += DirectionToVector(currentDir) * tileSize;
        }

        if (path.Count <= 0) return;
        Direction lastDir = path[^1];
        GameObject finishGO = Instantiate(finishPrefab, currentPosition, GetRotationForStraight(lastDir), transform);
        PathTile finishTile = finishGO.GetComponent<PathTile>();
        gameManager.ReplaceTileFromGrid((int)currentPosition.x, (int)currentPosition.z, finishTile);
        pathTiles.Add(finishTile);
    }

    private Vector3 DirectionToVector(Direction dir)
    {
        return dir switch
        {
            Direction.N => Vector3.forward,
            Direction.E => Vector3.right,
            Direction.S => Vector3.back,
            Direction.W => Vector3.left,
            _ => Vector3.zero
        };
    }

    private Quaternion GetRotationForStraight(Direction dir)
    {
        return dir switch
        {
            Direction.N => Quaternion.Euler(0, 180, 0),
            Direction.E => Quaternion.Euler(0, 270, 0),
            Direction.S => Quaternion.Euler(0, 0, 0),
            Direction.W => Quaternion.Euler(0, 90, 0),
            _ => Quaternion.identity
        };
    }

    private Quaternion GetRotationForTurn(Direction from, Direction to)
    {
        switch (from)
        {
            case Direction.N when to == Direction.E:
            case Direction.W when to == Direction.S:
                return Quaternion.Euler(0, 180, 0);
            case Direction.E when to == Direction.S:
            case Direction.N when to == Direction.W:
                return Quaternion.Euler(0, -90, 0);
            case Direction.S when to == Direction.W:
            case Direction.E when to == Direction.N:
                return Quaternion.Euler(0, 0, 0);
            case Direction.W when to == Direction.N:
            case Direction.S when to == Direction.E:
                return Quaternion.Euler(0, 90, 0);
            default:
                return Quaternion.identity;
        }
    }

    public List<PathTile> GetPathTiles()
    {
        return pathTiles;
    }
}