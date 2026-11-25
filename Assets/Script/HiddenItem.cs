using UnityEngine;

public class HiddenItem : MonoBehaviour
{
    public bool startInvisible = true;

    private Renderer[] renderers;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
        SetVisible(!startInvisible);
    }

    public void SetVisible(bool visible)
    {
        foreach (Renderer r in renderers)
        {
            if (r != null)
                r.enabled = visible;
        }
    }

    // Chamado quando o jogador pega o item
    public void Reveal()
    {
        SetVisible(true);
    }
}