using UnityEngine;
using TMPro;

public class GameOverPanelLogic : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI victoryText;
    [SerializeField] TextMeshProUGUI defeatText;
    [SerializeField] TextMeshProUGUI scoreText;

    [SerializeField] string finalScorePrepend = "Final Score: ";
    [SerializeField] GliderFormatType finalScoreFormat;
    float finalScoreValue;
    string finalScoreDisplayValue;


    public void SetGameOverState(float finalScore, bool victory)
    {
        victoryText.gameObject.SetActive(victory);
        defeatText.gameObject.SetActive(!victory);
        finalScoreValue = finalScore;
        finalScoreDisplayValue = GliderFormatText.FormatNumericText(finalScoreValue, finalScoreFormat);
        scoreText.SetText(finalScorePrepend + finalScoreDisplayValue);
    }
}


public enum GliderFormatType
{
    TIME_MS,
    TIME_SECONDS,
    INT,
    DP1,
    DP2
}

public static class GliderFormatText
{
    public static string FormatNumericText(float value, GliderFormatType formatType)
    {
        return formatType switch {
            GliderFormatType.TIME_MS => FormatValueAsTime(value, true),
            GliderFormatType.TIME_SECONDS => FormatValueAsTime(value, false),
            GliderFormatType.INT => FormatNumberInt(value),
            GliderFormatType.DP1 => FormatNumber1DP(value),
            GliderFormatType.DP2 => FormatNumber2DP(value),
            _ => value.ToString()
        };
    }

    private static string FormatValueAsTime(float timeSeconds, bool includeMS=true)
    {
        int milliseconds = (int)((timeSeconds - (int)timeSeconds) * 1000);
        int seconds = (int)timeSeconds % 60;
        int minutes = (int)(timeSeconds / 60);
        int hours = (int)(timeSeconds / 3600);

        string output = "";
        string end = "";
        if (hours > 0)
        {
            output += string.Format("{0}:", GetPaddedTimeValue(hours));
            end = " Hours";
        }
        if (minutes > 0 || end.Length > 0)
        {
            output += string.Format("{0}:", GetPaddedTimeValue(minutes));
            if (end.Length == 0) end = " Minutes";
        }
        if (seconds > 0 || end.Length > 0)
        {
            output += string.Format("{0}:", GetPaddedTimeValue(seconds));
            if (end.Length == 0) end = " Seconds";
        }
        if (includeMS && (milliseconds > 0 || end.Length > 0))
        {
            output += string.Format("{0}:", GetPaddedTimeValue(milliseconds));
            if (end.Length == 0) end = " Milliseconds";
        }

        output += end;

        return output;
    }

    private static string GetPaddedTimeValue(int value)
    {
        if (value <= 0) return "00";
        if (value < 10) return "0" + value.ToString();
        return value.ToString(); 
    }

    private static string FormatNumberInt(float value) => string.Format("{0:n0}", value);
    private static string FormatNumber1DP(float value) => string.Format("{0:n1}", value);
    private static string FormatNumber2DP(float value) => string.Format("{0:n2}", value);
}
