using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.XR;

public enum InterventionType
{
    Base,
    Icon,
    Grayscale
}

public class DataCollection : MonoBehaviour
{
    public PoorPostureDetection poorPostureDetection;

    string filename = "";

    public string playerName;

    public int timesPerSecond;

    public InterventionType interventionType;

    [System.Serializable]

    public class Player
    {
        public float x;
        public float y;
        public float z;
        public float pitch;
        public float yaw;
        public float roll;
        public int postureState;
        public int interventionTriggered; 
        public InterventionType intervention;
    }
    
    [SerializeField]
    public List<Player> player;
    public bool startCollectingData = false;
    public bool endCollectingData = false;
    public float timeToCollectData = 300f;

    private float timer = 0f;
    private float totalTimer = 0f;
    private bool isRecorded = false;

    private Quaternion m_centerEyeRotation;


    void Start()
    {
        filename = Application.dataPath + "/" + playerName + ".csv";
    }

    void CollectUserData()
    {
        if (startCollectingData && !endCollectingData)
        {
            timer += Time.fixedDeltaTime;
            totalTimer += Time.fixedDeltaTime;
            if (timer >= 1f / timesPerSecond)
            {
                timer = 0f;
                Player playerData = new Player();

                playerData.x = Camera.main.transform.localPosition.x;
                playerData.y = Camera.main.transform.localPosition.y;
                playerData.z = Camera.main.transform.localPosition.z;

                playerData.pitch = m_centerEyeRotation.eulerAngles.x;
                playerData.yaw = m_centerEyeRotation.eulerAngles.y;
                playerData.roll = m_centerEyeRotation.eulerAngles.z;

                if (playerData.pitch > 180f)
                {
                    playerData.pitch = playerData.pitch - 360f;
                }

                if (playerData.yaw > 180f)
                {
                    playerData.yaw = playerData.yaw - 360f;
                }

                if (playerData.roll > 180f)
                {
                    playerData.roll = playerData.roll - 360f;
                }
                
                playerData.postureState = (poorPostureDetection.m_isPoorPosture && poorPostureDetection.poorPostureTime >= poorPostureDetection.poorPostureTimeThreshold) ? 1 : 0;
                playerData.interventionTriggered = poorPostureDetection.interventionTriggered ? 1 : 0;
                playerData.intervention = interventionType;
                
                player.Add(playerData);
            }

            if (totalTimer >= timeToCollectData)
            {
                endCollectingData = true;
            }
        }
    }

    void WriteCSV()
    {
        if (player.Count > 0)
        {
            TextWriter tw;
            if (!new FileInfo(filename).Exists)
            {
                tw = new StreamWriter(filename, false);
                tw.WriteLine("x,y,z,pitch,yaw,roll,state,trigger,type");
                tw.Close();
            }
            
            tw = new StreamWriter(filename, true);

            for (int i = 0; i < player.Count; ++i)
            {
                tw.WriteLine(player[i].x + "," + player[i].y + "," + player[i].z + ","
                + player[i].pitch + "," + player[i].yaw + "," + player[i].roll + ","
                + player[i].postureState + "," + player[i].interventionTriggered + "," + player[i].intervention);
            }
            tw.Close();
        }
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

    void FixedUpdate()
    {
        bool canGetRotationAngle = TryGetCenterEyeRotation();
        CollectUserData();
        if (endCollectingData && isRecorded == false)
        {
            WriteCSV();
            isRecorded = true;
        }
    }
}
