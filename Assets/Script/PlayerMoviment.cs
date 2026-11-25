using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.2f;

    // --- Variáveis do Pivot de Pulo (Ground Check) ---
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private bool isGrounded;

    // Referência para o componente de áudio que tocará os passos
    public AudioSource footstepAudioSource;

    private CharacterController controller;
    private Vector2 moveInput;
    private float verticalVelocity;

    public Transform playerCamera;

    // --- NOVO: controle para cutscenes ---
    [HideInInspector] public bool controlEnabled = true;
    private Vector3 autoDirection;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    // NOVO: a cutscene chama isso para mover o player sozinho
    public void SetAutoMove(Vector3 direction)
    {
        autoDirection = Vector3.ClampMagnitude(direction, 1f);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!controlEnabled) return; // NOVO: sem pulo durante a cutscene

        if (context.performed)
        {
            if (isGrounded)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        Vector3 direction;

        if (controlEnabled)
        {
            Vector3 forward = playerCamera.forward;
            Vector3 right = playerCamera.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            direction = forward * moveInput.y + right * moveInput.x;
            direction = Vector3.ClampMagnitude(direction, 1f);
        }
        else
        {
            // NOVO: durante a cutscene, usa a direção enviada pelo script
            direction = autoDirection;
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = direction * speed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);

        // --- Lógica do Som de Passo ---
        bool isMoving = new Vector3(velocity.x, 0f, velocity.z).magnitude > 0.1f;

        if (isMoving && isGrounded)
        {
            if (!footstepAudioSource.isPlaying)
            {
                footstepAudioSource.Play();
            }
        }
        else
        {
            if (footstepAudioSource.isPlaying)
            {
                footstepAudioSource.Pause();
            }
        }
    }
}