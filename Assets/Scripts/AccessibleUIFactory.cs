using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public static class AccessibleUIFactory
{
    private static Sprite roundedSprite;

    public static void ConfigureEventSystemInput(MonoBehaviour context)
    {
        InputSystemUIInputModule module = Object.FindAnyObjectByType<InputSystemUIInputModule>();
        if (module == null)
        {
            Debug.LogError("No InputSystemUIInputModule was found for desktop UI input.", context);
            return;
        }

        InputActionAsset actions = InputSystem.actions;
        if (actions == null)
        {
            Debug.LogError("The project-wide Input Actions asset is not configured.", context);
            return;
        }

        InputActionMap uiMap = actions.FindActionMap("UI", false);
        if (uiMap == null)
        {
            Debug.LogError("The project-wide Input Actions asset has no UI action map.", context);
            return;
        }

        module.actionsAsset = actions;
        module.point = FindUIAction(uiMap, "Point", context);
        module.move = FindUIAction(uiMap, "Navigate", context);
        module.submit = FindUIAction(uiMap, "Submit", context);
        module.cancel = FindUIAction(uiMap, "Cancel", context);
        module.leftClick = FindUIAction(uiMap, "Click", context);
        module.rightClick = FindUIAction(uiMap, "RightClick", context);
        module.middleClick = FindUIAction(uiMap, "MiddleClick", context);
        module.scrollWheel = FindUIAction(uiMap, "ScrollWheel", context);
        module.trackedDevicePosition = FindUIAction(uiMap, "TrackedDevicePosition", context);
        module.trackedDeviceOrientation = FindUIAction(uiMap, "TrackedDeviceOrientation", context);
    }

    private static InputActionReference FindUIAction(
        InputActionMap uiMap,
        string actionName,
        MonoBehaviour context)
    {
        InputAction action = uiMap.FindAction(actionName, false);
        if (action == null)
        {
            Debug.LogError("The UI action map is missing the " + actionName + " action.", context);
            return null;
        }

        return InputActionReference.Create(action);
    }

    public static Button CreateButton(
        Transform parent,
        string objectName,
        string label,
        Vector2 size,
        Vector2 position,
        TMP_FontAsset font,
        Color background,
        UnityAction onClick)
    {
        GameObject buttonObject = new GameObject(
            objectName,
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image),
            typeof(Button));

        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.SetParent(parent, false);
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = position;
        buttonRect.sizeDelta = size;

        Image image = buttonObject.GetComponent<Image>();
        image.sprite = GetRoundedSprite();
        image.type = image.sprite == null ? Image.Type.Simple : Image.Type.Sliced;
        image.color = background;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        ColorBlock colors = button.colors;
        colors.normalColor = background;
        colors.highlightedColor = Color.Lerp(background, Color.white, 0.18f);
        colors.pressedColor = Color.Lerp(background, Color.black, 0.16f);
        colors.selectedColor = colors.highlightedColor;
        colors.disabledColor = new Color(0.45f, 0.5f, 0.52f, 0.65f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.12f;
        button.colors = colors;

        if (onClick != null)
            button.onClick.AddListener(onClick);

        GameObject labelObject = new GameObject(
            "Label",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(TextMeshProUGUI));
        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.SetParent(buttonRect, false);
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        TextMeshProUGUI labelText = labelObject.GetComponent<TextMeshProUGUI>();
        labelText.text = label;
        labelText.font = font;
        labelText.fontSize = 22f;
        labelText.fontSizeMin = 18f;
        labelText.fontSizeMax = 24f;
        labelText.enableAutoSizing = true;
        labelText.fontStyle = FontStyles.Bold;
        labelText.color = Color.white;
        labelText.alignment = TextAlignmentOptions.Center;
        labelText.textWrappingMode = TextWrappingModes.NoWrap;
        labelText.raycastTarget = false;

        return button;
    }

    public static GameObject CreatePanel(
        Transform parent,
        string objectName,
        Vector2 size,
        Color background)
    {
        GameObject panelObject = new GameObject(
            objectName,
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image));

        RectTransform panelRect = panelObject.GetComponent<RectTransform>();
        panelRect.SetParent(parent, false);
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = size;

        Image image = panelObject.GetComponent<Image>();
        image.sprite = GetRoundedSprite();
        image.type = image.sprite == null ? Image.Type.Simple : Image.Type.Sliced;
        image.color = background;

        return panelObject;
    }

    public static TextMeshProUGUI CreateLabel(
        Transform parent,
        string objectName,
        string text,
        Vector2 size,
        Vector2 position,
        TMP_FontAsset font,
        float fontSize,
        Color color,
        TextAlignmentOptions alignment)
    {
        GameObject labelObject = new GameObject(
            objectName,
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(TextMeshProUGUI));

        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.SetParent(parent, false);
        labelRect.anchorMin = new Vector2(0.5f, 0.5f);
        labelRect.anchorMax = new Vector2(0.5f, 0.5f);
        labelRect.anchoredPosition = position;
        labelRect.sizeDelta = size;

        TextMeshProUGUI label = labelObject.GetComponent<TextMeshProUGUI>();
        label.text = text;
        label.font = font;
        label.fontSize = fontSize;
        label.fontSizeMin = Mathf.Min(18f, fontSize);
        label.fontSizeMax = fontSize;
        label.enableAutoSizing = true;
        label.color = color;
        label.alignment = alignment;
        label.textWrappingMode = TextWrappingModes.Normal;
        label.raycastTarget = false;

        return label;
    }

    public static Sprite GetRoundedSprite()
    {
        if (roundedSprite == null)
        {
            const int size = 32;
            const float radius = 8f;
            const float innerEdge = size * 0.5f - radius;
            Color[] pixels = new Color[size * size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = Mathf.Max(Mathf.Abs(x + 0.5f - size * 0.5f) - innerEdge, 0f);
                    float dy = Mathf.Max(Mathf.Abs(y + 0.5f - size * 0.5f) - innerEdge, 0f);
                    pixels[y * size + x] = dx * dx + dy * dy <= radius * radius
                        ? Color.white
                        : Color.clear;
                }
            }

            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                name = "ACT-FAST Rounded UI Texture",
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.HideAndDontSave
            };
            texture.SetPixels(pixels);
            texture.Apply(false, true);

            roundedSprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, size, size),
                new Vector2(0.5f, 0.5f),
                size,
                0,
                SpriteMeshType.FullRect,
                new Vector4(radius, radius, radius, radius));
            roundedSprite.name = "ACT-FAST Rounded UI Sprite";
            roundedSprite.hideFlags = HideFlags.HideAndDontSave;
        }

        return roundedSprite;
    }
}
