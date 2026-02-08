using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] float sensitivity = 120f;
    [SerializeField] Transform playerBody;
    
    // clamp limits for pitch (x) and yaw (y)
    [SerializeField] float xClampMin = -90f;
    [SerializeField] float xClampMax = 90f;


    public float xRotation = 0;
    public float yRotation = 0;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        LookAround();
    }

    void LookAround()
    {
        // Use raw input for snappier response (GetAxis is smoothed/filtered)
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;

        // clamp pitch and yaw
        xRotation = Mathf.Clamp(xRotation, xClampMin, xClampMax);

        // apply pitch to camera
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        // apply clamped yaw to player body (absolute, not incremental)
        if (playerBody != null)
        {
            playerBody.localRotation = Quaternion.Euler(0f, yRotation, 0f);
        }
        
    } 
}
