using Unity.Netcode;
using UnityEngine;

public class InteractSoundFXNetwork : NetworkBehaviour
{
    [SerializeField] private AudioSource audioSource;

    public void PlaySound()
    {
        PlaySoundRPC();
    }

    [Rpc(SendTo.Everyone)]
    private void PlaySoundRPC()
    {
        audioSource.Play();
    }
}
