using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if UNITY_EDITOR
[CustomEditor(typeof(EnemyController))]
public class EnemyControllerEditor : Editor
{
    SerializedProperty lookRadius;
    SerializedProperty attackType;
    SerializedProperty projectileType;
    SerializedProperty damage;
    SerializedProperty attackCD;
    SerializedProperty preAttackCD;
    SerializedProperty projectileObject;
    SerializedProperty maxTime;
    SerializedProperty ProjectileSpeed;
    SerializedProperty AttackAngleMultiplier;
    SerializedProperty isAreaDamage;
    SerializedProperty AOERadius;
    SerializedProperty obstacleMask;
    SerializedProperty projectileOffset;
     SerializedProperty animateObject;
    private void OnEnable()
    {
        lookRadius = serializedObject.FindProperty("lookRadius");
        attackType = serializedObject.FindProperty("attackType");
        projectileType = serializedObject.FindProperty("projectileType");
        damage = serializedObject.FindProperty("damage");
        attackCD = serializedObject.FindProperty("attackCD");
        preAttackCD = serializedObject.FindProperty("preAttackCD");
        projectileObject = serializedObject.FindProperty("projectileObject");
        maxTime = serializedObject.FindProperty("maxTime");
        ProjectileSpeed = serializedObject.FindProperty("ProjectileSpeed");
        AttackAngleMultiplier = serializedObject.FindProperty("AttackAngleMultiplier");
        isAreaDamage = serializedObject.FindProperty("isAreaDamage");
        AOERadius = serializedObject.FindProperty("AOERadius");
        obstacleMask = serializedObject.FindProperty("obstacleMask");
        projectileOffset = serializedObject.FindProperty("ProjectileOffset");
        animateObject = serializedObject.FindProperty("AnimateObject");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(lookRadius);

        EditorGUILayout.PropertyField(attackType);

            EditorGUILayout.PropertyField(damage);
            EditorGUILayout.PropertyField(preAttackCD);
            EditorGUILayout.PropertyField(attackCD);
        if (attackType.enumValueIndex == (int)EnemyController.AttackType.Ranged)
        {
            EditorGUILayout.PropertyField(projectileType);
            EditorGUILayout.PropertyField(projectileObject);
            EditorGUILayout.PropertyField(maxTime);
            EditorGUILayout.PropertyField(projectileOffset);
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
        EditorGUILayout.PropertyField(animateObject);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif