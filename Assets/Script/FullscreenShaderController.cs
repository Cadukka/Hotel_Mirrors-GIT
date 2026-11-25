using UnityEngine;

public class FullscreenShaderController : MonoBehaviour
{
    public static FullscreenShaderController Instance;

    [SerializeField] Material fullscreenMaterial;      // o MESMO material usado no Full Screen Pass
    [SerializeField] string propertyName = "_Corruption"; // campo "Reference" no Shader Graph
    [SerializeField] float startValue = 0f;
    [SerializeField] float maxValue = 0.7f;            // limite de segurança
    [SerializeField] float smoothSpeed = 0.05f; // sobe ~0.05 por segundo

    float current;
    float target;

    void Awake()
    {
        Instance = this;
        current = target = startValue;
        fullscreenMaterial.SetFloat(propertyName, current); // reseta ao iniciar
    }

    void Update()
    {
        current = Mathf.MoveTowards(current, target, smoothSpeed * Time.deltaTime);
        fullscreenMaterial.SetFloat(propertyName, current);
    }

    public void AddIntensity(float amount)
    {
        // quanto mais perto do máximo, menor o efeito real do passo
        float room = (maxValue - target) / maxValue;   // 1 no início, 0 no máximo
        target = Mathf.Min(target + amount * room, maxValue);
    }
}