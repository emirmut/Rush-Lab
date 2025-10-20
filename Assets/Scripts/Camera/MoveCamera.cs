using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraTransform;

    private void Update()
    {
        transform.position = cameraTransform.position;
    }
}
