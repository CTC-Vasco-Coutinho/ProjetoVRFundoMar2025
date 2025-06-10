using UnityEngine;

[System.Serializable]
public class PontoMovimento
{
    public Transform local;
    public float tempoEspera = 0f; // Se > 0, para no local por este tempo
    public float velocidade = 0f; // Se for 0, usa a velocidade padrão do Walk_Away
}