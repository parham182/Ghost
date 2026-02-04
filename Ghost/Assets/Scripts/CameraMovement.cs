using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] float sensitivity = 120f;
    [SerializeField] Transform playerBody;

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
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up * mouseX);
        }
        
    } 
}
