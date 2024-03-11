using UnityEngine;

public class PostureCorrectionAnimation : MonoBehaviour
{
    public PoorPostureDetection poorPostureDetection;

    void Update()
    {
        DisplayAnimation();
    }

    private void DisplayAnimation()
    {
        if (poorPostureDetection.m_isPoorPosture && poorPostureDetection.poorPostureTime >= poorPostureDetection.poorPostureTimeThreshold)
        {
            this.GetComponent<Animator>().SetBool("IsPoorPosture", true);
        }
        else
        {
            this.GetComponent<Animator>().SetBool("IsPoorPosture", false);
        }
    }
}
