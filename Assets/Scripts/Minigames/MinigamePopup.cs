using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MinigamePopup : MonoBehaviour
{
    [Header("UI References")]
    public RawImage minigameDisplay;
    public CanvasGroup canvasGroup;
    public GameObject popupPanel;

    [Header("Settings")]
    public float fadeSpeed = 5f;

    private Camera minigameCamera;
    private RenderTexture renderTexture;

    void Start()
    {
        // Initially hide the popup
        canvasGroup.alpha = 0f;
        popupPanel.SetActive(false);
    }

    public void ShowMinigame(Camera camera, Vector2 popupSize)
    {
        Debug.Log($"ShowMinigame called with camera: {camera.name}");
        Debug.Log($"Camera enabled before: {camera.enabled}");
        Debug.Log($"Camera GameObject active: {camera.gameObject.activeInHierarchy}");

        minigameCamera = camera;

        // CRITICAL: Activate the camera GameObject
        if (!minigameCamera.gameObject.activeInHierarchy)
        {
            minigameCamera.gameObject.SetActive(true);
            Debug.Log($"Activated camera GameObject");
        }

        // Create render texture for the minigame camera
        int width = (int)popupSize.x;
        int height = (int)popupSize.y;

        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }

        renderTexture = new RenderTexture(width, height, 24);
        minigameCamera.targetTexture = renderTexture;
        minigameCamera.enabled = true; // Make sure camera component is enabled

        Debug.Log($"Camera enabled after: {minigameCamera.enabled}");
        Debug.Log($"Camera GameObject active after: {minigameCamera.gameObject.activeInHierarchy}");
        Debug.Log($"RenderTexture assigned: {minigameCamera.targetTexture != null}");
        Debug.Log($"RenderTexture dimensions: {renderTexture.width}x{renderTexture.height}");

        minigameDisplay.texture = renderTexture;
        Debug.Log($"RawImage texture assigned: {minigameDisplay.texture != null}");

        // Adjust the popup size
        RectTransform rectTransform = popupPanel.GetComponent<RectTransform>();
        rectTransform.sizeDelta = popupSize;

        // Show popup
        popupPanel.SetActive(true);
        StartCoroutine(FadeIn());
    }
    public void HideMinigame()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
    {
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }

        popupPanel.SetActive(false);

        // Clean up render texture
        if (minigameCamera != null)
        {
            minigameCamera.targetTexture = null;
            minigameCamera.enabled = false; // Disable the camera component
            minigameCamera.gameObject.SetActive(false); // Deactivate the GameObject
            Debug.Log($"Deactivated camera GameObject");
        }
        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }
    }
}