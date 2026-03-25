using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GridGenerator gridGenerator;
    private PathManager pathManager;
    private PathBuilder pathBuilder;
    private SplinePathBuilder splinePathBuilder;
    [SerializeField] private Vector2Int startPosition;
    private GameObject[,] grid;
    
    void Start()
    {
        gridGenerator = FindAnyObjectByType<GridGenerator>();
        pathManager = FindAnyObjectByType<PathManager>();
        pathBuilder = FindAnyObjectByType<PathBuilder>();
        splinePathBuilder = FindAnyObjectByType<SplinePathBuilder>();
        SetUpGame();
    }

    private void SetUpGame()
    {
        grid = gridGenerator.Generate();
        pathManager.GeneratePathSequence();
        pathBuilder.BuildPath();
        splinePathBuilder.BuildSpline();
        EventManager.Instance.PathGenerated();
    }
    
    public Vector2Int GetStartPosition() => startPosition;

    public void RemoveTileFromGrid(int x, int z)
    {
        if (grid[x, z] == null) return;
        Destroy(grid[x, z].gameObject);
        grid[x, z] = null;
    }
    
    public void ReplaceTileFromGrid(int x, int z, Tile newTile)
    {
        if (grid[x, z] == null) return;
        Tile prevTile = grid[x, z].GetComponent<Tile>();
        newTile.North = prevTile.North;
        newTile.South = prevTile.South;
        newTile.East = prevTile.East;
        newTile.West = prevTile.West;
        if (newTile.North) newTile.North.South = newTile;
        if (newTile.South) newTile.South.North = newTile;
        if (newTile.East) newTile.East.West = newTile;
        if (newTile.West) newTile.West.East = newTile;
        grid[x, z] = newTile.gameObject;
        Destroy(prevTile.gameObject);
    }
    
    public List<Direction> GetPath()
    {
        return pathManager.GetGeneratedPath();
    }

    public List<PathTile> GetPathTiles()
    {
        return pathBuilder.GetPathTiles();
    }

    public GameObject[,] GetGrid() => grid;
}
