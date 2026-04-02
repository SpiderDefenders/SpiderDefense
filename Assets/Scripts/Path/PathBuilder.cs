using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PathBuilder : MonoBehaviour
{
    [SerializeField] private GameObject straightPrefab;
    [SerializeField] private GameObject turnPrefab;
    [SerializeField] private GameObject endPrefab;
    [SerializeField] private GameObject startPrefab;
    [SerializeField] private float tileSize = 1f;
    
    private GridManager gridManager;
    private PathManager pathManager;
    private GameManager gameManager;
    private Vector3 currentPosition;
    
    private List<PathTile> pathTiles = new List<PathTile>();

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        gridManager = FindAnyObjectByType<GridManager>();
        pathManager = FindAnyObjectByType<PathManager>();
    }

    public void BuildPath()
    {
        List<Direction> path = pathManager.GetPath();
        Vector2Int startPos = gameManager.GetStartPosition();
        currentPosition = new Vector3(startPos.x, 0f, startPos.y);

        for (int i = 0; i < path.Count; i++)
        {
            Direction currentDir = path[i];
            Direction previousDir = i > 0 ? path[i - 1] : Direction.UNSPECIFIED;

            GameObject prefabToUse;
            Quaternion rotation;

            if (i == 0)
            {
                prefabToUse = startPrefab;
                rotation = currentDir.ToRotationStraight();
            }
            else
            {
                prefabToUse = GetPrefab(previousDir, currentDir);
                rotation = GetRotation(previousDir, currentDir);
            }
            AddTile(prefabToUse, currentPosition, rotation);
            currentPosition += currentDir.ToVector() * tileSize;
        }

        if (path.Count <= 0) return;
        AddTile(endPrefab, currentPosition, path[^1].ToRotationStraight() * Quaternion.Euler(0, 180, 0));
    }
    
    private GameObject GetPrefab(Direction prev, Direction current)
    {
        return prev == current ? straightPrefab : turnPrefab;
    }
    
    private Quaternion GetRotation(Direction prev, Direction current)
    {
        return prev == current ? current.ToRotationStraight() : prev.ToRotationTurn(current);
    }

    private PathTile AddTile(GameObject tileToAdd, Vector3 position, Quaternion rotation)
    {
        GameObject go = Instantiate(tileToAdd, position, rotation, transform);
        PathTile tile = go.GetComponent<PathTile>();
        gridManager.SetTile((int)position.x, (int)position.z, tile);
        pathTiles.Add(tile);
        return tile;
    }

    public List<PathTile> GetPathTiles()
    {
        return pathTiles;
    }
    
    public PathTile ExtendPath(Direction firstDirection, Direction secondLastDirection, Direction thirdLastDirection)
    {
        PathTile lastTile = pathTiles[^1];
        pathTiles.Remove(lastTile);
        AddTile(GetPrefab(firstDirection, secondLastDirection), lastTile.Position, GetRotation(firstDirection, secondLastDirection));
        currentPosition = lastTile.Position + firstDirection.Opposite().ToVector() * tileSize;
        return AddTile(startPrefab, currentPosition, firstDirection.ToRotationStraight());
    }
}