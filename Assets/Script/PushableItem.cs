using UnityEngine;

public class PushableItem : MonoBehaviour
{
    public Rigidbody rb;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }
}
