using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Linq;
#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(InventoryBehaviour))]
public class InventoryBehaviourEditor : UnityEditor.Editor
{
    private bool showWeaponIds = true;
    private bool showDropWeaponIds = true;

public override void OnInspectorGUI()
{
    base.OnInspectorGUI();

    InventoryBehaviour inventory = (InventoryBehaviour)target;

    EditorGUILayout.Space();

    showWeaponIds = EditorGUILayout.Foldout(showWeaponIds, "IDs Manager", true, new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, normal = { textColor = Color.red }});

    if (showWeaponIds)
    {
        for (int i = 0; i < inventory.WeaponIds.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Add", GUILayout.Width(60)))
            {
                var tempList = new List<GameObject>(inventory.WeaponIds);
                tempList.Insert(i, null);
                inventory.WeaponIds = tempList.ToArray();
            }

            inventory.WeaponIds[i] = (GameObject)EditorGUILayout.ObjectField(inventory.WeaponIds[i], typeof(GameObject), true);

            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                var tempList = new List<GameObject>(inventory.WeaponIds);
                tempList.RemoveAt(i);
                inventory.WeaponIds = tempList.ToArray();

                if (i >= inventory.WeaponIds.Length)
                {
                    i--;
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    EditorGUILayout.Space();

    showDropWeaponIds = EditorGUILayout.Foldout(showDropWeaponIds, "Drop IDs Manager", true, new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, normal = { textColor = Color.red }});

    if (showDropWeaponIds)
    {
        for (int i = 0; i < inventory.DropWeaponId.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Add", GUILayout.Width(60)))
            {
                var tempList = new List<GameObject>(inventory.DropWeaponId);
                tempList.Insert(i, null);
                inventory.DropWeaponId = tempList.ToArray();
            }

            inventory.DropWeaponId[i] = (GameObject)EditorGUILayout.ObjectField(inventory.DropWeaponId[i], typeof(GameObject), true);

            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                var tempList = new List<GameObject>(inventory.DropWeaponId);
                tempList.RemoveAt(i);
                inventory.DropWeaponId = tempList.ToArray();

                if (i >= inventory.DropWeaponId.Length)
                {
                    i--;
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }
        if (GUI.changed)
    {
        EditorUtility.SetDirty(inventory);
    }
}
}
#endif
public class InventoryBehaviour : MonoBehaviour
{
#pragma warning disable CS0067
    public event System.Action<int> WeaponIdChanged;
    public GameObject canvas;
    [SerializeField] private Sprite SlotNormal;
    [SerializeField] private Sprite SlotSelect;
   [SerializeField] private Text[] slotTexts = new Text[3];
    private int currentUsingId = 0;
    private Image crosshair;
    private float lastScrollTime = 0f;
    int scrollNumber = 0;
    [Header("Item Settings")]
    [SerializeField]
    public GameObject[] WeaponId;
    [Header("Note for DropWeaponId: Must Match the Item from WeaponId")]
    [SerializeField]
    public GameObject[] DropWeaponId;
    [Header("Inventory Settings")]
    public int[,] Inventory = new int[3, 3] { { 0, 0, 0 }, { 1, 0, 0 }, { 2, 0, 0 } };
    [SerializeField] private float scrollCooldown = 0.5f;
    [SerializeField] private GameObject FirstPersonCharacter;
    [SerializeField] private int[] InitialInventorySlot = new int[3];
    private GameObject slotTracker;
    Transform[] camSlotTransforms = new Transform[3];
    Transform[] OutlineSlotTransforms = new Transform[3];
    private bool isPickUp = false;
    public GameObject[] WeaponIds
    {
        get { return WeaponId; }
        set { WeaponId = value; }
    }
    public GameObject[] DropWeaponIds
    {
        get { return DropWeaponId; }
        set { DropWeaponId = value; }
    }
    private void UpdateInventory()
    {
        for (int i = 0; i < 3; i++)
        {
            Inventory[i, 0] = InitialInventorySlot[i];
        }
    }
    public void SetInventorySlot(int currentID, int id, int rAmmo, int tAmmo, bool IsSword)
    {
        if (IsSword  == false) { 
        Inventory[currentID, 0] = id;
        Inventory[currentID, 1] = rAmmo;
        Inventory[currentID, 2] = tAmmo;
            isPickUp = true;
        } else
        {
            Inventory[currentID, 0] = id;
            Inventory[currentID, 1] = 0;
            Inventory[currentID, 2] = 0;
        }
    }
   public int GetCurrentSlot()
    {
        return currentUsingId;
    }
    public void SettotalAmmo(int ammo)
    {
        GunController SetgunController = GetComponentInChildren<GunController>().GetComponent<GunController>(); ;
        
        Inventory[currentUsingId,2] += (SetgunController.GetAmmoCount()*ammo);
        
        SetgunController.SetTotalAmmo(Inventory[currentUsingId,2]);
        UpdateSlotTexts(Inventory[currentUsingId, 1], Inventory[currentUsingId, 2]);
    }

    public void UpdateInventoryAmmo(int ammoAmount)
    {
        Inventory[currentUsingId, 2] = ammoAmount;
    }

    

    void Start()
    {

        camSlotTransforms[0] = canvas.transform.Find("SlotTracker/CamSlot1").transform;
        camSlotTransforms[1] = canvas.transform.Find("SlotTracker/CamSlot2").transform;
        camSlotTransforms[2] = canvas.transform.Find("SlotTracker/CamSlot3").transform;
        OutlineSlotTransforms[0] = canvas.transform.Find("Slot1/BgSlot1").transform;
        OutlineSlotTransforms[1] = canvas.transform.Find("Slot2/BgSlot2").transform;
        OutlineSlotTransforms[2] = canvas.transform.Find("Slot3/BgSlot3").transform;

        UpdateInventory();

   
        
        if (canvas != null) {
            Debug.Log("PlayerUI Found");
        }
        if (canvas != null)
        {
            slotTexts[0] = canvas.transform.Find("Slot1/Clip").GetComponent<Text>();
            slotTexts[1] = canvas.transform.Find("Slot2/Clip").GetComponent<Text>();
            slotTexts[2] = canvas.transform.Find("Slot3/Clip").GetComponent<Text>();
            crosshair = canvas.transform.Find("Aim").GetComponent<Image>();
        }
        Setup();
        
    }

    void Update()
    {

        HandleInput();
    }
    private void Setup()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject weapon = Instantiate(WeaponId[Inventory[i, 0]]);
            GunController weaponController = weapon.GetComponent<GunController>();
            if (weaponController != null) // Check if the weapon has a GunController component
            {
                // Set the remain ammo and total ammo of the weapon from its corresponding GunController component
                int weaponRemainAmmo = weaponController.GetRemainAmmo();
                int weaponTotalAmmo = weaponController.GetTotalAmmo();
                Inventory[i, 1] = weaponRemainAmmo;
                Inventory[i, 2] = weaponTotalAmmo;
               
            }
            Destroy(weapon); // Destroy the instantiated weapon
        }
        EquipWeapon(currentUsingId);
    }
    private void HandleInput()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1) && currentUsingId != 0)
        {
            EquipWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && currentUsingId != 1)
        {
            EquipWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && currentUsingId != 2)
        {
            EquipWeapon(2);
        }
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");

        if (scrollDelta != 0f && Time.time > lastScrollTime + scrollCooldown)
{
            // The player has scrolled the mouse wheel
            int scrollDirection = Mathf.RoundToInt(Mathf.Sign(scrollDelta));
            scrollNumber += scrollDirection;
            if(scrollNumber > 2)
            {
                scrollNumber = 0;
            }
            if(scrollNumber < 0)
            {
                scrollNumber = 2;
            }
            scrollNumber = Mathf.Clamp(scrollNumber, 0, 3 - 1);

            EquipWeapon(scrollNumber);
            lastScrollTime = Time.time;
        }
    }

    public void EquipWeapon(int slot)
    {

        GunController currentWeaponController = GetComponentInChildren<GunController>();
        if (currentWeaponController != null)
        {
            if (isPickUp == false)
            {
             
                Inventory[currentUsingId, 1] = currentWeaponController.GetRemainAmmo();
                Inventory[currentUsingId, 2] = currentWeaponController.GetTotalAmmo();
            }

            // Update the slot text for the previous weapon
            UpdateSlotTexts(Inventory[currentUsingId, 1], Inventory[currentUsingId, 2]);
        }

        foreach (Transform child in FirstPersonCharacter.transform)
        {
            if (child.gameObject.GetComponent<GunController>() != null || child.gameObject.GetComponent<MeleeController>() != null)
            {
                Destroy(child.gameObject);
            }
        }

        // Update the currentUsingId variable after updating the slot text for the previous weapon
        currentUsingId = slot;

        int weaponId = Inventory[slot, 0];
        GameObject weapon = Instantiate(WeaponId[weaponId]);
        if(weapon != null)
        {
            if(weapon.transform.childCount > 0)
            {
                
                Transform childTransform = weapon.transform.GetChild(0);
                Debug.Log(childTransform.gameObject + " - 1");
                TransformSetup transformSetup = childTransform.gameObject.GetComponent<TransformSetup>();
                Vector3 rotate = transformSetup.getRotation();
                Vector3 scale = transformSetup.getScale();
                Vector3 position = transformSetup.getPosition();
                switch (currentUsingId)
                {
                    case 0:

                        for (int i = camSlotTransforms[0].childCount - 1; i >= 0; i--)
                        {
                            Destroy(camSlotTransforms[0].GetChild(i).gameObject);
                        }

                        // Instantiate the child object and set its parent to the CamSlot1 GameObject
                        slotTracker = Instantiate(childTransform.gameObject, new Vector3(position.x, position.y, position.z), Quaternion.Euler(rotate));
                        
                        slotTracker.transform.SetParent(camSlotTransforms[0]);
                        slotTracker.transform.localPosition = new Vector3(position.x, position.y, position.z);
                        slotTracker.transform.localScale = scale;
                        break;
                    case 1:
                        
                        for (int i = camSlotTransforms[1].childCount - 1; i >= 0; i--)
                            {
                                Destroy(camSlotTransforms[1].GetChild(i).gameObject);
                            }


                        // Instantiate the child object and set its parent to the CamSlot1 GameObject
                        slotTracker = Instantiate(childTransform.gameObject, new Vector3(position.x, position.y, position.z), Quaternion.Euler(rotate));
                        slotTracker.transform.SetParent(camSlotTransforms[1]);
                        slotTracker.transform.localPosition = new Vector3(position.x, position.y, position.z);
                        slotTracker.transform.localScale = scale;


                        break;
                    case 2:

                            for (int i = camSlotTransforms[2].childCount - 1; i >= 0; i--)
                            {
                                Destroy(camSlotTransforms[2].GetChild(i).gameObject);
                            }


                            // Instantiate the child object and set its parent to the CamSlot1 GameObject
                            slotTracker = Instantiate(childTransform.gameObject, new Vector3(position.x, position.y, position.z), Quaternion.Euler(rotate));
                            slotTracker.transform.SetParent(camSlotTransforms[2]);
                            slotTracker.transform.localPosition = new Vector3(position.x, position.y, position.z);
                        slotTracker.transform.localScale = scale;
                        break;
                }
            }
        }
        weapon.transform.SetParent(FirstPersonCharacter.transform);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        idBehaviour weaponData = weapon.GetComponent<idBehaviour>();
        if (weaponData != null)
        {
            weapon.transform.localPosition = weaponData.GetOriginalPosition();
            weapon.transform.localRotation = weaponData.GetOriginalRotation();
        }
        GunController WeaponEquip = weapon.GetComponent<GunController>();
        if (WeaponEquip != null)
        {
            int Remainammo = Inventory[currentUsingId, 1];
            int Totalammo = Inventory[currentUsingId, 2];

            WeaponEquip.SetRemainAmmo(Remainammo);
            WeaponEquip.SetTotalAmmo(Totalammo);


            UpdateSlotTexts(Remainammo, Totalammo);
        }
        else
        { UpdateSlotTexts(0, 0); }
        isPickUp = false;
    }
    public void setCrossHairState(bool isActive)
    {
        if(isActive == true)
        {
            crosshair.gameObject.SetActive(true);
        } else
        {
            crosshair.gameObject.SetActive(false);
        }

    }
    public void UpdateSlotTexts(int remainAmmo, int totalAmmo)
    {
       
        bool isSword = false;
        int CurrentRemainAmmo = remainAmmo;
        int CurrentTotalAmmo = totalAmmo;
        
        for (int i = 0; i < 3; i++)
        {
            if (OutlineSlotTransforms[i] != null)
            {
                Image image = OutlineSlotTransforms[i].GetComponent<Image>();
                if (i == currentUsingId)
                {
                    image.sprite = SlotSelect;
                    image.transform.localScale = new Vector3(1.19f, 1.2f, 1.2f);
                }
                else
                {
                    image.sprite = SlotNormal;
                    image.transform.localScale = new Vector3(1.2524f, 1.2524f, 1.2524f);
                }
            }
            int weaponId = Inventory[i, 0];
            GameObject weapon = WeaponId[weaponId];
            GunController IDBehaviour = weapon.GetComponent<GunController>();
            if (IDBehaviour == null)
            {
                isSword = true;
            }
            if (currentUsingId == i && isSword == false)
            {

                if (IDBehaviour != null)
                {
                    if (remainAmmo == -1 && totalAmmo == -1)
                    {
                        remainAmmo = IDBehaviour.GetRemainAmmo();
                        totalAmmo = IDBehaviour.GetTotalAmmo();
                    }
                }


            slotTexts[i].text = CurrentRemainAmmo + " / " + CurrentTotalAmmo;
            } else if (isSword == false)
            {
                remainAmmo = Inventory[i, 1];
                totalAmmo = Inventory[i, 2];
                slotTexts[i].text = remainAmmo + " / " + totalAmmo;
            }
            if(totalAmmo == 0 )
            {
                int UpdatetotalAmmo = Inventory[i, 2];
                slotTexts[i].text = remainAmmo + " / " + UpdatetotalAmmo;
            }
            if (isSword == true)
            {
                slotTexts[i].text = "";
            }

            isSword = false;
        }
    }
}