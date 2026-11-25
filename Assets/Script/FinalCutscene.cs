using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Coloque este script num objeto vazio com um BoxCollider (Is Trigger ligado)
// na última sala. O Player precisa ter a tag "Player".
public class FinalCutscene : MonoBehaviour
{
    [Header("Referências")]
    public PlayerMovement player;
    public Transform playerCamera;
    [Tooltip("Se ficar vazio, é procurado automaticamente no Player.")]
    public MouseCamera mouseCamera;
    [Tooltip("Arraste aqui o script de olhar com o mouse e o PlayerInput, para o jogador não mexer a câmera.")]
    public Behaviour[] scriptsToDisable;
    public CanvasGroup fadeGroup;      // painel preto em tela cheia, alpha 0

    [Header("Percurso")]
    public Transform[] waypoints;      // pontos por onde o player anda, o último fica na frente do espelho
    public Transform mirrorTarget;     // ponto no espelho para onde o player vai olhar
    [Range(0.1f, 1f)] public float walkSpeedFactor = 0.5f; // 0.5 = anda na metade da velocidade
    public float stopDistance = 0.15f;

    [Header("Tempos (segundos)")]
    public float turnDuration = 1.5f;  // tempo para virar até o espelho
    public float lookTime = 3f;        // tempo olhando o espelho
    public float fadeDuration = 2.5f;
    public float blackHold = 1f;       // tempo na tela preta antes do menu

    [Header("Cena")]
    public string menuSceneName = "MainMenu";

    private bool started;

    [Header("Reflexo Referencias")]
    public Transform _TransformReflexo;
    public Animator _AnimatorReflexo;

    void OnTriggerEnter(Collider other)
    {
        if (started || !other.CompareTag("Player")) return;
        started = true;
        StartCoroutine(RunCutscene());
    }

    IEnumerator RunCutscene()
    {
        // 1. Tira o controle do jogador
        player.controlEnabled = false;

        // Desativa o olhar do mouse (procura sozinho se não foi arrastado)
        if (mouseCamera == null)
            mouseCamera = player.GetComponent<MouseCamera>();
        if (mouseCamera != null)
            mouseCamera.enabled = false;

        // Desativa o PlayerInput para ignorar qualquer outro input
        var playerInput = player.GetComponent<UnityEngine.InputSystem.PlayerInput>();
        if (playerInput != null)
            playerInput.enabled = false;

        // Outros scripts que você queira desativar (ex.: PlayerItemPickup)
        foreach (var script in scriptsToDisable)
            if (script != null) script.enabled = false;

        // 2. Anda sozinho pelos waypoints
        foreach (Transform wp in waypoints)
        {
            while (true)
            {
                _AnimatorReflexo.Play("Walking");
                Vector3 toTarget = wp.position - player.transform.position;
                toTarget.y = 0f;

                if (toTarget.magnitude <= stopDistance) break;

                player.SetAutoMove(toTarget.normalized * walkSpeedFactor);
                yield return null;
            }
        }
        player.SetAutoMove(Vector3.zero);

        // 3. Vira a câmera suavemente para o espelho
        Quaternion startRot = playerCamera.rotation;
        Vector3 lookDir = mirrorTarget.position - playerCamera.position;
        Quaternion endRot = Quaternion.LookRotation(lookDir);

        //Vira o Reflexo também
        Quaternion startMirrorRot = _TransformReflexo.rotation;
        Vector3 lookDirReflexo = playerCamera.position - _TransformReflexo.position;
        Quaternion endRotReflexo = Quaternion.LookRotation(lookDirReflexo); float elapsed = 0f;

        while (elapsed < turnDuration)
        {
            elapsed += Time.deltaTime;

            float tempo = Mathf.Clamp01(elapsed / turnDuration);

            _TransformReflexo.rotation = Quaternion.Slerp(
                startMirrorRot, endRotReflexo, tempo
            );

            yield return null;
        }
        _AnimatorReflexo.Play("Mirror");

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / turnDuration;
            playerCamera.rotation = Quaternion.Slerp(startRot, endRot, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        // 4. Fica olhando o espelho
        yield return new WaitForSeconds(lookTime);

        // 5. Fade out
        float f = 0f;
        while (f < 1f)
        {
            f += Time.deltaTime / fadeDuration;
            fadeGroup.alpha = Mathf.Clamp01(f);
            yield return null;
        }

        yield return new WaitForSeconds(blackHold);

        // 6. Libera o mouse e vai para o menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(menuSceneName);
    }
}