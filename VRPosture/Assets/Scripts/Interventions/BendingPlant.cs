using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BendingPlant : MonoBehaviour
{

    public PoorPostureDetection postureDetectionMethod;
    public GameObject BendingPlants;
    public Material disintegrationMat;
    public float disintegrationDuration = 3f;

    private float disintegrationTime = 0f;
    private bool wasPoorPosture = false;
    
    void Bend()
    {
        for (int i = 0; i < BendingPlants.transform.childCount; ++i)
        {
            BendingPlants.transform.GetChild(i).gameObject.GetComponent<Animator>().SetBool("IsPoorPosture", true);
            BendingPlants.transform.GetChild(i).gameObject.GetComponent<Animator>().SetBool("ReturnNormal", false);
        }
    }

    void StraughtenUp()
    {
        for (int i = 0; i < BendingPlants.transform.childCount; ++i)
        {
            BendingPlants.transform.GetChild(i).gameObject.GetComponent<Animator>().SetBool("IsPoorPosture", false);
            BendingPlants.transform.GetChild(i).gameObject.GetComponent<Animator>().SetBool("ReturnNormal", true);
        }
    }

    IEnumerator DecreaseDisintegrationState()
    {

        while (disintegrationTime < disintegrationDuration && !wasPoorPosture)
        {
            float weight = Mathf.Lerp(1f, 0f, disintegrationTime / disintegrationDuration);
            disintegrationMat.SetFloat("_Weight", weight);
            disintegrationTime += Time.deltaTime * 0.5f;
            yield return null;
        }

        if (disintegrationTime >= disintegrationDuration)
        {
            disintegrationMat.SetFloat("_Weight", 0f);
            disintegrationTime = 0f;
            wasPoorPosture = true;
            Bend();
        }
    }

    IEnumerator IncreaseDisintegrationState()
    {

        while (disintegrationTime < disintegrationDuration && wasPoorPosture)
        {
            float weight = Mathf.Lerp(0f, 1f, disintegrationTime / disintegrationDuration);
            disintegrationMat.SetFloat("_Weight", weight);
            disintegrationTime += Time.deltaTime * 0.5f;
            yield return null;
        }

        if (disintegrationTime >= disintegrationDuration)
        {
            disintegrationMat.SetFloat("_Weight", 1f);
            disintegrationTime = 0f;
            wasPoorPosture = false;
        }
    }

    IEnumerator WaitForPlantsToStraightenUp()
    {
        yield return new WaitUntil(() => BendingPlants.transform.GetChild(BendingPlants.transform.childCount - 1).gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);
        yield return new WaitWhile(() => BendingPlants.transform.GetChild(BendingPlants.transform.childCount-1).gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);
        StartCoroutine(IncreaseDisintegrationState());
    }

    void Update()
    {
        if (postureDetectionMethod.m_isPoorPosture && postureDetectionMethod.poorPostureTime >= postureDetectionMethod.poorPostureTimeThreshold)
        {
            Bend();
            //StartCoroutine(DecreaseDisintegrationState());
            /*
            for (int i = 0; i < BendingPlants.transform.childCount; ++i)
            {
                Animator currentAnimator = BendingPlants.transform.GetChild(i).gameObject.GetComponent<Animator>();
                Debug.Log(currentAnimator.GetCurrentAnimatorStateInfo(0).IsName("Empty State"));
               
                if (currentAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f)
                {
                    currentAnimator.speed = 0;
                }
            }*/
        }
        else
        {
            //if (wasPoorPosture)
           // {
                StraughtenUp();
                //StartCoroutine(WaitForPlantsToStraightenUp());
            //}
        }

        
    }
}
