using UnityEngine;

public class ShaderStepBlock : MonoBehaviour
{
    [SerializeField] float step = 0.1f;
    bool used = false;

    void OnTriggerEnter(Collider other)   // 2D: OnTriggerEnter2D(Collider2D other)
    {
        if (used || !other.CompareTag("Player")) return;
        used = true;
        FullscreenShaderController.Instance.AddIntensity(step);
    }
}