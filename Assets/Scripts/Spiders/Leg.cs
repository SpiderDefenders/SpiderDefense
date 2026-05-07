using System.Collections.Generic;
using UnityEngine;

public class Leg
{
    public List<Vector3> joints;
    private float linkLength;

    public Leg(Vector3 basePosition, int jointCount, float linkLength)
    {
        this.linkLength = linkLength;
        joints = new List<Vector3>();

        for (int i = 0; i < jointCount; i++)
        {
            joints.Add(basePosition + new Vector3(0, -linkLength * i, 0));
        }
    }

    public void SolveIK(Vector3 hip, Vector3 foot)
    {
        joints[0] = hip;
        joints[2] = foot;

        float a = linkLength;
        float b = linkLength;
        float c = Vector3.Distance(hip, foot);

        c = Mathf.Min(c, a + b - 0.001f);
        Vector3 dir = (foot - hip).normalized;

        Vector3 right = Vector3.Cross(dir, Vector3.up);
        if (right.sqrMagnitude < 0.0001f)
            right = Vector3.Cross(dir, Vector3.right);
        right.Normalize();
        

        Vector3 forward = Vector3.Cross(right, dir).normalized;
        float s = c * 0.5f;
        float h = Mathf.Sqrt(a * a - (s * s));
        Vector3 mid = hip + dir * s;
        joints[1] = mid + forward * h;
    }

}
