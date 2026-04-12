using UnityEngine;

public abstract class PlayerInteractable : MonoBehaviour
{
    //Message when player is looking at the interactable
    public string promptMessage;
    public void BaseInteract()
    {
        Interact();
    }
    protected virtual void Interact()
    {
        //just a template function to be overridden by our subclasses 
    }
}
