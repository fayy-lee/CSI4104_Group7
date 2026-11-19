
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GameObject resourcesPanel;
    public GameObject closeResourcesButton;

    private Canvas menuCanvas;

    void Awake()
    {
        AccessibleUIFactory.ConfigureEventSystemInput(this);
    }

    void Start()
    {
        if (resourcesPanel == null)
        {
            Debug.LogError("MainMenuManager requires a resources panel.", this);
            enabled = false;
            return;
        }

        menuCanvas = resourcesPanel.GetComponentInParent<Canvas>();
        if (menuCanvas == null)
        {
            Debug.LogError("MainMenuManager could not find a Canvas for its menu.", this);
            enabled = false;
            return;
        }

        if (closeResourcesButton == null)
        {
            Transform closeButton = menuCanvas.transform.Find("CloseButton");
            if (closeButton != null)
                closeResourcesButton = closeButton.gameObject;
        }

        if (closeResourcesButton == null)
        {
            Debug.LogError("MainMenuManager requires the menu CloseButton.", this);
            enabled = false;
            return;
        }

        menuCanvas.transform.localScale = Vector3.one;
        CanvasScaler scaler = menuCanvas.GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
            scaler.matchWidthOrHeight = 0.5f;
        }

        StyleMenu();
        resourcesPanel.SetActive(false);
        closeResourcesButton.SetActive(false);
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("RehabilitationRoom");
    }

    public void ShowResources()
    {
        if (resourcesPanel == null || closeResourcesButton == null)
        {
            Debug.LogError("MainMenuManager cannot open resources because its UI references are missing.", this);
            return;
        }

        resourcesPanel.SetActive(true);
        closeResourcesButton.SetActive(true);
    }

    public void CloseResources()
    {
        if (resourcesPanel != null)
            resourcesPanel.SetActive(false);

        if (closeResourcesButton != null)
            closeResourcesButton.SetActive(false);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void StyleMenu()
    {
        TMP_Text title = FindText("TitleText");
        if (title == null || title.font == null)
        {
            Debug.LogError("MainMenuManager requires a titled TextMeshPro element with a font asset.", this);
            enabled = false;
            return;
        }

        GameObject background = AccessibleUIFactory.CreatePanel(
            menuCanvas.transform, "MenuBackground", Vector2.zero, new Color(0.82f, 0.91f, 0.95f, 1f));
        RectTransform backgroundRect = background.GetComponent<RectTransform>();
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.offsetMin = Vector2.zero;
        backgroundRect.offsetMax = Vector2.zero;
        background.transform.SetAsFirstSibling();

        GameObject card = AccessibleUIFactory.CreatePanel(
            menuCanvas.transform, "MenuCard", new Vector2(560f, 520f), Color.white);
        card.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.96f);
        card.transform.SetSiblingIndex(1);

        RectTransform titleRect = title.rectTransform;
        title.text = "ACT-FAST";
        title.fontSize = 64f;
        title.fontStyle = FontStyles.Bold;
        title.color = new Color(0.06f, 0.18f, 0.25f, 1f);
        titleRect.anchoredPosition = new Vector2(0f, 190f);
        titleRect.sizeDelta = new Vector2(500f, 90f);

        AccessibleUIFactory.CreateLabel(
            menuCanvas.transform,
            "MenuSubtitle",
            "An immersive virtual reality experience\nAccessible Care and Training for Stroke Rehabilitation",
            new Vector2(540f, 64f),
            new Vector2(0f, 125f),
            title.font,
            19f,
            new Color(0.16f, 0.34f, 0.42f, 1f),
            TextAlignmentOptions.Center);

        SetButtonLayout("StartButton", new Vector2(0f, 25f), new Vector2(360f, 64f));
        SetButtonLayout("ResourcesButton", new Vector2(0f, -55f), new Vector2(360f, 64f));
        SetButtonLayout("ExitButton", new Vector2(0f, -135f), new Vector2(360f, 64f));
        AccessibleUIFactory.CreateLabel(
            menuCanvas.transform,
            "DesktopControlsGuide",
            "Desktop demo: WASD to move  |  Mouse to look  |  Tab to unlock pointer  |  E to interact",
            new Vector2(520f, 48f),
            new Vector2(0f, -220f),
            title.font,
            16f,
            new Color(0.16f, 0.34f, 0.42f, 1f),
            TextAlignmentOptions.Center);

        Button[] buttons = menuCanvas.GetComponentsInChildren<Button>(true);
        for (int i = 0; i < buttons.Length; i++)
        {
            Button button = buttons[i];
            button.enabled = true;
            button.interactable = true;
            Image image = button.GetComponent<Image>();
            if (image != null)
            {
                image.enabled = true;
                image.sprite = AccessibleUIFactory.GetRoundedSprite();
                image.type = image.sprite == null ? Image.Type.Simple : Image.Type.Sliced;
                image.color = GetButtonColor(button.gameObject.name);
            }

            ColorBlock colors = button.colors;
            colors.normalColor = GetButtonColor(button.gameObject.name);
            colors.highlightedColor = Color.Lerp(colors.normalColor, Color.white, 0.18f);
            colors.pressedColor = Color.Lerp(colors.normalColor, Color.black, 0.16f);
            colors.selectedColor = colors.highlightedColor;
            button.colors = colors;

            TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
            if (label != null)
            {
                label.color = Color.white;
                label.fontStyle = FontStyles.Bold;
                label.fontSize = 23f;
                label.enableAutoSizing = true;
                label.fontSizeMin = 18f;
                label.fontSizeMax = 24f;
                label.raycastTarget = false;
            }
        }

        RectTransform resourcesRect = resourcesPanel.GetComponent<RectTransform>();
        resourcesRect.anchorMin = new Vector2(0.5f, 0.5f);
        resourcesRect.anchorMax = new Vector2(0.5f, 0.5f);
        resourcesRect.anchoredPosition = Vector2.zero;
        resourcesRect.sizeDelta = new Vector2(760f, 520f);
        Image resourcesImage = resourcesPanel.GetComponent<Image>();
        if (resourcesImage != null)
            resourcesImage.color = new Color(0.06f, 0.13f, 0.18f, 0.97f);

        TMP_Text resourceText = resourcesPanel.GetComponentInChildren<TMP_Text>(true);
        if (resourceText != null)
        {
            resourceText.text =
                "LEARNING & RESOURCES\n\nStroke basics\nGeneral information about stroke and how its effects can vary between people.\n\nRecovery & support\nA reminder that recovery is individual and personal questions belong with a qualified healthcare professional.\n\nAccessible activities\nOptional, self-paced demonstrations with no timer.\n\nThis academic prototype is educational only; it is not medical advice or personalized treatment.";
            resourceText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            resourceText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            resourceText.rectTransform.anchoredPosition = new Vector2(0f, 45f);
            resourceText.rectTransform.sizeDelta = new Vector2(660f, 340f);
            resourceText.fontSize = 26f;
            resourceText.fontSizeMin = 21f;
            resourceText.fontSizeMax = 30f;
            resourceText.enableAutoSizing = true;
            resourceText.alignment = TextAlignmentOptions.Center;
            resourceText.color = Color.white;
            resourceText.textWrappingMode = TextWrappingModes.Normal;
        }

        RectTransform closeRect = closeResourcesButton.GetComponent<RectTransform>();
        if (closeRect != null)
        {
            closeRect.anchoredPosition = new Vector2(0f, -205f);
            closeRect.sizeDelta = new Vector2(200f, 58f);
        }
    }

    private TMP_Text FindText(string objectName)
    {
        TMP_Text[] texts = menuCanvas.GetComponentsInChildren<TMP_Text>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i].gameObject.name == objectName)
                return texts[i];
        }

        return null;
    }

    private void SetButtonLayout(string objectName, Vector2 position, Vector2 size)
    {
        Transform button = menuCanvas.transform.Find(objectName);
        if (button == null)
        {
            Debug.LogError("MainMenuManager could not find menu button " + objectName + ".", this);
            return;
        }

        RectTransform rect = button.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }
    }

    private static Color GetButtonColor(string objectName)
    {
        if (objectName == "StartButton")
            return new Color(0.03f, 0.42f, 0.46f, 1f);

        if (objectName == "ExitButton")
            return new Color(0.22f, 0.34f, 0.4f, 1f);

        return new Color(0.16f, 0.34f, 0.48f, 1f);
    }
}
