using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class EscapeMenu : MonoBehaviour
{
    [SerializeField]private InputActionReference openEscapeMenu;
    [SerializeField]private GameObject escapeMenuUI;
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;
    private bool isEscapeMenuOpen = false;

    void Start()
    {
        resolutions = Screen.resolutions;

        List<Resolution> uniqueResolutions = new List<Resolution>();

        foreach (var res in resolutions)
        {
            bool exists = uniqueResolutions.Any(r =>
                r.width == res.width &&
                r.height == res.height);

            if (!exists)
                uniqueResolutions.Add(res);
        }

        resolutions = uniqueResolutions.ToArray();

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option =
                $"{resolutions[i].width} x {resolutions[i].height}";

            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);

        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    public void SetResolution(int index)
    {
        Resolution resolution = resolutions[index];

        Screen.SetResolution(
            resolution.width,
            resolution.height,
            isFullscreen
                ? FullScreenMode.FullScreenWindow
                : FullScreenMode.Windowed
        );


    }

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

        if (isEscapeMenuOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private bool isFullscreen = true;

    public void SetFullscreen(bool value)
    {
        isFullscreen = value;

        Screen.fullScreenMode = value
            ? FullScreenMode.FullScreenWindow
            : FullScreenMode.Windowed;
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
