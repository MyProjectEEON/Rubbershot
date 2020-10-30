using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] float time;

    void Start()
    {
        Invoke("Destroy_", time);
    }
    
    void Destroy_()
    {
        Destroy(gameObject);
    }
}
