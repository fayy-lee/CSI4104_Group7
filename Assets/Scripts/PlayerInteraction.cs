
using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionDistance = 4f;
    public TextMeshProUGUI interactionPrompt;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI completionText;

    private Camera playerCamera;
    private GameObject currentTarget;
    private int score = 0;

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        interactionPrompt.gameObject.SetActive(false);
        completionText.gameObject.SetActive(false);
        scoreText.text = "Targets Collected: 0";
    }

    void Update()
    {
        CheckForInteractable();

        if (Input.GetKeyDown(KeyCode.E) && currentTarget != null)
        {
            if (currentTarget.CompareTag("ExerciseStation"))
            {
                ArmExerciseManager manager =
                    currentTarget.GetComponent<ArmExerciseManager>();

                if (manager != null)
                    manager.StartActivity();
            }
            else if (currentTarget.CompareTag("ResourceStation"))
            {
                ResourceStationManager manager =
                    currentTarget.GetComponent<ResourceStationManager>();

                if (manager != null)
                    manager.OpenResources();
            }
            else if (currentTarget.CompareTag("Target") && score < 5)
            {
                CollectTarget();
            }
        }
    }

    void CheckForInteractable()
    {
        Ray ray = new Ray(playerCamera.transform.position,
                          playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.CompareTag("Target"))
            {
                currentTarget = hit.collider.gameObject;
                interactionPrompt.text = "Press E to collect target";
                interactionPrompt.gameObject.SetActive(true);
                return;
            }

            if (hit.collider.CompareTag("ExerciseStation"))
            {
                currentTarget = hit.collider.gameObject;
                interactionPrompt.text = "Press E to start arm exercise";
                interactionPrompt.gameObject.SetActive(true);
                return;
            }

            if (hit.collider.CompareTag("ResourceStation"))
            {
                currentTarget = hit.collider.gameObject;
                interactionPrompt.text = "Press E to view resources";
                interactionPrompt.gameObject.SetActive(true);
                return;
            }
        }

        currentTarget = null;
        interactionPrompt.gameObject.SetActive(false);
    }

    void CollectTarget()
    {
        score++;
        scoreText.text = "Targets Collected: " + score;

        if (score >= 5)
        {
            completionText.gameObject.SetActive(true);
            interactionPrompt.gameObject.SetActive(false);
            currentTarget = null;
            return;
        }

        currentTarget.transform.position = new Vector3(
            Random.Range(-1.5f, 1.5f),
            1.8f,
            Random.Range(1.5f, 2.5f)
        );

        currentTarget = null;
        interactionPrompt.gameObject.SetActive(false);
    }
}
