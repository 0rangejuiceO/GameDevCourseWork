using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetcodeUI : MonoBehaviour
{
    [SerializeField]private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private Camera UICamera;


    private void Awake()
    {
        hostButton.onClick.AddListener(() =>
        {
            Debug.Log("HOST");
            NetworkManager.Singleton.StartHost();
        });

        joinButton.onClick.AddListener(() =>
        {
            Debug.Log("JOIN");
            NetworkManager.Singleton.StartClient();
        });
    }
}
