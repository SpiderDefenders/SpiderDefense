using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PathManager : MonoBehaviour
{
    private Tile startTile;
    private Tile endTile;
    private List<Direction> generatedPath = new List<Direction>();
    private List<Tile> tilesPath = new List<Tile>();
    
    public void GeneratePathSequence()
    {
        GridManager gridManager = FindAnyObjectByType<GridManager>();
        startTile = gridManager.GetFirstTile();
        endTile = gridManager.GetLastTile();

        if (startTile == null || endTile == null) return;

        Queue<Tile> queue = new Queue<Tile>();
        Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();

        queue.Enqueue(startTile);
        cameFrom[startTile] = null;

        while (queue.Count > 0)
        {
            Tile current = queue.Dequeue();
            if (current == endTile) break;

            foreach (Direction dir in GetBiasedDirections(current))
            {
                Tile neighbor = current.GetNeighbor(dir);

                if (neighbor != null && !cameFrom.ContainsKey(neighbor))
                {
                    queue.Enqueue(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        }

        tilesPath = new List<Tile>();
        Tile temp = endTile;

        if (!cameFrom.ContainsKey(endTile))
        {
            Debug.LogWarning("No path found!");
            return;
        }

        while (temp != null)
        {
            tilesPath.Add(temp);
            temp = cameFrom[temp];
        }

        tilesPath.Reverse();

        for (int i = 0; i < tilesPath.Count - 1; i++)
        {
            Tile from = tilesPath[i];
            Tile to = tilesPath[i + 1];

            if (from.North == to) generatedPath.Add(Direction.N);
            else if (from.East == to) generatedPath.Add(Direction.E);
            else if (from.South == to) generatedPath.Add(Direction.S);
            else if (from.West == to) generatedPath.Add(Direction.W);
        }
    }


    private List<Direction> GetBiasedDirections(Tile current)
    {
        List<Direction> dirs = new List<Direction>()
        {
            Direction.N,
            Direction.E,
            Direction.S,
            Direction.W
        };

        dirs.Sort((a, b) =>
        {
            Tile aTile = current.GetNeighbor(a);
            Tile bTile = current.GetNeighbor(b);

            float aDist = aTile != null ? Vector3.Distance(aTile.Position, endTile.Position) : float.MaxValue;
            float bDist = bTile != null ? Vector3.Distance(bTile.Position, endTile.Position) : float.MaxValue;

            return aDist.CompareTo(bDist);
        });

        float randomness = 0.8f;

        if (Random.value < randomness)
        {
            int i = Random.Range(0, dirs.Count);
            int j = Random.Range(0, dirs.Count);
            (dirs[i], dirs[j]) = (dirs[j], dirs[i]);
        }

        return dirs;
    }
    
    public List<Direction> GetPath()
    {
        return generatedPath;
    }
    
    public List<Tile> GetTilesPath()
    {
        return tilesPath;
    }
}