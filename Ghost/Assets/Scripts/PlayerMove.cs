using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    
    Vector2 movement;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnMove(InputValue input)
    {
        movement = input.Get<Vector2>();
    }

    void FixedUpdate()
    {
        // Move relative to player/camera facing direction (FPS feel)
        Vector3 move =
            (transform.right * movement.x + transform.forward * movement.y) * moveSpeed;
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
    }

}
