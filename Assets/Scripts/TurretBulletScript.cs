using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBulletScript : MonoBehaviour
{
    public float bulletSpeed;

    private Transform target;

    void Start()
    {
        Destroy(gameObject, 3f);
        target = GameObject.FindGameObjectWithTag("Player").transform;
        transform.LookAt(target);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * bulletSpeed * Time.deltaTime);
    }
}
