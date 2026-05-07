using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    [SerializeField] private Tile startTile;
    [SerializeField] private Tile endTile;
    [SerializeField] private int tileSize = 1;

    [Header("Prefabs")]
    [SerializeField] private GameObject straightPrefab;
    [SerializeField] private GameObject turnPrefab;
    [SerializeField] private GameObject startPrefab;

    private Dictionary<Vector2Int, PathTile> pathDict = new Dictionary<Vector2Int, PathTile>();
    private List<Direction> generatedPath = new List<Direction>();

    private GridManager gridManager;

    private List<PathTile> pathTiles = new List<PathTile>();

    private SplinePathBuilder splinePathBuilder;

    private void Awake()
    {
        splinePathBuilder = FindAnyObjectByType<SplinePathBuilder>();
        gridManager = FindAnyObjectByType<GridManager>();

        BuildDict();
        BuildPath();
    }

    public void BuildPath()
    {
        Vector2Int currentPosition = new Vector2Int((int)startTile.Position.x, (int)startTile.Position.z);
        Direction currentDir = Direction.UNSPECIFIED;

        for (int i = 0; i < pathDict.Count; i++)
        {
            if (i == 0)
            {
                currentDir = GetStartTileDirection();
            }
            else
            {
                PathTile currTile = pathDict[currentPosition];
                if (!currTile.IsStraight())
                {
                    currentDir = GetTurnTileDirection(currentDir, currTile);
                }
            }
            AddTile(currentPosition);
            generatedPath.Add(currentDir);
            currentPosition += currentDir.ToVector2Int() * tileSize;
        }
    }

    private void BuildDict()
    {
        foreach (Transform child in transform)
        {
            PathTile tile = child.GetComponent<PathTile>();

            Vector2Int pos = new Vector2Int(
                Mathf.RoundToInt(child.position.x / tileSize),
                Mathf.RoundToInt(child.position.z / tileSize)
            );

            pathDict[pos] = tile;
        }
    }

    private Direction GetStartTileDirection()
    {
        int rotation = (int)startTile.transform.eulerAngles.y;
        return rotation switch
        {
            0 => Direction.N,
            90 => Direction.E,
            180 => Direction.S,
            270 => Direction.W,
        };
    }

    private Direction GetTurnTileDirection(Direction currentDir, PathTile pathTile)
    {
        int rotation = (int)pathTile.transform.eulerAngles.y;
        (Direction d1, Direction d2) = rotation switch
        {
            0 => (Direction.N, Direction.W),
            90 => (Direction.N, Direction.E),
            180 => (Direction.E, Direction.S),
            270 => (Direction.S, Direction.W),
        };

        return currentDir.Opposite() == d1 ? d2 : d1;

    }

    public Transform GetStartPosition()
    {
        return startTile.transform;
    }

    private void AddTile(Vector2Int position)
    {
        PathTile tileToAdd = pathDict[position];
        gridManager.SetTile((int)position.x, (int)position.y, tileToAdd);
        pathTiles.Add(tileToAdd);
    }

    // extend path procedure
    private PathTile AddTileFromPrefab(GameObject tileToAdd, Vector3 position, Quaternion rotation)
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

    private GameObject GetPrefab(Direction prev, Direction current)
    {
        return prev == current ? straightPrefab : turnPrefab;
    }

    private Quaternion GetRotation(Direction prev, Direction current)
    {
        return prev == current ? current.ToRotationStraight() : prev.ToRotationTurn(current);
    }

    public void ExtendPath(Direction dir)
    {
        generatedPath.Insert(0, dir);
        Direction firstDirection = generatedPath[0], secondLastDirection = generatedPath[1], thirdLastDirection = generatedPath[2];

        PathTile lastTile = pathTiles[^1];
        pathTiles.Remove(lastTile);
        AddTileFromPrefab(GetPrefab(firstDirection, secondLastDirection), lastTile.Position, GetRotation(firstDirection, secondLastDirection));
        Vector3 currentPosition = lastTile.Position + firstDirection.Opposite().ToVector() * tileSize;
        PathTile tile = AddTileFromPrefab(startPrefab, currentPosition, firstDirection.ToRotationStraight());

        splinePathBuilder.AddNewPathTileToSpline(tile);
    }
}