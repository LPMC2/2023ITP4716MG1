using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class idBehaviour : MonoBehaviour
{
    [Header("Weapon ID:")]
    [SerializeField] private int id = 1;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Awake()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    public Vector3 GetOriginalPosition()
    {
        return originalPosition;
    }

    public Quaternion GetOriginalRotation()
    {
        return originalRotation;
    }




    public int GetId()
    {
        return id;
    }
}