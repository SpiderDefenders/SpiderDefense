using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [Header("Grid settings")]
    public int width = 10;
    public int height = 10;
    public float tileSize = 1f;

    [Header("Tile reference")]
    public GameObject tilePrefab; // ustawiasz w Inspectorze

    private GameObject[,] grid;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        grid = new GameObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 position = new Vector3(
                    x * tileSize,
                    0,
                    z * tileSize
                );

                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                tile.name = $"Tile_{x}_{z}";

                grid[x, z] = tile;
            }
        }
    }
}