using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butterfly : MonoBehaviour
{
    
    public PoorPostureDetection poorPostureDetection;

    [SerializeField]
    GameObject butterFly;

    [SerializeField]
    GameObject followButterflyPosition;

    [SerializeField]
    GameObject destination;

    private Vector3 butterFlyInitialPosition;
    private Vector3 butterFlyInitialRotation;

    public float butterFlySpeed;

    private float butterFlyStartMovingTime = 0f;
    public float butterFlyEndMovingTime = 0f;

    void Start()
    {
        butterFlyInitialPosition = butterFly.transform.localPosition;
        butterFlyInitialRotation = butterFly.transform.localEulerAngles;
    }

    void Update()
    {
        EnableButterfly();
    }

    void EnableButterfly()
    {
        if (poorPostureDetection.m_isPoorPosture && poorPostureDetection.poorPostureTime >= poorPostureDetection.poorPostureTimeThreshold)
        {
            butterFly.SetActive(true);
            butterFly.transform.parent = null;

            butterFly.transform.position = Vector3.MoveTowards(butterFly.transform.position, destination.transform.position, butterFlySpeed * Time.deltaTime);
            butterFlyStartMovingTime += Time.deltaTime;
            if (butterFlyStartMovingTime >= butterFlyEndMovingTime)
            {
                butterFly.transform.parent = Camera.main.gameObject.transform;
                butterFly.transform.localPosition = butterFlyInitialPosition;
                butterFlyStartMovingTime = 0f;
            }
        }
        else
        {
            butterFly.transform.parent = Camera.main.gameObject.transform;
            butterFly.transform.localPosition = butterFlyInitialPosition;
            butterFly.transform.localEulerAngles = butterFlyInitialRotation;
            butterFly.SetActive(false);
        }
    }
}
