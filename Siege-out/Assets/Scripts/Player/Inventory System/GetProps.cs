using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation.Examples;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(GetProps))]
public class GetPropsEditor : Editor
{
    private SerializedProperty propType;
    private SerializedProperty addAmount;
    private SerializedProperty pickupSound;
    private SerializedProperty modifierType;
    private SerializedProperty modifierMultiplier;

    // Add these variables
    private SerializedProperty siegeModel1;
    private SerializedProperty siegeModel2;

    void OnEnable()
    {
        propType = serializedObject.FindProperty("propType");
        addAmount = serializedObject.FindProperty("AddAmount");
        pickupSound = serializedObject.FindProperty("pickupSound");
        modifierType = serializedObject.FindProperty("modifierType");
        modifierMultiplier = serializedObject.FindProperty("modiferMultiplier");

        // Get the serialized properties for the siege models
        siegeModel1 = serializedObject.FindProperty("siegeModel1");
        siegeModel2 = serializedObject.FindProperty("siegeModel2");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(propType);

        GetProps.PropType propTypeEnum = (GetProps.PropType)propType.enumValueIndex;
        if (propTypeEnum == GetProps.PropType.UpgradeSiege)
        {
            EditorGUILayout.Space();
            // Display the modifierType variable
            EditorGUILayout.LabelField("Modifier", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(modifierType);
            EditorGUILayout.PropertyField(modifierMultiplier);
            // Display the siege model variables
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Target Modifier", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(siegeModel1);
            EditorGUILayout.PropertyField(siegeModel2);
        }
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Other Settings", EditorStyles.boldLabel);
        if (propTypeEnum != GetProps.PropType.UpgradeSiege)
        {
            EditorGUILayout.PropertyField(addAmount);
        }
        EditorGUILayout.PropertyField(pickupSound);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
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
        Objective,
        UpgradeSiege
    }
    public enum ModifierType
    {
        Speed,
        Health,
        Damage
    }

    [SerializeField]
    private ModifierType modifierType;
    [SerializeField]
    private float modiferMultiplier = 1f;
    // Add these variables
    [SerializeField]
    private GameObject siegeModel1;

    [SerializeField]
    private GameObject siegeModel2;
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
            case PropType.UpgradeSiege:
                switch (modifierType)
                {
                    case ModifierType.Speed:
                        setSiegeStats(0, modiferMultiplier);
                        break;
                    case ModifierType.Health:
                        setSiegeStats(1, modiferMultiplier);
                        break;
                    case ModifierType.Damage:
                        setSiegeStats(2, modiferMultiplier);
                        break;
                }
                GetProps[] props = FindObjectsOfType<GetProps>();
                foreach (GetProps prop in props)
                {
                    if (prop.propType == PropType.UpgradeSiege)
                    {
                        Destroy(prop.gameObject);
                    }
                }
                break;
        }
    }
  private SiegeBehaviour siegeBehaviour1;
  private SiegeBehaviour siegeBehaviour2;
  private HealthBehaviour healthBehaviour1;
  private HealthBehaviour healthBehaviour2;
  private PathFollower pathFollower1;
  private PathFollower pathFollower2;
    public void setSiegeStats(int type, float multiplier)
    {

        if (siegeModel1 != null && siegeModel2 != null)
        {
             siegeBehaviour1 = siegeModel1.GetComponent<SiegeBehaviour>();
             siegeBehaviour2 = siegeModel2.GetComponent<SiegeBehaviour>();
             healthBehaviour1 = siegeModel1.GetComponent<HealthBehaviour>();
             healthBehaviour2 = siegeModel2.GetComponent<HealthBehaviour>();
             pathFollower1 = siegeModel1.GetComponent<PathFollower>();
             pathFollower2 = siegeModel2.GetComponent<PathFollower>();
        }
        switch (type)
        {
            case 0:
               if(pathFollower1 != null && pathFollower2 != null)
                {
                    pathFollower1.speed *= multiplier;
                    pathFollower2.speed *= multiplier;
                }

                break;
            case 1:
                if (healthBehaviour1 != null && healthBehaviour2 != null)
                {
                    healthBehaviour1.SetHealth(healthBehaviour1.GetInitialHealth() * multiplier);
                    healthBehaviour2.SetHealth(healthBehaviour2.GetInitialHealth() * multiplier);
                }
                break;
            case 2:
                if (siegeBehaviour1 != null && siegeBehaviour2 != null)
                {
                    siegeBehaviour1.setDamage(siegeBehaviour1.getDamage() * multiplier);
                    siegeBehaviour2.setDamage(siegeBehaviour2.getDamage() * multiplier);
                }
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
