using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArmExerciseManager : MonoBehaviour
{
    public GameObject exercisePanel;
    public TextMeshProUGUI exerciseText;

    private readonly string[] instructions =
    {
        "Step 1 of 5\n\nSettle into a comfortable seated or standing position.",
        "Step 2 of 5\n\nIf comfortable, slowly move one arm forward within a comfortable range.",
        "Step 3 of 5\n\nReturn your arm to a resting position when you are ready.",
        "Step 4 of 5\n\nYou may repeat on the other side, or skip this step.",
        "Step 5 of 5\n\nSettle into a comfortable resting position."
    };

    private enum ActivityView
    {
        Closed,
        Intro,
        Instructions,
        Paused,
        Complete
    }

    private ActivityView currentView = ActivityView.Closed;
    private int currentStep;
    private Button beginButton;
    private Button previousButton;
    private Button nextButton;
    private Button pauseButton;
    private Button resumeButton;
    private Button restartButton;
    private Button exitButton;

    public bool IsOpen
    {
        get { return exercisePanel != null && exercisePanel.activeSelf; }
    }

    void Start()
    {
        if (exercisePanel == null || exerciseText == null)
        {
            Debug.LogError("ArmExerciseManager requires an exercise panel and instruction text.", this);
            enabled = false;
            return;
        }

        exercisePanel.SetActive(false);
        ConfigurePanel();
        CreateControls();
    }

    void Update()
    {
        if (!IsOpen)
            return;

        if (currentView == ActivityView.Instructions)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                PreviousStep();
            else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.RightArrow))
                NextStep();
            else if (Input.GetKeyDown(KeyCode.P))
                PauseActivity();
        }
        else if (currentView == ActivityView.Paused && Input.GetKeyDown(KeyCode.P))
        {
            ResumeActivity();
        }
        else if (currentView == ActivityView.Complete && Input.GetKeyDown(KeyCode.R))
        {
            RestartActivity();
        }
    }

    public void StartActivity()
    {
        if (!enabled || exercisePanel == null || exerciseText == null)
            return;

        currentStep = 0;
        currentView = ActivityView.Intro;
        exercisePanel.SetActive(true);
        exerciseText.text =
            "Guided Movement\n\nA self-paced movement demonstration. Participation is optional; pause or leave at any time.\n\nThis academic prototype is not medical advice.";
        UpdateControls();
    }

    public void BeginActivity()
    {
        currentStep = 0;
        ShowStep();
    }

    public void PreviousStep()
    {
        if (currentView != ActivityView.Instructions || currentStep <= 0)
            return;

        currentStep--;
        ShowStep();
    }

    public void NextStep()
    {
        if (currentView != ActivityView.Instructions)
            return;

        if (currentStep >= instructions.Length - 1)
        {
            CompleteActivity();
            return;
        }

        currentStep++;
        ShowStep();
    }

    public void PauseActivity()
    {
        if (currentView != ActivityView.Instructions)
            return;

        currentView = ActivityView.Paused;
        exerciseText.text = "Paused\n\nResume when you are ready, or exit the activity.";
        UpdateControls();
    }

    public void ResumeActivity()
    {
        if (currentView != ActivityView.Paused)
            return;

        ShowStep();
    }

    public void RestartActivity()
    {
        currentStep = 0;
        ShowStep();
    }

    public void ExitActivity()
    {
        currentView = ActivityView.Closed;
        if (exercisePanel != null)
            exercisePanel.SetActive(false);
    }

    private void ShowStep()
    {
        currentView = ActivityView.Instructions;
        exerciseText.text = instructions[currentStep];
        UpdateControls();
    }

    private void CompleteActivity()
    {
        currentView = ActivityView.Complete;
        exerciseText.text =
            "Demonstration complete\n\nYou reached the end of the five-step activity. You may repeat it or return to the room.";
        UpdateControls();
    }

    private void ConfigurePanel()
    {
        Image panelImage = exercisePanel.GetComponent<Image>();
        if (panelImage != null)
            panelImage.color = new Color(0.06f, 0.13f, 0.18f, 0.96f);

        RectTransform textRect = exerciseText.rectTransform;
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.anchoredPosition = new Vector2(0f, 35f);
        textRect.sizeDelta = new Vector2(680f, 340f);

        exerciseText.fontSize = 28f;
        exerciseText.fontSizeMin = 22f;
        exerciseText.fontSizeMax = 32f;
        exerciseText.enableAutoSizing = true;
        exerciseText.alignment = TextAlignmentOptions.Center;
        exerciseText.color = Color.white;
        exerciseText.textWrappingMode = TextWrappingModes.Normal;
    }

    private void CreateControls()
    {
        if (exerciseText.font == null)
        {
            Debug.LogError("ArmExerciseManager requires a TextMeshPro font asset.", exerciseText);
            enabled = false;
            return;
        }

        Color teal = new Color(0.03f, 0.42f, 0.46f, 1f);
        Color blue = new Color(0.16f, 0.34f, 0.48f, 1f);
        Transform parent = exercisePanel.transform;
        beginButton = AccessibleUIFactory.CreateButton(parent, "BeginExerciseButton", "Begin",
            new Vector2(180f, 58f), new Vector2(0f, -175f), exerciseText.font, teal, BeginActivity);
        previousButton = AccessibleUIFactory.CreateButton(parent, "PreviousStepButton", "Previous",
            new Vector2(150f, 58f), new Vector2(-265f, -220f), exerciseText.font, blue, PreviousStep);
        nextButton = AccessibleUIFactory.CreateButton(parent, "NextStepButton", "Next",
            new Vector2(150f, 58f), new Vector2(-90f, -220f), exerciseText.font, teal, NextStep);
        pauseButton = AccessibleUIFactory.CreateButton(parent, "PauseExerciseButton", "Pause",
            new Vector2(150f, 58f), new Vector2(85f, -220f), exerciseText.font, blue, PauseActivity);
        resumeButton = AccessibleUIFactory.CreateButton(parent, "ResumeExerciseButton", "Resume",
            new Vector2(150f, 58f), new Vector2(85f, -220f), exerciseText.font, teal, ResumeActivity);
        restartButton = AccessibleUIFactory.CreateButton(parent, "RestartExerciseButton", "Restart",
            new Vector2(150f, 58f), new Vector2(-90f, -220f), exerciseText.font, teal, RestartActivity);
        exitButton = AccessibleUIFactory.CreateButton(parent, "ExitExerciseButton", "Exit",
            new Vector2(150f, 58f), new Vector2(260f, -220f), exerciseText.font, blue, ExitActivity);

        UpdateControls();
    }

    private void UpdateControls()
    {
        SetVisible(beginButton, currentView == ActivityView.Intro);
        SetVisible(previousButton, currentView == ActivityView.Instructions);
        SetVisible(nextButton, currentView == ActivityView.Instructions);
        SetVisible(pauseButton, currentView == ActivityView.Instructions);
        SetVisible(resumeButton, currentView == ActivityView.Paused);
        SetVisible(restartButton, currentView == ActivityView.Complete);
        SetVisible(exitButton, currentView != ActivityView.Closed);

        if (previousButton != null)
            previousButton.interactable = currentStep > 0;

        if (nextButton != null && currentView == ActivityView.Instructions)
            nextButton.GetComponentInChildren<TextMeshProUGUI>().text =
                currentStep == instructions.Length - 1 ? "Complete" : "Next";
    }

    private static void SetVisible(Button button, bool visible)
    {
        if (button != null)
            button.gameObject.SetActive(visible);
    }
}
