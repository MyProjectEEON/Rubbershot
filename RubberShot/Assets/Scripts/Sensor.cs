using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    public float Health;
    public GameObject DeathExplosion;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Die()
    {
        Instantiate(DeathExplosion, transform.position, transform.rotation);

        Destroy(gameObject);
    }
}
