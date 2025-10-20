using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider xSlider;      // 0..1
    [SerializeField] private Slider ySlider;      // 0..1
    [SerializeField] private TMP_Text xLabel;     // optional
    [SerializeField] private TMP_Text yLabel;     // optional
    [SerializeField] private bool liveUpdate = true;

    [Header("Targets")]
    [SerializeField] private PlayerCamera playerCamera;          // ← assign your PlayerCamera instance here
    [SerializeField] private GameObject optionsPanel;            // ← the Options root panel (inactive by default)
    [SerializeField] private MonoBehaviour[] gameplayToDisable;  // e.g., PlayerCamera, PlayerMovement, etc.

    [Header("Pause")]
    [SerializeField] private bool pauseTimeScale = false;

    // Slider [0..1] → raw values your PlayerCamera uses (you still multiply by deltaTime in PlayerCamera)
    private const float RawMin = 20f;
    private const float RawMax = 600f;

    private bool isOpen;

    private void OnEnable()
    {
        if (!xSlider || !ySlider) return;

        float rawX = PlayerPrefs.GetFloat("sensx_raw", 200f);
        float rawY = PlayerPrefs.GetFloat("sensy_raw", 200f);

        xSlider.SetValueWithoutNotify(RawToSlider(rawX));
        ySlider.SetValueWithoutNotify(RawToSlider(rawY));
        UpdateLabels(rawX, rawY);

        // Push to the camera immediately so the feel matches when panel opens
        Apply(rawX, rawY);

        xSlider.onValueChanged.AddListener(OnXSlider);
        ySlider.onValueChanged.AddListener(OnYSlider);
    }

    private void OnDisable()
    {
        if (xSlider) xSlider.onValueChanged.RemoveListener(OnXSlider);
        if (ySlider) ySlider.onValueChanged.RemoveListener(OnYSlider);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isOpen) CloseOptions();
            else OpenOptions();
        }
    }

    // ----- Slider handlers -----
    private void OnXSlider(float s01)
    {
        float raw = SliderToRaw(s01);
        if (liveUpdate) ApplyX(raw);
        UpdateLabels(raw, SliderToRaw(ySlider.value));
        PlayerPrefs.SetFloat("sensx_raw", raw);
        PlayerPrefs.Save();
    }

    private void OnYSlider(float s01)
    {
        float raw = SliderToRaw(s01);
        if (liveUpdate) ApplyY(raw);
        UpdateLabels(SliderToRaw(xSlider.value), raw);
        PlayerPrefs.SetFloat("sensy_raw", raw);
        PlayerPrefs.Save();
    }

    // ----- Buttons in the Options UI -----
    public void OnApplyButton()
    {
        float rawX = SliderToRaw(xSlider.value);
        float rawY = SliderToRaw(ySlider.value);
        Apply(rawX, rawY);
        PlayerPrefs.SetFloat("sensx_raw", rawX);
        PlayerPrefs.SetFloat("sensy_raw", rawY);
        PlayerPrefs.Save();
    }

    public void OnDefaultsButton()
    {
        float defX = 200f, defY = 200f;
        xSlider.SetValueWithoutNotify(RawToSlider(defX));
        ySlider.SetValueWithoutNotify(RawToSlider(defY));
        Apply(defX, defY);
        UpdateLabels(defX, defY);
        PlayerPrefs.SetFloat("sensx_raw", defX);
        PlayerPrefs.SetFloat("sensy_raw", defY);
        PlayerPrefs.Save();
    }

    // ----- Open / Close (ESC or Menu Button) -----
    public void OpenOptions()
    {
        if (optionsPanel) optionsPanel.SetActive(true);
        isOpen = true;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        SetGameplayEnabled(false);
        if (pauseTimeScale) Time.timeScale = 0f;
    }

    public void CloseOptions()
    {
        if (optionsPanel) optionsPanel.SetActive(false);
        isOpen = false;

        if (MinigameBase.IsAnyMinigameActive == false)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        SetGameplayEnabled(true);
        if (pauseTimeScale) Time.timeScale = 1f;
    }

    // Hook these to your main-menu Options / Back buttons if you use the same prefab there:
    public void OpenFromMainMenuButton() => OpenOptions();
    public void CloseFromButton() => CloseOptions();

    // ----- Helpers -----
    private static float SliderToRaw(float s01) => Mathf.Lerp(RawMin, RawMax, s01);
    private static float RawToSlider(float raw) => Mathf.InverseLerp(RawMin, RawMax, raw);

    private void Apply(float rawX, float rawY) { ApplyX(rawX); ApplyY(rawY); }
    private void ApplyX(float rawX) { if (playerCamera) playerCamera.SensitivityX = rawX; }
    private void ApplyY(float rawY) { if (playerCamera) playerCamera.SensitivityY = rawY; }

    private void UpdateLabels(float rawX, float rawY)
    {
        if (xLabel) xLabel.text = $"Mouse X: {rawX:0}";
        if (yLabel) yLabel.text = $"Mouse Y: {rawY:0}";
    }

    private void SetGameplayEnabled(bool enabled)
    {
        if (gameplayToDisable == null) return;
        foreach (var mb in gameplayToDisable)
            if (mb) mb.enabled = enabled;
    }

    public class SensitivityInitializer : MonoBehaviour
    {
        [SerializeField] private PlayerCamera playerCamera;

        private void Awake()
        {
            if (!playerCamera) playerCamera = FindFirstObjectByType<PlayerCamera>();
            if (!playerCamera) return;

            float rawX = PlayerPrefs.GetFloat("sensx_raw", 200f);
            float rawY = PlayerPrefs.GetFloat("sensy_raw", 200f);
            playerCamera.SensitivityX = rawX;
            playerCamera.SensitivityY = rawY;
        }
    }
}
