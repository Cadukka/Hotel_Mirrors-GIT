using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Transform door;
    public float openAngle = 90f;

    // Variáveis adicionadas para o som
    public AudioSource audioSource;
    public AudioClip openSound;

    private Quaternion closedRotation;
    private bool isOpen;

    public bool IsOpen => isOpen;

    private void Awake()
    {
        if (door == null)
            door = transform;

        // Tenta pegar o AudioSource automaticamente se não for atribuído no Inspector
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        closedRotation = door.rotation; // rotação no mundo
    }

    public bool TryOpenDoor()
    {
        if (isOpen)
            return true;

        isOpen = true;

        // Multiplicar pela ESQUERDA gira em torno do eixo do mundo (Vector3.up)
        door.rotation = Quaternion.AngleAxis(openAngle, Vector3.up) * closedRotation;

        // Toca o som de abrir a porta
        if (audioSource != null && openSound != null)
        {
            audioSource.PlayOneShot(openSound, 0.3f);
        }

        return true;
    }

    


}