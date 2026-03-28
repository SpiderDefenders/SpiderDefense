using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Vector2Int startPosition;
    [SerializeField] private Vector2Int startingGridSize;
    
    private GridManager gridManager;
    private PathManager pathManager;
    private PathBuilder pathBuilder;
    private SplinePathBuilder splinePathBuilder;
    
    void Start()
    {
        gridManager = FindAnyObjectByType<GridManager>();
        pathManager = FindAnyObjectByType<PathManager>();
        pathBuilder = FindAnyObjectByType<PathBuilder>();
        splinePathBuilder = FindAnyObjectByType<SplinePathBuilder>();
        SetUpGame();
    }

    private void SetUpGame()
    {
        gridManager.GenerateGrid(startingGridSize.x, startingGridSize.y);
        pathManager.GeneratePathSequence();
        pathBuilder.BuildPath();
        splinePathBuilder.BuildSpline();
        EventManager.Instance.PathGenerated();
    }
    
    public Vector2Int GetStartPosition() => startPosition;
}
