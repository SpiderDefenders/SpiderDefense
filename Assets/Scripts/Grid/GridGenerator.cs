using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [Header("Grid settings")]
    public float tileSize = 1f;

    [Header("Tile reference")]
    public GameObject tilePrefab;

    private int width;
    private int height;
    private Dictionary<Vector2Int, Tile> grid = new Dictionary<Vector2Int, Tile>();

    public Dictionary<Vector2Int, Tile> Generate(int width, int height)
    {
        this.width = width;
        this.height = height;
        GenerateGrid();
        AssignNeighbors();
        return grid;
    }

    private void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 position = new Vector3(x * tileSize, 0, z * tileSize);
                GameObject tileObj = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                tileObj.name = $"Tile_{x}_{z}";
                Tile tile = tileObj.GetComponent<Tile>();
                grid[new Vector2Int(x, z)] = tile;
            }
        }
    }
    
    void AssignNeighbors()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Tile tile = grid[new Vector2Int(x, z)];

                tile.North = (z + 1 < height) ? grid[new Vector2Int(x, z + 1)] : null;
                tile.South = (z - 1 >= 0) ? grid[new Vector2Int(x, z - 1)] : null;
                tile.East = (x + 1 < width) ? grid[new Vector2Int(x + 1, z)] : null;
                tile.West = (x - 1 >= 0) ? grid[new Vector2Int(x - 1, z)] : null;
            }
        }
    }
}