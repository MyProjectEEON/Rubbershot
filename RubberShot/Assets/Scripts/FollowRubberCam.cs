using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowRubberCam : MonoBehaviour
{
    public Rubber rubber;
    [SerializeField] Transform cam;
    [SerializeField] Transform camParent;

    [SerializeField] float sensitivity;

    void Start()
    {
        
    }

    void Update()
    {
        transform.position = rubber.transform.position;
        transform.Rotate(transform.up, Input.GetAxis("Mouse X") * sensitivity, Space.World);
        camParent.Rotate(camParent.right, -Input.GetAxis("Mouse Y") * sensitivity, Space.World);
    }
}
