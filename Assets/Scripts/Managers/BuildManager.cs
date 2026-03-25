using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    [SerializeField] private LayerMask tileLayer;
    [SerializeField] private Material unableToPlaceMaterial;
    [SerializeField] private Material ableToPlaceMaterial;

    private GameObject prefabToPlace;
    private GameObject previewObject;
    private IPlacable previewPlacable;

    private Renderer[] previewRenderers;

    void Awake()
    {
        Instance = this;
    }

    public void StartPlacement(GameObject prefab)
    {
        if (prefabToPlace != null)
        {
            CancelPlacement();
        }
        prefabToPlace = prefab;

        previewObject = Instantiate(prefab);
        previewPlacable = previewObject.GetComponent<IPlacable>();
        previewRenderers = previewObject.GetComponentsInChildren<Renderer>();
        previewObject.SetActive(false);
    }

    void Update()
    {
        if (prefabToPlace == null)
            return;

        HandlePlacement();
    }

    void HandlePlacement()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, tileLayer))
        {
            previewObject.SetActive(true);
            Tile tile = hit.collider.GetComponent<Tile>();

            if (tile == null)
                return;

            previewObject.transform.position = tile.transform.position + previewPlacable.GetPlacingOffset();

            bool canPlace = tile.CanPlace(previewPlacable);

            SetPreviewColor(canPlace ? ableToPlaceMaterial : unableToPlaceMaterial);

            if (Input.GetMouseButtonDown(0) && canPlace)
            {
                Place(tile);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                CancelPlacement();
            }
        }
    }

    void Place(Tile tile)
    {
        GameObject obj = Instantiate(prefabToPlace, tile.transform);

        IPlacable placable = obj.GetComponent<IPlacable>();

        tile.Place(placable);

        Destroy(previewObject);

        prefabToPlace = null;
    }

    void SetPreviewColor(Material material)
    {
        foreach (var r in previewRenderers)
        {
            r.material = material;
        }
    }

    void CancelPlacement()
    {
        previewObject.SetActive(false);
        previewPlacable = null;
        previewRenderers = null;
        prefabToPlace = null;
        Destroy(previewObject);
    }
}