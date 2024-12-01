using TMPro;
using UnityEngine;

public class SimpleTimer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    private float timer;
    private int minutes;
    private int seconds;
    private int cachedSeconds;

    private void FixedUpdate() {
        timer += Time.fixedDeltaTime;
        seconds = (int)timer % 60;
        if (cachedSeconds != seconds) {
            cachedSeconds = seconds;
            minutes = (int)timer / 60;
            if (seconds < 10) timerText.SetText(string.Format("{0}:0{1}", minutes, seconds));
            else timerText.SetText(string.Format("{0}:{1}", minutes, seconds));
        }
    }
}
