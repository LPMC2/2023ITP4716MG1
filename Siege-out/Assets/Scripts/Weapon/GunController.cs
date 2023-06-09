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

    [SerializeField] private UnityEvent<GameObject> HitFunction;
    [SerializeField] private UnityEvent ShootFunction;
    
    [Header("Aim Settings")]
    [SerializeField] private Vector3 AimPosition;
    [SerializeField] private float BulletSpreadMultiplier = 1f;
    [SerializeField] private bool removeAimSymbol = true;
    private Vector3 OriginalPosition;

    [Header("Recoil Settings")]
    [SerializeField]
    private float recoilAngle = 5f;
    // The speed at which the camera rotates back to its original position
    [SerializeField]
    private float recoilSpeed = 10f;
    // The maximum recoil rotation angle
    [SerializeField]
    private float maxRecoilAngle = 20f;
    private bool isRecoiling = false;
    // The current recoil angle
#if UNITY_EDITOR
[ReadOnly]
#endif
    [SerializeField] private float currentRecoilAngle = 0f;
    private Transform cameraTransform;
    private GameObject cameraObject;
    [Header("Other Settings")]
    [SerializeField] private float SpeedMultiplier = 1f;
    [SerializeField] private GameObject hitFX;
    [SerializeField] private bool ignoreAnimation = false;
    [SerializeField] private bool ignoreAudio = false;
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
    private SwayNBobScript SNBScript;
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
    public int GetBulletCount()
    {
        return bulletCount;
    }
    public float GetRange()
    {
        return Range;
    }
    public float GetHorAngle()
    {
        return horizontalSpreadAngle;
    }
    public float GetVerAngle()
    {
        return verticalSpreadAngle;
    }
    public float GetSpreadMul()
    {
        return BulletSpreadMultiplier;
    }
    public bool getIsAim()
    {
        return isAim;
    }
    public void SetTotalAmmo(int Ch_TotalAmmo)
    {
        TotalAmmo = Ch_TotalAmmo;
        
    }
    public void SetRemainAmmo(int Ch_RemainAmmo)
    {
        RemainAmmo = Ch_RemainAmmo;
    }
    public void setSpreadAngle(float hor, float ver)
    {
        horizontalSpreadAngle = hor;
        verticalSpreadAngle = ver;
    }
    public void setSpreadMul(float newSpreadMul)
    {
        BulletSpreadMultiplier = newSpreadMul;
    }
    public void setRange(float range)
    {
        Range = range;
    }
  public float GetShootingTime()
    {
        return ShootingTime;
    }
    public bool getActiveState()
    {
        return isActive;
    }
    private void Start()
    {
        cameraTransform = transform.parent;
        Player = GameObject.Find("FPSController");
        Inventory = Player.GetComponent<InventoryBehaviour>();
        GetSound();
        MuzzleFlash.Stop();
        OriginalPosition = transform.localPosition;
        PlayerMotor playerMotor = Player.GetComponent<PlayerMotor>();
        playerMotor.setSpeed(SpeedMultiplier);
        SNBScript = GetComponent<SwayNBobScript>();
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
        if (!isRecoiling)
        {
            currentRecoilAngle = Mathf.Lerp(currentRecoilAngle, 0f, recoilSpeed * Time.deltaTime);
        }
        // Rotate the camera back to its original position if the camera is recoiling, but clamp the angle to the current recoil angle
        else
        {
            currentRecoilAngle = Mathf.Clamp(currentRecoilAngle, 0f, maxRecoilAngle);
            currentRecoilAngle = Mathf.Lerp(currentRecoilAngle, 0f, recoilSpeed * Time.deltaTime);
            isRecoiling = currentRecoilAngle > 0f; // Set the flag to false if the recoil angle has reached 0
        }
        cameraTransform.Rotate(Vector3.left, currentRecoilAngle);
        if (isOut == false && RemainAmmo <=0 && TotalAmmo <= 0)
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
        if(isShoot == true)
        {
            fireCoolDown();
        }
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
            if(ReloadCD == 0)
            {
                ReloadFunction.Invoke();
                SNBScript.setPos(OriginalPosition);
            }
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
            if (ignoreAnimation == false)
            {
                gunAnimatorRecoil.GetComponent<Animator>().Play("GunRecoil");
            } else
            {
                if (gunAnimatorRecoil.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
                {
                    gunAnimatorRecoil.GetComponent<Animator>().Play("GunRecoil");
                }
            }
        }
        yield return new WaitForSeconds(ShootingTime);
        if (gunAnimatorRecoil != null)
        {
            if (ignoreAnimation == false)
            {
                gunAnimatorRecoil.GetComponent<Animator>().Play("Recoil_idle");
            } else
            {
                if (gunAnimatorRecoil.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
                {
                    gunAnimatorRecoil.GetComponent<Animator>().Play("Recoil_idle");
                }
            }
        }
    }
    IEnumerator StartReload()
    {
        
        Animator gunAnimator = Gun.GetComponent<Animator>();
        if (gunAnimator != null)
        {

            float speedMultiplier = 1.0f / ReloadingTime;
            gunAnimator.SetFloat("SpeedMultiplier", speedMultiplier);

            gunAnimator.Play("Reload");
        }
        yield return new WaitForSeconds(ReloadingTime);
        


    }

    private void recoilMove()
    {
        if (cameraTransform != null)
        {
            if (!isRecoiling)
            {
                currentRecoilAngle += recoilAngle;
                currentRecoilAngle = Mathf.Clamp(currentRecoilAngle, 0f, maxRecoilAngle); // Clamp the angle to the maximum value
                isRecoiling = true;
            }
            else // Add the recoil angle to the current recoil angle again if the camera is already recoiling
            {
                currentRecoilAngle += recoilAngle;
                currentRecoilAngle = Mathf.Clamp(currentRecoilAngle, 0f, maxRecoilAngle); // Clamp the angle to the maximum value
            }
        }
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
           if(ignoreAudio == true) 
            {
            if(audioSource.isPlaying == false)
                {
                    audioSource.PlayOneShot(ShootSound, 1);
                }
            } else
            {
                audioSource.PlayOneShot(ShootSound, 1);
            }
            
        }
        recoilMove();
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
                HitFunction.Invoke(hit.collider.gameObject);
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
                if (hit.collider.CompareTag("Dummy"))
                {
                    
                    HitScore hitScore = hit.collider.GetComponent<HitScore>();
                    if (hitScore != null)
                    {
                        if (hitScore.enabled)
                        {
                            hitScore.Hit();
                        }
                        if (hitFX != null)
                        {
                            GameObject fx = Instantiate(hitFX, hit.point, Quaternion.identity);
                            fx.transform.rotation = Quaternion.LookRotation(hit.normal);
                        }
                    }
                }
                if (hit.collider.CompareTag("Enemy"))
                {
                    HealthBehaviour healthBehaviour = hit.collider.GetComponent<HealthBehaviour>();
                    if (healthBehaviour != null)
                    {
                        if (!hitEnemies.Contains(hit.collider.gameObject))
                        {
                            healthBehaviour.TakeDamage(Damage);
                            hitEnemies.Add(hit.collider.gameObject);
                        }
                        if (isPiercing == false)
                        {
                            
                            hitEnemies.Clear();
                            if (bulletCount < 2)
                            {
                                break;
                            }
                        }
                        else if (isPiercing)
                        {

                            continue;
                        }
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
            hitEnemies.Clear();

        fireTimer = 0f;
       
        //Update the Inventory UI
       
    }
    private float fireTimer = 0f;
    private void fireCoolDown()
    {
        fireTimer += Time.deltaTime;
        if(fireTimer >= ShootingTime * 2)
        {
            isShoot = false;
            if (MuzzleFlash != null)
            {
                MuzzleFlash.Stop();
            }
            if (muzzleFlashLight != null)
            {
                muzzleFlashLight.SetActive(false);
            }

            fireTimer = 0f;
        }
    }
    private void Aim()
    {

       

        if (isReload == false)
        {
            if (Input.GetButton("Fire2"))
            {
                
                transform.localPosition = Vector3.Lerp(transform.localPosition, AimPosition, Time.deltaTime * 5f);
                if (SNBScript != null)
                {
                    SNBScript.setPos(AimPosition);
                }
                if (isAim == false)
                {
                    horizontalSpreadAngle *= BulletSpreadMultiplier;
                    verticalSpreadAngle *= BulletSpreadMultiplier;
                    if (removeAimSymbol == true)
                    {
                        Inventory.setCrossHairState(false);
                    }
                    isAim = true;
                }
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, OriginalPosition, Time.deltaTime * 5f);
                if (SNBScript != null)
                {
                    SNBScript.setPos(OriginalPosition);
                }
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
    public void UpdateInv()
    {
        if (Inventory != null)
        {
            Inventory.UpdateSlotTexts(RemainAmmo, TotalAmmo);
        }
    }

}