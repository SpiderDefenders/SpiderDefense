using System.Collections.Generic;
using UnityEngine;

public class TowerModeManager
{
    List<TowerModeType> availableModes;
    int currentModeIdx;


    public TowerModeManager(List<TowerModeType> availableModes, TowerModeType currentMode)
    {
        this.availableModes = availableModes;
        currentModeIdx = availableModes.IndexOf(currentMode);
    }

    public void Next()
    {
        currentModeIdx = (currentModeIdx + 1) % availableModes.Count;
    }

    public void Previous()
    {
        currentModeIdx = (currentModeIdx - 1) % availableModes.Count;
    }

    public float GetModeValue(Enemy enemy)
    {
        TowerModeType mode = availableModes[currentModeIdx];
        switch (mode)
        {
            case TowerModeType.First:
                return enemy.GetProgress();

            case TowerModeType.Last:
                return -enemy.GetProgress();

            case TowerModeType.Strong:
                return enemy.GetHealth();

            case TowerModeType.Weak:
                return -enemy.GetHealth();
        }
        // default
        return enemy.GetProgress();
    }
}