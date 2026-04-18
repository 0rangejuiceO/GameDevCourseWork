using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MiniGameLight : MonoBehaviour
{
    private Button light;

    [Header("Colours")]
    [SerializeField] private Color baseColour = new Color(0f,0f,0f,1f);
    private Color normalColour = new Color(1f, 0f, 0f, 0.8f);
    private Button button;

    private void OnValidate()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (button == null)
            return;

        var colours = button.colors;
        colours.normalColor = baseColour; 
        colours.disabledColor = baseColour * new Color(1f, 1f, 1f, 0.65f);
        button.colors = colours;
    }

    private void Awake()
    {
        normalColour = baseColour * new Color(1f, 1f, 1f, 0.65f);

        light=GetComponent<Button>();
    }
    public void TurnOn()
    {
        Debug.Log("Turning on");
        light.interactable = false;
        SetButtonColour(baseColour);
    }

    public void TurnOff()
    {
        light.interactable = false;
        SetButtonColour(normalColour); 
    }

    public void BecomeInteractable()
    {
        light.interactable = true;

        ColorBlock colours = light.colors;
        colours.normalColor = normalColour;
        colours.highlightedColor = normalColour;
        colours.pressedColor = baseColour;
        colours.selectedColor = normalColour;
        light.colors = colours;
    }

    private void SetButtonColour(Color colour)
    {
        ColorBlock colours = light.colors;
        colours.disabledColor = colour;
        light.colors = colours;
    }
}