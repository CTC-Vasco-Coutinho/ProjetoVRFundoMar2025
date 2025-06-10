using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class UnderwaterSystem : MonoBehaviour
{
    [Header("Configurações da Água")]
    public float nivelDaAgua = 0f;
    public Color corSubaquatica = new Color(0.1f, 0.4f, 0.6f, 1f);
    public float distanciaNeblina = 20f;
    
    [Header("Efeitos Visuais")]
    public Material materialFiltroAgua;
    public GameObject particulasSubaquaticas;
    public Light luzSubaquatica;
    
    [Header("Efeitos de Áudio")]
    public AudioSource audioSubaquatico;
    public AudioClip somBolhas;
    
    [Header("Câmera")]
    public Camera cameraPlayer;
    public Volume postProcessVolume; // Volume para efeitos subaquáticos
    public Volume postProcessVolumeNormal; // Volume para efeitos normais
    
    private bool estaSubmerso = false;
    private Vignette vignette;
    private ColorAdjustments colorAdjustments;
    
    void Start()
    {
        
        // Configura Post-Processing para volume subaquático
        if (postProcessVolume != null)
        {
            postProcessVolume.profile.TryGet(out vignette);
            postProcessVolume.profile.TryGet(out colorAdjustments);
        }
        
        // Configura volumes iniciais
        ConfigurarVolumes(false); // Inicia com volume normal ativo
        
        // Desativa efeitos inicialmente
        if (particulasSubaquaticas != null)
            particulasSubaquaticas.SetActive(false);
            
        if (audioSubaquatico != null)
            audioSubaquatico.Stop();
    }
    
    void Update()
    {
        VerificarPosicaoNaAgua();
    }
    
    void VerificarPosicaoNaAgua()
    {
        bool agora_submerso = transform.position.y < nivelDaAgua;
        
        if (agora_submerso != estaSubmerso)
        {
            estaSubmerso = agora_submerso;
            
            if (estaSubmerso)
                EntrarNaAgua();
            else
                SairDaAgua();
        }
    }
    
    void EntrarNaAgua()
    {
        // Configura fog subaquático
        RenderSettings.fog = true;
        RenderSettings.fogColor = corSubaquatica;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = 0.02f;
        RenderSettings.fogStartDistance = 1f;
        RenderSettings.fogEndDistance = distanciaNeblina;
        
        // Ativa efeitos visuais
        if (particulasSubaquaticas != null)
            particulasSubaquaticas.SetActive(true);
            
        if (luzSubaquatica != null)
        {
            luzSubaquatica.color = corSubaquatica;
            luzSubaquatica.intensity = 0.5f;
        }
        
        // Troca para volume subaquático
        ConfigurarVolumes(true);
        
        // Configura Post-Processing subaquático
        ConfigurarPostProcessingSubaquatico();
        
        // Ativa áudio subaquático
        if (audioSubaquatico != null)
        {
            audioSubaquatico.clip = somBolhas;
            audioSubaquatico.Play();
        }
        
        // Aplica filtro na câmera
        if (materialFiltroAgua != null && cameraPlayer != null)
        {
            var filtroComponent = cameraPlayer.gameObject.GetComponent<FiltroSubaquatico>();
            if (filtroComponent == null)
                filtroComponent = cameraPlayer.gameObject.AddComponent<FiltroSubaquatico>();
            
            filtroComponent.material = materialFiltroAgua;
            filtroComponent.enabled = true;
        }
    }
    
    void SairDaAgua()
    {

        RenderSettings.fog = false; 

        // Desativa efeitos visuais
        if (particulasSubaquaticas != null)
            particulasSubaquaticas.SetActive(false);
            
        if (luzSubaquatica != null)
        {
            luzSubaquatica.color = Color.white;
            luzSubaquatica.intensity = 1f;
        }
        
        // Troca para volume normal
        ConfigurarVolumes(false);
        
        // Para áudio subaquático
        if (audioSubaquatico != null)
            audioSubaquatico.Stop();
        
        // Remove filtro da câmera
        var filtroComponent = cameraPlayer.GetComponent<FiltroSubaquatico>();
        if (filtroComponent != null)
            filtroComponent.enabled = false;
    }
    
    void ConfigurarVolumes(bool subaquatico)
    {
        if (subaquatico)
        {
            // Ativa volume subaquático e desativa normal
            if (postProcessVolume != null)
                postProcessVolume.enabled = true;
                
            if (postProcessVolumeNormal != null)
                postProcessVolumeNormal.enabled = false;
        }
        else
        {
            // Ativa volume normal e desativa subaquático
            if (postProcessVolume != null)
                postProcessVolume.enabled = false;
                
            if (postProcessVolumeNormal != null)
                postProcessVolumeNormal.enabled = true;
        }
    }
    
    void ConfigurarPostProcessingSubaquatico()
    {
        if (vignette != null)
        {
            vignette.intensity.value = 0.3f;
            vignette.color.value = corSubaquatica;
        }
        
        if (colorAdjustments != null)
        {
            colorAdjustments.colorFilter.value = corSubaquatica;
            colorAdjustments.saturation.value = -20f;
            colorAdjustments.contrast.value = -10f;
        }
    }
    
    // Método público para trocar volumes manualmente
    public void TrocarParaVolumeSubaquatico()
    {
        ConfigurarVolumes(true);
        ConfigurarPostProcessingSubaquatico();
    }
    
    public void TrocarParaVolumeNormal()
    {
        ConfigurarVolumes(false);
    }
    
    // Propriedades para verificar estado atual
    public bool EstaSubmerso => estaSubmerso;
    
    // Método para forçar verificação de posição
    public void ForcarVerificacaoPosicao()
    {
        VerificarPosicaoNaAgua();
    }
}