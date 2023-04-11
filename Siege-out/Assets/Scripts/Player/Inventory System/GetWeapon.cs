using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetWeapon : MonoBehaviour
{
   private bool isSword;
    private GameObject player;
    private int CurrentSlot;
    public int StoreWeaponID;
    private InventoryBehaviour inventoryBehaviour;
    int RemainAmmo;
    int TotalAmmo;
    private void Start()
    {
        player = GameObject.Find("FPSController");
        inventoryBehaviour = player.GetComponent<InventoryBehaviour>();


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

        public void ObtainWeapon()
        {
            CurrentSlot = inventoryBehaviour.GetCurrentSlot();

        GameObject weaponObject = inventoryBehaviour.WeaponId[StoreWeaponID];
        GunController gunController = weaponObject.GetComponent<GunController>();
        if (gunController != null)
        {
            isSword = false;
            TotalAmmo = gunController.GetTotalAmmo();
            RemainAmmo = gunController.GetRemainAmmo();
        } else
        {
            isSword = true;
        }
        
        inventoryBehaviour.SetInventorySlot(CurrentSlot, StoreWeaponID, RemainAmmo, TotalAmmo, isSword);
            inventoryBehaviour.EquipWeapon(CurrentSlot);
        
        }

        public void DestroyItem()
        {
            Destroy(gameObject);
        }
    }
