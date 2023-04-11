using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{

    
    private Animator anim;
    


    [Header("Gun Clip Settings")]
    [SerializeField] private int TotalAmmo;

     [SerializeField] private int AmmoCount;

    [SerializeField] private int RemainAmmo;
    [SerializeField] private float ReloadingTime;
    [Header("Gun Attack Settings")]
    [SerializeField] private float ShootingTime;

    

    [SerializeField] private float Damage;
    [SerializeField] private float Range;
    [SerializeField] private int bulletCount = 1;
    [SerializeField] private float horizontalSpreadAngle = 1f;
    [SerializeField] private float verticalSpreadAngle = 1f;
    [Header("Aim Settings")]
    [SerializeField] private Vector3 AimPosition;
    private Vector3 OriginalPosition;
    [Header("Other Settings")]
    [SerializeField] private GameObject hitFX;
    public GameObject Gun;
    [SerializeField] private float SwitchingCD;
    [SerializeField] private GameObject muzzleFlashLight;
    [SerializeField] private ParticleSystem MuzzleFlash;
    [SerializeField] private AudioClip ShootSound;
    [SerializeField] private AudioClip ReloadSound;

    private float ReloadCD = 0;
    private bool isActive = false;
    private float ActiveTime = 0;
    private float ShootingCD = 0;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isShoot = false;
    private bool isReload = false;
    private bool isAim = false;
    private float iceRadius = 2f;
    private float iceThickness = 2f;
    private AudioSource audioSource;

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
  
    private void Start()
    {
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


            float speedMultiplier = 1.0f / SwitchingCD;
            gunAnimator.SetFloat("SpeedMultiplier", speedMultiplier);

            gunAnimator.Play("LoadUp");
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
        yield return new WaitForSeconds(ReloadingTime);
        


    }


   

    private IEnumerator Shoot()
    {

        isShoot = true;
        RemainAmmo--;
        muzzleFlashLight.SetActive(true);
        MuzzleFlash.Play();
        if (audioSource != null)
        {
            audioSource.clip = ShootSound;
            audioSource.Play();
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
                    Debug.Log("Hit enemy, dealt " + Damage + " damage.");
                    HealthBehaviour healthBehaviour = hit.collider.GetComponent<HealthBehaviour>();
                    healthBehaviour.TakeDamage(Damage);
                    if (Gun.name == "RayFreezer")
                    {
                        AddFreezeMeter(hit.collider.gameObject);
                        CreateIceAroundEnemy(hit.collider.gameObject);
                    }
                }
                if (hit.collider.CompareTag("Ice"))
                {
                    if (Gun.name == "RayFreezer")
                    {
                        CreateIce(hit);
                        MergeIce(hit);
                    }
                }

            }
        }
        
        
        StartCoroutine(StartRecoil());
        yield return new WaitForSeconds(ShootingTime*0.1f);
        MuzzleFlash.Stop();
        muzzleFlashLight.SetActive(false);
        yield return new WaitForSeconds(ShootingTime*0.9f);
        isShoot = false;
        
       
        //Update the Inventory UI
       
    }
    private void Aim()
    {
        GameObject canvas = GameObject.Find("PlayerUI");
        GameObject aim = canvas.transform.Find("Aim").gameObject;

        if (isReload == false)
        {
            if (Input.GetButton("Fire2"))
            {
                
                transform.localPosition = Vector3.Lerp(transform.localPosition, AimPosition, Time.deltaTime * 5f);
                if (isAim == false)
                {
                    horizontalSpreadAngle /= 2f;
                    verticalSpreadAngle /= 2f;
                    aim.SetActive(false);
                    isAim = true;
                }
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, OriginalPosition, Time.deltaTime * 5f);
                if (isAim == true)
                {
                    horizontalSpreadAngle *= 2f;
                    verticalSpreadAngle *= 2f;
                    aim.SetActive(true);
                    isAim = false;
                }
            }
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, OriginalPosition, Time.deltaTime * 5f);
            if (isAim == true)
            {
                horizontalSpreadAngle *= 2f;
                verticalSpreadAngle *= 2f;
                aim.SetActive(true);
                isAim = false;
            }
        }
    }
    private void UpdateInv()
    {
        GameObject Player = GameObject.Find("FPSController");
        InventoryBehaviour Inventory = Player.GetComponent<InventoryBehaviour>();
        Inventory.UpdateSlotTexts(RemainAmmo, TotalAmmo);
    }
    private void CreateIce(RaycastHit hit)
    {
        Collider[] colliders = Physics.OverlapSphere(hit.point, iceRadius, LayerMask.GetMask("Ice"));
        if (colliders.Length == 0)
        {
            GameObject ice = Instantiate(hitFX, hit.point, Quaternion.identity);
            ice.transform.localScale = new Vector3(iceRadius, iceThickness, iceRadius);
        }
    }

    // Merge ice if hit object is ice
    private void MergeIce(RaycastHit hit)
    {
        //Ice ice = hit.collider.GetComponent<Ice>();
        //if (ice != null)
        //{
        //    ice.Merge();
        //}
    }

    // Add a freeze meter and slow down enemy
    private void AddFreezeMeter(GameObject enemy)
    {
        //FreezeBehaviour freezeBehaviour = enemy.GetComponent<FreezeBehaviour>();
        //if (freezeBehaviour != null)
        //{
        //    //freezeBehaviour.AddFreezeMeter();
        //    SetMove(enemy, 1, 1, freezeBehaviour.GetFreezePercentage());
        //    CreateIceAroundEnemy(enemy);
        //}
    }

    // Set the move speed of an enemy based on freeze percentage
    private void SetMove(GameObject enemy, float runSpeed, float walkSpeed, float freezePercentage)
    {
        walkSpeed *= freezePercentage;
        runSpeed *= freezePercentage;
        AIController aiController = enemy.GetComponent<AIController>();

    }

    // Create ice around an enemy
    private void CreateIceAroundEnemy(GameObject enemy)
    {
        Vector3 position = enemy.transform.position;
        position.y -= enemy.GetComponent<CharacterController>().height / 2;
        GameObject ice = Instantiate(hitFX, position, Quaternion.identity);
        ice.transform.localScale = new Vector3(iceRadius, iceThickness, iceRadius);
        Vector3 topPosition = position;
        topPosition.y += enemy.GetComponent<CharacterController>().height;
        ice = Instantiate(hitFX, topPosition, Quaternion.identity);
        ice.transform.localScale = new Vector3(iceRadius, iceThickness, iceRadius);
    }
}