using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformSetup : MonoBehaviour
{
    [Header("GameObject Transform Settings")]
    [SerializeField] private Vector3 SetPosition;
    [SerializeField] private Vector3 SetRotation;
    [SerializeField] private Vector3 SetScale;
    public Vector3 getPosition()
    {
        return SetPosition;
    }
    public Vector3 getRotation()
    {
        return SetRotation;
    }
    public Vector3 getScale()
    {
        return SetScale;
    }

}
