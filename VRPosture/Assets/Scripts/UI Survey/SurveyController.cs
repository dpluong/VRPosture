using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SurveyController : MonoBehaviour
{
    [SerializeField] GameObject surveyPanel;
   
    public GameObject RightHandRay;
    public SaveSurveyData saveData;
    public InputActionReference toggleSurvey = null;
    public Transform playerTransform;
    public float surveyDistance = 5f;
    public GameObject heightCalibration;
    public GameObject postureInstruction;

    private void Awake() 
    {
        toggleSurvey.action.started += ToggleUISurvey;
    }

    private void OnDestroy() 
    {
        toggleSurvey.action.started -= ToggleUISurvey;
    }

    void ToggleUISurvey(InputAction.CallbackContext context)
    {
        bool isActive = !surveyPanel.activeSelf;
        surveyPanel.SetActive(isActive);
        bool isRayActive = !RightHandRay.activeSelf;
        RightHandRay.SetActive(isActive);
        surveyPanel.GetComponentInChildren<Button>().interactable = true;
        // set survey position
        float x = playerTransform.position.x;
        float z = playerTransform.position.z;
        Vector3 surveyPos = new Vector3 (x, surveyPanel.transform.position.y, z);
        surveyPanel.transform.position = surveyPos + playerTransform.forward * surveyDistance;
        // set survey rotation
        Vector3 relativePos = -playerTransform.position + surveyPanel.transform.position;
        relativePos = new Vector3(relativePos.x, surveyPanel.transform.position.y, relativePos.z);
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        surveyPanel.transform.rotation = rotation;

        if (heightCalibration.activeSelf)
        {
            postureInstruction.SetActive(false);
        }
        else
        {
            postureInstruction.SetActive(!isActive);
        }
    }

    void CleanOldSurveyData()
    {
        Slider[] listOfSliders = surveyPanel.GetComponentsInChildren<Slider>();
        for (int i = 0; i < listOfSliders.Length; ++i)
        {
            listOfSliders[i].value = 0f;
        }
        surveyPanel.GetComponentInChildren<Button>().interactable = false;
        RightHandRay.SetActive(false);
        surveyPanel.SetActive(false);
    }

    public void OnClickSaveButton()
    {
        // Loop through sliders and send data over to SaveSurveyData.cs
        Slider[] listOfSliders = surveyPanel.GetComponentsInChildren<Slider>();
        int scoreArrayLength = listOfSliders.Length;
        float[] scores = new float[scoreArrayLength];
        for (int i = 0; i < scoreArrayLength; ++i)
        {
            scores[i] = listOfSliders[i].value;
        }

        Dropdown interventionType = surveyPanel.GetComponentInChildren<Dropdown>();
        saveData.WriteCSV(scores, interventionType.options[interventionType.value].text);
        CleanOldSurveyData();
        if (heightCalibration.activeSelf)
        {
            postureInstruction.SetActive(false);
        }
        else
        {
            if (!postureInstruction.activeSelf)
            {
                postureInstruction.SetActive(true);
            }
        }
        
    }

}
