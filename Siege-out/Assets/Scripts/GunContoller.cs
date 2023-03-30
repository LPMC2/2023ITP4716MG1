using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunContoller : MonoBehaviour
{

    [SerializeField] private float TotalAmmo;

    [SerializeField] private float AmmoCount;

    [SerializeField] private float RemainAmmo;

    [SerializeField] private float ShootingTime;

    [SerializeField] private float ReloadingTime;

    [SerializeField] private float Damage;



    private float ReloadCD = 0;

    private float ShootingCD = 0;

    private bool isShoot = false;


    private void Update()

    {

        ShootDetect();

        Reload();

        ShootCoolDown();

    }



    private void ShootDetect()

    {

        if (Input.GetButtonDown("Fire1") && ReloadCD <= 0 && ShootingCD <= 0)

        {

            Shoot();

        }

    }



    private void Reload()

    {

        if (RemainAmmo <= 0)

        {

            ReloadCD += Time.deltaTime;

            if (ReloadCD >= ReloadingTime)

            {

                RemainAmmo = AmmoCount;

                ReloadCD = 0;

            }

        }

    }



    private void ShootCoolDown()

    {
        if (isShoot == true)
        {
            if (ShootingCD > 0)

            {

                ShootingCD -= Time.deltaTime;

                if (ShootingCD < 0)

                {

                    ShootingCD = 0;
                    isShoot = false;
                }

            }
        }
    }



    private void Shoot()

    {

        RemainAmmo--;



        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out hit))

        {

            if (hit.collider.CompareTag("Enemy"))

            {

                Debug.Log("Hit enemy, dealt " + Damage + " damage.");

            }

        }



        Debug.DrawLine(transform.position, hit.point, Color.red, 2f);
        isShoot = true;
        

    }

}