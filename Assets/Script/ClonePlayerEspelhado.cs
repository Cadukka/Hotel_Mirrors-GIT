using UnityEngine;

public class ClonePlayerEspelhado : MonoBehaviour
{
    public Transform player;

    [Header("Salas (trocadas pelo trigger)")]
    public Transform salaReal;
    public Transform salaEspelhada;

    public enum Eixo { X, Y, Z }
    public Eixo eixoDoEspelho = Eixo.Z;

    public enum Estado { Espelhando, IndoAoCentro, Encarando }
    [Header("Estado")]
    public Estado estado = Estado.Espelhando;

    [Header("Centro / Encarar")]
    public Transform centroSala;          // objeto vazio no centro da sala espelhada
    public float velocidadeAteCentro = 3f;
    public float distanciaParaChegar = 0.1f;
    public float velocidadeGiro = 8f;
    public bool travarAlturaDoPlayer = true; // olha só no plano horizontal

    [Header("Estáticas (glitch)")]
    public float tremorPosicao = 0.03f;   // intensidade do tremor
    public float tremorRotacao = 2f;      // graus
    public float chanceDeGlitch = 0.15f;  // chance por frame de dar um "pulo" maior
    public float intensidadeGlitch = 0.15f;
    public Renderer[] renderers;          // renderers do clone (para piscar)
    public float chanceDePiscar = 0.05f;

    Vector3 posBase;

    public void TrocarSalas(Transform real, Transform espelhada)
    {
        salaReal = real;
        salaEspelhada = espelhada;
    }

    // Chame isso quando o player SAIR da sala (ex: no OnTriggerExit de um trigger)
    public void PararDeSeguir(Transform novoCentro = null)
    {
        if (novoCentro != null) centroSala = novoCentro;
        else if (centroSala == null) centroSala = salaEspelhada;

        estado = Estado.IndoAoCentro;
    }

    // Se quiser voltar a espelhar em outra sala
    public void VoltarAEspelhar()
    {
        estado = Estado.Espelhando;
        SetRenderersVisiveis(true);
    }

    Vector3 Inverter(Vector3 v)
    {
        if (eixoDoEspelho == Eixo.X) v.x = -v.x;
        else if (eixoDoEspelho == Eixo.Y) v.y = -v.y;
        else v.z = -v.z;
        return v;
    }

    void LateUpdate()
    {
        switch (estado)
        {
            case Estado.Espelhando:
                Espelhar();
                break;
            case Estado.IndoAoCentro:
                IrAoCentro();
                break;
            case Estado.Encarando:
                Encarar();
                break;
        }
    }

    void Espelhar()
    {
        if (salaReal == null || salaEspelhada == null) return;

        Vector3 posLocal = Inverter(salaReal.InverseTransformPoint(player.position));
        transform.position = salaEspelhada.TransformPoint(posLocal);

        Vector3 frente = Inverter(salaReal.InverseTransformDirection(player.forward));
        Vector3 cima = Inverter(salaReal.InverseTransformDirection(player.up));
        transform.rotation = Quaternion.LookRotation(
            salaEspelhada.TransformDirection(frente),
            salaEspelhada.TransformDirection(cima));
    }

    void IrAoCentro()
    {
        if (centroSala == null) { estado = Estado.Encarando; return; }

        Vector3 alvo = centroSala.position;
        transform.position = Vector3.MoveTowards(
            transform.position, alvo, velocidadeAteCentro * Time.deltaTime);

        OlharPara(player.position);

        if (Vector3.Distance(transform.position, alvo) <= distanciaParaChegar)
        {
            posBase = alvo;
            estado = Estado.Encarando;
        }
    }

    void Encarar()
    {
        // fica no centro, com tremor
        Vector3 offset = Random.insideUnitSphere * tremorPosicao;
        // glitch ocasional: um "pulo" maior
        if (Random.value < chanceDeGlitch)
            offset += Random.insideUnitSphere * intensidadeGlitch;

        transform.position = posBase + offset;

        OlharPara(player.position);

        // tremor de rotação
        transform.rotation *= Quaternion.Euler(
            Random.Range(-tremorRotacao, tremorRotacao),
            Random.Range(-tremorRotacao, tremorRotacao),
            Random.Range(-tremorRotacao, tremorRotacao));

        // piscar (estática visual)
        if (renderers != null && renderers.Length > 0 && Random.value < chanceDePiscar)
            StartCoroutine(Piscar());
    }

    void OlharPara(Vector3 alvo)
    {
        Vector3 dir = alvo - transform.position;
        if (travarAlturaDoPlayer) dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion rot = Quaternion.LookRotation(dir.normalized, Vector3.up);
        transform.rotation = Quaternion.Slerp(
            transform.rotation, rot, velocidadeGiro * Time.deltaTime);
    }

    bool piscando;
    System.Collections.IEnumerator Piscar()
    {
        if (piscando) yield break;
        piscando = true;

        SetRenderersVisiveis(false);
        yield return new WaitForSeconds(Random.Range(0.03f, 0.1f));
        SetRenderersVisiveis(true);

        piscando = false;
    }

    void SetRenderersVisiveis(bool visivel)
    {
        if (renderers == null) return;
        foreach (var r in renderers)
            if (r != null) r.enabled = visivel;
    }
}