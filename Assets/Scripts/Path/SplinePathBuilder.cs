using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplinePathBuilder : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private float heightOffset = 0.5f;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    public void BuildSpline()
    {
        List<PathTile> tiles = gameManager.GetPathTiles();
        if (tiles == null || tiles.Count < 2)
        {
            Debug.LogWarning("Not enough tiles to build spline");
            return;
        }

        // 🔥 Optional but HIGHLY recommended: simplify path (only corners)
        List<Vector3> points = GetCornerPoints(tiles);

        var spline = new Spline();
        splineContainer.Spline = spline;
        spline.Clear();

        foreach (var point in points)
        {
            spline.Add(new BezierKnot(point + Vector3.up * heightOffset));
        }

        // Auto smooth tangents
        spline.SetTangentMode(TangentMode.AutoSmooth);
    }

    // 🔥 Extract only corners (big visual upgrade)
    private List<Vector3> GetCornerPoints(List<PathTile> tiles)
    {
        List<Vector3> result = new List<Vector3>();

        result.Add(tiles[0].Position);

        for (int i = 1; i < tiles.Count - 1; i++)
        {
            Vector3 prev = tiles[i - 1].Position;
            Vector3 current = tiles[i].Position;
            Vector3 next = tiles[i + 1].Position;

            Vector3 dir1 = (current - prev).normalized;
            Vector3 dir2 = (next - current).normalized;

            // If direction changes → it's a corner
            if (dir1 != dir2)
            {
                result.Add(tiles[i].GetWaypointPosition());
            }
        }

        result.Add(tiles[^1].GetWaypointPosition());

        return result;
    }
}