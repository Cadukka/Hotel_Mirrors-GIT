using UnityEngine;

public class DrawerController : MonoBehaviour
{
    public int roomId;
    public Transform drawer;
    public GameObject keyObject;
    public Vector3 openOffset = new Vector3(0f, 0f, -0.4f);

    // Variáveis adicionadas para o som
    public AudioSource audioSource;
    public AudioClip openSound;

    private Vector3 closedPosition;
    private bool isOpen;

    private void Awake()
    {
        if (drawer == null) drawer = transform;
        closedPosition = drawer.localPosition;

        if (keyObject != null) keyObject.SetActive(false);

        // Tenta pegar o AudioSource automaticamente se não for atribuído no Inspector
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable() { GameEvents.DrawerOpened += OpenDrawer; }
    private void OnDisable() { GameEvents.DrawerOpened -= OpenDrawer; }

    private void OpenDrawer(int id)
    {
        if (id != roomId || isOpen) return;
        isOpen = true;

        drawer.localPosition = closedPosition + openOffset;

        // Toca o som apenas no momento em que a gaveta abre
        if (audioSource != null && openSound != null)
        {
            audioSource.PlayOneShot(openSound);
        }

        if (keyObject != null) keyObject.SetActive(true);
    }
}