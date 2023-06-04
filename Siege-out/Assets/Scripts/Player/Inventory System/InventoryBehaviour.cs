using System.Collections;
using System.IO;
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
    private bool showInventory = true;
    private bool showSettings = false;
    private int selectedRow = -1;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        InventoryBehaviour inventory = (InventoryBehaviour)target;
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        // Add this section to display the Inventory array in the Inspector
        showInventory = EditorGUILayout.Foldout(showInventory, "Inventory", true, new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, normal = { textColor = Color.blue }, margin = new RectOffset(0, 0, 0, 0), padding = new RectOffset(0, 0, 0, 0) });

        GUILayout.ExpandWidth(false);
        if (showInventory)
        {
            if (GUILayout.Button(EditorGUIUtility.IconContent("_Popup"), GUILayout.Width(20f), GUILayout.Height(20f), GUILayout.ExpandWidth(false)))
            {
                showSettings = !showSettings;
            }
        }
        EditorGUILayout.EndHorizontal();
        if (showInventory)
        {



            for (int i = 0; i < inventory.Inventory.GetLength(0); i++)
            {
                EditorGUILayout.BeginHorizontal();
                if (showSettings == true)
                {
                    if (GUILayout.Button(selectedRow == i ? "\u2714" : "", new GUIStyle(GUI.skin.button) { normal = new GUIStyleState() { textColor = Color.white } }, GUILayout.Width(20f)))
                    {
                        selectedRow = selectedRow == i ? -1 : i;
                    }
                }
                for (int j = 0; j < inventory.Inventory.GetLength(1); j++)
                {


                    if (j == 0) // First column
                    {
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.LabelField("Id", GUILayout.Width(20f));
                        EditorGUILayout.TextField(inventory.Inventory[i, j].ToString(), GUILayout.Width(50f));
                        EditorGUI.EndDisabledGroup();
                    }
                    else
                    {
                        inventory.Inventory[i, j] = EditorGUILayout.IntField(inventory.Inventory[i, j], GUILayout.Width(50f));
                    }
                }

                EditorGUILayout.EndHorizontal();
            }
            if (showSettings)
            {
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("+", GUILayout.Width(30f)))
                {
                    if (selectedRow >= 0 && selectedRow < inventory.Inventory.GetLength(0))
                    {
                        int numRows = inventory.Inventory.GetLength(0);
                        int numCols = inventory.Inventory.GetLength(1);
                        int[,] newInventory = new int[numRows + 1, numCols];

                        for (int i = 0; i <= selectedRow; i++)
                        {
                            for (int j = 0; j < numCols; j++)
                            {
                                newInventory[i, j] = inventory.Inventory[i, j];
                            }
                        }

                        for (int j = 0; j < numCols; j++)
                        {
                            newInventory[selectedRow + 1, j] = 0;
                        }

                        for (int i = selectedRow + 1; i < numRows + 1; i++)
                        {
                            for (int j = 0; j < numCols; j++)
                            {
                                newInventory[i, j] = inventory.Inventory[i - 1, j];
                            }
                        }

                        inventory.Inventory = newInventory;
                       
                                                for(int i=0; i<inventory.Inventory.GetLength(0); i++) {
                        Debug.Log("Test");
                        }
                        if (GUI.changed)
                        {
                        inventory.SaveInventory();
                            EditorUtility.SetDirty(inventory);
                               
                        }
                    }

                }

                if (GUILayout.Button("-", GUILayout.Width(30f)))
                {
                    if (selectedRow >= 0 && selectedRow < inventory.Inventory.GetLength(0))
                    {
                        int numCols = inventory.Inventory.GetLength(1);
                        int[,] newInventory = new int[inventory.Inventory.GetLength(0) - 1, numCols];

                        for (int i = 0; i < selectedRow; i++)
                        {
                            for (int j = 0; j < numCols; j++)
                            {
                                newInventory[i, j] = inventory.Inventory[i, j];
                            }
                        }

                        for (int i = selectedRow + 1; i < inventory.Inventory.GetLength(0); i++)
                        {
                            for (int j = 0; j < numCols; j++)
                            {
                                newInventory[i - 1, j] = inventory.Inventory[i, j];
                            }
                        }

                        inventory.Inventory = newInventory;
                        inventory.SaveInventory();
                        for(int i=0; i<inventory.Inventory.GetLength(0); i++) {
                        Debug.Log("Test2");
                        }
                        if (selectedRow > inventory.Inventory.GetLength(0) - 1) // if the last row was deleted, deselect the last row
                        {
                            selectedRow = -1;
                        }
                        if (GUI.changed)
                        {
                        inventory.SaveInventory();
                            EditorUtility.SetDirty(inventory);

                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(inventory);
        }

        EditorGUILayout.Space();

        showWeaponIds = EditorGUILayout.Foldout(showWeaponIds, "IDs Manager", true, new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, normal = { textColor = Color.red } });

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

        showDropWeaponIds = EditorGUILayout.Foldout(showDropWeaponIds, "Drop IDs Manager", true, new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, normal = { textColor = Color.red } });

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
    [Header("Other Settings")]
    [SerializeField] private bool useData = false;
    public GameObject canvas;
    [SerializeField] private Sprite SlotNormal;
    [SerializeField] private Sprite SlotSelect;
    [SerializeField] private Camera gunCamera;
    [SerializeField] private Text[] slotTexts = new Text[3];
    [SerializeField] private AudioClip useSound;
    private AudioSource audioSource;
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
    public void SaveInventory()
    {
        string fileName = "Assets/Resources/InventoryData.txt";
        int numRows = Inventory.GetLength(0);
        int numCols = Inventory.GetLength(1);

        // Create a string array to hold the inventory data
        string[] inventoryData = new string[numRows];

        // Populate the inventory data array with the content of the Inventory array
        for (int i = 0; i < numRows; i++)
        {
            string rowStr = "";
            for (int j = 0; j < numCols; j++)
            {
                rowStr += Inventory[i, j].ToString() + " ";
            }
            inventoryData[i] = rowStr.TrimEnd();
        }

        // Write the inventory data to a text file
        File.WriteAllLines(fileName, inventoryData);

        // Debug log the file path and content of the Inventory array
        string inventoryStr = string.Join("\n", inventoryData);
        Debug.Log("Inventory saved to file:\n" + fileName + "\n" + inventoryStr);
    }

    // Load the Inventory array from PlayerPrefs
    public void LoadInventory(string fileName)
    {
            Inventory = new int[1, 3] { { 0, 0, 0 }};
    //// Check if the file exists
    //if (!File.Exists(fileName))
    //{
    //    Debug.LogError("Inventory file not found: " + fileName);
    //    return;
    //}

    //// Read the inventory data from the text file
    //string[] inventoryData = File.ReadAllLines(fileName);

    //// Parse the inventory data and populate the Inventory array
    //int numRows = inventoryData.Length;
    //int numCols = inventoryData[0].Split(' ').Length;

    //Inventory = new int[numRows, numCols];

    //for (int i = 0; i < numRows; i++)
    //{
    //    string[] rowStrArray = inventoryData[i].Split(' ');
    //    for (int j = 0; j < numCols; j++)
    //    {
    //        int value;
    //        bool success = int.TryParse(rowStrArray[j], out value);
    //        if (success)
    //        {
    //            Inventory[i, j] = value;
    //        }
    //        else
    //        {
    //            Debug.LogWarning("Invalid value in inventory file at row " + i.ToString() + ",column " + j.ToString() + ": " + rowStrArray[j]);
    //        }
    //    }
    //}

    //// Debug log the file path and content of the Inventory array
    //string inventoryStr = string.Join("\n", inventoryData);
    //Debug.Log("Inventory loaded from file:\n" + fileName + "\n" + inventoryStr);
}
    public void setInitialInv(int chId)
    {
        InitialInventorySlot[0] = chId;
    }
    public void UpdateInventory()
    {

        for (int i = 0; i < Inventory.GetLength(0); i++)
        {
            Inventory[i, 0] = InitialInventorySlot[i];
        }
        for (int i = 0; i < Inventory.GetLength(0); i++)
        {
            int weaponId = Inventory[i, 0];
            GameObject weapon = WeaponId[weaponId];
            if (weapon != null)
            {
                if (weapon.transform.childCount > 0)
                {


                    Transform childTransform = weapon.transform.GetChild(0);

                    TransformSetup transformSetup = childTransform.gameObject.GetComponent<TransformSetup>();
                    Vector3 rotate = transformSetup.getRotation();
                    Vector3 scale = transformSetup.getScale();
                    Vector3 position = transformSetup.getPosition();

                    switch (i)
                    {
                        case 0:

                            for (int n = camSlotTransforms[0].childCount - 1; n >= 0; n--)
                            {
                                Destroy(camSlotTransforms[0].GetChild(n).gameObject);
                            }

                            // Instantiate the child object and set its parent to the CamSlot1 GameObject
                            slotTracker = Instantiate(childTransform.gameObject, new Vector3(position.x, position.y, position.z), Quaternion.Euler(rotate));
                            slotTracker.layer = LayerMask.NameToLayer("Default");
                            foreach (Transform child in slotTracker.transform)
                            {
                                child.gameObject.layer = LayerMask.NameToLayer("Default");
                            }
                            slotTracker.transform.SetParent(camSlotTransforms[0]);
                            slotTracker.transform.localPosition = new Vector3(position.x, position.y, position.z);
                            slotTracker.transform.localScale = scale;
                            break;
                        case 1:

                            for (int n = camSlotTransforms[1].childCount - 1; n >= 0; n--)
                            {
                                Destroy(camSlotTransforms[1].GetChild(n).gameObject);
                            }


                            // Instantiate the child object and set its parent to the CamSlot1 GameObject
                            slotTracker = Instantiate(childTransform.gameObject, new Vector3(position.x, position.y, position.z), Quaternion.Euler(rotate));
                            slotTracker.layer = LayerMask.NameToLayer("Default");
                            foreach (Transform child in slotTracker.transform)
                            {
                                child.gameObject.layer = LayerMask.NameToLayer("Default");
                            }
                            slotTracker.transform.SetParent(camSlotTransforms[1]);
                            slotTracker.transform.localPosition = new Vector3(position.x, position.y, position.z);
                            slotTracker.transform.localScale = scale;


                            break;
                        case 2:

                            for (int n = camSlotTransforms[2].childCount - 1; n >= 0; n--)
                            {
                                Destroy(camSlotTransforms[2].GetChild(n).gameObject);
                            }


                            // Instantiate the child object and set its parent to the CamSlot1 GameObject
                            slotTracker = Instantiate(childTransform.gameObject, new Vector3(position.x, position.y, position.z), Quaternion.Euler(rotate));
                            slotTracker.layer = LayerMask.NameToLayer("Default");
                            foreach (Transform child in slotTracker.transform)
                            {
                                child.gameObject.layer = LayerMask.NameToLayer("Default");
                                MeshRenderer meshRenderer = child.gameObject.GetComponent<MeshRenderer>();
                                if (meshRenderer != null)
                                {
                                    meshRenderer.receiveShadows = false;
                                }
                                foreach (Transform childs in child.transform)
                                {
                                    childs.gameObject.layer = LayerMask.NameToLayer("Default");
                                    MeshRenderer meshRenderer1 = childs.gameObject.GetComponent<MeshRenderer>();
                                    if (meshRenderer1 != null)
                                    {
                                        meshRenderer1.receiveShadows = false;
                                    }
                                }
                            }
                            slotTracker.transform.SetParent(camSlotTransforms[2]);
                            slotTracker.transform.localPosition = new Vector3(position.x, position.y, position.z);
                            slotTracker.transform.localScale = scale;
                            break;
                    }
                }
            }
        }
    }
    public void SetInventorySlot(int currentID, int id, int rAmmo, int tAmmo, bool IsSword)
    {
        if (IsSword == false)
        {
            Inventory[currentID, 0] = id;
            Inventory[currentID, 1] = rAmmo;
            Inventory[currentID, 2] = tAmmo;
            isPickUp = true;
        }
        else
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

        Inventory[currentUsingId, 2] += (SetgunController.GetAmmoCount() * ammo);

        SetgunController.SetTotalAmmo(Inventory[currentUsingId, 2]);
        UpdateSlotTexts(Inventory[currentUsingId, 1], Inventory[currentUsingId, 2]);
    }
    public void SetRemainAmmo(int ammo)
    {
        GunController SetgunController = GetComponentInChildren<GunController>().GetComponent<GunController>(); ;

        Inventory[currentUsingId, 1] = ammo;

        SetgunController.SetRemainAmmo(Inventory[currentUsingId, 1]);
        EquipWeapon(currentUsingId);
        UpdateSlotTexts(Inventory[currentUsingId, 1], Inventory[currentUsingId, 2]);
    }
    public void UpdateInventoryAmmo(int ammoAmount)
    {
        Inventory[currentUsingId, 2] = ammoAmount;
    }



    void Start()
    {
        if (useData)
        {
            LoadInventory("Assets/Resources/InventoryData.txt");
        }
        
        audioSource = GetComponent<AudioSource>();
        for (int i = 0; i < Inventory.GetLength(0); i++)
        {
            camSlotTransforms[i] = canvas.transform.Find("SlotTracker/CamSlot" + (i + 1)).transform;
            OutlineSlotTransforms[i] = canvas.transform.Find("Slot" + (i + 1) + "/BgSlot" + (i + 1)).transform;
        }

        UpdateInventory();


        if (canvas != null)
        {
            slotTexts = new Text[Inventory.GetLength(0)];
            for (int i = 0; i < Inventory.GetLength(0); i++)
            {
                slotTexts[i] = canvas.transform.Find("Slot" + (i + 1) + "/Clip").GetComponent<Text>();
            }
            crosshair = canvas.transform.Find("Aim").GetComponent<Image>();
        }
        Setup();

    }

    void Update()
    {

        HandleInput();
    }
    public void Setup()
    {
        for (int i = 0; i < Inventory.GetLength(0); i++)
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
        else if ( Inventory.GetLength(0) > 1 && Input.GetKeyDown(KeyCode.Alpha2) && currentUsingId != 1)
        {
            EquipWeapon(1);
        }
        else if (Inventory.GetLength(0) > 2 && Input.GetKeyDown(KeyCode.Alpha3) && currentUsingId != 2)
        {
            EquipWeapon(2);
        }
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");

        if (scrollDelta != 0f && Time.time > lastScrollTime + scrollCooldown)
        {
            // The player has scrolled the mouse wheel
            int scrollDirection = Mathf.RoundToInt(Mathf.Sign(scrollDelta));
            scrollNumber += scrollDirection;
            if (scrollNumber > 2)
            {
                scrollNumber = 0;
            }
            if (scrollNumber < 0)
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
        if (audioSource != null && useSound != null)
        {
            audioSource.clip = useSound;
            audioSource.Play();
        }
        gunCamera.enabled = false;
        gunCamera.enabled = true;
        gunCamera.Render();
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
            if (child.gameObject.GetComponent<GunController>() != null || child.gameObject.GetComponent<MeleeController>() != null || child.gameObject.GetComponent<GetWeapon>() != null)
            {
                Destroy(child.gameObject);
            }
        }

        // Update the currentUsingId variable after updating the slot text for the previous weapon
        currentUsingId = slot;

        int weaponId = Inventory[slot, 0];
        GameObject weapon = Instantiate(WeaponId[weaponId]);
        if (weapon != null)
        {
            if (weapon.transform.childCount > 0)
            {

                Transform childTransform = weapon.transform.GetChild(0);

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
                        slotTracker.layer = LayerMask.NameToLayer("Default");
                        foreach (Transform child in slotTracker.transform)
                        {
                            child.gameObject.layer = LayerMask.NameToLayer("Default");
                        }
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
                        slotTracker.layer = LayerMask.NameToLayer("Default");
                        foreach (Transform child in slotTracker.transform)
                        {
                            child.gameObject.layer = LayerMask.NameToLayer("Default");
                        }
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
                        slotTracker.layer = LayerMask.NameToLayer("Default");
                        foreach (Transform child in slotTracker.transform)
                        {
                            child.gameObject.layer = LayerMask.NameToLayer("Default");
                        }
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
        if (isActive == true)
        {
            crosshair.gameObject.SetActive(true);
        }
        else
        {
            crosshair.gameObject.SetActive(false);
        }

    }
    public void UpdateSlotTexts(int remainAmmo, int totalAmmo)
    {

        bool isSword = false;
        int CurrentRemainAmmo = remainAmmo;
        int CurrentTotalAmmo = totalAmmo;

        for (int i = 0; i < Inventory.GetLength(0); i++)
        {
            if (OutlineSlotTransforms[i] != null)
            {
                Image image = OutlineSlotTransforms[i].GetComponent<Image>();
                if (i == currentUsingId)
                {
                    image.sprite = SlotSelect;
                    image.transform.localScale = new Vector3(1.32f, 1.32f, 1.32f);
                }
                else
                {
                    image.sprite = SlotNormal;
                    image.transform.localScale = new Vector3(1f, 1f, 1f);
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
            }
            else if (isSword == false)
            {
                remainAmmo = Inventory[i, 1];
                totalAmmo = Inventory[i, 2];
                slotTexts[i].text = remainAmmo + " / " + totalAmmo;
            }
            if (totalAmmo == 0)
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