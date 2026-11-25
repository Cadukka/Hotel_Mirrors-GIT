using UnityEngine;

public class SalaEspelho : MonoBehaviour
{
    public Transform salaReal;
    public Transform salaEspelhada;
    public ClonePlayerEspelhado clone;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            clone.TrocarSalas(salaReal, salaEspelhada);
    }
}