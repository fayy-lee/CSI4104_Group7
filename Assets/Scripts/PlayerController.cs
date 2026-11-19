
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;

    private CharacterController controller;
    private Transform playerCamera;

    private float verticalRotation = 0f;
    private float gravity = -9.81f;
    private float verticalVelocity = 0f;
    private bool isPaused;
    private XRInteractionSimulator interactionSimulator;
    private bool simulatorPointAndClickBeforePause;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        interactionSimulator = FindAnyObjectByType<XRInteractionSimulator>();

        Camera cameraComponent = GetComponentInChildren<Camera>();

        if (cameraComponent == null)
        {
            Debug.LogError("PlayerController: No child camera found.");
            enabled = false;
            return;
        }

        playerCamera = cameraComponent.transform;
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (isPaused || controller == null || playerCamera == null)
            return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            bool unlockCursor = Cursor.lockState == CursorLockMode.Locked;
            Cursor.lockState = unlockCursor ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = unlockCursor;
        }

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            transform.Rotate(Vector3.up * mouseX);

            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f);

            playerCamera.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement =
            transform.right * horizontal +
            transform.forward * vertical;

        controller.Move(movement * moveSpeed * Time.deltaTime);

        // Gravity
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        controller.Move(
            Vector3.up * verticalVelocity * Time.deltaTime
        );

    }

    public void SetPaused(bool paused)
    {
        if (interactionSimulator != null)
        {
            if (paused && !isPaused)
            {
                simulatorPointAndClickBeforePause = interactionSimulator.usePointAndClick;
                interactionSimulator.usePointAndClick = false;
            }
            else if (!paused && isPaused)
            {
                interactionSimulator.usePointAndClick = simulatorPointAndClickBeforePause;
            }
        }

        isPaused = paused;
        if (!paused)
            Time.timeScale = 1f;

        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = paused;
    }
}
