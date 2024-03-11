using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : MonoBehaviour
{
    public PoorPostureDetection poorPostureDetection;
    public Transform xrOrigin;
    public GameObject ring;
    public GameObject puzzlePiece;

    bool CheckIfPuzzleMatched()
    {
        if ((poorPostureDetection.GetCenterEyeAngle() >= 0f && poorPostureDetection.GetCenterEyeAngle() <= 4f)
            || (poorPostureDetection.GetCenterEyeAngle() >= 356f && poorPostureDetection.GetCenterEyeAngle() <= 360f))
        {
            return true;
        }
        return false;
    }

    IEnumerator TurnOffPuzzle()
    {
        yield return new WaitForSeconds(2f);
        ring.GetComponent<SpriteRenderer>().enabled = false;
        puzzlePiece.GetComponent<MeshRenderer>().enabled = false;

    }
    
    void Update()
    {
        this.transform.position = new Vector3 (Camera.main.transform.position.x, xrOrigin.position.y + poorPostureDetection.GetHeight(), Camera.main.transform.position.z);
        this.transform.rotation = Camera.main.transform.rotation;

        if (poorPostureDetection.m_isPoorPosture && poorPostureDetection.poorPostureTime >= poorPostureDetection.poorPostureTimeThreshold)
        {
            ring.GetComponent<SpriteRenderer>().enabled = true;
            ring.GetComponent<SpriteRenderer>().color = Color.red;
            puzzlePiece.GetComponent<MeshRenderer>().enabled = true;
        }
        if (!poorPostureDetection.m_isPoorPosture && CheckIfPuzzleMatched())
        {
            ring.GetComponent<SpriteRenderer>().color = Color.green;
            StartCoroutine(TurnOffPuzzle());
        }
    }
}
