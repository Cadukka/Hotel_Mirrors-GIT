using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public Rigidbody rb;
    public Collider[] itemColliders;

    [Header("Configurações de Áudio")]
    public AudioClip pickupSound; // O arquivo de áudio que será tocado
    [Range(0f, 1f)]
    public float soundVolume = 1f; // Controle de volume (0 a 1)

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (itemColliders == null || itemColliders.Length == 0)
            itemColliders = GetComponentsInChildren<Collider>();
    }

    // Chame este método no seu script de interação/player quando o item for pego
    public void Coletar()
    {
        Debug.Log("Item pego, tocando som!"); // Para você ter certeza que rodou
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, Camera.main.transform.position, soundVolume);
        }
    }
}