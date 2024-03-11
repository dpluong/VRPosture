using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AngleIndicator : MonoBehaviour
{
    public PoorPostureDetection poorPostureDetection;
    public Image angleUpperHalf;
    public Image angleLowerHalf;
    public GameObject angleImage;
    public Transform headset;
    public GameObject correctPosture;
    public GameObject wrongPosture;
    public float colorThreshold = 10f;
    public float degreesPerSecond = 5f;
   // public float testAngle = 0f;

    private void Start() {
        correctPosture.SetActive(false);
    }

    void RotateHeadset()
    {
        float angle = poorPostureDetection.GetCenterEyeAngle();
        if (angle >= 0 && angle <= 180f)
        {
            if (angle > 90f)
            {
                angle = 90f;
            }

            angleLowerHalf.fillAmount = angle / 90f;
            if (angle <= poorPostureDetection.upperAngleThreshold)
            {
                angleLowerHalf.color = Color.green;
            }
            else if (angle > poorPostureDetection.upperAngleThreshold)
            {
                angleLowerHalf.color = Color.red;
            }

            Vector3 currentRotation = headset.rotation.eulerAngles;
            headset.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, -angle);
        }
        else
        {
            angle = 360f - angle;
            if (angle > 90f)
            {
                angle = 90f;
            }

            angleUpperHalf.fillAmount = angle / 90f;
            if (angle <= poorPostureDetection.upperAngleThreshold)
            {
                angleUpperHalf.color = Color.green;
            }
            else if (angle > poorPostureDetection.upperAngleThreshold)
            {
                angleUpperHalf.color = Color.red;
            }

            Vector3 currentRotation = headset.rotation.eulerAngles;
            headset.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, angle);
        }
    }

    IEnumerator TurnOffIndicator()
    {
        yield return new WaitForSeconds(2f);
        if (!poorPostureDetection.m_isPoorPosture)
            correctPosture.SetActive(false);
    }
   
    void Update()
    {
        RotateHeadset();
        if (poorPostureDetection.m_isPoorPosture && poorPostureDetection.poorPostureTime > poorPostureDetection.poorPostureTimeThreshold)
        {
            angleImage.SetActive(true);
            wrongPosture.SetActive(true);
            correctPosture.SetActive(true);
            Image correct = correctPosture.GetComponent<Image>();
            correct.color = new Color(correct.color.r, correct.color.g, correct.color.b, 0.5f);
        }
        else
        {
            angleImage.SetActive(false);
            wrongPosture.SetActive(false);
            Image correct = correctPosture.GetComponent<Image>();
            correct.color = new Color(correct.color.r, correct.color.g, correct.color.b, 1f);
            StartCoroutine(TurnOffIndicator());
        }
    }
}
