using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Certifique-se de que o pacote Input System está instalado
public class Walk_Away : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public List<PontoMovimento> pontos = new List<PontoMovimento>();
    public float velocidade = 5f; // Velocidade padrão
    public bool loopInfinito = true;
    public bool voltarAoInicio = false;
    
    [Header("Debug")]
    public int indiceAtual = 0;
    public bool estaMovendo = true;
    public bool estaEsperando = false;
    public bool chegouAoFinal = false;
    
    private float tempoEsperaRestante = 0f;
    private bool indoParaFrente = true;
    
    void Start()
    {
        InicializarMovimento();
    }
    
    void Update()
    {
        // Tecla F1 para reiniciar
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            ReiniciarMovimento();
        }
        
        if (pontos.Count == 0 || chegouAoFinal) return;
        
        if (estaEsperando)
        {
            tempoEsperaRestante -= Time.deltaTime;
            if (tempoEsperaRestante <= 0f)
            {
                estaEsperando = false;
                IniciarProximoMovimento();
            }
        }
        else if (estaMovendo)
        {
            MoverParaDestino();
        }
    }
    
    void InicializarMovimento()
    {
        if (pontos.Count > 0)
        {
            indiceAtual = 0;
            estaMovendo = true;
            estaEsperando = false;
            chegouAoFinal = false;
            indoParaFrente = true;
            transform.position = pontos[0].local.position;
            IniciarProximoMovimento();
        }
    }
    
    public void ReiniciarMovimento()
    {
        Debug.Log("Reiniciando movimento - F1 pressionado");
        InicializarMovimento();
    }
    
    void MoverParaDestino()
    {
        Vector3 destino = pontos[indiceAtual].local.position;
        
        // Usa a velocidade específica do ponto atual, ou a velocidade padrão se não estiver definida
        float velocidadeAtual = pontos[indiceAtual].velocidade > 0 ? pontos[indiceAtual].velocidade : velocidade;
        
        transform.position = Vector3.MoveTowards(transform.position, destino, velocidadeAtual * Time.deltaTime);
        
        // Verifica se chegou ao destino
        if (Vector3.Distance(transform.position, destino) < 0.1f)
        {
            ChegouAoDestino();
        }
    }
    
    void ChegouAoDestino()
    {
        estaMovendo = false;
        
        // Se tem tempo de espera, para no local
        if (pontos[indiceAtual].tempoEspera > 0f)
        {
            estaEsperando = true;
            tempoEsperaRestante = pontos[indiceAtual].tempoEspera;
        }
        else
        {
            IniciarProximoMovimento();
        }
    }
    
    void IniciarProximoMovimento()
    {
        // Calcula o próximo índice
        if (voltarAoInicio)
        {
            // Vai e volta (ping-pong)
            if (indoParaFrente)
            {
                indiceAtual++;
                if (indiceAtual >= pontos.Count - 1)
                {
                    indiceAtual = pontos.Count - 1;
                    indoParaFrente = false;
                }
            }
            else
            {
                indiceAtual--;
                if (indiceAtual <= 0)
                {
                    indiceAtual = 0;
                    indoParaFrente = true;
                }
            }
        }
        else
        {
            // Loop normal
            indiceAtual++;
            if (indiceAtual >= pontos.Count)
            {
                if (loopInfinito)
                {
                    indiceAtual = 0;
                }
                else
                {
                    // Para no último ponto
                    indiceAtual = pontos.Count - 1;
                    chegouAoFinal = true;
                    estaMovendo = false;
                    Debug.Log("Chegou ao final do percurso e parou");
                    return;
                }
            }
        }
        
        estaMovendo = true;
    }
    
    void OnDrawGizmosSelected()
    {
        if (pontos.Count < 2) return;
        
        // Desenha as linhas conectando os pontos
        Gizmos.color = Color.blue;
        for (int i = 0; i < pontos.Count - 1; i++)
        {
            if (pontos[i].local != null && pontos[i + 1].local != null)
            {
                Gizmos.DrawLine(pontos[i].local.position, pontos[i + 1].local.position);
            }
        }
        
        // Se for loop infinito, conecta o último ao primeiro
        if (loopInfinito && pontos[pontos.Count - 1].local != null && pontos[0].local != null)
        {
            Gizmos.DrawLine(pontos[pontos.Count - 1].local.position, pontos[0].local.position);
        }
        
        // Desenha os pontos
        for (int i = 0; i < pontos.Count; i++)
        {
            if (pontos[i].local != null)
            {
                Gizmos.color = i == indiceAtual ? Color.red : Color.green;
                Gizmos.DrawSphere(pontos[i].local.position, 0.5f);
                
                // Desenha indicador visual para pontos com tempo de espera
                if (pontos[i].tempoEspera > 0)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(pontos[i].local.position, 1f);
                }
            }
        }
    }
}