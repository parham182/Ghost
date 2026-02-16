using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] float sensitivity = 120f;
    [SerializeField] Transform playerBody;

    // Clamp limits for pitch.
    [SerializeField] float xClampMin = -90f;
    [SerializeField] float xClampMax = 90f;

    float xRotation;
    float yRotation;

    void Start()
    {
        if (playerBody == null)
        {
            playerBody = transform.parent;
        }
        
        // Initialize from current scene rotation to avoid snapping on first frame.
        xRotation = transform.localEulerAngles.x;
        if (xRotation > 180f)
        {
            xRotation -= 360f;
        }

        if (playerBody != null)
        {
            yRotation = playerBody.localEulerAngles.y;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        LookAround();
    }

    void LookAround()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            return;
        }

        // Use raw input for snappier response (GetAxis is smoothed/filtered)
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;

        // Clamp pitch.
        xRotation = Mathf.Clamp(xRotation, xClampMin, xClampMax);

        // Apply pitch to camera.
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Apply yaw to player body.
        if (playerBody != null)
        {
            playerBody.localRotation = Quaternion.Euler(0f, yRotation, 0f);
        }
    }

    void OnValidate()
    {
        if (xClampMin > xClampMax)
        {
            float tmp = xClampMin;
            xClampMin = xClampMax;
            xClampMax = tmp;
        }

        sensitivity = Mathf.Max(0f, sensitivity);
    }
}
