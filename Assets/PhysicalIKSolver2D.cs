using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using UnityEngine;
using UnityEngine.XR;

public class PhysicalIKSolver2D : MonoBehaviour
{
    public List<PhysicalBone2D> bones;
    public Transform target;
    public int iterationCount = 20;
    public float speed = 10f;
    public float maxAngularVelocity = 2f;
    public int stepCount = 10;

    void Start()
    {
        for (int i = 0; i < bones.Count - 1; i++)
        {
            bones[i].transform.parent = bones[i + 1].transform;
            bones[i].transform.localPosition = Vector3.up * bones[i + 1].length;
            bones[i].transform.localRotation = Quaternion.AngleAxis(bones[i].angle, Vector3.forward);
        }
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < stepCount; i++)
        {
            UpdateX(Time.fixedDeltaTime / stepCount);
        }
    }

    void UpdateX(float dt)
    {
        var tipPoint = bones[0].transform.TransformPoint(Vector3.up * bones[0].length);
        var targetPoint = target.position;
        var targetVelocity = targetPoint - tipPoint;
        var currentVelocity = Vector3.zero; //bones.Select(bone => Vector3.Cross(bone.transform.forward, (tipPoint - bone.transform.position))*bone.angularVelocity).Aggregate((vector1, vector2) => vector1 + vector2) * 0.1f;
        var remainingVelocity = targetVelocity - currentVelocity;
        var angularVelocities = new float[bones.Count];
        var velocities = new Vector3[bones.Count];

        for (int i = 0; i < iterationCount; i++)
        {
            var index = i % bones.Count; //bones.Select((currentBone, boneIndex) => Mathf.Pow(Vector3.Dot(Vector3.Cross(currentBone.transform.forward, (tipPoint - currentBone.transform.position) / ((tipPoint - currentBone.transform.position).magnitude)), remainingVelocity), 2f)).MaxIndex();
            //Debug.Log(index);
            var bone = bones[index];
            var boneToTip = (tipPoint - bone.transform.position);
            var velocityDirection = Vector3.Cross(bone.transform.forward, boneToTip);
            var velocity = Vector3.Project(remainingVelocity, velocityDirection);
            //var s = Vector3.Project(remainingVelocity, velocityDirection);
            //var k = velocityDirection * remainingVelocity.sqrMagnitude / Vector3.Dot(velocityDirection, remainingVelocity);

            //velocity = s * Mathf.Sqrt(k.magnitude / s.magnitude);
            angularVelocities[index] += Vector3.Dot(velocity, velocityDirection) / boneToTip.sqrMagnitude;
            velocities[index] += velocity;
            //Debug.Log(targetVelocity);
            //Debug.Log(angularVelocities[index]);
            currentVelocity += velocity;
            remainingVelocity -= velocity;
        }

        //Debug.DrawRay(tipPoint+currentVelocity, remainingVelocity, Color.red);

        //Debug.DrawRay(tipPoint, currentVelocity, Color.yellow);

        for (int i = 0; i < bones.Count; i++)
        {
            var bone = bones[i];
            bone.angularVelocity = angularVelocities[i]; //Mathf.Lerp(angularVelocities[i], bone.angularVelocity, Mathf.Pow(0.5f, dt * 1000f));
            //Debug.DrawRay(bone.transform.position + bone.transform.up * bone.length, -bone.transform.right * bone.angularVelocity * speed /10f * bone.length);

            //bones[i].angularVelocity *= 0.90f;
            bone.transform.localRotation = bone.transform.localRotation * Quaternion.AngleAxis(bone.angularVelocity * speed * 180f / Mathf.PI * dt, Vector3.forward);
            bone.Draw(new Color(1f, 1f, 1f, dt * 100f));
        }
    }
}

public static class MaxIndexHelper
{
    public static int MaxIndex<T>(this IEnumerable<T> sequence)
where T : IComparable<T>
    {
        int maxIndex = -1;
        T maxValue = default(T); // Immediately overwritten anyway

        int index = 0;
        foreach (T value in sequence)
        {
            if (value.CompareTo(maxValue) > 0 || maxIndex == -1)
            {
                maxIndex = index;
                maxValue = value;
            }
            index++;
        }
        return maxIndex;
    }
}
