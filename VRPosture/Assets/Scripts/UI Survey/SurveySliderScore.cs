using UnityEngine;
using UnityEngine.UI;

public class SurveySliderScore : MonoBehaviour
{
    [SerializeField] GameObject slider;
    [SerializeField] GameObject score;

    void Update()
    {
        float currentScore = slider.GetComponent<Slider>().value;
        score.GetComponent<TMPro.TextMeshProUGUI>().text = currentScore.ToString();
    }
}
