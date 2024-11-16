using System;
using SOEvents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelDisplay : MonoBehaviour
{
    [SerializeField] Float2SOEvent gainXpEvent;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Slider xpSlider;
    // Start is called before the first frame update
    void Awake() {
        gainXpEvent.AddListener(UpdateDisplay);
    }

    private void UpdateDisplay(float level, float progress) {
        levelText.SetText(((int)level).ToString());
        if (progress < 0.01f) progress = 0f;
        xpSlider.value = Mathf.Clamp(progress, 0f, 1f);
    }
}
