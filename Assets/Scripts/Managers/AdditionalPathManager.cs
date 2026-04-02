using System.Collections.Generic;
using UnityEngine;

public class AdditionalPathManager : MonoBehaviour
{
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private GameObject temporaryTilePrefab;
    
    private Dictionary<Tile, Material> originalMaterials = new Dictionary<Tile, Material>();
    private List<Tile> temporaryTiles = new List<Tile>();

    private GridManager gridManager;
    private PathBuilder pathBuilder;
    private BuildingMenu buildingMenu;
    private PathManager pathManager;

    private List<Tile> availableTiles = new List<Tile>();
    private PathTile lastTile;

    private void Start()
    {
        gridManager = FindAnyObjectByType<GridManager>();
        pathBuilder = FindAnyObjectByType<PathBuilder>();
        buildingMenu = FindAnyObjectByType<BuildingMenu>();
        pathManager = FindAnyObjectByType<PathManager>();
    }
    
    public void StartAdditionalPathBuilding()
    {
        buildingMenu.CloseEverything();
        GenerateAvailableTiles();
    }

    public void GenerateAvailableTiles()
    {
        ClearHighlights();

        lastTile = pathBuilder.GetPathTiles()[^1];

        CheckDirection(lastTile, Direction.N);
        CheckDirection(lastTile, Direction.E);
        CheckDirection(lastTile, Direction.S);
        CheckDirection(lastTile, Direction.W);
    }

    private void CheckDirection(Tile tile, Direction dir)
    {
        Tile neighbor = tile.GetNeighbor(dir);

        if (neighbor != null)
        {
            // ❗ tylko Buildable
            if (neighbor is PathTile) return;
            if (neighbor.Type != TileType.Buildable) return;

            HighlightTile(neighbor, dir, false);
        }
        else
        {
            Vector3 pos = tile.Position + dir.ToVector();

            GameObject temp = Instantiate(temporaryTilePrefab, pos, Quaternion.identity);
            Tile newTile = temp.GetComponent<Tile>();

            gridManager.SetTile((int)pos.x, (int)pos.z, newTile);

            temporaryTiles.Add(newTile);

            HighlightTile(newTile, dir, true);
        }
    }

    private void HighlightTile(Tile tile, Direction dir, bool isTemporary)
    {
        if (availableTiles.Contains(tile)) return;

        Renderer r = tile.GetComponent<Renderer>();

        if (r != null)
        {
            // 🔥 zapisuj tylko dla istniejących tile
            if (!isTemporary && !originalMaterials.ContainsKey(tile))
            {
                originalMaterials[tile] = r.material;
            }

            // 🔥 highlight dla WSZYSTKICH
            r.material = highlightMaterial;
        }

        var clickable = tile.gameObject.AddComponent<ExpandableTile>();
        clickable.Init(this, dir);

        availableTiles.Add(tile);
    }

    private void ClearHighlights()
    {
        foreach (var tile in availableTiles)
        {
            if (tile == null) continue;

            // usuń kliknięcie
            Destroy(tile.GetComponent<ExpandableTile>());

            Renderer r = tile.GetComponent<Renderer>();

            // 🔥 przywróć materiał jeśli był zmieniony
            if (r != null && originalMaterials.ContainsKey(tile))
            {
                r.material = originalMaterials[tile];
            }
        }

        // 🔥 usuń tymczasowe tile
        foreach (var tempTile in temporaryTiles)
        {
            if (tempTile == null) continue;

            Vector3 pos = tempTile.Position;
            Tile tile = gridManager.GetTile((int)pos.x, (int)pos.z);
            if (tile != null && tile.Type != TileType.Path)
            {
                gridManager.RemoveTile((int)pos.x, (int)pos.z);
            }
        }

        availableTiles.Clear();
        originalMaterials.Clear();
        temporaryTiles.Clear();
    }

    public void ExtendPath(Direction direction)
    {
        pathManager.ExtendPath(direction);
        Tile newEnd = pathBuilder.GetPathTiles()[^1];
        temporaryTiles.RemoveAll(t => t == newEnd);
        ClearHighlights();
    }
}