using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GetWeapon : MonoBehaviour
{
    private int StoreTotalAmmo;
    private int StoreRemainAmmo;
    private bool isSword;
    private GameObject player;
    private int CurrentSlot;
    public int StoreWeaponID;
    private InventoryBehaviour inventoryBehaviour;
    [SerializeField] private int RemainAmmo;
    [SerializeField] private int TotalAmmo;
    private void Start()
    {
        player = GameObject.Find("FPSController");
        inventoryBehaviour = player.GetComponent<InventoryBehaviour>();

    }
    public void SetTotalAmmo(int TAmmo)
    {
        StoreTotalAmmo = TAmmo;
    }
    public void SetRemainAmmo(int RAmmo)
    {
        StoreRemainAmmo = RAmmo;
    }
    public int TotalAmmoSet
    {
        get { return TotalAmmo; }
        set { TotalAmmo = value; }
    }

    public int RemainAmmoSet
    {
        get { return RemainAmmo; }
        set { RemainAmmo = value; }
    }
    private void OnValidate()
    {
        // Show the name of the weapon corresponding to the ObtainWeaponID value
        if (inventoryBehaviour != null && inventoryBehaviour.WeaponId.Length > StoreWeaponID)
        {
            string weaponName = inventoryBehaviour.WeaponId[StoreWeaponID].name;
            Debug.Log($"Weapon name: {weaponName}");
        }
    }
    private int num;
    private GameObject DropweaponObject;
#if UNITY_EDITOR
[ReadOnly] 
#endif
public bool isOriginal = false;
    private int[] store = new int[2];
    public void ObtainWeapon()
    {
        CurrentSlot = inventoryBehaviour.GetCurrentSlot();
        
        GameObject weaponObject = inventoryBehaviour.WeaponId[StoreWeaponID];
        
      DropweaponObject = inventoryBehaviour.DropWeaponId[inventoryBehaviour.Inventory[CurrentSlot,0]];
        
               
               
            
        
       
        GunController gunController = weaponObject.GetComponent<GunController>();
        Transform firstPersonController = player.transform.GetChild(0);
        Transform WeaponTransform = firstPersonController.GetChild(0);
        GunController PlayergunController = WeaponTransform.GetComponent<GunController>();
        if (gunController != null)
        {
            isSword = false;

            store[1] = TotalAmmo;
            store[0] = RemainAmmo;
            if (PlayergunController != null)
            {
                TotalAmmo = PlayergunController.GetTotalAmmo();
                RemainAmmo = PlayergunController.GetRemainAmmo();
            }
            
        } else
        {
            isSword = true;
        }
        if (DropweaponObject != null)
        {
            GameObject DroppedWeapon = Instantiate(DropweaponObject);
            DroppedWeapon.transform.position = gameObject.transform.position;
            GetWeapon getWeapon = DroppedWeapon.GetComponent<GetWeapon>();
            
            getWeapon.RemainAmmo = RemainAmmo;
            getWeapon.TotalAmmo = TotalAmmo;

            inventoryBehaviour.SetInventorySlot(CurrentSlot, StoreWeaponID, store[0], store[1], isSword);
            inventoryBehaviour.EquipWeapon(CurrentSlot);

        }

        DropweaponObject = null;
    }
    public void DestroyItem()
    {
        Destroy(gameObject);
    }
}
