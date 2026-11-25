using UnityEngine;

public class GatilhoClone : MonoBehaviour
{
    public enum Acao { IrAoCentro, Desativar }

    [Header("Referências")]
    public ClonePlayerEspelhado clone;

    [Header("O que essa caixa faz")]
    public Acao acao = Acao.IrAoCentro;

    [Header("Se for IrAoCentro")]
    public Transform centroDaSala;   // centro da sala onde o clone vai ficar te encarando

    [Header("Se for Desativar")]
    public bool desligarObjeto = true; // true = some de vez | false = volta a espelhar
    public Transform novaSalaReal;     // só usado se desligarObjeto = false
    public Transform novaSalaEspelhada;

    [Header("Geral")]
    public string tagPlayer = "Player";
    public bool usarUmaVez = true;

    bool jaUsado;

    void OnTriggerEnter(Collider other)
    {
        if (jaUsado && usarUmaVez) return;
        if (!other.CompareTag(tagPlayer)) return;
        if (clone == null) return;

        jaUsado = true;

        if (acao == Acao.IrAoCentro)
        {
            clone.gameObject.SetActive(true);
            clone.PararDeSeguir(centroDaSala);
        }
        else // Desativar
        {
            if (desligarObjeto)
            {
                clone.gameObject.SetActive(false);
            }
            else
            {
                clone.TrocarSalas(novaSalaReal, novaSalaEspelhada);
                clone.VoltarAEspelhar();
            }
        }
    }
}