using UnityEngine;

public class Walk_Animal : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float velocidade = 2f;
    public float tempoParar = 2f;
    public float raioMovimento = 10f;
    
    private Vector3 posicaoInicial;
    private Vector3 destino;
    private bool estaAndando = false;
    private float tempoRestante = 0f;
    
    void Start()
    {
        posicaoInicial = transform.position;
        EscolherNovoDestino();
        estaAndando = true;
    }

    void Update()
    {
        if (estaAndando)
        {
            // Move em direção ao destino
            transform.position = Vector3.MoveTowards(transform.position, destino, velocidade * Time.deltaTime);
            
            // Rotaciona para olhar na direção do movimento
            Vector3 direcao = (destino - transform.position).normalized;
            if (direcao != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direcao);
            }
            
            // Verifica se chegou ao destino
            if (Vector3.Distance(transform.position, destino) < 0.1f)
            {
                estaAndando = false;
                tempoRestante = Random.Range(0f, tempoParar);
            }
        }
        else
        {
            // Está parado
            tempoRestante -= Time.deltaTime;
            if (tempoRestante <= 0f)
            {
                EscolherNovoDestino();
                estaAndando = true;
            }
        }
    }
    
    void EscolherNovoDestino()
    {
        // Gera uma posição aleatória dentro do raio de movimento
        Vector2 pontoAleatorio = Random.insideUnitCircle * raioMovimento;
        destino = posicaoInicial + new Vector3(pontoAleatorio.x, 0, pontoAleatorio.y);
    }
    
    void OnDrawGizmosSelected()
    {
        // Desenha o raio de movimento no editor
        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(posicaoInicial, raioMovimento);
            
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(destino, 0.3f);
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, raioMovimento);
        }
    }
}