using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FootIK : MonoBehaviour
{
    [SerializeField]
    Animator animator;
    [SerializeField]
    float footRadius;
    [SerializeField]
    float maxFootLift = 0.2f;
    private void OnAnimatorIK(int layerIndex)
    {
        DoFoot(AvatarIKGoal.LeftFoot, HumanBodyBones.LeftFoot, true);
        DoFoot(AvatarIKGoal.RightFoot, HumanBodyBones.RightFoot);
        //animator.stabilizeFeet = true;
    }

    private void DoFoot(AvatarIKGoal ikGoal, HumanBodyBones bone, bool inverseRotation = false)
    {
        var footTransform = animator.GetBoneTransform(bone);
        var n = 20;
        var footBottomHeight = (bone == HumanBodyBones.RightFoot ? animator.rightFeetBottomHeight : animator.leftFeetBottomHeight);
        var footHeight = (transform.InverseTransformPoint(footTransform.position).y - footBottomHeight) * 100f;

        var range = Mathf.Lerp(1f, 2f, footHeight);
        Debug.Log(ikGoal.ToString() + " " + footHeight);

        RaycastHit[] hits = new RaycastHit[n];
        for(int i = 0; i < n; i++)
        {
            ShowRaycast(new Ray(footTransform.position + (inverseRotation ? -1f : 1f) * footTransform.forward * (float)i / (float)n * footRadius * range + Vector3.up * maxFootLift, -Vector3.up), out RaycastHit hit);
            hits[i] = hit;
        }
        if (hits.All(hit => hit.distance > 0f))
        {
            float averageY = hits.Average(hit => hit.point.y);
            float maxY = hits.Max(hit => hit.point.y);
            float y = Mathf.Lerp(maxY, averageY,  footHeight);
            float diff = (y - transform.position.y);
            animator.SetIKPosition(ikGoal, footTransform.position + Vector3.up * diff);
            animator.SetIKPositionWeight(ikGoal, 1f);
            animator.SetIKRotation(ikGoal, Quaternion.FromToRotation(Vector3.up, hits[0].normal) * (inverseRotation ?  footTransform.rotation * Quaternion.AngleAxis(180f, Vector3.right) : footTransform.rotation));
            animator.SetIKRotationWeight(ikGoal, 1f);
        }
        else
        {
            animator.SetIKPositionWeight(ikGoal, 0f);
            animator.SetIKRotationWeight(ikGoal, 0f);
        }
    }

    private void OnDisable()
    {
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0f);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0f);
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0f);
        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0f);
    }

    private bool ShowRaycast(Ray ray, out RaycastHit hit)
    {
        
        bool result = Physics.Raycast(ray, out hit);
        if (!result)
        {
            Debug.DrawLine(ray.origin, ray.direction);
        } else
        {
            Debug.DrawLine(ray.origin, hit.point);
        }
        return result;
    }

}
