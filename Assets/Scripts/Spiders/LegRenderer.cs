using System.Collections.Generic;
using UnityEngine;

public class LegRenderer : MonoBehaviour
{
    [SerializeField] private GameObject jointPrefab;
    [SerializeField] private GameObject bonePrefab;
    [SerializeField] private float linkLength = 1f;
    [SerializeField] private float boneWeight = 0.3f;
    [SerializeField] private List<Transform> joints;
    private List<GameObject> bones = new List<GameObject>();
    private Leg leg;

    void OnEnable()
    {
        leg = new Leg(joints[0].position, 3, linkLength);
        for (int i = 0; i < joints.Count - 1; i++)
        {
            GameObject bone = Instantiate(bonePrefab, transform);
            bone.transform.localScale = new Vector3(boneWeight, 1f, boneWeight);
            bones.Add(bone);
        }
    }

    void Update()
    {
        if (joints[joints.Count - 1] != null)
        {
            leg.SolveIK(joints[0].position, joints[joints.Count - 1].position);
        }

        for (int i = 0; i < joints.Count; i++)
        {
            joints[i].transform.position = leg.joints[i];
        }

        for (int i = 0; i < bones.Count; i++)
        {
            Vector3 start = joints[i].transform.position;
            Vector3 end = joints[i + 1].transform.position;
            Vector3 mid = (start + end) / 2f;
            Vector3 dir = end - start;
            bones[i].transform.position = mid;
            bones[i].transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);
            Vector3 scale = bones[i].transform.localScale;
            scale.y = dir.magnitude * 2.5f;
            bones[i].transform.localScale = scale;
        }
    }
}
