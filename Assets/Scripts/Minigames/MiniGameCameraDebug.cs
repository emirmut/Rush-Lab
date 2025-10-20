using UnityEngine;

public class MinigameCameraDebug : MonoBehaviour
{
    public Camera minigameCamera;
    public Transform[] objectsToView; // All objects that should be visible

    [Header("Debug Options")]
    public bool showGizmos = true;
    public bool logDetailsOnStart = true;

    void Start()
    {
        if (logDetailsOnStart && minigameCamera != null)
        {
            Debug.Log($"=== Camera Debug: {minigameCamera.name} ===");
            Debug.Log($"Camera Position: {minigameCamera.transform.position}");
            Debug.Log($"Camera Rotation: {minigameCamera.transform.eulerAngles}");
            Debug.Log($"Camera Forward: {minigameCamera.transform.forward}");
            Debug.Log($"Camera Enabled: {minigameCamera.enabled}");
            Debug.Log($"Camera Target Texture: {minigameCamera.targetTexture}");
            Debug.Log($"Camera Culling Mask: {LayerMask.LayerToName(minigameCamera.cullingMask)}");

            if (objectsToView != null && objectsToView.Length > 0)
            {
                foreach (var obj in objectsToView)
                {
                    if (obj != null)
                    {
                        float distance = Vector3.Distance(minigameCamera.transform.position, obj.position);
                        Debug.Log($"Distance to {obj.name}: {distance:F2}");
                        Debug.Log($"Object Position: {obj.position}");
                        Debug.Log($"Object Layer: {LayerMask.LayerToName(obj.gameObject.layer)}");
                    }
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (!showGizmos || minigameCamera == null) return;

        // Draw camera frustum
        Gizmos.color = Color.yellow;
        Matrix4x4 temp = Gizmos.matrix;
        Gizmos.matrix = minigameCamera.transform.localToWorldMatrix;
        Gizmos.DrawFrustum(Vector3.zero, minigameCamera.fieldOfView,
                          minigameCamera.farClipPlane,
                          minigameCamera.nearClipPlane,
                          minigameCamera.aspect);
        Gizmos.matrix = temp;

        // Draw lines to all objects
        if (objectsToView != null)
        {
            foreach (var obj in objectsToView)
            {
                if (obj != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(minigameCamera.transform.position, obj.position);
                    Gizmos.DrawWireSphere(obj.position, 0.5f);
                }
            }
        }

        // Draw camera position
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(minigameCamera.transform.position, 0.3f);

        // Draw forward direction
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(minigameCamera.transform.position, minigameCamera.transform.forward * 3f);
    }
}