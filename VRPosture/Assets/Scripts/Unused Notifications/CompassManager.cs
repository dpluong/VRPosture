using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassManager : MonoBehaviour
{
    public PoorPostureDetection poorPostureDetection;

    public RectTransform forwardPosture;
    public RectTransform lookUpPosture;
    public RectTransform goodPosture;

    public float speed = 1f;

    void Update() 
    {
        UpdateCompassArrow();
    }

    private void UpdateCompassArrow()
    {
        // Calculate position of the arrow based on angle
        

        Vector3 targetPosition = Vector3.zero;

        float neckAngle = Camera.main.transform.eulerAngles.x;

        if (neckAngle > 15f && neckAngle <= 180f)
        {
            neckAngle = 15f;
        }
        else if (neckAngle < 345f && neckAngle > 180f)
        {
            neckAngle = 345f;
        }

        if (neckAngle >= 0f && neckAngle <= 15f)
        {
            float ratio = neckAngle / 15f;
            targetPosition = (forwardPosture.localPosition - goodPosture.localPosition) * ratio;
            Debug.Log(targetPosition);
            Debug.Log(neckAngle);
        }
        else if (neckAngle >= 345f && neckAngle <= 360f)
        {
            float ratio = (360f - neckAngle) / 15f;
            targetPosition = (lookUpPosture.localPosition - goodPosture.localPosition) * ratio;
        }
        
        GetComponent<RectTransform>().localPosition = Vector3.MoveTowards(GetComponent<RectTransform>().localPosition, targetPosition, speed * Time.deltaTime);
    }
}
