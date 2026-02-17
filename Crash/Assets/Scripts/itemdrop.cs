using UnityEngine;

public class itemdrop : MonoBehaviour
{
    [Header("Drop Input")]
    [SerializeField] private KeyCode dropKey = KeyCode.G;

    [Header("Manual Gravity")]
    [SerializeField] private float gravity = 20f;
    [SerializeField] private LayerMask groundMask = ~0;
    [SerializeField] private float groundOffset = 0.02f;

    private bool isFalling;
    private Vector3 velocity;

    private void Update()
    {
        if (!isFalling && Input.GetKeyDown(dropKey))
        {
            StartDrop();
        }

        if (isFalling)
        {
            SimulateFall();
        }
    }

    private void StartDrop()
    {
        transform.SetParent(null, true);
        velocity = Vector3.zero;
        isFalling = true;
    }

    private void SimulateFall()
    {
        float dt = Time.deltaTime;
        velocity += Vector3.down * gravity * dt;

        Vector3 displacement = velocity * dt;
        float fallDistance = -displacement.y;

        if (fallDistance > 0f && Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, fallDistance + groundOffset, groundMask, QueryTriggerInteraction.Ignore))
        {
            transform.position = hit.point + Vector3.up * groundOffset;
            velocity = Vector3.zero;
            isFalling = false;
            return;
        }

        transform.position += displacement;
    }
}
