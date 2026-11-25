using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerItemPickup : MonoBehaviour
{
    // Referências
    public Camera playerCamera;
    public Transform holdPoint;
    public GameObject handIcon1;
    public GameObject handIcon2;

    // Configurações
    public float pickupDistance = 3f;

    private PickupItem itemHeld;
    private PickupItem itemLookingAt;

    private PushableItem pushLookingAt;
    private PushableItem pushTarget;

    // Objetos interativos
    private ButtonAction buttonLookingAt;
    private DoorController doorLookingAt;

    // Animator está no modelo filho do Player
    private Animator animator;

    private Vector3 lastPlayerPosition;

    void Start()
    {
        if (handIcon1 != null)
            handIcon1.SetActive(false);
        if (handIcon2 != null)
            handIcon2.SetActive(false);

        // Procura o Animator no modelo filho, inclusive se estiver inativo
        animator = GetComponentInChildren<Animator>(true);

        if (animator == null)
            Debug.LogWarning("Animator não encontrado no Player ou nos filhos.");

        lastPlayerPosition = transform.position;
    }

    void Update()
    {
        if (Time.timeScale == 0f)
        {
            if (handIcon1 != null)
                handIcon1.SetActive(false);

            if (handIcon2 != null)
                handIcon2.SetActive(false);

            lastPlayerPosition = transform.position;
            StopPushVelocity();
            return;
        }

        // Mede o deslocamento do jogador
        Vector3 playerDelta = transform.position - lastPlayerPosition;
        lastPlayerPosition = transform.position;

        // Atualiza a velocidade da animação
        if (animator != null && Time.deltaTime > 0f)
        {
            if (pushTarget != null)
            {
                // Impede a animação Walk de interromper o empurrão
                animator.SetFloat("Speed", 0f);
            }
            else
            {
                Vector3 horizontalDelta = new Vector3(playerDelta.x, 0f, playerDelta.z);

                float speed = horizontalDelta.magnitude / Time.deltaTime;
                animator.SetFloat("Speed", speed);
            }
        }

        // Faz a caixa acompanhar o movimento horizontal
        if (pushTarget != null && pushTarget.rb != null)
        {
            Vector3 currentVelocity = pushTarget.rb.linearVelocity;

            if (Time.deltaTime > 0f)
            {
                currentVelocity.x = playerDelta.x / Time.deltaTime;
                currentVelocity.z = playerDelta.z / Time.deltaTime;
            }

            pushTarget.rb.linearVelocity = currentVelocity;
        }

        if (Keyboard.current == null)
            return;

        // G: soltar o item carregado
        if (Keyboard.current.gKey.wasPressedThisFrame && itemHeld != null)
        {
            DropItem();
        }

        // Limpa os objetos detectados anteriormente
        itemLookingAt = null;
        pushLookingAt = null;
        buttonLookingAt = null;
        doorLookingAt = null;

        // Raycast: detecta o objeto à frente da câmera
        if (playerCamera != null &&
            Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit,
                pickupDistance))
        {
            // Primeiro verifica se é um botão
            buttonLookingAt = hit.collider.GetComponentInParent<ButtonAction>();

            if (buttonLookingAt == null)
            {
                doorLookingAt = hit.collider.GetComponentInParent<DoorController>();

                // Ignora portas que já estão abertas (sem ícone e sem interação)
                if (doorLookingAt != null && doorLookingAt.IsOpen)
                    doorLookingAt = null;
            }

            // Depois verifica caixas e itens
            if (buttonLookingAt == null && doorLookingAt == null)
            {
                pushLookingAt = hit.collider.GetComponentInParent<PushableItem>();

                if (pushLookingAt == null)
                {
                    itemLookingAt = hit.collider.GetComponentInParent<PickupItem>();

                    if (itemLookingAt != null && !itemLookingAt.enabled)
                        itemLookingAt = null;
                }
            }
        }
        // handIcon1: interagir com portas, botões e caixas
        if (handIcon1 != null)
        {
            handIcon1.SetActive(buttonLookingAt != null || doorLookingAt != null || pushLookingAt != null);
        }
        // handIcon2: pegar itens
        if (handIcon2 != null)
        {
            handIcon2.SetActive(itemLookingAt != null);
        }

        // E: interagir
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            // Para de empurrar se já estiver empurrando
            if (pushTarget != null)
            {
                StopPushing();
            }
            else if (buttonLookingAt != null)
            {
                buttonLookingAt.Interact();
            }
            // Abre a porta usando a chave
            else if (doorLookingAt != null)
            {
                if (itemHeld != null)
                {
                    KeyAvailability key = itemHeld.GetComponent<KeyAvailability>();

                    if (key != null)
                    {
                        bool opened = doorLookingAt.TryOpenDoor();

                        if (opened)
                        {
                            // Toca a animação de abrir a porta
                            if (animator != null)
                                animator.SetTrigger("OpeningDoor");

                            // Consome a chave após abrir a porta
                            Destroy(itemHeld.gameObject);
                            itemHeld = null;
                        }
                    }
                    else
                    {
                        Debug.Log("Este item não é a chave!");
                    }
                }
                else
                {
                    Debug.Log("Você precisa estar segurando a chave!");
                }
            }
            // Começa a empurrar uma caixa
            else if (pushLookingAt != null)
            {
                if (itemHeld != null)
                    DropItem();

                StartPushing(pushLookingAt);
            }
            // Pega um item
            else if (itemLookingAt != null && itemLookingAt != itemHeld)
            {
                if (itemHeld != null)
                    DropItem();

                PickUpItem(itemLookingAt);
            }
        }
    }

    void StartPushing(PushableItem item)
    {
        if (item == null || item.rb == null)
        {
            Debug.LogWarning("A caixa não possui Rigidbody atribuído.");
            return;
        }

        pushTarget = item;

        pushTarget.rb.isKinematic = false;
        pushTarget.rb.useGravity = true;
        pushTarget.rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Toca a animação de começar a empurrar
        if (animator != null)
            animator.SetTrigger("PushEnter");
    }

    void StopPushVelocity()
    {
        if (pushTarget == null || pushTarget.rb == null)
            return;

        Vector3 velocity = pushTarget.rb.linearVelocity;
        velocity.x = 0f;
        velocity.z = 0f;

        pushTarget.rb.linearVelocity = velocity;
    }

    void StopPushing()
    {
        if (pushTarget == null)
            return;

        StopPushVelocity();

        if (pushTarget.rb != null)
        {
            pushTarget.rb.useGravity = true;
            pushTarget.rb.isKinematic = false;
        }

        pushTarget = null;

        // Toca a animação de terminar de empurrar
        if (animator != null)
            animator.SetTrigger("PushExit");
    }

    void PickUpItem(PickupItem item)
    {
        if (item == null || holdPoint == null)
            return;

        itemHeld = item;

        // A lógica original de coleta
        item.Coletar();

        itemHeld.transform.SetParent(holdPoint);
        itemHeld.transform.localPosition = Vector3.zero;
        itemHeld.transform.localRotation = Quaternion.identity;

        if (itemHeld.rb != null)
        {
            itemHeld.rb.isKinematic = true;
            itemHeld.rb.useGravity = false;
        }

        if (itemHeld.itemColliders != null)
        {
            foreach (Collider col in itemHeld.itemColliders)
            {
                if (col != null)
                    col.enabled = false;
            }
        }

        // A lógica dos itens escondidos
        HiddenItem hidden = itemHeld.GetComponent<HiddenItem>();

        if (hidden != null)
            hidden.Reveal();

        // Toca a animação de pegar o item
        if (animator != null)
            animator.SetTrigger("Picking");
    }

    void DropItem()
    {
        if (itemHeld == null)
            return;

        PickupItem itemToDrop = itemHeld;
        itemHeld = null;

        itemToDrop.transform.SetParent(null);

        if (itemToDrop.itemColliders != null)
        {
            foreach (Collider col in itemToDrop.itemColliders)
            {
                if (col != null)
                    col.enabled = true;
            }
        }

        if (itemToDrop.rb != null)
        {
            itemToDrop.rb.isKinematic = false;
            itemToDrop.rb.useGravity = true;
        }
    }

    void OnDisable()
    {
        if (pushTarget != null && pushTarget.rb != null)
        {
            StopPushVelocity();
            pushTarget.rb.useGravity = true;
            pushTarget.rb.isKinematic = false;
            pushTarget = null;
        }
    }

}