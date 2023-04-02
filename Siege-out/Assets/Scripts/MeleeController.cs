using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeController : MonoBehaviour
{
    public GameObject Weapon;
    [SerializeField] private float AttackSpeed;
    [SerializeField] private float AttackCD = 0;
    [SerializeField] private float Damage;
    public enum AttackModeType
    {
        Sword,
        Knife
    }
    [SerializeField] private AttackModeType AttackMode;
    private bool isAttack = false;
    void Update()
    {
     

        if (Input.GetButton("Fire1") && isAttack == false)
        {
            
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {

        isAttack = true;
        StartCoroutine(AttackAnim());
        yield return new WaitForSeconds(AttackSpeed);
        yield return new WaitForSeconds(AttackCD);
        isAttack = false;
      
    }

    private IEnumerator AttackAnim()
    {
       
        Animator weaponAnimator = Weapon.GetComponent<Animator>();
        float speedMultiplier = 1.0f / AttackSpeed;
        weaponAnimator.SetFloat("SpeedMultiplier", speedMultiplier);
        switch (AttackMode)
        {
            case AttackModeType.Sword:
                Weapon.GetComponent<Animator>().Play("SwordSwing");
                break;
            case AttackModeType.Knife:
                Weapon.GetComponent<Animator>().Play("KnifeSwing");
                break;
            default:
                Debug.LogWarning("Invalid AttackMode: " + AttackMode);
                break;
        }

 

        
        yield return new WaitForSeconds(weaponAnimator.GetCurrentAnimatorStateInfo(0).length / speedMultiplier);
        Weapon.GetComponent<Animator>().Play("Idle");
    }


}