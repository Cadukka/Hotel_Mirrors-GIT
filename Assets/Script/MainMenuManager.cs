using UnityEngine;
using UnityEngine.SceneManagement; // Biblioteca obrigatória para trocar de cenas

public class MainMenuManager : MonoBehaviour
{
    [Header("Configurações de Cena")]
    [Tooltip("Digite o nome exato da cena do jogo (ex: Level1)")]
    public string nomeDaCenaDeJogo = "Level1";

    [Header("Telas de UI")]
    [Tooltip("Arraste o Canvas ou Painel do Menu Principal aqui")]
    public GameObject telaDoMenuPrincipal; // Nova variável para o menu principal

    [Tooltip("Arraste o Canvas ou Painel de Créditos aqui")]
    public GameObject telaDeCreditos;
    [Tooltip("Arraste o Painel de Créditos de Assets aqui")]
    public GameObject telaDeCreditosAssets;

    private void Start()
    {
        // Garante que o menu principal comece ativado e a tela de créditos desativada
        if (telaDoMenuPrincipal != null)
        {
            telaDoMenuPrincipal.SetActive(true);
        }

        if (telaDeCreditos != null)
        {
            telaDeCreditos.SetActive(false);
        }
    }

    // 1. Função para o Botão PLAY (Trocar de cena)
    public void Jogar()
    {
        // Carrega a cena baseada no nome que você definiu na variável nomeDaCenaDeJogo
        SceneManager.LoadScene(nomeDaCenaDeJogo);
    }

    // 2. Função para o Botão CRÉDITOS (Desativar menu e ativar créditos)
    public void AbrirCreditos()
    {
        // Desliga o menu principal
        if (telaDoMenuPrincipal != null)
        {
            telaDoMenuPrincipal.SetActive(false);
        }

        // Liga a tela de créditos
        if (telaDeCreditos != null)
        {
            telaDeCreditos.SetActive(true);
        }
    }

    // 3. Função para o botão VOLTAR dentro dos créditos (Desativar créditos e ativar menu)
    public void FecharCreditos()
    {
        // Desliga a tela de créditos
        if (telaDeCreditos != null)
        {
            telaDeCreditos.SetActive(false);
        }

        // Liga o menu principal novamente
        if (telaDoMenuPrincipal != null)
        {
            telaDoMenuPrincipal.SetActive(true);
        }
    }
    public void AbrirCreditosAssets()
    {
        // Desliga a tela de créditos
        if (telaDeCreditos != null)
        {
            telaDeCreditos.SetActive(false);
        }

        // Desliga o menu principal
        if (telaDoMenuPrincipal != null)
        {
            telaDoMenuPrincipal.SetActive(false);
        }
        // Liga a tela de créditos Assets
        if (telaDeCreditos != null)
        {
            telaDeCreditosAssets.SetActive(true);
        }
    }
    public void FecharCreditosAssets()
    {
        // Liga a tela de créditos
        if (telaDeCreditos != null)
        {
            telaDeCreditos.SetActive(true);
        }

        // Desliga o menu principal
        if (telaDoMenuPrincipal != null)
        {
            telaDoMenuPrincipal.SetActive(false);
        }

        // Desliga a tela de créditos Assets
        if (telaDeCreditos != null)
        {
            telaDeCreditosAssets.SetActive(false);
        }
    }

    // 4. Função para o Botão SAIR (Fechar o jogo)
    public void SairDoJogo()
    {
        // O comando Application.Quit() não fecha o jogo enquanto você estiver testando dentro do Editor da Unity.
        // O Debug.Log serve para você ver no console que o botão está funcionando durante o teste.
        Debug.Log("O jogo está sendo encerrado!");
        Application.Quit();
    }
}