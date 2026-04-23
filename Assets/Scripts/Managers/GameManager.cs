using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Vector2Int startPosition;
    [SerializeField] private Vector2Int startingGridSize;

    private int currentHP = 100;
    
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
        EventManager.Instance.OnEnemyReachedTheEnd += DealDamage;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnEnemyReachedTheEnd -= DealDamage;
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

    public void DealDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP > 0) return;
        currentHP = 0;
        GameOver();
    }

    private void GameOver()
    {
        EventManager.Instance.GameOver();
    }
}

