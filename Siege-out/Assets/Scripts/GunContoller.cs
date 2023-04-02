using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunContoller : MonoBehaviour
{
    

    private Animator anim;
    [SerializeField] private float TotalAmmo;

    [SerializeField] private float AmmoCount;

    [SerializeField] private float RemainAmmo;

    [SerializeField] private float ShootingTime;

    [SerializeField] private float ReloadingTime;

    [SerializeField] private float Damage;
    [SerializeField] private float MaxDistance;
    [SerializeField] private GameObject hitFX;
    [SerializeField] private int bulletCount = 1;
    [SerializeField] private float horizontalSpreadAngle = 1f;
    [SerializeField] private float verticalSpreadAngle = 1f;
    public GameObject Gun;
    private float ReloadCD = 0;

    private float ShootingCD = 0;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isShoot = false;



    private void Awake()
    {
        
    }
    private void Update()

    {

        ShootDetect();

        Reload();

    

    }



    private void ShootDetect()
    {
        if (Input.GetButton("Fire1") && ReloadCD <= 0 && ShootingCD <= 0 && RemainAmmo > 0 && !isShoot)
        {
            

               
                StartCoroutine(Shoot());
            
            
        }
    }


    private void Reload()

    {

        if (RemainAmmo <= 0)

        {
            StartCoroutine(StartReload());
            ReloadCD += Time.deltaTime;

            if (ReloadCD >= ReloadingTime)

            {
                
                RemainAmmo = AmmoCount;

                ReloadCD = 0;

            }

        }

    }

    IEnumerator StartRecoil()
    {
        Gun.GetComponent<Animator>().Play("GunRecoil");
        yield return new WaitForSeconds(ShootingTime);
        Gun.GetComponent<Animator>().Play("Recoil_idle");
    }
    IEnumerator StartReload()
    {
        Animator gunAnimator = Gun.GetComponent<Animator>();

  
        float speedMultiplier = 1.0f / ReloadingTime;
        gunAnimator.SetFloat("SpeedMultiplier", speedMultiplier);

        gunAnimator.Play("Reload");
        yield return new WaitForSeconds(ReloadingTime-0.1f);
       



    }





    private IEnumerator Shoot()
    {
        

       
       


        Vector3 raycastOrigin = Camera.main.transform.position + Vector3.up * 0.1f;
        LayerMask layerMask = LayerMask.GetMask("Default", "Wall", "Enemy");

        for (int i = 0; i < bulletCount; i++)
        {
            float randomHorizontalAngle = Random.Range(-horizontalSpreadAngle / 2, horizontalSpreadAngle / 2);
            float randomVerticalAngle = Random.Range(-verticalSpreadAngle / 2, verticalSpreadAngle / 2);
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraUp = Camera.main.transform.up;
            Vector3 cameraRight = Camera.main.transform.right;
            Quaternion horizontalRotation = Quaternion.AngleAxis(randomHorizontalAngle, cameraUp);
            Quaternion verticalRotation = Quaternion.AngleAxis(randomVerticalAngle, cameraRight);
            Quaternion spreadRotation = horizontalRotation * verticalRotation;

            // Calculate the spread direction by rotating the transformed camera's right vector with the spread rotation
            Vector3 spreadDirection = spreadRotation * cameraForward;

            // Draw a debug ray to show the direction of spreadDirection
            Debug.DrawRay(Camera.main.transform.position, spreadDirection * MaxDistance, Color.blue, 2f);

            RaycastHit hit;

            if (Physics.Raycast(Camera.main.transform.position, spreadDirection, out hit, MaxDistance, layerMask))
            {
                
                if (hit.collider.CompareTag("Enemy"))
                {
                    Debug.Log("Hit enemy, dealt " + Damage + " damage.");
                }
                else if (hit.collider.CompareTag("Wall"))
                {
                    GameObject fx = Instantiate(hitFX, hit.point, Quaternion.identity);
                    fx.transform.rotation = Quaternion.LookRotation(hit.normal);
                }

            }
        }
        
        
        StartCoroutine(StartRecoil());
        isShoot = true;
        yield return new WaitForSeconds(ShootingTime);
        isShoot = false;
        RemainAmmo--;
    }

}