using TMPro;
using UnityEngine;

public class PlayerInfoPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI unlockPointsText;

    public void UpdateText(int level, int unlockPoints) {
        levelText.SetText(string.Format("Level {0}", level));
        unlockPointsText.SetText(string.Format("Unlock Points {0}", unlockPoints));
    }
}

