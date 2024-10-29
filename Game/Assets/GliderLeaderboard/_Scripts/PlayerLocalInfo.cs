using UnityEngine;

namespace GliderServices
{
    public static class PlayerLocalInfo
    {
        public static bool IsSetup() => PlayerPrefs.GetInt("PREFS_SETUP", 0) == 1;

        public static void SetupPlayerPrefs(LeaderboardInfo[] leaderboardInfoObjects, int startScoreValue = 0, bool highscoreMode = true)
        {
            PlayerPrefs.SetString("player-name", GenerateRandomNameLowerCase(8, 12));
            foreach (var info in leaderboardInfoObjects)
            {
                string idAddon = info.LocalID.ToString();
                PlayerPrefs.SetInt("best-score-" + idAddon, info.UseHighscoreMode ? info.ScoreFloor : info.ScoreCap);
                PlayerPrefs.SetInt("num-scores-submitted" + idAddon, 0);
                PlayerPrefs.SetInt("score-mode" + idAddon, info.UseHighscoreMode ? 1 : 0);
            }
            PlayerPrefs.SetInt("PREFS_SETUP", 1);
        }

        public static int GetBestScore(int localID) => PlayerPrefs.GetInt("best-score" + localID.ToString());
        public static bool GetHighscoreMode(int localID) => PlayerPrefs.GetInt("score-mode" + localID.ToString()) == 1;
        public static int GetNumScoresSubmitted(int localID) => PlayerPrefs.GetInt("num-scores-submitted" + localID.ToString());

        public static bool UpdateBestScore(int localID, int newScore) {
            int storedScore = GetBestScore(localID);
            bool highscoreMode = GetHighscoreMode(localID);

            if ((newScore > storedScore && highscoreMode) || (newScore < storedScore && !highscoreMode)) {
                PlayerPrefs.SetInt("best-score" + localID.ToString(), newScore);
                return true;
            }
            return false;
        }

        public static void IncrementNumScoresSubmitted(int localID) {
            string prefsKey = "num-scores-submitted" + localID.ToString();
            PlayerPrefs.SetInt(prefsKey, PlayerPrefs.GetInt(prefsKey) + 1);
        }

        public static string PlayerName
        {
            get => PlayerPrefs.GetString("player-name");
            set => PlayerPrefs.SetString("player-name", value);
        }

        public static string GenerateRandomNameLowerCase(int minLen = 8, int maxLen = 12)
        {
            string output = "";
            int nameLength = Random.Range(minLen, maxLen);
            for (int i = 0; i < nameLength; i++) output += (char) Random.Range(97, 123);
            return output;
        }
    }
}