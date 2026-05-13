using UnityEngine;
using UnityEngine.Events;

public class HoldInteractable : MonoBehaviour
{
    public UnityEvent<string> onHoldInteract;
    
    public string interactionPrompt = "Hold E to interact";
    [SerializeField] public string requiredItem;
    [SerializeField]private InteractSoundFXNetwork soundFX;

    public void Interact(string currentItem)
    {
        if (currentItem.Contains(requiredItem))
        {
            onHoldInteract.Invoke(interactionPrompt);

            if (soundFX != null)
            {
                soundFX.PlaySound();
            }
        }
    }

}
