using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainPlayerInfoDisplay : MonoBehaviour
{
    [SerializeField] PlayerLevel playerLevel;
    [SerializeField] ScoreCalculator scoreCalculator;

    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Slider xpSlider;

    [SerializeField] TextMeshProUGUI scoreText;

    private int updateTimer;
    private int updateFrequency = 25;

    private void FixedUpdate() {
        UpdateLevelDisplay();
        updateTimer++;
        if (updateTimer % updateFrequency == 0) UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay() {
        float alienKillsScore = scoreCalculator.GetKillsScore();
        float levelScore = scoreCalculator.GetLevelScore();
        float populationScore = scoreCalculator.GetPopulationScore();
        float timeScore = scoreCalculator.GetTimeScore();

        float missionScore = scoreCalculator.pointsFromMissions;
        float victoryUnlocksScore = scoreCalculator.pointsFromVictoryUnlocks;

        float totalScore = alienKillsScore + levelScore + populationScore + timeScore + missionScore + victoryUnlocksScore;

        scoreText.SetText(string.Format("Score\n{0:n0}", (int)(totalScore / 10) * 10));
    }

    private void UpdateLevelDisplay()
    {
        levelText.SetText(string.Format("Level {0}", playerLevel.Level));
        xpSlider.value = playerLevel.XpPercent;
    }
}

