using Unity.Netcode;
using UnityEngine;

public class Locker : NetworkBehaviour
{
    [SerializeField] private Animator animator;

    private bool open = false;
    public void UseLocker()
    {
        UseLockerRPC();


    }

    [Rpc(SendTo.Server)]
    private void UseLockerRPC()
    {
        ToggleLocker();
    }

    private void ToggleLocker()
    {
        open = !open;
        SendAnimationRPC(open);
    }

    [Rpc(SendTo.Everyone)]
    private void SendAnimationRPC(bool isOpen)
    {
        if (isOpen)
        {
            animator.SetTrigger("Open");
        }
        else
        {
            animator.SetTrigger("Close");
        }
    }


}
