using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MiniGameLight : MonoBehaviour
{
    private Button light;

    [Header("Colors")]
    [SerializeField] private Color normalColor = new Color(1f, 0f, 0f, 0.8f);
    [SerializeField] private Color highlightedColor = new Color(1f, 0f, 0f, 0.8f);
    [SerializeField] private Color pressedColor = new Color(1f, 0f, 0f, 1f);
    [SerializeField] private Color disabledColor = new Color(1f, 0f, 0f, 1f);

    private void Awake()
    {
        light=GetComponent<Button>();
    }
    public void TurnOn()
    {
        light.interactable = false;
        SetButtonColor(disabledColor);
    }

    public void TurnOff()
    {
        light.interactable = false;
        SetButtonColor(disabledColor * new Color(1f, 1f, 1f, 0.8f)); // slightly transparent
    }

    public void BecomeInteractable()
    {
        light.interactable = true;

        ColorBlock colors = light.colors;
        colors.normalColor = normalColor;
        colors.highlightedColor = highlightedColor;
        colors.pressedColor = pressedColor;
        colors.selectedColor = normalColor;
        light.colors = colors;
    }

    private void SetButtonColor(Color color)
    {
        ColorBlock colors = light.colors;
        colors.disabledColor = color;
        light.colors = colors;
    }
}