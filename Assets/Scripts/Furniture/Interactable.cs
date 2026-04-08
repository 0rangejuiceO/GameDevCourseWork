using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent onInteract;
    public string interactionPrompt = "Press E to interact";

    public void Interact()
    {
        onInteract.Invoke();
    }


}
