using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GetWeapon))]
public class GetWeaponEditor : Editor
{
    private InventoryBehaviour inventoryBehaviour;
    private string[] weaponNames;
    private int[] weaponIds;

    private SerializedProperty storeWeaponIDProperty;

    private void OnEnable()
    {
        inventoryBehaviour = FindObjectOfType<InventoryBehaviour>();
        inventoryBehaviour.WeaponIdChanged += OnWeaponIdChanged;

        // Get reference to StoreWeaponID property
        storeWeaponIDProperty = serializedObject.FindProperty("StoreWeaponID");

        UpdateWeaponNames();
    }

    private void OnDisable()
    {
        inventoryBehaviour.WeaponIdChanged -= OnWeaponIdChanged;
    }

    private void OnWeaponIdChanged(int newWeaponId)
    {
        UpdateWeaponNames();
        Repaint();
    }

    private void UpdateWeaponNames()
    {
        weaponNames = new string[inventoryBehaviour.WeaponId.Length];
        weaponIds = new int[inventoryBehaviour.WeaponId.Length];

        for (int i = 0; i < inventoryBehaviour.WeaponId.Length; i++)
        {
            weaponNames[i] = $"{i + 1} - {inventoryBehaviour.WeaponId[i].name}";
            weaponIds[i] = i;
        }

        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GetWeapon getWeapon = (GetWeapon)target;

        // Show the name of the selected weapon
        EditorGUILayout.LabelField(GetSelectedWeaponName(), EditorStyles.boldLabel);

        // Show the dropdown to select the weapon
        int selectedWeaponIndex = EditorGUILayout.IntPopup("Store Weapon ID", getWeapon.StoreWeaponID, weaponNames, weaponIds);

        // Apply changes to the StoreWeaponID property
        if (selectedWeaponIndex != getWeapon.StoreWeaponID)
        {
            storeWeaponIDProperty.intValue = selectedWeaponIndex;
            serializedObject.ApplyModifiedProperties();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private string GetSelectedWeaponName()
    {
        if (inventoryBehaviour != null && inventoryBehaviour.WeaponId.Length > 0 && storeWeaponIDProperty.intValue < inventoryBehaviour.WeaponId.Length)
        {
            return $"Selected Weapon: {inventoryBehaviour.WeaponId[storeWeaponIDProperty.intValue].name}";
        }

        return "No Weapon Selected";
    }
}