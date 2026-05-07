using UnityEngine;
using System.Collections.Generic;

public class Ant : MonoBehaviour
{
    [SerializeField] private Transform lastJointsTransform;
    [SerializeField] private List<Transform> legs;

    private void Start()
    {
        foreach (Transform leg in this.legs) {
            leg.GetChild(2).SetParent(lastJointsTransform);
        }
    }
}
