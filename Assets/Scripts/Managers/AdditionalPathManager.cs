using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditionalPathManager : MonoBehaviour
{
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private GameObject temporaryTilePrefab;
    [SerializeField] private float delayBeforePathBuilding = 1f;
    [SerializeField] private InGameMenu inGameMenu;

    private Dictionary<Tile, Material> originalMaterials = new Dictionary<Tile, Material>();
    private List<Tile> temporaryTiles = new List<Tile>();

    private GridManager gridManager;
    private MenuManager menuManager;
    private PathManager pathManager;

    private List<Tile> availableTiles = new List<Tile>();
    private PathTile lastTile;

    private Vector2 xBounds;
    private Vector2 zBounds;

    private void Start()
    {
        gridManager = FindAnyObjectByType<GridManager>();
        menuManager = FindAnyObjectByType<MenuManager>();
        pathManager = FindAnyObjectByType<PathManager>();

        CalculateInitialBounds();
    }

    void OnEnable()
    {
        EventManager.Instance.OnWaveCompleted += StartAdditionalPathBuildingWithDelay;
    }

    void OnDisable()
    {
        EventManager.Instance.OnWaveCompleted -= StartAdditionalPathBuildingWithDelay;
    }

    private void CalculateInitialBounds()
    {
        xBounds = new Vector2(float.MaxValue, float.MinValue);
        zBounds = new Vector2(float.MaxValue, float.MinValue);

        foreach (PathTile tile in pathManager.GetPathTiles())
        {
            ExpandBounds(tile.Position);
        }

        EventManager.Instance.PathBoundsChanged(xBounds, zBounds);
    }

    private void ExpandBounds(Vector3 position)
    {
        bool changed = false;

        if (position.x < xBounds.x) { xBounds.x = position.x; changed = true; }
        if (position.x > xBounds.y) { xBounds.y = position.x; changed = true; }
        if (position.z < zBounds.x) { zBounds.x = position.z; changed = true; }
        if (position.z > zBounds.y) { zBounds.y = position.z; changed = true; }

        if (changed)
            EventManager.Instance.PathBoundsChanged(xBounds, zBounds);
    }

    private void StartAdditionalPathBuildingWithDelay(int _, bool isLastWave)
    {
        if (isLastWave) return;
        StartCoroutine(Delay(delayBeforePathBuilding));
    }

    private IEnumerator Delay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        StartAdditionalPathBuilding();
    }

    public void StartAdditionalPathBuilding()
    {
        EventManager.Instance.AdditionalPathPlacingStarted();
        menuManager.CloseAll();
        if (GenerateAvailableTiles())
        {
            inGameMenu.ToggleMenu();
            return;
        }
        inGameMenu.CompletelyHideMenu();
        EventManager.Instance.AdditionalPathPlacingCompleted();
    }

    public bool GenerateAvailableTiles()
    {
        ClearHighlights();

        lastTile = pathManager.GetPathTiles()[^1];
        FindAnyObjectByType<SimpleRtsCamera.Scripts.SimpleRtsCamera>().GoTo(lastTile.transform);

        CheckDirection(lastTile, Direction.N);
        CheckDirection(lastTile, Direction.E);
        CheckDirection(lastTile, Direction.S);
        CheckDirection(lastTile, Direction.W);
        return availableTiles.Count != 0;
    }

    private void CheckDirection(Tile tile, Direction dir)
    {
        Tile neighbor = tile.GetNeighbor(dir);

        if (neighbor != null)
        {
            if (neighbor is PathTile) return;
            if (neighbor.Type != TileType.Buildable || neighbor.IsOccupied) return;

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
            if (!isTemporary && !originalMaterials.ContainsKey(tile))
            {
                originalMaterials[tile] = r.material;
            }

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

            Destroy(tile.GetComponent<ExpandableTile>());
            Renderer r = tile.GetComponent<Renderer>();

            if (r != null && originalMaterials.ContainsKey(tile))
            {
                r.material = originalMaterials[tile];
            }
        }

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
        Tile newEnd = pathManager.GetPathTiles()[^1];
        temporaryTiles.RemoveAll(t => t == newEnd);
        ExpandBounds(newEnd.Position);
        ClearHighlights();
        EventManager.Instance.AdditionalPathPlacingCompleted();
        inGameMenu.CompletelyHideMenu();
    }
}