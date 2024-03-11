using UnityEngine;
using System.IO;

public class SaveSurveyData : MonoBehaviour
{
    string filename = "";
    public string playerName;

    void Start()
    {
        filename = Application.dataPath + "/Survey/" + playerName + ".csv";
    }

    public void WriteCSV(float[] scores, string interventionType)
    {
        TextWriter tw;
        if (!new FileInfo(filename).Exists)
        {
            tw = new StreamWriter(filename, false);
            tw.WriteLine("Intuitiveness, Intrusiveness, FutureUsage, Intervention");
            tw.Close();
        }

        tw = new StreamWriter(filename, true);

        
        tw.WriteLine(scores[0] + "," + scores[1] + "," + scores[2] + "," + interventionType);

        tw.Close();
    }
}
