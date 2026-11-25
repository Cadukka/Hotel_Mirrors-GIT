using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class TriggerDeDialogo : MonoBehaviour
{
    [Header("Configurações do Texto")]
    [TextArea(3, 5)]
    [Tooltip("Escreva aqui o texto que vai aparecer")]
    public string mensagem;
    [Tooltip("Tempo de espera entre cada letra (ex: 0.05)")]
    public float velocidadeDigitacao = 0.05f;
    [Tooltip("Quantos segundos o texto fica na tela depois de terminar de digitar")]
    public float tempoAposTerminar = 5f;

    [Header("Referências da UI")]
    public GameObject painelDeDialogo;
    public TextMeshProUGUI textoDialogoUI;

    [Header("Configurações do Gatilho")]
    public string tagDoJogador = "Player";
    public bool mostrarApenasUmaVez = true;
    [Tooltip("Tecla para fechar o diálogo")]
    public Key teclaFechar = Key.Space;

    private bool jaFoiAtivado = false;
    private bool dialogoAberto = false;
    private Coroutine rotinaDeDialogo;

    // Guarda qual diálogo está na tela no momento (compartilhado entre todos os triggers)
    private static TriggerDeDialogo dialogoAtivo;

    private void Start()
    {
        if (painelDeDialogo != null)
            painelDeDialogo.SetActive(false);
    }

    private void Update()
    {
        if (!dialogoAberto) return;

        // Novo Input System: verifica se a tecla foi pressionada neste frame
        if (Keyboard.current != null && Keyboard.current[teclaFechar].wasPressedThisFrame)
        {
            EncerrarDialogo();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(tagDoJogador)) return;
        if (mostrarApenasUmaVez && jaFoiAtivado) return;

        // Se outro diálogo estiver na tela, encerra ele primeiro
        if (dialogoAtivo != null && dialogoAtivo != this)
        {
            dialogoAtivo.EncerrarDialogo();
        }

        // Se este mesmo diálogo já estava rodando, reinicia do zero
        if (rotinaDeDialogo != null)
        {
            StopCoroutine(rotinaDeDialogo);
        }

        jaFoiAtivado = true;
        dialogoAberto = true;
        dialogoAtivo = this;

        painelDeDialogo.SetActive(true);
        rotinaDeDialogo = StartCoroutine(RotinaDoDialogo());
    }

    private IEnumerator RotinaDoDialogo()
    {
        textoDialogoUI.text = "";

        // Efeito de máquina de escrever
        foreach (char letra in mensagem)
        {
            textoDialogoUI.text += letra;
            yield return new WaitForSeconds(velocidadeDigitacao);
        }

        // Texto completo: espera o tempo configurado e fecha
        yield return new WaitForSeconds(tempoAposTerminar);
        EncerrarDialogo();
    }

    public void EncerrarDialogo()
    {
        if (rotinaDeDialogo != null)
        {
            StopCoroutine(rotinaDeDialogo);
            rotinaDeDialogo = null;
        }

        dialogoAberto = false;

        if (dialogoAtivo == this)
            dialogoAtivo = null;

        if (textoDialogoUI != null)
            textoDialogoUI.text = "";

        if (painelDeDialogo != null)
            painelDeDialogo.SetActive(false);
    }
}