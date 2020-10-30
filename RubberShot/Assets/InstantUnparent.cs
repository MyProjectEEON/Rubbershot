using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantUnparent : MonoBehaviour
{
    void Start()
    {
        if (transform.parent)
        {
            transform.parent = transform.parent.parent;
        }
    }

}
