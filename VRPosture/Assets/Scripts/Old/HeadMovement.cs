using UnityEngine;
using UnityEngine.XR;

public class HeadMovement : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject Screen;
    [SerializeField]
    private GameObject Head;
    [SerializeField]
    private GameObject FakeZone;
    [SerializeField]
    private GameObject SafeZone;

    public Vector3 CorrectHeadPosition;
    

    private void Start() 
    {
        CorrectHeadPosition = Head.transform.position;
    }

    private void OnTriggerExit(Collider other) 
    {
        Screen.GetComponent<Renderer>().material.color = new Color(0, 0, 0);
    }

    private void OnTriggerEnter(Collider other) 
    {
        Screen.GetComponent<Renderer>().material.color = new Color(1, 1, 1);
    }
}
