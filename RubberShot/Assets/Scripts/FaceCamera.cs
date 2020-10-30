using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [SerializeField] bool Follow;

    void Start()
    {
        transform.LookAt(Camera.main.transform.position);
    }

    void Update()
    {
        if (Follow)
        {
            transform.LookAt(Camera.main.transform.position);
        }
    }
}
