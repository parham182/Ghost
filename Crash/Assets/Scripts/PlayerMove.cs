using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMove : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] AudioSource walksound;
    [SerializeField] AudioClip[] walkClips;
    [SerializeField] Vector2 walkVolumeRange = new Vector2(0.6f, 1f);
    [SerializeField] float walkVolumeChangeInterval = 0.25f;
    public bool IsMoving { get; private set; }

    Vector2 movement;
    Rigidbody rb;
    bool wasMoving;
    float nextWalkVolumeTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (walksound != null)
        {
            walksound.loop = true;
        }
    }

    void OnMove(InputValue input)
    {
        movement = input.Get<Vector2>();
    }

    void FixedUpdate()
    {
        IsMoving = movement.sqrMagnitude > 0.001f;
        if (walksound != null)
        {
            if (IsMoving && !wasMoving)
            {
                AudioClip clipToPlay = null;
                if (walkClips != null && walkClips.Length > 0)
                {
                    int index = Random.Range(0, walkClips.Length);
                    clipToPlay = walkClips[index];
                }

                if (clipToPlay != null)
                {
                    walksound.clip = clipToPlay;
                    SetRandomWalkVolume();
                    nextWalkVolumeTime = Time.time + walkVolumeChangeInterval;
                    walksound.Play();
                }
            }
            else if (!IsMoving && wasMoving)
                walksound.Stop();
        }
        if (IsMoving && walksound != null && walksound.isPlaying && Time.time >= nextWalkVolumeTime)
        {
            SetRandomWalkVolume();
            nextWalkVolumeTime = Time.time + walkVolumeChangeInterval;
        }
        wasMoving = IsMoving;
        // Move relative to player/camera facing direction (FPS feel)
        // Normalize to horizontal plane to avoid adding vertical velocity
        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = transform.right;
        right.y = 0f;
        right.Normalize();

        Vector3 move = (right * movement.x + forward * movement.y) * moveSpeed;
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
    }

    void SetRandomWalkVolume()
    {
        float min = Mathf.Clamp01(walkVolumeRange.x);
        float max = Mathf.Clamp01(walkVolumeRange.y);
        if (max < min)
        {
            float temp = min;
            min = max;
            max = temp;
        }
        walksound.volume = Random.Range(min, max);
    }

}
