using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Cleaning Progress")]
    public Slider cleaningBar;
    public TextMeshProUGUI cleaningText;

    [Header("Tool Inventory UI")]
    public Image tool1Icon;
    public Image tool2Icon;
    public GameObject tool1Selected;
    public GameObject tool2Selected;

    public void UpdateCleaningBar(float progress)
    {
        if (cleaningBar != null)
        {
            cleaningBar.value = progress;
        }

        if (cleaningText != null)
        {
            cleaningText.text = $"{Mathf.RoundToInt(progress * 100)}%";
        }
    }

    public void UpdateToolInventory(Tool[] tools, int currentIndex)
    {
        // Tool 1
        if (tool1Icon != null)
        {
            if (tools[0] != null)
            {
                tool1Icon.sprite = tools[0].icon;
                tool1Icon.enabled = true;
            }
            else
            {
                tool1Icon.enabled = false;
            }
        }

        // Tool 2
        if (tool2Icon != null)
        {
            if (tools[1] != null)
            {
                tool2Icon.sprite = tools[1].icon;
                tool2Icon.enabled = true;
            }
            else
            {
                tool2Icon.enabled = false;
            }
        }

        // Indicador de herramienta seleccionada
        if (tool1Selected != null)
            tool1Selected.SetActive(currentIndex == 0);

        if (tool2Selected != null)
            tool2Selected.SetActive(currentIndex == 1);
    }
}
