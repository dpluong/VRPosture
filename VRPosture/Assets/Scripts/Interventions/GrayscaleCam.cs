using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GrayscaleCam : MonoBehaviour
{
    public PoorPostureDetection poorPostureDetection;
    public float colorStep = 0.05f;
    ColorGrading colorGradingLayer = null;
    PostProcessVolume postProcessVolume;

    private float poorPostureTimeThreshold;

    void Start() 
    {
        postProcessVolume = Camera.main.GetComponent<PostProcessVolume>();
        poorPostureTimeThreshold = poorPostureDetection.poorPostureTimeThreshold;
        postProcessVolume.profile.TryGetSettings(out colorGradingLayer);
    }

    void Update()
    {
        if (poorPostureDetection.m_isPoorPosture && poorPostureDetection.poorPostureTime >= poorPostureTimeThreshold)
        {
            poorPostureDetection.interventionTriggered = true;
            if (poorPostureDetection.poorPostureTime - poorPostureTimeThreshold >= 0.1f 
            && poorPostureDetection.poorPostureTime - poorPostureTimeThreshold <= 0.2f)
            {
                colorGradingLayer.saturation.value = -30f;
            }   
            else if (poorPostureDetection.poorPostureTime - poorPostureTimeThreshold > 1f)
            {
                if (colorGradingLayer.saturation.value > -100f)
                {
                    colorGradingLayer.saturation.value -= colorStep;
                }
            }             
        }
        else
        {
            colorGradingLayer.saturation.value = 0;
            poorPostureDetection.interventionTriggered = false;
        }
    }
}
