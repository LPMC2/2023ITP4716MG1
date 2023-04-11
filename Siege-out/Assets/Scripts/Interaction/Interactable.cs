using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public abstract class Interactable : MonoBehaviour
{
    
    public bool useEvents;
    [SerializeField, TextArea] public string promptMessage;
    // Start is called before the first frame update
    public virtual string OnLook()
    {
        return promptMessage;
    }

    // Update is called once per frame
    public void BaseInteract()
    {
        if (useEvents)
            GetComponent<InteractionEvent>().OnInteract.Invoke();
        Interact();
    }
    protected virtual void Interact()
    {

    }
}
