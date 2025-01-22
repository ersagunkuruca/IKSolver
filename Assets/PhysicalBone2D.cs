using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalBone2D : MonoBehaviour
{
    public float length = 1f;
    public float radius = 0.2f;
    public float angle;
    public float angularVelocity;
    public float angularAcceleration;

    public void Draw(Color color)
    {
        float r = radius * length;
        int count = 20;
        var pos = transform.position;
        var up = transform.up;
        var upLength = up * length;
        var tipPos = pos + upLength;
        var right = transform.right;
        var rightR = right * r;
        //Debug.DrawRay(pos, up * length);

        Debug.DrawLine(pos + rightR, tipPos + rightR, color);
        Debug.DrawLine(pos - rightR, tipPos - rightR, color);
        for (int i = 0; i < count; i++)
        {
            var q1 = Quaternion.AngleAxis((float)i / count * 180, Vector3.forward);
            var q2 = Quaternion.AngleAxis((float)(i + 1) / count * 180, Vector3.forward);
            var v11 = q1 * -rightR;
            var v12 = q2 * -rightR;
            var v21 = q1 * rightR;
            var v22 = q2 * rightR;
            Debug.DrawLine(pos + v11, pos + v12, color);
            Debug.DrawLine(tipPos + v22, tipPos + v21, color);
        }
    }
}
