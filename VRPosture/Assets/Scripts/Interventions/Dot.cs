using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    public PoorPostureDetection poorPostureDetection;

    [SerializeField] private float distance = 3.0f;

    public float dotSpeed;

    private float dotStartMovingTime = 0f;
    public float dotEndMovingTime = 0f;

    public float targetScale;
    public float timeToLerp = 0.25f;
    //float scaleModifier = 1;

    public Transform crosshair;

    bool isFocusing = true;
    float degreesPerSecond = 30;


    float time = 0f;
    Vector3 startScale;

    void Start()
    {
        startScale = crosshair.localScale;
    }

    void Update()
    {
       DotMovement();
       transform.LookAt(Camera.main.transform);
    }

    void DotMovement()
    {
        if (poorPostureDetection.m_isPoorPosture && poorPostureDetection.poorPostureTime >= poorPostureDetection.poorPostureTimeThreshold)
        {
            if (gameObject.GetComponent<MeshRenderer>().enabled == false)
            {
                gameObject.GetComponent<Renderer>().material.SetColor("_Color", new Color(255, 0, 0));
                gameObject.GetComponent<MeshRenderer>().enabled = true;
                crosshair.GetComponent<SpriteRenderer>().enabled = true;
                crosshair.localScale = Vector3.one;
            }
            FocusOnDot();
            CircleAround();
            dotStartMovingTime += Time.deltaTime;
            gameObject.transform.position = FindTargetPosition() + Vector3.up * dotSpeed * dotStartMovingTime;
            
            
            if (dotStartMovingTime >= dotEndMovingTime)
            {
                gameObject.transform.position = FindTargetPosition();
                dotStartMovingTime = 0f;
            }
            poorPostureDetection.interventionTriggered = true;
        }
        if (!poorPostureDetection.m_isPoorPosture)
        {
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", new Color(0, 255, 0));
            crosshair.localScale = new Vector3(targetScale, targetScale, 0f);
            poorPostureDetection.interventionTriggered = false;
            //gameObject.transform.SetParent(Camera.main.transform);
            //gameObject.transform.position = FindTargetPosition();
            StartCoroutine(WaitBeforeDisableDot());            
        }
    }

    private Vector3 FindTargetPosition()
    {
        return Camera.main.transform.position + (Camera.main.transform.forward * distance);
    }

    void CircleAround()
    {
        crosshair.Rotate(new Vector3(0, 0, degreesPerSecond) * Time.deltaTime);
    }

    IEnumerator LerpScale(float endValue, float duration)
    {
        while (time < duration && isFocusing)
        {
            crosshair.localScale = startScale * Mathf.Lerp(1f, endValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        
        if (time >= duration)
        {
            crosshair.localScale = startScale * endValue;
            time = 0f;
            startScale = crosshair.localScale;
            isFocusing = false;
        }
    }

    IEnumerator LerpScale2(float endValue, float duration)
    {
        while (time < duration && !isFocusing)
        {
            crosshair.localScale = startScale * Mathf.Lerp(1f, endValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        if (time >= duration)
        {
            crosshair.localScale = startScale * endValue;
            time = 0f;
            startScale = crosshair.localScale;
            isFocusing = true;
        }
    }

    private void FocusOnDot()
    {
        if (isFocusing)
        {
            StartCoroutine(LerpScale(targetScale, timeToLerp));
        }
        else
        {
            StartCoroutine(LerpScale2(2f, timeToLerp));
        }
    }

    IEnumerator WaitBeforeDisableDot()
    {
        yield return new WaitForSeconds(1f);
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        crosshair.GetComponent<SpriteRenderer>().enabled = false;
        //gameObject.transform.parent = null;
    }
}
