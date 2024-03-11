using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherPosture : MonoBehaviour
{    
    public List<Material> materials;

    public PoorPostureDetection postureDetectionMethod;

    public float duration;

    private float[] startWindStrength;

    private bool isWindStrengthIncreased = false;

    public AudioSource audioSource;

    public Light sunLight;

    public Material skybox;

    [Header("Weather Switch Settings")]
    public Transform pivotPoint;
    public float degreesPerSecond;
    public GameObject WeatherSwitch;

    float time;

    [Header("Bending Tree Settings")]
    public Transform XROrigin;
    public float spawnRadius;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject coconut;
    private GameObject postureTree = null;


    IEnumerator IncreaseWindStrength()
    {
    
        while (time < duration && !isWindStrengthIncreased)
        {
            for (int matIndex = 0; matIndex < materials.Count; ++matIndex)
            {
                float windStrength = Mathf.Lerp(startWindStrength[matIndex], 1f, time / duration);
                materials[matIndex].SetFloat("_WindStrength", windStrength);
            }
            
            sunLight.intensity = Mathf.Lerp(2f, 1f, time / duration);
            audioSource.volume = Mathf.Lerp(0f, 1f, time / duration);
            skybox.SetFloat("_Exposure", Mathf.Lerp(1f, 0.5f, time / duration));
            time += Time.deltaTime * 0.5f;
            yield return null;
        }
        
        if (time >= duration)
        {
            for (int matIndex = 0; matIndex < materials.Count; ++matIndex)
            {
                materials[matIndex].SetFloat("_WindStrength", 1f);
            }
            isWindStrengthIncreased = true;
            time = 0f;
        }
    }

    IEnumerator DecreaseWindStrength()
    {

        while (time < duration && isWindStrengthIncreased)
        {
            for (int matIndex = 0; matIndex < materials.Count; ++matIndex)
            {
                float windStrength = Mathf.Lerp(1f, startWindStrength[matIndex], time / duration);
                materials[matIndex].SetFloat("_WindStrength", windStrength);
            }
            sunLight.intensity = Mathf.Lerp(1f, 2f, time / duration);
            audioSource.volume = Mathf.Lerp(1f, 0f, time / duration);
            skybox.SetFloat("_Exposure", Mathf.Lerp(0.5f, 1f, time / duration));
            time += Time.deltaTime * 0.5f;

            yield return null;
        }

        if (time >= duration)
        {
            ResetWindStrength();
            isWindStrengthIncreased = false;
            time = 0f;
        }
    }

    void ResetWindStrength()
    {
        for (int matIndex = 0; matIndex < materials.Count; ++matIndex)
        {
            materials[matIndex].SetFloat("_WindStrength", startWindStrength[matIndex]);
        }
    }

    void InitWindStrengthValues()
    {
        for (int matIndex = 0; matIndex < materials.Count; ++matIndex)
        {
            startWindStrength[matIndex] = 0.1f;
            materials[matIndex].SetFloat("_WindStrength", startWindStrength[matIndex]);
        }
    }

    IEnumerator WaitForWeatherSwitchToDisappear()
    {
        yield return new WaitForSeconds(1f);
        WeatherSwitch.SetActive(false);
    }

    void SwitchWeather()
    {
        if(!postureDetectionMethod.m_isPoorPosture)
        {
            if (pivotPoint.eulerAngles.z - 360f <= -90f)
            {
                pivotPoint.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0f, 255f, 0f);
                StartCoroutine(WaitForWeatherSwitchToDisappear());
                return;
            }
            pivotPoint.Rotate(-Vector3.forward, degreesPerSecond * Time.deltaTime);
        }
        else if (postureDetectionMethod.m_isPoorPosture && postureDetectionMethod.poorPostureTime >= postureDetectionMethod.poorPostureTimeThreshold)
        {
            pivotPoint.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(255f, 0f, 0f);
            if (pivotPoint.eulerAngles.z - 360f >= -30f)
            { 
                return;
            }
            pivotPoint.Rotate(Vector3.forward, degreesPerSecond * Time.deltaTime);
        }
    }

    void Start()
    {
        startWindStrength = new float[materials.Count];
        time = 0f;
        InitWindStrengthValues();
        //SpawnPosturePlant();
    }



    void SpawnPosturePlant()
    {
        // Find position to spawn
        if (postureTree == null)
        {
            Vector3 positionToShootRay = XROrigin.position + new Vector3(0f, 2f, 0f) + XROrigin.forward * spawnRadius;
            RaycastHit hit;
            if (Physics.Raycast(positionToShootRay, Vector3.down, out hit, Mathf.Infinity, groundLayer))
            {
                //Debug.DrawRay(positionToShootRay, Vector3.down * hit.distance, Color.yellow);
                //Debug.Log("Did Hit");
                postureTree = Instantiate(coconut, hit.point, Quaternion.identity);
            }
        }

        if (postureTree != null)
        {
            postureTree.GetComponent<Animator>().SetBool("IsPoorPosture", true);
            postureTree.GetComponent<Animator>().SetBool("ReturnNormal", false);
        }
    }

    void Update()
    {
        if (postureDetectionMethod.m_isPoorPosture && postureDetectionMethod.poorPostureTime >= postureDetectionMethod.poorPostureTimeThreshold)
        {
            postureDetectionMethod.interventionTriggered = true;
            WeatherSwitch.SetActive(true);
            StartCoroutine(IncreaseWindStrength());
            //SpawnPosturePlant();
        }
        if (!postureDetectionMethod.m_isPoorPosture)
        {
            postureDetectionMethod.interventionTriggered = false;
            StartCoroutine(DecreaseWindStrength());
            //if (postureTree != null)
            //{
            //    postureTree.GetComponent<Animator>().SetBool("IsPoorPosture", false);
            //    postureTree.GetComponent<Animator>().SetBool("ReturnNormal", true);
            //}
        }
        SwitchWeather();
    }
}
