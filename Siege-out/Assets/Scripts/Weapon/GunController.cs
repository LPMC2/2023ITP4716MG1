using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class GunController : MonoBehaviour
{

  
    private Animator anim;


    [Header("Gun Clip Settings")]


    [SerializeField] private int TotalAmmo;

     [SerializeField] private int AmmoCount;

    [SerializeField] private int RemainAmmo;
    [SerializeField] private float ReloadingTime;
    [SerializeField] private UnityEvent ReloadFunction;
    [Header("Gun Attack Settings")]
 

    [SerializeField] private float ShootingTime;

    [SerializeField] private float Damage;
    [SerializeField] private float Range;
    [SerializeField] private int bulletCount = 1;
    [SerializeField] private float horizontalSpreadAngle = 1f;
    [SerializeField] private float verticalSpreadAngle = 1f;
    [SerializeField] private bool isPiercing = false;
    [SerializeField] private UnityEvent HitFunction;
    [SerializeField] private UnityEvent ShootFunction;
    [Header("Aim Settings")]
    [SerializeField] private Vector3 AimPosition;
    [SerializeField] private float BulletSpreadMultiplier = 1f;
    private Vector3 OriginalPosition;
    [Header("Other Settings")]
   
    [SerializeField] private GameObject hitFX;
    public GameObject Gun;
    [SerializeField] private float SwitchingCD;
    [SerializeField] private GameObject muzzleFlashLight;
    [SerializeField] private ParticleSystem MuzzleFlash;
    [SerializeField] private AudioClip ShootSound;
    [SerializeField] private AudioClip ReloadSound;
    [HideInInspector]public bool isOut = false;
    private GameObject Player;
    private InventoryBehaviour Inventory;
    private float ReloadCD = 0;
    private bool isActive = false;
    private float ActiveTime = 0;
    private float ShootingCD = 0;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isShoot = false;
    private bool isReload = false;
    private bool isAim = false;

    private AudioSource audioSource;
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();
    public float GetDamage()
    {
        return Damage;
    }
    public int GetTotalAmmo()
    {
        return TotalAmmo;
    }
    public int GetRemainAmmo()
    {
        return RemainAmmo;
    }
    public int GetAmmoCount()
    {
        return AmmoCount;
    }
    public void SetTotalAmmo(int Ch_TotalAmmo)
    {
        TotalAmmo = Ch_TotalAmmo;
        
    }
    public void SetRemainAmmo(int Ch_RemainAmmo)
    {
        RemainAmmo = Ch_RemainAmmo;
    }
  public float GetShootingTime()
    {
        return ShootingTime;
    }
    private void Start()
    {
        Player = GameObject.Find("FPSController");
        Inventory = Player.GetComponent<InventoryBehaviour>();
        GetSound();
        MuzzleFlash.Stop();
        OriginalPosition = transform.localPosition;

    }
    private void GetSound()
    {
        audioSource = GetComponent<AudioSource>();


        
    }
    public void SetVolume(float Volume)
    {
       
        audioSource.volume *= Volume;

    }
    private void Update()

    {
        if(isOut == false && RemainAmmo <=0 && TotalAmmo <= 0)
        {
            isOut = true;
        } else if(isOut == true)
        {
            isOut = false;
        }
        if (isActive == false && SwitchingCD > 0) 
        { 
            SetActive(); 
        } else if (isActive == false && SwitchingCD == 0) 
        {
            isActive = true;
        }
        if (isActive == true)
        {
            ShootDetect();
            ReloadDetect();
            Reload();
            Aim();
        }

    }

    private void SetActive()
    {
        if(ActiveTime == 0)
        {
            Animator gunAnimator = Gun.GetComponent<Animator>();
            if (gunAnimator != null)
            {

                float speedMultiplier = 1.0f / SwitchingCD;
                gunAnimator.SetFloat("SpeedMultiplier", speedMultiplier);

                gunAnimator.Play("LoadUp");
            }
        }
        ActiveTime += Time.deltaTime;
        if(ActiveTime >= SwitchingCD)
        {
            isActive = true;
        }
        
    }

    private void ReloadDetect()
    {
        if (Input.GetKey(KeyCode.R) && ReloadCD <= 0 && RemainAmmo < AmmoCount && isShoot == false && isReload == false)
        {

            isReload = true;
        }
    }
    private void ShootDetect()
    {
        if (Input.GetButton("Fire1") && ReloadCD <= 0 && ShootingCD <= 0 && RemainAmmo > 0 && !isShoot && !isReload)
        {


          
                StartCoroutine(Shoot());
            
            
        }
    }

    private bool hasSound = false;
    private void Reload()

    {

        if ((isReload == true || RemainAmmo <= 0) && TotalAmmo > 0 && isShoot == false)
            
        {
            isReload = true;
            if (audioSource != null && hasSound == false)
            {
                audioSource.clip = ReloadSound;
                audioSource.Play();
                hasSound = true;
            }
            
            StartCoroutine(StartReload());
            ReloadCD += Time.deltaTime;

            if (ReloadCD >= ReloadingTime)

            {
                int StoreRemain = RemainAmmo;
                
                if (TotalAmmo >= AmmoCount)
                {
                    RemainAmmo = AmmoCount;
                } else if (TotalAmmo < AmmoCount && (TotalAmmo+StoreRemain <= AmmoCount))
                {
                    RemainAmmo = TotalAmmo+StoreRemain;
                    
                } else
                {
                    RemainAmmo = AmmoCount;
                }
                TotalAmmo -= (AmmoCount - StoreRemain);
                if (TotalAmmo < 0)
                {
                    TotalAmmo = 0;
                }
                UpdateInv();
                ReloadCD = 0;
                hasSound = false;
                if (audioSource != null)
                {
                    audioSource.clip = null;
                }
                InventoryBehaviour inventoryBehaviour = GameObject.Find("FPSController").GetComponent<InventoryBehaviour>();
                if(inventoryBehaviour)
                {
                    inventoryBehaviour.UpdateInventoryAmmo(TotalAmmo);
                }
                isReload = false;
            }

        }

    }

    IEnumerator StartRecoil()
    {
        Animator gunAnimatorRecoil = Gun.GetComponent<Animator>();
        if (gunAnimatorRecoil != null)
        {
            gunAnimatorRecoil.GetComponent<Animator>().Play("GunRecoil");
        }
        yield return new WaitForSeconds(ShootingTime);
        if (gunAnimatorRecoil != null)
        {
            gunAnimatorRecoil.GetComponent<Animator>().Play("Recoil_idle");
        }
    }
    IEnumerator StartReload()
    {
        ReloadFunction.Invoke();
        Animator gunAnimator = Gun.GetComponent<Animator>();
        if (gunAnimator != null)
        {

            float speedMultiplier = 1.0f / ReloadingTime;
            gunAnimator.SetFloat("SpeedMultiplier", speedMultiplier);

            gunAnimator.Play("Reload");
        }
        yield return new WaitForSeconds(ReloadingTime);
        


    }


   

    private IEnumerator Shoot()
    {
        ShootFunction.Invoke();
        isShoot = true;
        RemainAmmo--;
        if (muzzleFlashLight != null)
        {
            muzzleFlashLight.SetActive(true);
        }
        if (MuzzleFlash != null)
        {
            MuzzleFlash.Play();
        }
        if (audioSource != null)
        {
           
            audioSource.PlayOneShot(ShootSound, 1);
        }
        
        UpdateInv();

        Vector3 raycastOrigin = Camera.main.transform.position + Vector3.up * 0.1f;
        LayerMask layerMask = LayerMask.GetMask("Default", "Wall", "Enemy","Obstacles");

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
            Debug.DrawRay(Camera.main.transform.position, spreadDirection * Range, Color.blue, 2f);

            RaycastHit hit;

            if (Physics.Raycast(Camera.main.transform.position, spreadDirection, out hit, Range, layerMask))
            {
                if (hit.collider.CompareTag("Projectile"))
                {
                    continue;
                }
                
                if (hit.collider.CompareTag("Wall"))
                {
                    Destructable destructable = hit.collider.GetComponent<Destructable>();
                    if (destructable != null)
                    {
                        destructable.DestroyObject(hit, Damage, hitFX, true);
                    } else
                    if (hitFX != null)
                    {
                        GameObject fx = Instantiate(hitFX, hit.point, Quaternion.identity);
                        fx.transform.rotation = Quaternion.LookRotation(hit.normal);
                    }

                }
                if (hit.collider.CompareTag("Enemy"))
                {

                    HitFunction.Invoke();
                    HealthBehaviour healthBehaviour = hit.collider.GetComponent<HealthBehaviour>();
                    if (!isPiercing || !hitEnemies.Contains(hit.collider.gameObject))
                    {
                        healthBehaviour.TakeDamage(Damage);
                        hitEnemies.Add(hit.collider.gameObject);
                    }
                    if (Gun.name == "RayFreezer")
                    {

                    }
                    if (!isPiercing || i == bulletCount - 1)
                    {
                        hitEnemies.Clear();
                    }
                    if (isPiercing == true)
                    {
                        continue;
                    }
                }
                if (hit.collider.CompareTag("Ice"))
                {
                    if (Gun.name == "RayFreezer")
                    {

                    }
                }

            }
        }
        
        
        StartCoroutine(StartRecoil());
        yield return new WaitForSeconds(ShootingTime*0.1f);
        if (MuzzleFlash != null)
        {
            MuzzleFlash.Stop();
        }
        if (muzzleFlashLight != null)
        {
            muzzleFlashLight.SetActive(false);
        }
        yield return new WaitForSeconds(ShootingTime*0.9f);
        isShoot = false;
        
       
        //Update the Inventory UI
       
    }
    private void Aim()
    {

       

        if (isReload == false)
        {
            if (Input.GetButton("Fire2"))
            {
                
                transform.localPosition = Vector3.Lerp(transform.localPosition, AimPosition, Time.deltaTime * 5f);
                if (isAim == false)
                {
                    horizontalSpreadAngle *= BulletSpreadMultiplier;
                    verticalSpreadAngle *= BulletSpreadMultiplier;
                    Inventory.setCrossHairState(false);
                    isAim = true;
                }
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, OriginalPosition, Time.deltaTime * 5f);
                if (isAim == true)
                {
                    horizontalSpreadAngle /= BulletSpreadMultiplier;
                    verticalSpreadAngle /= BulletSpreadMultiplier;
                    Inventory.setCrossHairState(true);
                    isAim = false;
                }
            }
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, OriginalPosition, Time.deltaTime * 5f);
            if (isAim == true)
            {
                horizontalSpreadAngle /= BulletSpreadMultiplier;
                verticalSpreadAngle /= BulletSpreadMultiplier;
                Inventory.setCrossHairState(true);
                isAim = false;
            }
        }
    }
    private void UpdateInv()
    {

        Inventory.UpdateSlotTexts(RemainAmmo, TotalAmmo);
    }

}