using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public static GunScript instance;
    public float shootCooldown;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public GameObject muzzleFlash;

    float currentShootTime;
    void Start()
    {
        instance = this;
    }

    void Update()
    {
        if(currentShootTime<=shootCooldown)
        {
            currentShootTime += Time.deltaTime;
        }

        shootCooldown = 1-GameManager.instance.ShootingRateSlider.value;
    }

    public void shoot()
    {
        if(currentShootTime>=shootCooldown)
        {
            Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
            GameObject muzzle = Instantiate(muzzleFlash, bulletSpawnPoint.position, Quaternion.identity);
            GameManager.instance.PlaySfx(GameManager.instance.PlayerShoot);
            Destroy(muzzle, 1f);

            currentShootTime = 0;
        }
    }
}
