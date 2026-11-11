
using UnityEngine;
using TMPro;

public class ArmExerciseManager : MonoBehaviour
{
    public GameObject exercisePanel;
    public TextMeshProUGUI exerciseText;

    private bool activityStarted = false;
    private int currentStep = 0;

    private string[] instructions =
    {
        "Step 1 of 5\n\nSit comfortably and prepare for the activity.\n\nPress SPACE to continue.",
        "Step 2 of 5\n\nGently raise one arm forward, within a comfortable range.\n\nPress SPACE when ready.",
        "Step 3 of 5\n\nSlowly lower your arm back to a resting position.\n\nPress SPACE when ready.",
        "Step 4 of 5\n\nRepeat the movement on the other side, if comfortable.\n\nPress SPACE when ready.",
        "Step 5 of 5\n\nReturn to a comfortable resting position.\n\nPress SPACE to finish."
    };

    void Start()
    {
        exercisePanel.SetActive(false);
    }

    void Update()
    {
        if (!exercisePanel.activeSelf)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            exercisePanel.SetActive(false);
            activityStarted = false;
            return;
        }

        if (!activityStarted)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentStep++;

            if (currentStep >= instructions.Length)
            {
                exerciseText.text =
                    "ACTIVITY COMPLETE!\n\nYou completed the guided arm-reach activity.\n\nPress ESC to close.";

                activityStarted = false;
            }
            else
            {
                exerciseText.text = instructions[currentStep];
            }
        }
    }

    public void StartActivity()
    {
        currentStep = 0;
        activityStarted = true;
        exercisePanel.SetActive(true);
        exerciseText.text = instructions[currentStep];
    }
}
