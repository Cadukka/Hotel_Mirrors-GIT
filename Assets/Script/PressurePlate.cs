using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public int roomId;                  // Mesmo roomId da gaveta
    public string boxTag = "Box";       // Tag da caixa

    private bool activated;

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;

        if (other.CompareTag(boxTag))
        {
            activated = true;
            GameEvents.CallDrawerOpened(roomId);
        }
    }
}