using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(Spawner))]
public class SpawnerEditor : Editor
{
    private SerializedProperty spawnTimeProperty;
    private SerializedProperty spawnCountLimitProperty;
    private SerializedProperty spawnAmountProperty;
    private SerializedProperty monstersProperty;

    private void OnEnable()
    {
        spawnTimeProperty = serializedObject.FindProperty("SpawnTime");
        spawnCountLimitProperty = serializedObject.FindProperty("SpawnCountLimit");
        spawnAmountProperty = serializedObject.FindProperty("SpawnAmout");
        monstersProperty = serializedObject.FindProperty("monsters");
    }

    public override void OnInspectorGUI()
    {
       
        serializedObject.Update();
        Spawner spawner = (Spawner)target;

        EditorGUILayout.PropertyField(spawnTimeProperty);
        EditorGUILayout.PropertyField(spawnCountLimitProperty);
        EditorGUILayout.PropertyField(spawnAmountProperty);
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