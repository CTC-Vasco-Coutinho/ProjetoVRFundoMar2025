using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FiltroSubaquatico : MonoBehaviour
{
    public Material material;
    
    [Header("Parâmetros do Filtro")]
    [Range(0f, 1f)]
    public float intensidade = 0.5f;
    
    [Range(0f, 2f)]
    public float distorcao = 0.1f;
    
    public float velocidadeOnda = 1f;
    
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (material != null && enabled)
        {
            // Atualiza parâmetros do shader
            material.SetFloat("_Intensity", intensidade);
            material.SetFloat("_Distortion", distorcao);
            material.SetFloat("_Time", Time.time * velocidadeOnda);
            
            Graphics.Blit(src, dest, material);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}