using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pistol : NetworkBehaviour
{
    [SerializeField]private float range = 25f;
    [SerializeField]private int damage = 10;
    [SerializeField]private float hitForce = 100f;
    [SerializeField]private LayerMask shootableLayers;
    [SerializeField]private AudioSource shootSound;
    private int ammo = 6;
    private Camera cam;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            cam = Camera.main;
        }

    }


    public void shoot()
    {
        if(ammo <= 0)
        {
            Debug.Log("Out of ammo!");
            return;
        }

        PlayShootSoundRPC();

        RaycastHit hit;

        if (Physics.Raycast(cam.transform.position,cam.transform.forward,out hit, range,shootableLayers))
        {
            Debug.Log("hit" + hit.transform.name);

            if (hit.rigidbody != null)
            {
                if(!(hit.collider.gameObject.tag == "Enemy") && !(hit.collider.gameObject.tag == "Player"))
                {
                    hit.rigidbody.AddForce(-hit.normal * hitForce);
                }
                

            }

            if (hit.collider.gameObject.tag == "Player")
            {
                Debug.Log("Hit Player");
                hit.collider.gameObject.GetComponentInParent<PlayerHealth>().RequestDamageRPC(damage);
            }


        }
        ammo--;
    }

    [Rpc(SendTo.Everyone)]
    private void PlayShootSoundRPC()
    {
        shootSound.Play();
    }


}
