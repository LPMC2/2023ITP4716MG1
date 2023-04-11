using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeController : MonoBehaviour
{
    public GameObject Weapon;
    [SerializeField] private float AttackSpeed;
    [SerializeField] private float AttackCD = 0;
    [SerializeField] private float Damage;
    [SerializeField] private float hitboxSizeX = 1f;
    [SerializeField] private float hitboxSizeY = 1f;
    [SerializeField] private float maxDistance = 1f;
    [SerializeField] private AudioClip AttackSound;
    private AudioSource audioSource;
    public enum AttackModeType
    {
        Sword,
        Knife,
        HeavyStrike
    }
    [SerializeField] private AttackModeType AttackMode;
    private bool isAttack = false;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
     

        if (Input.GetButton("Fire1") && isAttack == false)
        {
            
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        if (audioSource != null)
        {
            audioSource.clip = AttackSound;
            audioSource.Play();
        }
        isAttack = true;
        StartCoroutine(AttackAnim());
        yield return new WaitForSeconds(AttackSpeed/2f);
        hitbox();
        
        yield return new WaitForSeconds(AttackSpeed/2f);
        yield return new WaitForSeconds(AttackCD);
        isAttack = false;
        if (audioSource != null)
        {
            audioSource.clip = null;
        }
    }
    private HashSet<GameObject> hitObjects = new HashSet<GameObject>();

    private void hitbox()
    {

        // Calculate the area of effect
        // Calculate the area of effect
        float halfWidth = hitboxSizeX / 2f;
        float halfHeight = hitboxSizeY / 2f;
        Vector3 origin = Camera.main.transform.position + Camera.main.transform.forward;
        Vector3 corner1 = origin + Camera.main.transform.right * halfWidth + Camera.main.transform.up * halfHeight;
        Vector3 corner2 = origin - Camera.main.transform.right * halfWidth - Camera.main.transform.up * halfHeight;

        // Cast the boxcast
        Vector3 size = new Vector3(halfWidth, halfHeight, maxDistance);
        RaycastHit[] hits = Physics.BoxCastAll(origin, size, Camera.main.transform.forward * -1f, Quaternion.identity, maxDistance);
        Vector3[] corners = new Vector3[8];
        corners[0] = origin + Camera.main.transform.right * halfWidth + Camera.main.transform.up * halfHeight;
        corners[1] = origin + Camera.main.transform.right * halfWidth - Camera.main.transform.up * halfHeight;
        corners[2] = origin - Camera.main.transform.right * halfWidth - Camera.main.transform.up * halfHeight;
        corners[3] = origin - Camera.main.transform.right * halfWidth + Camera.main.transform.up * halfHeight;
        corners[4] = corners[0] + Camera.main.transform.forward * maxDistance;
        corners[5] = corners[1] + Camera.main.transform.forward * maxDistance;
        corners[6] = corners[2] + Camera.main.transform.forward * maxDistance;
        corners[7] = corners[3] + Camera.main.transform.forward * maxDistance;
        Debug.DrawRay(corners[0], corners[1] - corners[0], Color.red, 2f);
        Debug.DrawRay(corners[1], corners[2] - corners[1], Color.red, 2f);
        Debug.DrawRay(corners[2], corners[3] - corners[2], Color.red, 2f);
        Debug.DrawRay(corners[3], corners[0] - corners[3], Color.red, 2f);
        Debug.DrawRay(corners[4], corners[5] - corners[4], Color.red, 2f);
        Debug.DrawRay(corners[5], corners[6] - corners[5], Color.red, 2f);
        Debug.DrawRay(corners[6], corners[7] - corners[6], Color.red, 2f);
        Debug.DrawRay(corners[7], corners[4] - corners[7], Color.red, 2f);
        Debug.DrawRay(corners[0], corners[4] - corners[0], Color.red, 2f);
        Debug.DrawRay(corners[1], corners[5] - corners[1], Color.red, 2f);
        Debug.DrawRay(corners[2], corners[6] - corners[2], Color.red, 2f);
        Debug.DrawRay(corners[3], corners[7] - corners[3], Color.red, 2f);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                if (!hitObjects.Contains(hit.collider.gameObject)) // Check if the object has already been hit
                {
                    hitObjects.Add(hit.collider.gameObject); // Add the object to the HashSet
                    IDamageable damageable = hit.collider.gameObject.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(Damage);
                    }
                }
            }
            if (hit.collider.CompareTag("Wall"))
            {
                if (!hitObjects.Contains(hit.collider.gameObject)) // Check if the object has already been hit
                {
                    hitObjects.Add(hit.collider.gameObject); // Add the object to the HashSet
                    Destructable destructable = hit.collider.gameObject.GetComponent<Destructable>();
                    if (destructable != null)
                    {
                        destructable.DestroyObject(hit, Damage, hit.collider.gameObject, false);
                    }
                }
            }
        }
        hitObjects.Clear();
    }
    private IEnumerator AttackAnim()
    {
       
        Animator weaponAnimator = Weapon.GetComponent<Animator>();
        float speedMultiplier = (1.0f / AttackSpeed);
        weaponAnimator.SetFloat("SpeedMultiplier", speedMultiplier);
        switch (AttackMode)
        {
            case AttackModeType.Sword:
                Weapon.GetComponent<Animator>().Play("SwordSwing");
                break;
            case AttackModeType.Knife:
                Weapon.GetComponent<Animator>().Play("KnifeSwing");
                break;
            case AttackModeType.HeavyStrike:
                Weapon.GetComponent<Animator>().Play("HeavySwing");
                break;
            default:
                Debug.LogWarning("Invalid AttackMode: " + AttackMode);
                break;
        }

 

        
        yield return new WaitForSeconds(weaponAnimator.GetCurrentAnimatorStateInfo(0).length / speedMultiplier);
        Weapon.GetComponent<Animator>().Play("Idle");
    }


}