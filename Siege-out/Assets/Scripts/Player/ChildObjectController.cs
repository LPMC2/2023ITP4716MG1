using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildObjectController : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
    }
}
