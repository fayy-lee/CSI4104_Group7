using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionDistance = 4f;
    public float holdDistance = 1.5f;
    public Camera playerCamera;

    [Header("UI References")]
    public TextMeshProUGUI interactionPrompt;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI completionText;

    private enum ActivityMode
    {
        None,
        ReachAndPlace,
        EverydayActivities,
        GuidedMovement,
        Resources
    }

    private GameObject[] targetObjects;
    private Vector3[] targetStartPositions;
    private Quaternion[] targetStartRotations;
    private GameObject[] everydayObjects;
    private Vector3[] everydayStartPositions;
    private Quaternion[] everydayStartRotations;
    private readonly HashSet<GameObject> placedObjects = new HashSet<GameObject>();

    private GameObject currentInteractable;
    private GameObject heldObject;
    private GameObject reachPlacementZone;
    private GameObject everydayPlacementZone;
    private GameObject pausePanel;
    private GameObject completionPanel;
    private Button pauseButton;
    private Button restartButton;
    private TextMeshProUGUI pauseText;
    private PlayerController playerController;
    private ArmExerciseManager exerciseManager;
    private ResourceStationManager resourceManager;
    private Renderer highlightedRenderer;
    private MaterialPropertyBlock originalHighlightBlock;
    private ActivityMode currentActivity;
    private ActivityMode completedActivity;
    private int placedTargetCount;
    private int placedEverydayCount;
    private bool stationOverlayWasOpen;
    private Color reachPadColor = new Color(0.04f, 0.55f, 0.55f, 1f);
    private Color everydayPadColor = new Color(0.12f, 0.48f, 0.55f, 1f);
    private readonly Color validPlacementColor = new Color(0.05f, 0.72f, 0.55f, 1f);
    private readonly Color invalidPlacementColor = new Color(0.78f, 0.28f, 0.26f, 1f);
    private string temporaryFeedback;
    private float temporaryFeedbackUntil;

    void Awake()
    {
        AccessibleUIFactory.ConfigureEventSystemInput(this);
    }

    void Start()
    {
        if (playerCamera == null)
        {
            Camera childCamera = GetComponentInChildren<Camera>();
            playerCamera = childCamera != null ? childCamera : Camera.main;
        }

        if (playerCamera == null)
        {
            Debug.LogError("PlayerInteraction requires a player camera.", this);
            enabled = false;
            return;
        }

        if (interactionPrompt == null || scoreText == null || completionText == null)
        {
            Debug.LogError("PlayerInteraction requires prompt, progress, and completion TextMeshPro references.", this);
            enabled = false;
            return;
        }

        playerController = GetComponent<PlayerController>();
        if (playerController == null)
            playerController = FindAnyObjectByType<PlayerController>();
        if (playerController == null)
            Debug.LogError("PlayerInteraction could not find a PlayerController; pause controls will not stop movement.", this);

        LoadActivityObjects();
        CacheStationManagers();
        ConfigureHud();
        CreatePlacementZones();
        CreateStationMarkers();
        CreateOverlayControls();
        UpdateProgressText();
    }

    void Update()
    {
        if (StationOverlayIsOpen())
        {
            if (!stationOverlayWasOpen)
                SetPlayerPaused(true);

            if (pauseButton != null)
                pauseButton.gameObject.SetActive(false);
            stationOverlayWasOpen = true;
            ClearHighlight();
            if (Input.GetKeyDown(KeyCode.Escape))
                CloseStationOverlay();
            return;
        }

        if (stationOverlayWasOpen)
        {
            stationOverlayWasOpen = false;
            if (pauseButton != null)
                pauseButton.gameObject.SetActive(true);
            SetPlayerPaused(false);
        }

        if (completionPanel != null && completionPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                CloseCompletionScreen();
            return;
        }

        if (pausePanel != null && pausePanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                ClosePauseMenu();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenPauseMenu();
            return;
        }

        if (heldObject != null)
            MoveHeldObject();

        CheckForInteractable();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject != null)
                TryPlaceHeldObject();
            else if (currentInteractable != null)
                InteractWithCurrentTarget();
        }
    }

    private void LoadActivityObjects()
    {
        targetObjects = GameObject.FindGameObjectsWithTag("Target");
        Array.Sort(targetObjects, CompareObjectNames);
        targetStartPositions = new Vector3[targetObjects.Length];
        targetStartRotations = new Quaternion[targetObjects.Length];
        for (int i = 0; i < targetObjects.Length; i++)
        {
            targetStartPositions[i] = targetObjects[i].transform.position;
            targetStartRotations[i] = targetObjects[i].transform.rotation;
        }

        everydayObjects = FindAndTagEverydayObjects();
        Array.Sort(everydayObjects, CompareObjectNames);
        everydayStartPositions = new Vector3[everydayObjects.Length];
        everydayStartRotations = new Quaternion[everydayObjects.Length];
        for (int i = 0; i < everydayObjects.Length; i++)
        {
            everydayStartPositions[i] = everydayObjects[i].transform.position;
            everydayStartRotations[i] = everydayObjects[i].transform.rotation;
        }

        if (targetObjects.Length != 5)
            Debug.LogError("Reach & Place expects five Target objects; found " + targetObjects.Length + ".", this);
        if (everydayObjects.Length != 3)
            Debug.LogError("Everyday Activities expects Cup, Block, and Ball tagged EverydayObject; found " +
                everydayObjects.Length + ".", this);
    }

    private GameObject[] FindAndTagEverydayObjects()
    {
        GameObject station = GameObject.Find("EverydayActivitiesStation");
        if (station == null)
        {
            Debug.LogError("PlayerInteraction could not find EverydayActivitiesStation.", this);
            return new GameObject[0];
        }

        string[] requiredNames = { "Cup", "Block", "Ball" };
        List<GameObject> objects = new List<GameObject>(requiredNames.Length);
        Transform[] children = station.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < requiredNames.Length; i++)
        {
            GameObject match = null;
            for (int childIndex = 0; childIndex < children.Length; childIndex++)
            {
                if (children[childIndex].name == requiredNames[i])
                {
                    match = children[childIndex].gameObject;
                    break;
                }
            }

            if (match == null)
            {
                Debug.LogError("PlayerInteraction could not find " + requiredNames[i] +
                    " under EverydayActivitiesStation.", this);
                continue;
            }

            match.tag = "EverydayObject";
            objects.Add(match);
        }

        return objects.ToArray();
    }

    private void CacheStationManagers()
    {
        GameObject exerciseStation = GameObject.FindGameObjectWithTag("ExerciseStation");
        if (exerciseStation != null)
            exerciseManager = exerciseStation.GetComponent<ArmExerciseManager>();
        if (exerciseManager == null)
            Debug.LogError("PlayerInteraction could not find ArmExerciseManager on the ExerciseStation.", this);

        GameObject resourceStation = GameObject.FindGameObjectWithTag("ResourceStation");
        if (resourceStation != null)
            resourceManager = resourceStation.GetComponent<ResourceStationManager>();
        if (resourceManager == null)
            Debug.LogError("PlayerInteraction could not find ResourceStationManager on the ResourceStation.", this);
    }

    private void ConfigureHud()
    {
        Canvas canvas = scoreText.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("PlayerInteraction could not find a Canvas for its HUD.", this);
            enabled = false;
            return;
        }

        canvas.transform.localScale = Vector3.one;
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
            scaler.matchWidthOrHeight = 0.5f;
        }

        ConfigureText(scoreText, new Vector2(0f, 315f), new Vector2(900f, 54f), 28f,
            TextAlignmentOptions.Center);
        ConfigureText(interactionPrompt, new Vector2(0f, -300f), new Vector2(1000f, 82f), 22f,
            TextAlignmentOptions.Center);

        scoreText.color = new Color(0.04f, 0.19f, 0.25f, 1f);
        interactionPrompt.color = new Color(0.04f, 0.19f, 0.25f, 1f);
        interactionPrompt.gameObject.SetActive(true);
        Transform legacyInstructions = canvas.transform.Find("ActivityInstructions");
        if (legacyInstructions != null)
            legacyInstructions.gameObject.SetActive(false);
        interactionPrompt.text =
            "Look at an object and press E to pick it up. Move near its teal pad or aim at it, then press E. Tab unlocks the pointer; Esc opens controls.";

        Button[] existingButtons = canvas.GetComponentsInChildren<Button>(true);
        for (int i = 0; i < existingButtons.Length; i++)
        {
            Button button = existingButtons[i];
            Image image = button.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = AccessibleUIFactory.GetRoundedSprite();
                image.type = image.sprite == null ? Image.Type.Simple : Image.Type.Sliced;
                image.color = new Color(0.16f, 0.34f, 0.48f, 1f);
            }

            TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
            if (label != null)
            {
                label.color = Color.white;
                label.fontStyle = FontStyles.Bold;
                label.fontSize = 21f;
                label.raycastTarget = false;
            }
        }
    }

    private static void ConfigureText(
        TextMeshProUGUI text,
        Vector2 position,
        Vector2 size,
        float fontSize,
        TextAlignmentOptions alignment)
    {
        RectTransform rect = text.rectTransform;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        text.fontSize = fontSize;
        text.fontSizeMin = fontSize - 4f;
        text.fontSizeMax = fontSize + 2f;
        text.enableAutoSizing = true;
        text.fontStyle = FontStyles.Bold;
        text.alignment = alignment;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.raycastTarget = false;
    }

    private void CreatePlacementZones()
    {
        GameObject reachTable = GameObject.Find("ActivityTable");
        GameObject everydayTable = GameObject.Find("EverydayActivityTable");
        if (reachTable == null || everydayTable == null)
        {
            Debug.LogError("PlayerInteraction requires ActivityTable and EverydayActivityTable in the room.", this);
            enabled = false;
            return;
        }

        CreateTableLegs(reachTable);
        CreateTableLegs(everydayTable);

        reachPlacementZone = CreatePlacementZone(
            reachTable.transform,
            "ReachPlacementZone",
            "ReachPlaceZone",
            new Vector3(0f, 0.515f, 0.25f),
            new Vector3(0.88f, 0.025f, 0.18f),
            reachPadColor);

        everydayPlacementZone = CreatePlacementZone(
            everydayTable.transform,
            "EverydayPlacementZone",
            "EverydayPlaceZone",
            new Vector3(0f, 0.515f, 0.25f),
            new Vector3(0.75f, 0.025f, 0.18f),
            everydayPadColor);
    }

    private static void CreateTableLegs(GameObject table)
    {
        Renderer tabletop = table.GetComponent<Renderer>();
        if (tabletop == null)
            return;

        Bounds bounds = tabletop.bounds;
        float legHeight = bounds.min.y;
        if (legHeight <= 0.1f)
            return;

        float xOffset = bounds.extents.x * 0.86f;
        float zOffset = bounds.extents.z * 0.72f;
        Vector3[] offsets =
        {
            table.transform.right * xOffset + table.transform.forward * zOffset,
            table.transform.right * xOffset - table.transform.forward * zOffset,
            -table.transform.right * xOffset + table.transform.forward * zOffset,
            -table.transform.right * xOffset - table.transform.forward * zOffset
        };

        for (int i = 0; i < offsets.Length; i++)
        {
            GameObject leg = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leg.name = table.name + "Leg";
            leg.transform.position = table.transform.position + offsets[i] + Vector3.up * (legHeight * 0.5f);
            leg.transform.localScale = new Vector3(0.16f, legHeight, 0.16f);
            leg.GetComponent<Renderer>().sharedMaterial = tabletop.sharedMaterial;
            Destroy(leg.GetComponent<Collider>());
        }
    }

    private void CreateStationMarkers()
    {
        CreateStationMarker(
            "ReachAndPlaceStation",
            "ReachAndPlaceFloorMarker",
            new Vector3(3.8f, 0.025f, 3.2f),
            new Color(0.84f, 0.93f, 0.94f, 1f));
        CreateStationMarker(
            "GuidedMovementStation",
            "GuidedMovementFloorMarker",
            new Vector3(3.2f, 0.025f, 3.6f),
            new Color(0.86f, 0.93f, 0.96f, 1f));
        CreateStationMarker(
            "EverydayActivitiesStation",
            "EverydayActivitiesFloorMarker",
            new Vector3(3.8f, 0.025f, 3.2f),
            new Color(0.85f, 0.94f, 0.91f, 1f));
        CreateStationMarker(
            "LearningResourcesStation",
            "LearningResourcesFloorMarker",
            new Vector3(3.2f, 0.025f, 3.6f),
            new Color(0.91f, 0.93f, 0.97f, 1f));
    }

    private void CreateStationMarker(string stationName, string markerName, Vector3 size, Color color)
    {
        GameObject station = GameObject.Find(stationName);
        if (station == null)
        {
            Debug.LogError("PlayerInteraction could not find " + stationName + " for its floor marker.", this);
            return;
        }

        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
        marker.name = markerName;
        marker.transform.SetParent(station.transform, false);
        marker.transform.localPosition = new Vector3(0f, 0.015f, 0f);
        marker.transform.localScale = size;
        marker.GetComponent<Renderer>().material.color = color;
        Destroy(marker.GetComponent<Collider>());
    }

    private GameObject CreatePlacementZone(
        Transform parent,
        string objectName,
        string objectTag,
        Vector3 localPosition,
        Vector3 localScale,
        Color color)
    {
        GameObject zone = GameObject.CreatePrimitive(PrimitiveType.Cube);
        zone.name = objectName;
        zone.tag = objectTag;
        zone.transform.SetParent(parent, false);
        zone.transform.localPosition = localPosition;
        zone.transform.localScale = localScale;
        Renderer renderer = zone.GetComponent<Renderer>();
        renderer.material.color = color;
        return zone;
    }

    private void CreateOverlayControls()
    {
        TMP_FontAsset font = scoreText.font;
        if (font == null)
        {
            Debug.LogError("PlayerInteraction requires a TextMeshPro font asset for its controls.", this);
            enabled = false;
            return;
        }

        Canvas canvas = scoreText.GetComponentInParent<Canvas>();
        Color teal = new Color(0.03f, 0.42f, 0.46f, 1f);
        Color blue = new Color(0.16f, 0.34f, 0.48f, 1f);

        pauseButton = AccessibleUIFactory.CreateButton(
            canvas.transform, "PauseMenuButton", "Pause",
            new Vector2(150f, 58f), Vector2.zero, font, teal, OpenPauseMenu);
        SetTopRight(pauseButton.GetComponent<RectTransform>(), new Vector2(-24f, -24f), new Vector2(150f, 58f));

        pausePanel = AccessibleUIFactory.CreatePanel(
            canvas.transform, "PausePanel", new Vector2(640f, 430f), new Color(0.06f, 0.13f, 0.18f, 0.97f));
        pausePanel.SetActive(false);
        pauseText = AccessibleUIFactory.CreateLabel(
            pausePanel.transform, "PauseInstructions", "Paused\n\nTake a break or continue when you are ready.",
            new Vector2(560f, 170f), new Vector2(0f, 60f), font, 28f, Color.white, TextAlignmentOptions.Center);
        AccessibleUIFactory.CreateButton(
            pausePanel.transform, "ResumeButton", "Resume",
            new Vector2(170f, 60f), new Vector2(-190f, -140f), font, teal, ClosePauseMenu);
        restartButton = AccessibleUIFactory.CreateButton(
            pausePanel.transform, "RestartActivityButton", "Restart",
            new Vector2(170f, 60f), new Vector2(0f, -140f), font, blue, RestartCurrentActivity);
        AccessibleUIFactory.CreateButton(
            pausePanel.transform, "ExitPauseMenuButton", "Exit Activity",
            new Vector2(190f, 60f), new Vector2(190f, -140f), font, blue, ExitCurrentActivity);

        completionPanel = AccessibleUIFactory.CreatePanel(
            canvas.transform, "CompletionPanel", new Vector2(680f, 430f), new Color(0.06f, 0.13f, 0.18f, 0.97f));
        completionPanel.SetActive(false);
        completionText.transform.SetParent(completionPanel.transform, false);
        completionText.gameObject.SetActive(true);
        ConfigureText(completionText, new Vector2(0f, 50f), new Vector2(590f, 230f), 32f,
            TextAlignmentOptions.Center);
        completionText.color = Color.white;

        AccessibleUIFactory.CreateButton(
            completionPanel.transform, "RestartCompletedActivityButton", "Restart",
            new Vector2(190f, 60f), new Vector2(-120f, -155f), font, teal, RestartCompletedActivity);
        AccessibleUIFactory.CreateButton(
            completionPanel.transform, "ReturnToRoomButton", "Return to Room",
            new Vector2(210f, 60f), new Vector2(120f, -155f), font, blue, CloseCompletionScreen);
    }

    private static void SetTopRight(RectTransform rect, Vector2 position, Vector2 size)
    {
        rect.anchorMin = Vector2.one;
        rect.anchorMax = Vector2.one;
        rect.pivot = Vector2.one;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    private bool StationOverlayIsOpen()
    {
        return (exerciseManager != null && exerciseManager.IsOpen) ||
            (resourceManager != null && resourceManager.IsOpen);
    }

    private void CloseStationOverlay()
    {
        if (exerciseManager != null && exerciseManager.IsOpen)
            exerciseManager.ExitActivity();
        if (resourceManager != null && resourceManager.IsOpen)
            resourceManager.CloseResources();
    }

    private void CheckForInteractable()
    {
        currentInteractable = null;
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (heldObject != null)
        {
            GameObject placementZone = GetPlacementZoneForHeldObject();
            bool canPlace = IsHeldObjectNearPlacementZone(placementZone) ||
                IsPointerAimingAtPlacementZone(placementZone, ray);

            if (placementZone != null)
                SetPlacementPreviewColor(placementZone, canPlace);

            if (canPlace)
            {
                currentInteractable = placementZone;
                interactionPrompt.text = "Carrying " + heldObject.name +
                    ". Press E to place it on the highlighted pad.";
            }
            else
            {
                interactionPrompt.text = "Carrying " + heldObject.name +
                    ". Move it near the matching pad or aim at the pad, then press E.";
            }

            ApplyHighlight(heldObject);
            ShowTemporaryFeedbackIfActive();
            return;
        }

        RaycastHit[] hits = Physics.RaycastAll(ray, interactionDistance);
        Array.Sort(hits, delegate(RaycastHit left, RaycastHit right)
        {
            return left.distance.CompareTo(right.distance);
        });

        for (int i = 0; i < hits.Length; i++)
        {
            GameObject hitObject = hits[i].collider.gameObject;
            if (placedObjects.Contains(hitObject))
                continue;

            if (hitObject.CompareTag("Target") || hitObject.CompareTag("EverydayObject") ||
                hitObject.CompareTag("ExerciseStation") || hitObject.CompareTag("ResourceStation"))
            {
                currentInteractable = hitObject;
                interactionPrompt.text = GetInteractionPrompt(hitObject);
                break;
            }
        }

        if (currentInteractable == null)
            interactionPrompt.text =
                "Look at an object and press E to pick it up. Move near its teal pad or aim at it, then press E. Tab unlocks the pointer; Esc opens controls.";

        ApplyHighlight(currentInteractable != null ? currentInteractable : heldObject);
        ShowTemporaryFeedbackIfActive();
    }

    private GameObject GetPlacementZoneForHeldObject()
    {
        if (heldObject == null)
            return null;

        if (heldObject.CompareTag("Target"))
            return reachPlacementZone;
        if (heldObject.CompareTag("EverydayObject"))
            return everydayPlacementZone;

        return null;
    }

    private bool IsHeldObjectNearPlacementZone(GameObject zone)
    {
        if (zone == null || heldObject == null)
            return false;

        Collider zoneCollider = zone.GetComponent<Collider>();
        if (zoneCollider == null)
            return false;

        Bounds forgivingBounds = zoneCollider.bounds;
        forgivingBounds.Expand(new Vector3(1.2f, 1f, 1.2f));
        return forgivingBounds.Contains(heldObject.transform.position);
    }

    private bool IsPointerAimingAtPlacementZone(GameObject zone, Ray ray)
    {
        if (zone == null)
            return false;

        Collider zoneCollider = zone.GetComponent<Collider>();
        if (zoneCollider == null)
            return false;

        Bounds forgivingBounds = zoneCollider.bounds;
        forgivingBounds.Expand(new Vector3(1.2f, 1f, 1.2f));
        return forgivingBounds.IntersectRay(ray, out float distance) && distance <= interactionDistance;
    }

    private void SetPlacementPreviewColor(GameObject zone, bool canPlace)
    {
        Renderer renderer = zone != null ? zone.GetComponent<Renderer>() : null;
        if (renderer != null)
            renderer.material.color = canPlace ? validPlacementColor : invalidPlacementColor;
    }

    private void ShowTemporaryFeedback(string message)
    {
        temporaryFeedback = message;
        temporaryFeedbackUntil = Time.unscaledTime + 1.5f;
        interactionPrompt.text = message;
    }

    private void ShowTemporaryFeedbackIfActive()
    {
        if (Time.unscaledTime < temporaryFeedbackUntil)
            interactionPrompt.text = temporaryFeedback;
    }

    private string GetInteractionPrompt(GameObject hitObject)
    {
        if (hitObject.CompareTag("Target"))
            return "Press E to pick up " + hitObject.name + " for Reach & Place.";
        if (hitObject.CompareTag("EverydayObject"))
            return "Press E to pick up " + hitObject.name + " for Everyday Activities.";
        if (hitObject.CompareTag("ExerciseStation"))
            return "Press E to open Guided Movement.";
        if (hitObject.CompareTag("ResourceStation"))
            return "Press E to open Learning & Resources.";

        return "Press E to interact.";
    }

    private void InteractWithCurrentTarget()
    {
        if (heldObject != null)
        {
            TryPlaceHeldObject();
            return;
        }

        if (currentInteractable.CompareTag("Target"))
        {
            heldObject = currentInteractable;
            currentActivity = ActivityMode.ReachAndPlace;
            return;
        }

        if (currentInteractable.CompareTag("EverydayObject"))
        {
            heldObject = currentInteractable;
            currentActivity = ActivityMode.EverydayActivities;
            return;
        }

        if (currentInteractable.CompareTag("ExerciseStation"))
        {
            currentActivity = ActivityMode.GuidedMovement;
            if (exerciseManager != null)
                exerciseManager.StartActivity();
            return;
        }

        if (currentInteractable.CompareTag("ResourceStation"))
        {
            currentActivity = ActivityMode.Resources;
            if (resourceManager != null)
                resourceManager.OpenResources();
        }
    }

    private void MoveHeldObject()
    {
        heldObject.transform.position =
            playerCamera.transform.position + playerCamera.transform.forward * holdDistance;
    }

    private void TryPlaceHeldObject()
    {
        if (heldObject == null)
            return;

        GameObject zone = GetPlacementZoneForHeldObject();
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        bool canPlace = IsHeldObjectNearPlacementZone(zone) || IsPointerAimingAtPlacementZone(zone, ray);
        if (!canPlace)
        {
            if (zone != null)
                SetPlacementPreviewColor(zone, false);
            ShowTemporaryFeedback("Move the carried object closer to its matching marked area before placing.");
            return;
        }

        if (heldObject.CompareTag("Target") && zone != null && zone.CompareTag("ReachPlaceZone"))
        {
            if (placedTargetCount >= targetObjects.Length)
                return;

            GameObject placedObject = heldObject;
            string placedName = placedObject.name;
            float[] slotOffsets = { -1.3f, -0.65f, 0f, 0.65f, 1.3f };
            if (!TryGetPlacementPosition(
                    placedObject, zone, slotOffsets[placedTargetCount], out Vector3 placementPosition))
                return;

            placedObject.transform.position = placementPosition;
            placedObjects.Add(placedObject);
            placedTargetCount++;
            heldObject = null;
            UpdatePadColor(reachPlacementZone, reachPadColor, placedTargetCount, targetObjects.Length);
            UpdateProgressText();
            ShowTemporaryFeedback("Placed " + placedName + ".");

            if (placedTargetCount >= targetObjects.Length)
                ShowCompletion(ActivityMode.ReachAndPlace,
                    "Reach & Place complete\n\nYou placed all " + targetObjects.Length + " targets in the marked area.");
            return;
        }

        if (heldObject.CompareTag("EverydayObject") && zone != null && zone.CompareTag("EverydayPlaceZone"))
        {
            if (placedEverydayCount >= everydayObjects.Length)
                return;

            GameObject placedObject = heldObject;
            string placedName = placedObject.name;
            float[] slotOffsets = { -0.9f, 0f, 0.9f };
            if (!TryGetPlacementPosition(
                    placedObject, zone, slotOffsets[placedEverydayCount], out Vector3 placementPosition))
                return;

            placedObject.transform.position = placementPosition;
            placedObjects.Add(placedObject);
            placedEverydayCount++;
            heldObject = null;
            UpdatePadColor(everydayPlacementZone, everydayPadColor, placedEverydayCount, everydayObjects.Length);
            UpdateProgressText();
            ShowTemporaryFeedback("Placed " + placedName + ".");

            if (placedEverydayCount >= everydayObjects.Length)
                ShowCompletion(ActivityMode.EverydayActivities,
                    "Everyday Activities complete\n\nYou placed all " + everydayObjects.Length + " familiar objects.");
        }
    }

    private bool TryGetPlacementPosition(
        GameObject objectToPlace,
        GameObject zone,
        float slotOffset,
        out Vector3 placementPosition)
    {
        placementPosition = Vector3.zero;
        Collider zoneCollider = zone.GetComponent<Collider>();
        Renderer objectRenderer = objectToPlace.GetComponentInChildren<Renderer>();
        if (zoneCollider == null || objectRenderer == null)
        {
            Debug.LogError("Cannot place " + objectToPlace.name +
                ": the object renderer or placement-zone collider is missing.", this);
            return false;
        }

        placementPosition = zone.transform.position + zone.transform.right * slotOffset;
        placementPosition.y = zoneCollider.bounds.max.y + objectRenderer.bounds.extents.y;
        return true;
    }

    private void ShowCompletion(ActivityMode activity, string message)
    {
        completedActivity = activity;
        completionText.text = message;
        completionPanel.SetActive(true);
        SetPlayerPaused(true);
    }

    private void OpenPauseMenu()
    {
        if (pausePanel == null || completionPanel == null || StationOverlayIsOpen())
            return;

        pausePanel.SetActive(true);
        restartButton.interactable =
            currentActivity == ActivityMode.ReachAndPlace ||
            currentActivity == ActivityMode.EverydayActivities;
        pauseText.text = "Paused\n\nTake a break or continue when you are ready.";
        SetPlayerPaused(true);
    }

    private void ClosePauseMenu()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
        SetPlayerPaused(false);
    }

    private void RestartCurrentActivity()
    {
        if (currentActivity == ActivityMode.ReachAndPlace)
            ResetReachActivity();
        else if (currentActivity == ActivityMode.EverydayActivities)
            ResetEverydayActivity();

        ClosePauseMenu();
    }

    private void ExitCurrentActivity()
    {
        if (currentActivity == ActivityMode.ReachAndPlace)
            ResetReachActivity();
        else if (currentActivity == ActivityMode.EverydayActivities)
            ResetEverydayActivity();
        else if (currentActivity == ActivityMode.GuidedMovement && exerciseManager != null)
            exerciseManager.ExitActivity();
        else if (currentActivity == ActivityMode.Resources && resourceManager != null)
            resourceManager.CloseResources();

        currentActivity = ActivityMode.None;
        ClosePauseMenu();
    }

    private void RestartCompletedActivity()
    {
        if (completedActivity == ActivityMode.ReachAndPlace)
            ResetReachActivity();
        else if (completedActivity == ActivityMode.EverydayActivities)
            ResetEverydayActivity();

        CloseCompletionScreen();
    }

    private void ResetReachActivity()
    {
        ReturnHeldObjectToStart();
        for (int i = 0; i < targetObjects.Length; i++)
        {
            targetObjects[i].transform.position = targetStartPositions[i];
            targetObjects[i].transform.rotation = targetStartRotations[i];
            placedObjects.Remove(targetObjects[i]);
        }

        placedTargetCount = 0;
        UpdatePadColor(reachPlacementZone, reachPadColor, placedTargetCount, targetObjects.Length);
        UpdateProgressText();
    }

    private void ResetEverydayActivity()
    {
        ReturnHeldObjectToStart();
        for (int i = 0; i < everydayObjects.Length; i++)
        {
            everydayObjects[i].transform.position = everydayStartPositions[i];
            everydayObjects[i].transform.rotation = everydayStartRotations[i];
            placedObjects.Remove(everydayObjects[i]);
        }

        placedEverydayCount = 0;
        UpdatePadColor(everydayPlacementZone, everydayPadColor, placedEverydayCount, everydayObjects.Length);
        UpdateProgressText();
    }

    private void ReturnHeldObjectToStart()
    {
        if (heldObject == null)
            return;

        for (int i = 0; i < targetObjects.Length; i++)
        {
            if (heldObject == targetObjects[i])
            {
                heldObject.transform.position = targetStartPositions[i];
                heldObject.transform.rotation = targetStartRotations[i];
                heldObject = null;
                return;
            }
        }

        for (int i = 0; i < everydayObjects.Length; i++)
        {
            if (heldObject == everydayObjects[i])
            {
                heldObject.transform.position = everydayStartPositions[i];
                heldObject.transform.rotation = everydayStartRotations[i];
                heldObject = null;
                return;
            }
        }
    }

    private void CloseCompletionScreen()
    {
        if (completionPanel != null)
            completionPanel.SetActive(false);
        SetPlayerPaused(false);
    }

    private void UpdateProgressText()
    {
        scoreText.text =
            "Reach & Place  " + placedTargetCount + " / " + targetObjects.Length +
            "     |     Everyday Activities  " + placedEverydayCount + " / " + everydayObjects.Length;
    }

    private static void UpdatePadColor(GameObject pad, Color baseColor, int placed, int total)
    {
        if (pad == null)
            return;

        Renderer renderer = pad.GetComponent<Renderer>();
        if (renderer == null)
            return;

        float progress = total <= 0 ? 0f : (float)placed / total;
        Color completedColor = new Color(0.05f, 0.72f, 0.55f, 1f);
        renderer.material.color = Color.Lerp(baseColor, completedColor, progress);
    }

    private void SetPlayerPaused(bool paused)
    {
        if (playerController != null)
            playerController.SetPaused(paused);
    }

    private void ApplyHighlight(GameObject target)
    {
        Renderer nextRenderer = target != null ? target.GetComponent<Renderer>() : null;
        if (nextRenderer == highlightedRenderer)
            return;

        ClearHighlight();
        if (nextRenderer == null)
            return;

        highlightedRenderer = nextRenderer;
        originalHighlightBlock = new MaterialPropertyBlock();
        highlightedRenderer.GetPropertyBlock(originalHighlightBlock);
        MaterialPropertyBlock highlightBlock = new MaterialPropertyBlock();
        highlightedRenderer.GetPropertyBlock(highlightBlock);
        highlightBlock.SetColor("_Color", new Color(0.2f, 0.95f, 0.85f, 1f));
        highlightBlock.SetColor("_BaseColor", new Color(0.2f, 0.95f, 0.85f, 1f));
        highlightedRenderer.SetPropertyBlock(highlightBlock);
    }

    private void ClearHighlight()
    {
        if (highlightedRenderer != null && originalHighlightBlock != null)
            highlightedRenderer.SetPropertyBlock(originalHighlightBlock);

        highlightedRenderer = null;
        originalHighlightBlock = null;
    }

    private static int CompareObjectNames(GameObject left, GameObject right)
    {
        return string.CompareOrdinal(left.name, right.name);
    }
}
