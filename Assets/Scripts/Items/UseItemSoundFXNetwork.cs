using Unity.Netcode;
using UnityEngine;

public class UseItemSoundFXNetwork : NetworkBehaviour
{
    [SerializeField]private AudioSource audioSource;

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
