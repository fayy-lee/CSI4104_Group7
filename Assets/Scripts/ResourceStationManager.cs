using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceStationManager : MonoBehaviour
{
    public GameObject resourcePanel;

    private readonly string[] resourcePages =
    {
        "STROKE BASICS\n\nA stroke can affect movement, speech, thinking, mood, or other abilities. Effects vary between people.\n\nThis prototype shares general educational information only.",
        "RECOVERY & SUPPORT\n\nRecovery experiences are individual. A qualified healthcare professional can discuss personal questions, goals, and available supports.\n\nACT-FAST is not a clinical tool.",
        "ACCESSIBLE ACTIVITY\n\nActivities here are simple demonstrations with no timer. Participation is optional; pause or leave at any time.\n\nThis prototype does not provide diagnosis or medical advice."
    };

    private TextMeshProUGUI resourceText;
    private Button backButton;
    private Button nextButton;
    private Button closeButton;
    private int currentPage;

    public bool IsOpen
    {
        get { return resourcePanel != null && resourcePanel.activeSelf; }
    }

    void Start()
    {
        if (resourcePanel == null)
        {
            Debug.LogError("ResourceStationManager requires a resource panel.", this);
            enabled = false;
            return;
        }

        resourceText = resourcePanel.GetComponentInChildren<TextMeshProUGUI>(true);
        if (resourceText == null)
        {
            Debug.LogError("ResourceStationManager could not find resource text inside its panel.", this);
            enabled = false;
            return;
        }

        ConfigurePanel();
        CreateControls();
        resourcePanel.SetActive(false);
    }

    public void OpenResources()
    {
        if (!enabled || resourcePanel == null)
            return;

        currentPage = 0;
        ShowPage();
        resourcePanel.SetActive(true);
    }

    public void PreviousPage()
    {
        if (currentPage <= 0)
            return;

        currentPage--;
        ShowPage();
    }

    public void NextPage()
    {
        if (currentPage >= resourcePages.Length - 1)
            return;

        currentPage++;
        ShowPage();
    }

    public void CloseResources()
    {
        if (resourcePanel != null)
            resourcePanel.SetActive(false);
    }

    private void ConfigurePanel()
    {
        Image panelImage = resourcePanel.GetComponent<Image>();
        if (panelImage != null)
            panelImage.color = new Color(0.06f, 0.13f, 0.18f, 0.96f);

        RectTransform textRect = resourceText.rectTransform;
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.anchoredPosition = new Vector2(0f, 45f);
        textRect.sizeDelta = new Vector2(680f, 340f);

        resourceText.fontSize = 25f;
        resourceText.fontSizeMin = 20f;
        resourceText.fontSizeMax = 28f;
        resourceText.enableAutoSizing = true;
        resourceText.alignment = TextAlignmentOptions.Left;
        resourceText.color = Color.white;
        resourceText.textWrappingMode = TextWrappingModes.Normal;
    }

    private void CreateControls()
    {
        if (resourceText.font == null)
        {
            Debug.LogError("ResourceStationManager requires a TextMeshPro font asset.", resourceText);
            enabled = false;
            return;
        }

        Color teal = new Color(0.03f, 0.42f, 0.46f, 1f);
        Color blue = new Color(0.16f, 0.34f, 0.48f, 1f);
        Transform parent = resourcePanel.transform;
        backButton = AccessibleUIFactory.CreateButton(parent, "ResourceBackButton", "Back",
            new Vector2(180f, 58f), new Vector2(-210f, -220f), resourceText.font, blue, PreviousPage);
        nextButton = AccessibleUIFactory.CreateButton(parent, "ResourceNextButton", "Next",
            new Vector2(180f, 58f), new Vector2(0f, -220f), resourceText.font, teal, NextPage);
        closeButton = AccessibleUIFactory.CreateButton(parent, "ResourceCloseButton", "Close",
            new Vector2(180f, 58f), new Vector2(210f, -220f), resourceText.font, blue, CloseResources);

        UpdateControls();
    }

    private void ShowPage()
    {
        resourceText.text = resourcePages[currentPage] +
            "\n\n" + (currentPage + 1) + " of " + resourcePages.Length;
        UpdateControls();
    }

    private void UpdateControls()
    {
        if (backButton != null)
            backButton.interactable = currentPage > 0;

        if (nextButton != null)
            nextButton.gameObject.SetActive(currentPage < resourcePages.Length - 1);
    }
}
