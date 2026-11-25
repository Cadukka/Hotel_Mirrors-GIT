
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseCamera : MonoBehaviour
{
    public Transform cameraPivot;
    public float sensibilidade = 0.1f;

    private float rotacaoVertical = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Time.timeScale == 0f)
            return;
        if (Mouse.current == null)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        // O Olhar para os lados: gira o personagem
        transform.Rotate(Vector3.up,mouseDelta.x * sensibilidade);

        // O Olhar para cima e para baixo: gira a câmera
        rotacaoVertical -= mouseDelta.y * sensibilidade;
        rotacaoVertical = Mathf.Clamp(rotacaoVertical, -80f, 80f);

        if (cameraPivot != null)
        {
            cameraPivot.localRotation =
                Quaternion.Euler(rotacaoVertical, 0f, 0f);
        }
    }
}

