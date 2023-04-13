using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
#if UNITY_EDITOR
[CustomEditor(typeof(EnemyController))]
public class EnemyControllerEditor : Editor
{
    SerializedProperty lookRadius;
    SerializedProperty attackType;
    SerializedProperty projectileType;
    SerializedProperty damage;
    SerializedProperty attackCD;
    SerializedProperty projectileObject;
    SerializedProperty maxTime;
    SerializedProperty ProjectileSpeed;
    SerializedProperty AttackAngleMultiplier;
    SerializedProperty isAreaDamage;
    SerializedProperty AOERadius;
    SerializedProperty obstacleMask;

    private void OnEnable()
    {
        lookRadius = serializedObject.FindProperty("lookRadius");
        attackType = serializedObject.FindProperty("attackType");
        projectileType = serializedObject.FindProperty("projectileType");
        damage = serializedObject.FindProperty("damage");
        attackCD = serializedObject.FindProperty("attackCD");
        projectileObject = serializedObject.FindProperty("projectileObject");
        maxTime = serializedObject.FindProperty("maxTime");
        ProjectileSpeed = serializedObject.FindProperty("ProjectileSpeed");
        AttackAngleMultiplier = serializedObject.FindProperty("AttackAngleMultiplier");
        isAreaDamage = serializedObject.FindProperty("isAreaDamage");
        AOERadius = serializedObject.FindProperty("AOERadius");
        obstacleMask = serializedObject.FindProperty("obstacleMask");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(lookRadius);

        EditorGUILayout.PropertyField(attackType);

        if (attackType.enumValueIndex == (int)EnemyController.AttackType.Melee)
        {
            EditorGUILayout.PropertyField(damage);
            EditorGUILayout.PropertyField(attackCD);
        }
        else if (attackType.enumValueIndex == (int)EnemyController.AttackType.Ranged)
        {
            EditorGUILayout.PropertyField(damage);
            EditorGUILayout.PropertyField(attackCD);
            EditorGUILayout.PropertyField(projectileType);
            EditorGUILayout.PropertyField(projectileObject);
            EditorGUILayout.PropertyField(maxTime);
            EditorGUILayout.PropertyField(obstacleMask);
            EditorGUILayout.PropertyField(ProjectileSpeed);
            EditorGUILayout.PropertyField(isAreaDamage);
            if(isAreaDamage.boolValue == true)
            {
                EditorGUILayout.PropertyField(AOERadius);
            }
            if (projectileType.enumValueIndex == (int)EnemyController.ProjectileType.InstantForce)
            {
                
                EditorGUILayout.PropertyField(AttackAngleMultiplier);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif