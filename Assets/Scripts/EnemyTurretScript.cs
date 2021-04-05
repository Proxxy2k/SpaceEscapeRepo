using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurretScript : MonoBehaviour
{
    public Transform[] Raycasters;
    public float turretHealth;
    public GameObject turretDestroyParticle;
    public float shootCooldown;
    public GameObject turretBulletPrefab;
    public Transform bulletSpawnPoint;
    public GameObject muzzleFlash;

    RaycastHit hitInfo;
    float currentShootTime;
    bool isShotOnce;

    void Update()
    {
        for(int i=0; i<Raycasters.Length; i++)
        {
            bool ishit = Physics.Raycast(Raycasters[i].position, -transform.forward,out hitInfo,10);
            if(ishit)
            {
                if(hitInfo.transform.CompareTag("Player"))
                {
                    //shoot turret
                    ShootTurret();
                }
            }
        }

        if (currentShootTime <= shootCooldown)
        {
            currentShootTime += Time.deltaTime;
        }

        if(turretHealth<=0)
        {
            Instantiate(turretDestroyParticle, transform.position, Quaternion.identity);
            GameManager.instance.PlaySfx(GameManager.instance.TurretExplosion);
            Destroy(gameObject);
        }

        //for changing attributes
        //health doesnt change once turret is shot
        if(!isShotOnce)
        {
            turretHealth = GameManager.instance.EnemyHealth.value;
        }
        shootCooldown = 1 - GameManager.instance.EnemyShootingRateSlider.value;
    }

    public void ShootTurret()
    {
        if (currentShootTime >= shootCooldown)
        {
            Instantiate(turretBulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
            GameObject muzzle = Instantiate(muzzleFlash, bulletSpawnPoint.position, Quaternion.identity);
            GameManager.instance.PlaySfx(GameManager.instance.TurretShoot);
            Destroy(muzzle, 1f);

            currentShootTime = 0;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("PlayerBullet"))
        {
            isShotOnce = true;
            turretHealth--;
            if(collision.gameObject.activeInHierarchy)
            {
                Destroy(collision.gameObject);
            }
        }
    }
}
