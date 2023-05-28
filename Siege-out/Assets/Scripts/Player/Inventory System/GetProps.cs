using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetProps : MonoBehaviour
{

    private GameObject player;
        [SerializeField]
    private PropType propType;
    [SerializeField]
    private int AddAmount = 0;
    [SerializeField] private GameObject pickupSound;
    private float time = 0;
    private bool isNotVaild = false;
    private GameObject mainGameObject;

    MainGame mainGameScript;
    public enum PropType
    {
        Ammo,
        Health,
        TargetItem,
        Objective
    }
  
    private void Update()
    {
        if(isNotVaild == true)
        {
            if(propType == PropType.Ammo)
            {
                ShowMessage("Not Vaild Type!");
            }
            
        }
    }
    private void Awake()
    {
         player = GameObject.Find("FPSController");
    }
    private void Start()
    {
        mainGameObject = GameObject.FindWithTag("MainGame");
        mainGameScript = mainGameObject.GetComponent<MainGame>();
        
    }
public void GetProp()
    {
        if(pickupSound != null)
        {
            GameObject pickupSoundObj = Instantiate(pickupSound, transform.position, Quaternion.identity);
        }
        PlayerUI playerUI = player.GetComponent<PlayerUI>();
        isNotVaild = false;
        switch (propType) 
        {

            case PropType.Ammo:

                InventoryBehaviour inventoryBehaviour = player.GetComponent<InventoryBehaviour>();

                GunController gunController = player.GetComponentInChildren<GunController>();
                if (gunController != null)
                {
                    inventoryBehaviour.SettotalAmmo(AddAmount);
                    Destroy(gameObject);
                } else
                {
                    isNotVaild = true;
                   
                }
                
                break;
            case PropType.Health:
                HealthBehaviour healthBehaviour = player.GetComponent<HealthBehaviour>();
                float HP = healthBehaviour.GetHealth();
                if(HP < healthBehaviour.GetInitialHealth())
                {
                    
                    healthBehaviour.SetHealth(AddAmount);

                    Destroy(gameObject);
                }
                
                break;
            case PropType.TargetItem:
               
            
                mainGameScript.setCurrentItemCount(1);
                Destroy(gameObject);
                break;
            case PropType.Objective:
              
                mainGameScript.setCurrentItemCount(1);
                
                break;
        }
    }
    public void DestroyItem()
    {
    Destroy(gameObject);
    }
    private void ShowMessage(string Text)
    {
        PlayerUI playerUI = player.GetComponent<PlayerUI>();
        time += Time.deltaTime;
        playerUI.UpdateText(Text);
        if (time >= 4f) 
        {

            time = 0;
            playerUI.UpdateText("");
            isNotVaild = false;
        }
    }
}
