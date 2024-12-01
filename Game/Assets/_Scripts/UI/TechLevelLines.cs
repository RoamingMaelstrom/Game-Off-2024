using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TechLevelLines : MonoBehaviour
{
    [SerializeField] TechLevelLine[] lines;
    [SerializeField] Color levelHighEnoughColour;
    [SerializeField] Color levelTooLowColour;

    public void SetLineColours(int level) {
        foreach (var line in lines)
        {
            if (line.level > level) {
                line.line.color = levelTooLowColour;
                line.text.color = levelTooLowColour;
            }
            else {
                line.line.color = levelHighEnoughColour;
                line.text.color = levelHighEnoughColour; 
            }
        }
    }
}

[System.Serializable]
public class TechLevelLine
{
    public int level;
    public Image line;
    public TextMeshProUGUI text;
}
