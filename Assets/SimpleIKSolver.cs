using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimpleIKSolver : MonoBehaviour
{
    
    public bool follow;
    public Transform target;
    public List<SimpleIKBone> bones;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (!Input.GetButton("Jump")) { return; }
        var fixedPoint = bones.Last().transform.position;
        var targetPoint = target.position;
        for (int i = 0; i < bones.Count; i++)
        {
            var bone = bones[i];
            //bone.transform.rotation = Quaternion.LookRotation(targetPoint - bone.transform.position, Vector3.forward) * Quaternion.AngleAxis(90f, Vector3.right);

            bone.transform.rotation = Quaternion.FromToRotation( Vector3.up, targetPoint - bone.transform.position);

            bone.transform.position = targetPoint - bone.transform.up * bone.length;
            targetPoint = bone.transform.position;
        }
        
        targetPoint -= fixedPoint;
        if (follow)
        {
            return;
        }
        //targetPoint *= Mathf.Exp(-Time.deltaTime / followTime);

        for (int j = 0; j < bones.Count; j++)
        {
            var bone = bones[j];
            bone.transform.position = bone.transform.position - targetPoint;
        }

    }
}
