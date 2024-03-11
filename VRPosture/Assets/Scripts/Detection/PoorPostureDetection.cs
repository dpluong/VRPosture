using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;
//using UnityEngine.InputSystem;


public class PoorPostureDetection : MonoBehaviour
{
    public InputDeviceCharacteristics controllerCharacteristics;
    public DataCollection dataCollection;
    //public InputActionReference rightController;

    private InputDevice m_targetDevice;

    private float m_height;
    private float cameraHeightWorldPosition;
    private float m_minHeight;
    private float m_neck;
    private Quaternion m_centerEyeRotation;

    private bool m_isHeightRecorded = false;
    private bool m_isMinHeightRecorded = false;

    //public float heightThreshold;
    public float upperAngleThreshold;
    public float lowerAngleThreshold;


    public float holdAndReleaseTime;

    public bool m_isPoorPosture = false;

    public bool interventionTriggered = false;
    
    [SerializeField]
    GameObject angleValue;

    [SerializeField]
    GameObject slider;

    public float poorPostureTime = 0f;

    public float poorPostureTimeThreshold = 3f;
    public float heightThreshold = 0.01f;
    public float heightThresholdLookUp = 0.05f;
    public GameObject heightCalibration;
    //public GameObject postureInstruction;
    

    void Start()
    {
        TryInitialize();
    }

    void TryInitialize()
    {
        List<InputDevice> devices = new List<InputDevice>();

        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);
        if (devices.Count > 0)
        {
            m_targetDevice = devices[0];
        }
    }

    IEnumerator HoldButtonSlider()
    {
        float time = 0f;
        slider.SetActive(true);
        while (time < holdAndReleaseTime)
        {
            slider.GetComponent<Slider>().value = Mathf.Lerp(0f, 1f, time / holdAndReleaseTime);
            time += Time.deltaTime;
            yield return null;
        }

        slider.GetComponent<Slider>().value = 1f;
        slider.SetActive(false);
    }

    public float GetHeight()
    {
        return m_height;
    }

    public float GetCenterEyeAngle()
    {
        return m_centerEyeRotation.eulerAngles.x;
    }

    void RecordHeight()
    {
        m_targetDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerValue);
        if (triggerValue)
        {
            StartCoroutine(HoldButtonSlider());
            m_height = Camera.main.transform.localPosition.y;
            m_isHeightRecorded = true;
        }
    }

    void RecordMinHeight()
    {
        if (Mathf.Round(m_centerEyeRotation.eulerAngles.x) <= 17f && Mathf.Round(m_centerEyeRotation.eulerAngles.x) >= 15f)
        {
            m_targetDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerValue);
            if (triggerValue)
            {
                StartCoroutine(HoldButtonSlider());
                m_minHeight = Camera.main.transform.localPosition.y;
                //m_neck = (m_height - m_minHeight) / (1f - Mathf.Cos(m_centerEyeRotation.eulerAngles.x * Mathf.Deg2Rad));
                m_neck = (m_height - m_minHeight) / Mathf.Abs(Mathf.Sin(m_centerEyeRotation.eulerAngles.x * Mathf.Deg2Rad));
                m_isMinHeightRecorded = true;
                angleValue.SetActive(false);
                heightCalibration.SetActive(!heightCalibration.activeSelf);
                //postureInstruction.SetActive(!postureInstruction.activeSelf);
            }
            //dataCollection.startCollectingData = true;
        }
    }

    float CalculateSafeHeight(float angle)
    {
        float angleRad = angle * Mathf.Deg2Rad;
        //float safeHeight = m_height - m_neck + m_neck * Mathf.Cos(angleRad);
        float safeHeight = m_height - m_neck * Mathf.Abs(Mathf.Sin(angleRad));
        return safeHeight;
    }

    float CalculateSafeHeightLookUp(float angle)
    {
        float angleRad = angle * Mathf.Deg2Rad;
        // float safeHeight = m_height - m_neck + m_neck * Mathf.Abs(Mathf.Cos(angleRad)) + heightThresholdLookUp;
        float safeHeight = m_height + m_neck * Mathf.Abs(Mathf.Sin(angleRad));
        return safeHeight;
    }

    void PostureDetection()
    {
        
        float currentHeight = Camera.main.transform.localPosition.y;

        if (m_centerEyeRotation.eulerAngles.x < upperAngleThreshold)
        {
            float angle = Mathf.Round(m_centerEyeRotation.eulerAngles.x);
            float safeHeight = CalculateSafeHeight(m_centerEyeRotation.eulerAngles.x);
        
            if (safeHeight - currentHeight >= heightThreshold)
            {
                m_isPoorPosture = true;
            }
            else
            {
                m_isPoorPosture = false;
            }
        }
        else
        {
            if (m_centerEyeRotation.eulerAngles.x < 180f)
                m_isPoorPosture = true;
        }

        if (m_centerEyeRotation.eulerAngles.x < 360f && m_centerEyeRotation.eulerAngles.x >= lowerAngleThreshold)
        {
            float angle = Mathf.Round(m_centerEyeRotation.eulerAngles.x);
            float safeHeight = CalculateSafeHeightLookUp(m_centerEyeRotation.eulerAngles.x);

            if (safeHeight - currentHeight >= heightThreshold)
            {
                m_isPoorPosture = true;
            }
            else
            {
                m_isPoorPosture = false;
            }
        }
        else if (m_centerEyeRotation.eulerAngles.x < lowerAngleThreshold && m_centerEyeRotation.eulerAngles.x > 180f)
        {
            m_isPoorPosture = true;
        }

        /*
        if (m_centerEyeRotation.eulerAngles.x > lowerAngleThreshold)
        {
            if (m_height > currentHeight)
            {
                m_isPoorPosture = true;
            }
            else
            {
                m_isPoorPosture = false;
            }
        }*/
    }

    void DisplayTiltAngle()
    {
        float angle = Mathf.Round(m_centerEyeRotation.eulerAngles.x);
        angleValue.GetComponent<TMPro.TextMeshProUGUI>().text = angle.ToString();
    }

    bool TryGetCenterEyeRotation()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye);
        if (device.isValid)
        {
            if (device.TryGetFeatureValue(CommonUsages.centerEyeRotation, out m_centerEyeRotation))
            {
                return true;
            }
        }
        
        m_centerEyeRotation = Quaternion.identity;
        return false;
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!TryGetCenterEyeRotation())
        {
            //Debug.Log("Device is not valid or not active");
        }

        if (!m_targetDevice.isValid)
        {
            TryInitialize();
        }
        else
        {
            if (!m_isHeightRecorded)
            {
                RecordHeight();
            }

            if (m_isHeightRecorded && !m_isMinHeightRecorded )
            {
                RecordMinHeight();
            }
            
            if (m_isHeightRecorded && m_isMinHeightRecorded)
            {
                PostureDetection();
            }
            else
            {
                DisplayTiltAngle();
            }
            
        }

        if (m_isPoorPosture)
        {
            poorPostureTime += Time.fixedDeltaTime;
        }
        else
        {
            poorPostureTime = 0f;
        }


    }
}
