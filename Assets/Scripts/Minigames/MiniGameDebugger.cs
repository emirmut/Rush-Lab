using UnityEngine;

public class MinigameDebugger : MonoBehaviour
{
    public CleaningMinigame cleaningMinigame;
    public SignalMinigame signalMinigame;
    public BasketballMinigame basketballMinigame;

    void Update()
    {
        // Test keys to manually trigger minigames
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Testing Cleaning Minigame");
            TestCameraView(cleaningMinigame.minigameCamera, "Cleaning");
            // cleaningMinigame.StartMinigame();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Testing Signal Minigame");
            TestCameraView(signalMinigame.minigameCamera, "Signal");
            // signalMinigame.StartMinigame();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Testing Basketball Minigame");
            TestCameraView(basketballMinigame.minigameCamera, "Basketball");
            // basketballMinigame.StartMinigame();
        }
    }

    void TestCameraView(Camera cam, string name)
    {
        if (cam == null)
        {
            Debug.LogError($"{name} camera is NULL!");
            return;
        }

        Debug.Log($"=== {name} Camera Info ===");
        Debug.Log($"Position: {cam.transform.position}");
        Debug.Log($"Rotation: {cam.transform.eulerAngles}");
        Debug.Log($"Enabled: {cam.enabled}");
        Debug.Log($"Culling Mask: {cam.cullingMask}");
    }
}