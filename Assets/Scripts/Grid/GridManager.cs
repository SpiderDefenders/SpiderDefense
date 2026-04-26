using System;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int tileSize = 1;
    private Dictionary<Vector2Int, Tile> grid = new Dictionary<Vector2Int, Tile>();

    private void Awake()
    {
        foreach (Transform child in transform)
        {
            Tile tile = child.GetComponent<Tile>();

            Vector2Int pos = new Vector2Int(
                Mathf.RoundToInt(child.position.x / tileSize),
                Mathf.RoundToInt(child.position.z / tileSize)
            );

            grid[pos] = tile;
        }

        AssignNeighbors();
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

    void AssignNeighbors()
    {
        foreach (var element in grid)
        {
            Vector2Int pos = element.Key;
            Tile tile = element.Value;

            grid.TryGetValue(pos + Vector2Int.up, out tile.North);
            grid.TryGetValue(pos + Vector2Int.down, out tile.South);
            grid.TryGetValue(pos + Vector2Int.right, out tile.East);
            grid.TryGetValue(pos + Vector2Int.left, out tile.West);
        }
    }

    public Dictionary<Vector2Int, Tile> GetGrid() => grid;
}
