using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // for RawImage / Text
using UnityEngine.EventSystems;

public class SignalMinigame : MinigameBase
{
    [Header("Signal Settings")]
    public SignalButton[] signalButtons;
    public float signalDuration = 0.5f;
    public float pauseBetweenSignals = 0.3f;
    public int sequenceLength = 5;
    public AudioClip buttonSound;

    [Header("UI Feedback")]
    public Text instructionText;

    [Header("Display (optional)")]
    [Tooltip("If your minigame camera outputs to a RenderTexture shown in UI, assign that RawImage here.")]
    public RawImage display; // optional; if null, we assume full-screen camera

    private List<int> sequence = new List<int>();
    private List<int> playerInput = new List<int>();
    private bool acceptingInput = false;
    private AudioSource audioSource;

    public override void Initialize(MinigamePopup popup)
    {
        base.Initialize(popup);
        this.display = popup.minigameDisplay;

        if (buttonSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        for (int i = 0; i < signalButtons.Length; i++)
        {
            int index = i;
            signalButtons[i].Initialize(index, () => OnButtonPressed(index));
        }

    }

    protected override void OnMinigameStart()
    {
        sequence.Clear();
        playerInput.Clear();
        acceptingInput = false;

        foreach (var button in signalButtons)
            button.TurnOff();

        GenerateSequence();

        if (instructionText != null)
            instructionText.text = "Watch the sequence...";

        StartCoroutine(PlaySequence());
    }

    private void GenerateSequence()
    {
        for (int i = 0; i < sequenceLength; i++)
            sequence.Add(Random.Range(0, signalButtons.Length));

        Debug.Log($"Generated sequence: {string.Join(", ", sequence)}");
    }

    private IEnumerator PlaySequence()
    {
        acceptingInput = false;
        yield return new WaitForSeconds(1f);

        foreach (int index in sequence)
        {
            yield return StartCoroutine(LightUpButton(index));
            yield return new WaitForSeconds(pauseBetweenSignals);
        }

        acceptingInput = true;

        if (instructionText != null)
            instructionText.text = "Repeat the sequence!";
    }

    private IEnumerator LightUpButton(int index)
    {
        signalButtons[index].TurnOn();

        if (audioSource != null && buttonSound != null)
            audioSource.PlayOneShot(buttonSound);

        yield return new WaitForSeconds(signalDuration);
        signalButtons[index].TurnOff();
    }

    protected override void MinigameUpdate()
    {
        if (!acceptingInput) return;

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse clicked in Signal minigame");

            if (IsMouseOverMinigameDisplay())
            {
                Debug.Log("Mouse is over minigame display");

                Ray ray = GetMouseRayInMinigameCamera();
                if (Physics.Raycast(ray, out RaycastHit hit, 100f))
                {
                    Debug.Log($"Raycast hit: {hit.collider.gameObject.name}");

                    for (int i = 0; i < signalButtons.Length; i++)
                    {
                        if (signalButtons[i].IsHit(hit.collider))
                        {
                            Debug.Log($"Button {i} was hit!");
                            OnButtonPressed(i);
                            break;
                        }
                    }
                }
                else
                {
                    Debug.Log("Raycast hit nothing");
                }
            }
            else
            {
                Debug.Log("Mouse is NOT over minigame display");
            }
        }
    }

    private void OnButtonPressed(int index)
    {
        if (!acceptingInput) return;

        Debug.Log($"Button {index} pressed");

        StartCoroutine(ButtonPressedFeedback(index));
        playerInput.Add(index);

        // Check correctness up to current input length
        if (playerInput[playerInput.Count - 1] != sequence[playerInput.Count - 1])
        {
            if (instructionText != null)
                instructionText.text = "Wrong! Try again.";

            StartCoroutine(DelayedEndMinigame(false, 1f));
            return;
        }

        // Completed sequence
        if (playerInput.Count >= sequence.Count)
        {
            if (instructionText != null)
                instructionText.text = "Success!";

            StartCoroutine(DelayedEndMinigame(true, 1f));
        }
    }

    private IEnumerator ButtonPressedFeedback(int index)
    {
        signalButtons[index].TurnOn();

        if (audioSource != null && buttonSound != null)
            audioSource.PlayOneShot(buttonSound);

        yield return new WaitForSeconds(0.2f);
        signalButtons[index].TurnOff();
    }

    private IEnumerator DelayedEndMinigame(bool success, float delay)
    {
        acceptingInput = false;
        yield return new WaitForSeconds(delay);
        EndMinigame(success);
    }

    protected override void OnMinigameEnd(bool success)
    {
        StopAllCoroutines();
        Debug.Log("Signal minigame " + (success ? "completed!" : "failed!"));
    }

    // --- Added implementations ---

    /// <summary>
    /// Returns true if the mouse is over the minigame's visual area.
    /// If a RawImage (display) is assigned, checks within that rect. Otherwise assumes full-screen camera view.
    /// </summary>
    private bool IsMouseOverMinigameDisplay()
    {
        if (display == null) return true; // full-screen / no UI gate

        // (Optional) ignore clicks through UI if the pointer is over other UI that isn't our display
        // If you want to block when over ANY UI but not our display, uncomment:
        // if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        //     return RectTransformUtility.RectangleContainsScreenPoint(display.rectTransform, Input.mousePosition, null);

        return RectTransformUtility.RectangleContainsScreenPoint(display.rectTransform, Input.mousePosition, null);
    }

    /// <summary>
    /// Builds a ray from the minigameCamera through the mouse position.
    /// If using a RawImage display (RenderTexture), converts screen coords to the camera's viewport.
    /// </summary>
    private Ray GetMouseRayInMinigameCamera()
    {
        if (minigameCamera == null)
        {
            Debug.LogWarning("minigameCamera not assigned; falling back to main camera.");
            return Camera.main != null
                ? Camera.main.ScreenPointToRay(Input.mousePosition)
                : new Ray(Vector3.zero, Vector3.forward);
        }

        if (display == null)
        {
            // Full-screen / direct camera rendering
            return minigameCamera.ScreenPointToRay(Input.mousePosition);
        }

        // Convert screen point to RawImage local point
        RectTransform rt = display.rectTransform;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, Input.mousePosition, null, out Vector2 local))
        {
            // Fallback
            return minigameCamera.ScreenPointToRay(Input.mousePosition);
        }

        // Map local point (centered rect) to normalized [0..1] UV
        Rect r = rt.rect; // local space rect, centered at (0,0)
        float u = (local.x - r.x) / r.width;   // r.x is typically -width/2
        float v = (local.y - r.y) / r.height;  // r.y is typically -height/2

        // If the cursor is outside, just build a ray anyway (caller already checked IsMouseOverMinigameDisplay)
        Vector3 viewport = new Vector3(Mathf.Clamp01(u), Mathf.Clamp01(v), 0f);
        return minigameCamera.ViewportPointToRay(viewport);
    }
}
