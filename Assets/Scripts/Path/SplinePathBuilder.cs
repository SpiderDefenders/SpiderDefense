using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplinePathBuilder : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private float heightOffset = 0.5f;
    private PathManager pathManager;

    private void Start()
    {
        pathManager = FindAnyObjectByType<PathManager>();
    }

    public void BuildSpline()
    {
        List<PathTile> tiles = pathManager.GetPathTiles();
        if (tiles == null || tiles.Count < 2)
        {
            Debug.LogWarning("Not enough tiles to build spline");
            return;
        }
        tiles.Reverse();

        List<Vector3> points = GetCornerPoints(tiles);

        var spline = new Spline();
        splineContainer.Spline = spline;
        spline.Clear();

        foreach (var point in points)
        {
            spline.Add(new BezierKnot(point + Vector3.up * heightOffset));
        }
        
        spline.SetTangentMode(TangentMode.AutoSmooth);
    }

    private List<Vector3> GetCornerPoints(List<PathTile> tiles)
    {
        List<Vector3> result = new List<Vector3>();

        result.Add(tiles[^1].Position);

        for (int i = tiles.Count - 2; i > 0; i--)
        {
            Vector3 prev = tiles[i + 1].Position;
            Vector3 current = tiles[i].Position;
            Vector3 next = tiles[i - 1].Position;

            Vector3 dir1 = (current - prev).normalized;
            Vector3 dir2 = (next - current).normalized;

            if (dir1 != dir2)
            {
                result.Add(tiles[i].GetWaypointPosition());
            }
        }

        result.Add(tiles[0].GetWaypointPosition());

        return result;
    }
    
    public void AddNewPathTileToSpline(PathTile tile)
    {
        if (splineContainer == null || splineContainer.Spline == null) return;
        Vector3 point = tile.GetWaypointPosition();
        splineContainer.Spline.Insert(0, new BezierKnot(point + Vector3.up * heightOffset));
        splineContainer.Spline.SetTangentMode(TangentMode.AutoSmooth);
    }
}