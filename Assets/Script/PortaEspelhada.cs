using UnityEngine;

public class PortaEspelhada : MonoBehaviour
{
    public Transform portaReal;          // a porta da sala real
    public bool inverterAngulo = true;   // se abrir pro lado errado, desmarque/marque

    Quaternion rotInicialReal;
    Quaternion rotInicialEspelhada;

    void Start()
    {
        rotInicialReal = portaReal.localRotation;
        rotInicialEspelhada = transform.localRotation;
    }

    void LateUpdate()
    {
        // quanto a porta real girou em relação à posição fechada
        Quaternion delta = portaReal.localRotation * Quaternion.Inverse(rotInicialReal);

        // converte para ângulo/eixo e inverte o sentido se necessário
        delta.ToAngleAxis(out float angulo, out Vector3 eixo);
        if (inverterAngulo) angulo = -angulo;

        transform.localRotation = Quaternion.AngleAxis(angulo, eixo) * rotInicialEspelhada;
    }
}