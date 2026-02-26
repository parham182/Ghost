using UnityEngine;

public class Pickup : MonoBehaviour
{
    bool canPickup = false;

    public LayerMask targetLayers = -1;
    
    const string itemName = "Item";

    [Header("Swap Settings")]
    public Item item;
    
    void OnTriggerEnter(Collider other)
    {
        canPickup = true;
    }
    void OnTriggerExit(Collider other)
    {
        canPickup = false;
    }

    void Update()
    {
        if (Camera.main == null) return;

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;


        if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayers))
        {
            if (!canPickup) return;
                
            if (hit.collider.CompareTag(itemName))
            {
                PickupItem(hit);
                canPickup = true;
            } 
        }

        // رسم ray برای دیباگ
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);
    }

    void Awake()
    {
        if (item == null) item = GetComponent<Item>();
    }

    private void PickupItem(RaycastHit hit)
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;

        if (item == null)
        {
            Debug.LogWarning("Pickup: Item reference is missing.", this);
            return;
        }

        var targetItemTransform = ResolveTargetItemTransform(hit.collider);
        if (targetItemTransform == null)
        {
            Debug.LogWarning("Pickup: Could not resolve target item transform.", this);
            return;
        }

        if (item.SwapWithGround(targetItemTransform))
        {
            Debug.Log("Picked up: " + targetItemTransform.gameObject.name);
        }

    }

    Transform ResolveTargetItemTransform(Collider hitCollider)
    {
        if (hitCollider == null) return null;

        if (hitCollider.attachedRigidbody != null)
            return hitCollider.attachedRigidbody.transform;

        var taggedParent = hitCollider.transform;
        while (taggedParent != null)
        {
            if (taggedParent.CompareTag(itemName))
                return taggedParent;
            taggedParent = taggedParent.parent;
        }

        return hitCollider.transform;
    }
}
