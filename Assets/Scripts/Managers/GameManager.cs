using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Vector2Int startPosition;
    [SerializeField] private Vector2Int startingGridSize;

    private int currentHP = 100;
    
    private SplinePathBuilder splinePathBuilder;

    private bool isGameOver = false;
    
    void Start()
    {
        splinePathBuilder = FindAnyObjectByType<SplinePathBuilder>();
        SetUpGame();
    }

    private void OnEnable()
    {
        EventManager.Instance.OnEnemyReachedTheEnd += DealDamage;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnEnemyReachedTheEnd -= DealDamage;
    }

    private void SetUpGame()
    {
        splinePathBuilder.BuildSpline();
        EventManager.Instance.PathGenerated();
        EventManager.Instance.HPChanged(currentHP);
    }
    
    public Vector2Int GetStartPosition() => startPosition;

    public void DealDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Math.Max(0, currentHP);
        EventManager.Instance.HPChanged(currentHP);
        if (currentHP > 0) return;
        currentHP = 0;
        GameOver();
    }

    private void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        EventManager.Instance.GameOver();
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }
    
    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
}

