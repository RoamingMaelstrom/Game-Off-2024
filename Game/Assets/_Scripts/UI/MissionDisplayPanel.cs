using TMPro;
using UnityEngine;
//using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MissionDisplayPanel : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI headingText;
    [SerializeField] TextMeshProUGUI objectivesText;
    [SerializeField] Slider slider;

    private Sprite cachedIconSprite;
    private string cachedHeadingText;
    private string cachedObjectivesText;
    private float cachedSliderPercent;



    public void UpdateValues(Sprite iconSprite, string heading, string objectives, float sliderPercent) {
        cachedIconSprite = iconSprite;
        cachedHeadingText = heading;
        cachedObjectivesText = objectives;
        cachedSliderPercent = sliderPercent;

        icon.sprite = cachedIconSprite;
        headingText.SetText(cachedHeadingText);
        objectivesText.SetText(cachedObjectivesText);
        slider.value = cachedSliderPercent;
    }
    public void UpdateHeadingText(string content) => headingText.SetText(content);
    public void UpdateObjectiveText(string content) => objectivesText.SetText(content);
    public void UpdateSliderValue(float value) => slider.value = Mathf.Clamp(value, 0f, 1f);
}
