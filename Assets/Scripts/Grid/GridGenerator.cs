using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [Header("Grid settings")]
    public int width = 10;
    public int height = 10;
    public float tileSize = 1f;

    [Header("Tile reference")]
    public GameObject tilePrefab;

    private GameObject[,] grid;

    public GameObject[,] Generate()
    {
        GenerateGrid();
        AssignNeighbors();
        return grid;
    }

    private void GenerateGrid()
    {
        grid = new GameObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 position = new Vector3(x * tileSize, 0, z * tileSize);
                GameObject tileObj = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                tileObj.name = $"Tile_{x}_{z}";
                grid[x, z] = tileObj;
            }
        }
    }
    
    void AssignNeighbors()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Tile tile = grid[x, z].GetComponent<Tile>();

                tile.North = (z + 1 < height) ? grid[x, z + 1].GetComponent<Tile>() : null;
                tile.South = (z - 1 >= 0) ? grid[x, z - 1].GetComponent<Tile>() : null;
                tile.East = (x + 1 < width) ? grid[x + 1, z].GetComponent<Tile>() : null;
                tile.West = (x - 1 >= 0) ? grid[x - 1, z].GetComponent<Tile>() : null;
            }
        }
    }

    public Tile GetTile(int x, int z)
    {
        return grid[x, z].GetComponent<Tile>();
    }
}