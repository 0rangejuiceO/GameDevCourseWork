using UnityEngine;
using UnityEngine.InputSystem;

public class EscapeMenu : MonoBehaviour
{
    [SerializeField]private InputActionReference openEscapeMenu;
    [SerializeField]private GameObject escapeMenuUI;
    private bool isEscapeMenuOpen = false;

    private void OnEnable()
    {
        openEscapeMenu.action.performed += ToggleEscapeMenu;
        openEscapeMenu.action.Enable();
    }

    private void OnDisable()
    {
        openEscapeMenu.action.performed -= ToggleEscapeMenu;
        openEscapeMenu.action.Disable();
    }

    private void ToggleEscapeMenu(InputAction.CallbackContext context)
    {
        isEscapeMenuOpen = !isEscapeMenuOpen;
        escapeMenuUI.SetActive(isEscapeMenuOpen);
    }

    public void Quit()
    {
        Debug.Log("Quitting game...");
        Application.Quit();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void QuitToMainMenu()
    {
        Debug.Log("Quitting to Main Menu...");
        GameLobby.Instance.LeaveLobby();
    }
}
