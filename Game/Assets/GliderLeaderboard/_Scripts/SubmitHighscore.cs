using UnityEngine;
using Unity.Services.Leaderboards;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace GliderServices
{ 
    public static class SubmitHighscore
    {
        public static async Task<bool> TrySubmitScore(LeaderboardInfo info, int newScore)
        {
            ScoreCheckLog log = new();
            int storedScore = PlayerLocalInfo.GetBestScore(info.LocalID);
            int numScoresSubmitted = PlayerLocalInfo.GetNumScoresSubmitted(info.LocalID);
            bool highscoreMode = PlayerLocalInfo.GetHighscoreMode(info.LocalID);

            if (RunSubmitScoreChecks(ref log, info, newScore, storedScore, numScoresSubmitted, highscoreMode))
            {
                await SubmitScoreNoChecks(info.ServerID, newScore);
                LogCheck(ref log, true, VALID_HIGHSCORE_CHECK_NAME.SCORE_SUBMITTED);
                return true;
            }

            Debug.Log(log.GetStateAsString());
            return false;
        }

        private static bool RunSubmitScoreChecks(ref ScoreCheckLog log, LeaderboardInfo info, int newScore, int storedScore, int numScoresSubmitted, bool highscoreMode)
        {
            int scoreMode = info.UseHighscoreMode ? 1 : -1;
            Debug.Log(newScore);
            Debug.Log(storedScore);
            if (!LogCheck(ref log, IsBestScore(newScore, scoreMode), VALID_HIGHSCORE_CHECK_NAME.BEST_SCORE)) return false;
            if (!LogCheck(ref log, NumScoreSubmissionNotTooHigh(), VALID_HIGHSCORE_CHECK_NAME.NUMBER_SUBMITTED_CAP_NOT_EXCEEDED)) return false;
            if (!LogCheck(ref log, ScoreWithinBounds(), VALID_HIGHSCORE_CHECK_NAME.MEETS_THRESHOLD)) return false;
            if (!LogCheck(ref log, ServiceConnection.IsSignedIn(), VALID_HIGHSCORE_CHECK_NAME.USER_SIGNED_IN)) return false;
            if (!LogCheck(ref log, ServiceConnection.IsValidAccessToken(), VALID_HIGHSCORE_CHECK_NAME.HAS_VALID_ACCESS_TOKEN)) return false;
            if (!LogCheck(ref log, ServiceConnection.IsConnectedToNetwork(), VALID_HIGHSCORE_CHECK_NAME.CONNECTED_TO_LOCAL_NETWORK)) return false;
            return true;

            bool IsBestScore(int score, int scoreMode) => score * scoreMode >= storedScore * scoreMode;
            bool NumScoreSubmissionNotTooHigh() => numScoresSubmitted < info.MaxScoreSubmissionPerPlayer;
            bool ScoreWithinBounds() => (newScore <= info.ScoreCap && newScore >= info.ScoreFloor) || info.ScoreCap == info.ScoreFloor;
        }

        private static bool LogCheck(ref ScoreCheckLog log, bool checkFunctionOutput, VALID_HIGHSCORE_CHECK_NAME checkKey)
        {
            log[checkKey] = checkFunctionOutput ? SCORE_PASS_CHECK_STATUS.TRUE : SCORE_PASS_CHECK_STATUS.FALSE;
            return checkFunctionOutput;
        }

        private static async Task SubmitScoreNoChecks(string leaderboardId, int score)
        {
            Debug.Log("Submitting Score...");
            await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardId, score);            
            Debug.Log("Score Submitted.");
        }
    }



    public class ScoreCheckLog : Dictionary<VALID_HIGHSCORE_CHECK_NAME, SCORE_PASS_CHECK_STATUS>
    {
        public ScoreCheckLog()
        {
            this[VALID_HIGHSCORE_CHECK_NAME.BEST_SCORE] = SCORE_PASS_CHECK_STATUS.NOT_RUN;
            this[VALID_HIGHSCORE_CHECK_NAME.NUMBER_SUBMITTED_CAP_NOT_EXCEEDED] = SCORE_PASS_CHECK_STATUS.NOT_RUN;
            this[VALID_HIGHSCORE_CHECK_NAME.MEETS_THRESHOLD] = SCORE_PASS_CHECK_STATUS.NOT_RUN;
            this[VALID_HIGHSCORE_CHECK_NAME.USER_SIGNED_IN] = SCORE_PASS_CHECK_STATUS.NOT_RUN;
            this[VALID_HIGHSCORE_CHECK_NAME.HAS_VALID_ACCESS_TOKEN] = SCORE_PASS_CHECK_STATUS.NOT_RUN;
            this[VALID_HIGHSCORE_CHECK_NAME.CONNECTED_TO_LOCAL_NETWORK] = SCORE_PASS_CHECK_STATUS.NOT_RUN;
            this[VALID_HIGHSCORE_CHECK_NAME.SCORE_SUBMITTED] = SCORE_PASS_CHECK_STATUS.FALSE;
        }

        public string GetStateAsString()
        {
            string output = "Check_Valid_Submission_Log.\n";
            foreach (var kvp in this)
            {
                string pass_status = kvp.Value switch
                {
                    SCORE_PASS_CHECK_STATUS.FALSE => "False",
                    SCORE_PASS_CHECK_STATUS.TRUE => "True",
                    _ => "Not Run",
                };

                output += string.Format("Check Name = {0}, Pass Status = {1}\n", kvp.Key, pass_status);
            }

            return output;
        }

    }

    public enum SCORE_PASS_CHECK_STATUS
    {
        FALSE = 0,
        TRUE = 1,
        NOT_RUN = 2
    }

    public enum VALID_HIGHSCORE_CHECK_NAME
    {
        BEST_SCORE,
        NUMBER_SUBMITTED_CAP_NOT_EXCEEDED,
        MEETS_THRESHOLD,
        USER_SIGNED_IN,
        HAS_VALID_ACCESS_TOKEN,
        CONNECTED_TO_LOCAL_NETWORK,
        SCORE_SUBMITTED
    }   

}
