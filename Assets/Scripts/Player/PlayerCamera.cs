using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] public float SensitivityX;
    [SerializeField] public float SensitivityY;
    [SerializeField] private Transform Orientation;
    private float RotationX;
    private float RotationY;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * SensitivityX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * SensitivityY;

        RotationY += mouseX;
        RotationX -= mouseY;
        RotationX = Mathf.Clamp(RotationX, -90f, 90f);

        transform.rotation = Quaternion.Euler(RotationX, RotationY, 0f);
        Orientation.rotation = Quaternion.Euler(0f, RotationY, 0f);
    }
}
