using UnityEngine;
using TMPro;
using SOEvents;
using GliderServices;
using UnityEditor;

public class GameEndPanel : MonoBehaviour
{
    [SerializeField] LeaderboardInfo mainLeaderboard;
    [SerializeField] FloatSOEvent gameLostEvent;
    [SerializeField] TechObjectDisplaySOEvent unlockTechEvent;
    [SerializeField] ScoreCalculator scoreCalculator;
    [SerializeField] TogglePause togglePause;
    [SerializeField] BasePlayerController basePlayerController;
    [SerializeField] Difficulty difficultyObject;

    [SerializeField] GameObject content;
    [SerializeField] TextMeshProUGUI headingText;
    [SerializeField] TextMeshProUGUI outcomeText;
    [SerializeField] TextMeshProUGUI scoreBreakdownText;
    [SerializeField] TextMeshProUGUI finalScoreText;

    [SerializeField] int alienKillsScore;
    [SerializeField] int levelScore;
    [SerializeField] int populationScore;
    [SerializeField] int timeScore;
    [SerializeField] int missionScore;
    [SerializeField] int victoryUnlocksScore;
    private int totalScore;
    private bool triggered = false;
    private float difficultyModifier;
    private bool playerHasWon = false;

    private readonly TechUpgradeHandler techUpgradeHandler = TechUpgradeHandler.___VICTORY___;

    private void Awake() {
        gameLostEvent.AddListener(HandleDefeat);
        unlockTechEvent.AddListener(ProcessUnlock);
        content.SetActive(false);
    }

    private void OnApplicationQuit() {
        SetScoreValues(playerHasWon);
        TrySubmitHighscore();
    }

    private void ProcessUnlock(TechObjectDisplay tOD) {
        if (!tOD.techObject.ContainsHandler(techUpgradeHandler)) return;
        foreach (var effect in tOD.techObject.effects)
        {
            if (effect.effectType == EffectType.WIN_GAME) {
                playerHasWon = true;
                ActivateContent();
                SetVictoryText();
                TrySubmitHighscore();
                break;
            }
        }

    }

    private void ActivateContent() {
        if (triggered) return;
        triggered = true;
        basePlayerController.controlsActive = false;
        content.SetActive(true);
        togglePause.Toggle();
    }

    private void HandleDefeat(float arg0) {
        ActivateContent();
        SetDefeatText();
        TrySubmitHighscore();
    }

    private void TrySubmitHighscore() {
        AccountSystem accountSystem = FindObjectOfType<AccountSystem>();
        if (accountSystem != null) accountSystem.TrySubmitScore(mainLeaderboard.LocalID, totalScore);
        else Debug.Log("Highscore Submission currently not working...");
    }

    private void SetScoreValues(bool victory) {
        alienKillsScore = scoreCalculator.GetKillsScore();
        levelScore = scoreCalculator.GetLevelScore();
        populationScore = scoreCalculator.GetPopulationScore();
        timeScore = scoreCalculator.GetTimeScore();

        missionScore = scoreCalculator.pointsFromMissions;
        victoryUnlocksScore = scoreCalculator.pointsFromVictoryUnlocks;

        totalScore = alienKillsScore + levelScore + populationScore + timeScore + missionScore + victoryUnlocksScore;

        difficultyModifier = difficultyObject.GetDifficultyScoreModifier();
        totalScore = (int)(totalScore * difficultyModifier);
        totalScore = (totalScore / 10) * 10;

        if (victory) totalScore *= 2;
        int value = difficultyObject.selectedDifficulty switch
        {
            0 => 0,
            1 => 2,
            3 => 6,
            4 => 8,
            _ => 4,
        };
        if (victory) value ++;
        totalScore += value;
    }


    public void SetVictoryText() {
        SetScoreValues(true);

        headingText.SetText("Victory");
        outcomeText.SetText("The Earth survived the Alien onslaught!");
        scoreBreakdownText.SetText(GetScoreBreakdownText(true));
        finalScoreText.SetText(string.Format("Final Score - {0:n0}", (totalScore / 10) * 10));
    }

    public void SetDefeatText() {
        SetScoreValues(false);

        headingText.SetText("Defeat");
        outcomeText.SetText("The Earth's Population was wiped out by the Aliens");
        scoreBreakdownText.SetText(GetScoreBreakdownText(false));
        finalScoreText.SetText(string.Format("Final Score - {0:n0}", (totalScore / 10) * 10));
    }

    private string GetScoreBreakdownText(bool victory) {
        string content = "";
        content += string.Format("Score from Alien Kills: {0}\n", alienKillsScore);
        content += string.Format("Score from Level: {0}\n", levelScore);
        content += string.Format("Score from Victory Unlocks: {0}\n", victoryUnlocksScore);
        content += string.Format("Score from Completed Missions: {0}\n", missionScore);
        content += string.Format("Score from Population: {0}\n", populationScore);
        content += string.Format("Score from Time Alive: {0}\n", timeScore);
        content += string.Format("Difficulty Score Modifier: {0:n0}%", difficultyModifier * 100f);
        if (victory) content += string.Format("Victory Score Multiplier: 2x");

        return content;
    }
}
