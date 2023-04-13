using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
[CustomEditor(typeof(GetWeapon))]
public class GetWeaponEditor : Editor
{
    private InventoryBehaviour inventoryBehaviour;
    private string[] weaponNames;
    private int[] weaponIds;
    private int TotalAmmo;
    private int RemainAmmo;
    private SerializedProperty TotalAmmoProperty;
    private SerializedProperty RemainAmmoProperty;
    private SerializedProperty storeWeaponIDProperty;

    private void OnEnable()
    {
        inventoryBehaviour = FindObjectOfType<InventoryBehaviour>();
        inventoryBehaviour.WeaponIdChanged += OnWeaponIdChanged;

        // Get reference to StoreWeaponID property
        storeWeaponIDProperty = serializedObject.FindProperty("StoreWeaponID");
        TotalAmmoProperty = serializedObject.FindProperty("TotalAmmo");
        RemainAmmoProperty = serializedObject.FindProperty("RemainAmmo");

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

            // Get the GunController component of the selected weapon and set TotalAmmo and RemainAmmo
            GameObject selectedWeapon = inventoryBehaviour.WeaponId[selectedWeaponIndex];
            if (selectedWeapon != null)
            {
                GunController gunController = selectedWeapon.GetComponent<GunController>();
                if (gunController != null)
                {
                    getWeapon.TotalAmmoSet = gunController.GetTotalAmmo();
                    getWeapon.RemainAmmoSet = gunController.GetRemainAmmo();
                }
            }
        }

        // Show the TotalAmmo and RemainAmmo fields
        EditorGUILayout.PropertyField(TotalAmmoProperty);
        EditorGUILayout.PropertyField(RemainAmmoProperty);

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
#endif