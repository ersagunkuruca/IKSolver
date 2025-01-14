using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePosition : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float s = 0.1f;
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 10f);
        Debug.DrawLine(transform.position - s * Vector3.up, transform.position + s * Vector3.up);
        Debug.DrawLine(transform.position - s * Vector3.right, transform.position + s * Vector3.right);
    }
}
