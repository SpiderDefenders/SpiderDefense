using System;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int fakeGridWidth = 200;
    [SerializeField] private int fakeGridHeight= 200;
    private Dictionary<Vector2Int, Tile> grid = new Dictionary<Vector2Int, Tile>();
    private GridGenerator gridGenerator;
    private Tile firstTile;
    private Tile lastTile;
    private GameManager gameManager;

    private void Start()
    {
        gridGenerator = FindAnyObjectByType<GridGenerator>();
        gameManager = FindAnyObjectByType<GameManager>();
    }

    public void GenerateGrid(int width, int height)
    {
        grid = gridGenerator.Generate(width, height);
        firstTile = GetTile(gameManager.GetStartPosition().x, gameManager.GetStartPosition().y);
        lastTile = GetTile(width - 1, height - 1);
    }
    
    public Tile GetTile(int x, int z)
    {
        return grid.TryGetValue(new Vector2Int(x, z), out Tile tile) ? tile : null;
    }

    public void SetTile(int x, int z, Tile tile)
    {
        RemoveTile(x, z);
        Tile northTile = GetTile(x, z + 1);
        Tile southTile = GetTile(x, z - 1);
        Tile eastTile = GetTile(x + 1, z);
        Tile westTile = GetTile(x - 1, z);
        tile.North = northTile;
        tile.South = southTile;
        tile.East = eastTile;
        tile.West = westTile;
        if (tile.North) tile.North.South = tile;
        if (tile.South) tile.South.North = tile;
        if (tile.East) tile.East.West = tile;
        if (tile.West) tile.West.East = tile;
        grid[new Vector2Int(x, z)] = tile;
    }
    
    public void RemoveTile(int x, int z)
    {
        if (GetTile(x, z) == null) return;
        Destroy(GetTile(x, z).gameObject);
        grid.Remove(new Vector2Int(x, z));
    }
    
    public Dictionary<Vector2Int, Tile> GetGrid() => grid;
    
    public Tile GetFirstTile() => firstTile;
    public Tile GetLastTile() => lastTile;
}
