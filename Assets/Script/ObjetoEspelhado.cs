using UnityEngine;

public class ObjetoEspelhado : MonoBehaviour
{
    [Header("Objeto real")]
    public Transform objetoReal;

    [Header("Salas")]
    public Transform salaReal;
    public Transform salaEspelhada;

    public enum Eixo { X, Y, Z }
    public Eixo eixoDoEspelho = Eixo.Z;

    Vector3 Inverter(Vector3 v)
    {
        if (eixoDoEspelho == Eixo.X) v.x = -v.x;
        else if (eixoDoEspelho == Eixo.Y) v.y = -v.y;
        else v.z = -v.z;
        return v;
    }

    void LateUpdate()
    {
        if (objetoReal == null || salaReal == null || salaEspelhada == null) return;

        // Só a posição é espelhada
        Vector3 posLocal = Inverter(salaReal.InverseTransformPoint(objetoReal.position));
        transform.position = salaEspelhada.TransformPoint(posLocal);

    }
}