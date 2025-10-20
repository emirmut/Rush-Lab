using UnityEngine;

public class MinigameCameraSetup : MonoBehaviour
{
    public enum MinigameType { Cleaning, Signal, Basketball, Navigation }

    [Header("Setup")]
    public MinigameType minigameType;
    public Camera targetCamera;
    public Transform[] objectsInScene;

    [Header("Auto-Position Settings")]
    public bool autoPositionCamera = false;
    public float distanceFromObjects = 5f;

    [ContextMenu("Auto-Setup Camera")]
    void AutoSetupCamera()
    {
        if (targetCamera == null || objectsInScene == null || objectsInScene.Length == 0)
        {
            Debug.LogError("Please assign camera and objects first!");
            return;
        }

        // Calculate center of all objects
        Vector3 center = Vector3.zero;
        foreach (var obj in objectsInScene)
        {
            if (obj != null)
                center += obj.position;
        }
        center /= objectsInScene.Length;

        // Position camera based on minigame type
        switch (minigameType)
        {
            case MinigameType.Cleaning:
                // Position camera in front and slightly above
                targetCamera.transform.position = center + new Vector3(0, 1, distanceFromObjects);
                break;

            case MinigameType.Signal:
                // Position camera to see all buttons from above
                targetCamera.transform.position = center + new Vector3(0, distanceFromObjects * 0.5f, distanceFromObjects);
                break;

            case MinigameType.Basketball:
                // Position camera from the side to see throw arc
                targetCamera.transform.position = center + new Vector3(distanceFromObjects, distanceFromObjects * 0.5f, 0);
                break;

            case MinigameType.Navigation:
                // Position camera from above and to the side
                targetCamera.transform.position = center + new Vector3(distanceFromObjects * 0.5f, distanceFromObjects, distanceFromObjects * 0.5f);
                break;
        }

        // Make camera look at center
        targetCamera.transform.LookAt(center);

        Debug.Log($"Camera positioned at {targetCamera.transform.position}, looking at {center}");
    }

    [ContextMenu("Test Camera View")]
    void TestCameraView()
    {
        if (targetCamera == null)
        {
            Debug.LogError("No camera assigned!");
            return;
        }

        Debug.Log($"=== Camera Test for {minigameType} ===");
        Debug.Log($"Camera Position: {targetCamera.transform.position}");
        Debug.Log($"Camera Rotation: {targetCamera.transform.eulerAngles}");

        foreach (var obj in objectsInScene)
        {
            if (obj != null)
            {
                // Check if object is in front of camera
                Vector3 dirToObject = (obj.position - targetCamera.transform.position).normalized;
                float dot = Vector3.Dot(targetCamera.transform.forward, dirToObject);
                bool inView = dot > 0;

                Debug.Log($"{obj.name}: Distance={Vector3.Distance(targetCamera.transform.position, obj.position):F2}, InView={inView}, Dot={dot:F2}");
            }
        }
    }
}