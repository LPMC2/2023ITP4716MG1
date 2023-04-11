using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterId : MonoBehaviour
{
    private int ID;
    private void Awake()
    {
        ID = Random.Range(1, 100000);
    }
    public int GetID()
    {
        return ID;
    }
}
