using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class DrawingController : MonoBehaviour
{
    public InputDeviceCharacteristics controllerCharacteristics;
    public Transform photoAttachedPoint;

    private InputDevice m_targetDevice;

    
    
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

    void ShowDrawing()
    {
        m_targetDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool gripValue);
        if(gripValue)
        {
            HoldPhoto();
        }
        else
        {
            transform.position = photoAttachedPoint.position;
            ReleasePhoto();
        }
    }

    void Update()
    {
        if (!m_targetDevice.isValid)
        {
            TryInitialize();
        }
        else
            ShowDrawing();
    }
    
    public void HoldPhoto()
    {
        transform.position = photoAttachedPoint.position;
        gameObject.GetComponentInChildren<SpriteRenderer>().enabled = true;
    }

    public void ReleasePhoto()
    {
        gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
    }
}
