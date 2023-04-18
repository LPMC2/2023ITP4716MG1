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


    private int currentUsingId = 0;

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

    private Text[] slotTexts = new Text[3];

    void Start()
    {

        UpdateInventory();

   
        GameObject canvas = GameObject.Find("PlayerUI");
        if (canvas != null) {
            Debug.Log("PlayerUI Found");
        }
        if (canvas != null)
        {
            slotTexts[0] = canvas.transform.Find("Slot1/Clip").GetComponent<Text>();
            slotTexts[1] = canvas.transform.Find("Slot2/Clip").GetComponent<Text>();
            slotTexts[2] = canvas.transform.Find("Slot3/Clip").GetComponent<Text>();
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
            Destroy(child.gameObject);
        }

        // Update the currentUsingId variable after updating the slot text for the previous weapon
        currentUsingId = slot;

        int weaponId = Inventory[slot, 0];
        GameObject weapon = Instantiate(WeaponId[weaponId]);
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

    public void UpdateSlotTexts(int remainAmmo, int totalAmmo)
    {
       
        bool isSword = false;
        int CurrentRemainAmmo = remainAmmo;
        int CurrentTotalAmmo = totalAmmo;
        
        for (int i = 0; i < 3; i++)
        {
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