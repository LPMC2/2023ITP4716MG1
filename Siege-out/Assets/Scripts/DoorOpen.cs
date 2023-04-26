using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DoorOpen : MonoBehaviour
{

    [SerializeField]
    private float openDistance = 13f;

    [SerializeField]
    private float openSpeed = 2f;
#if UNITY_EDITOR
    [ReadOnly]
#endif
    [SerializeField] private bool isDoorOpen = false;
    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpening = false;
    private void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + new Vector3(openDistance, 0f, 0f);
    }

    private void Update()
    {

    }
    public void setDoorState()
    {
        if (isOpening == false)
        {
            if (isDoorOpen)
            {
                StartCoroutine(AnimateDoorPosition(transform.position, closedPosition));
                isDoorOpen = false;

            }
            else
            if (!isDoorOpen)
            {
                StartCoroutine(AnimateDoorPosition(transform.position, openPosition));
                isDoorOpen = true;

            }
        }
    }
    private IEnumerator AnimateDoorPosition(Vector3 startPosition, Vector3 endPosition)
    {
        isOpening = true;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            if(t >= 1f)
            {
                isOpening = false;
            }
            yield return null;
        }
    }
}

    

