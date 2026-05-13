using Unity.Netcode;
using UnityEngine;

public class DoorCreak : NetworkBehaviour
{
    public Transform door;
    public float movementThreshold = 5f;

    private Quaternion lastRotation;
    [SerializeField]private AudioSource audioSource;

    public override void OnNetworkSpawn()
    {
        lastRotation = door.rotation;
    }

    void Update()
    {
        float angleMoved = Quaternion.Angle(lastRotation, door.rotation);

        float velocity = angleMoved / Time.deltaTime;

        bool moving = velocity > movementThreshold;

        if (moving)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }

            audioSource.volume = Mathf.Lerp(audioSource.volume, 1f, Time.deltaTime * 8f);
            audioSource.pitch = Mathf.Clamp(velocity * 0.02f, 0.8f, 1.5f);
        }
        else
        {
            // Fade out instead of stopping instantly
            audioSource.volume = Mathf.Lerp(audioSource.volume, 0f, Time.deltaTime * 8f);

            // Stop completely once nearly silent
            if (audioSource.volume < 0.01f)
            {
                audioSource.Stop();
            }
        }

        lastRotation = door.rotation;
    }
}