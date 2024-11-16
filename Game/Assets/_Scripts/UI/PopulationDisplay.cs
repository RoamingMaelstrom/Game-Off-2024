using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SOEvents;

public class PopulationDisplay : MonoBehaviour
{
    [SerializeField] FloatSOEvent growthEvent;
    [SerializeField] TextMeshProUGUI populationText;
    [SerializeField] Slider populationSlider;
    [SerializeField] float maxPlayerPopulation;

    void Awake() {
        growthEvent.AddListener(UpdateDisplay);
    }

    private void UpdateDisplay(float population) {
        populationText.SetText(population.ToString("n0"));
        float fill = population / maxPlayerPopulation;
        if (fill > 0.99f) fill = 1f;
        populationSlider.value = Mathf.Clamp(fill, 0f, 1f);
    }
}
