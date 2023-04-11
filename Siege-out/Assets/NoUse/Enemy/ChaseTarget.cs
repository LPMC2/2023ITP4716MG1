using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseTarget : MonoBehaviour
{
    private bool isBeingChased = false;

    public bool IsBeingChased()
    {
        return isBeingChased;
    }

    public void SetChased(bool chased)
    {
        isBeingChased = chased;
    }
}