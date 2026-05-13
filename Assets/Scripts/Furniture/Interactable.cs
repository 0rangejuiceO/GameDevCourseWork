using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent onInteract;
    public string interactionPrompt = "Press E to interact";
    [SerializeField]private InteractSoundFXNetwork soundFX;

    public void Interact()
    {
        onInteract.Invoke();
        if(soundFX != null)
        {
            soundFX.PlaySound();
        }
    }


}
