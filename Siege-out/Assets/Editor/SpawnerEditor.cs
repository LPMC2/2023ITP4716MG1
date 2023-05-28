using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
#if UNITY_EDITOR
[CustomEditor(typeof(Spawner))]
public class SpawnerEditor : Editor
{
    private SerializedProperty maxSpawnTimeProperty;
    private SerializedProperty minSpawnTimeProperty;
    private SerializedProperty maxSpawnRadiusProperty;
    private SerializedProperty minSpawnRadiusProperty;
    private SerializedProperty spawnCountLimitProperty;
    private SerializedProperty spawnAmountProperty;
    private SerializedProperty stayTimeProperty;
    private SerializedProperty monstersProperty;
        private SerializedProperty targetProperty;

    private void OnEnable()
    {
        minSpawnTimeProperty = serializedObject.FindProperty("minSpawnTime");
        maxSpawnTimeProperty = serializedObject.FindProperty("maxSpawnTime");
        minSpawnRadiusProperty = serializedObject.FindProperty("minSpawnRadius");
        maxSpawnRadiusProperty = serializedObject.FindProperty("maxSpawnRadius");
        spawnCountLimitProperty = serializedObject.FindProperty("SpawnCountLimit");
        spawnAmountProperty = serializedObject.FindProperty("SpawnAmout");
        stayTimeProperty = serializedObject.FindProperty("StayTime");
        monstersProperty = serializedObject.FindProperty("monsters");
        targetProperty = serializedObject.FindProperty("TargetObject");
    }

    public override void OnInspectorGUI()
    {
       
        serializedObject.Update();
        Spawner spawner = (Spawner)target;
         EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(minSpawnTimeProperty);
        EditorGUILayout.PropertyField(maxSpawnTimeProperty);
         EditorGUILayout.EndHorizontal();
         EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(minSpawnRadiusProperty);
        EditorGUILayout.PropertyField(maxSpawnRadiusProperty);
         EditorGUILayout.EndHorizontal();
        EditorGUILayout.PropertyField(spawnCountLimitProperty);
        EditorGUILayout.PropertyField(spawnAmountProperty);
        EditorGUILayout.PropertyField(stayTimeProperty);
        EditorGUILayout.PropertyField(targetProperty);
        EditorGUILayout.PropertyField(monstersProperty);
        for (int i = 0; i < monstersProperty.arraySize; i++)
        {
            SerializedProperty monsterDataProperty = monstersProperty.GetArrayElementAtIndex(i);
            SerializedProperty monsterProperty = monsterDataProperty.FindPropertyRelative("Monster");
            SerializedProperty spawnChanceProperty = monsterDataProperty.FindPropertyRelative("SpawnChance");

            EditorGUILayout.BeginHorizontal();
            monsterProperty.objectReferenceValue = (GameObject)EditorGUILayout.ObjectField(monsterProperty.objectReferenceValue, typeof(GameObject), true);
            spawnChanceProperty.floatValue = EditorGUILayout.Slider(spawnChanceProperty.floatValue, 0f, 100f);
            EditorGUILayout.LabelField($"%", GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
        }

        

        serializedObject.ApplyModifiedProperties();
    }
}
#endif