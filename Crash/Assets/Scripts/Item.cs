using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Hand / Held Item")]
    public Transform hand;
    public Transform heldItem;

    [Header("Attach Settings")]
    public Vector3 heldLocalPosition = Vector3.zero;
    public Vector3 heldLocalEulerAngles = Vector3.zero;
    public bool disablePhysicsWhileHeld = true;

    public bool SwapWithGround(Transform groundItem)
    {
        if (groundItem == null)
        {
            Debug.LogWarning("SwapWithGround: groundItem is null.", this);
            return false;
        }

        if (hand == null)
        {
            Debug.LogWarning("SwapWithGround: hand is not set.", this);
            return false;
        }

        // Drop held item to ground item position
        if (heldItem != null)
        {
            var dropPos = groundItem.position;
            var dropRot = groundItem.rotation;

            SetHeldState(heldItem, false);
            heldItem.SetParent(null, true);
            heldItem.position = dropPos;
            heldItem.rotation = dropRot;
        }

        // Pick up ground item into hand
        heldItem = groundItem;
        SetHeldState(heldItem, true);
        heldItem.SetParent(hand, true);
        heldItem.localPosition = heldLocalPosition;
        heldItem.localRotation = Quaternion.Euler(heldLocalEulerAngles);

        return true;
    }

    void SetHeldState(Transform item, bool isHeld)
    {
        if (!disablePhysicsWhileHeld) return;

        var rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = isHeld;
        }

        var col = item.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = !isHeld;
        }
    }
}
