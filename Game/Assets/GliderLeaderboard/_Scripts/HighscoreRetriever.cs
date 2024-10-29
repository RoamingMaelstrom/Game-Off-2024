using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;

namespace GliderServices
{
    public class HighscoreRetriever
    {
        public RetrievedScoreObject[] ScoreObjects {get; private set;}
        private readonly GetScoresOptions scoreOptions;
        public readonly int leaderboardLocalID;
        private readonly string leaderboardServerID;
        private readonly int numScoresToRetrieve;
        private LeaderboardScoresPage LatestScoresResponse;

        public HighscoreRetriever(int leaderboardLocalID, string leaderboardServerID, int numScoresToRetrieve) {
            this.leaderboardLocalID = leaderboardLocalID;
            this.leaderboardServerID = leaderboardServerID;
            this.numScoresToRetrieve = numScoresToRetrieve;
            scoreOptions = CreateScoreOptions(numScoresToRetrieve);
        }

    
        public async Task LoadScores()
        {
            Debug.Log("Leaderboard Retrieve Initiated.");
            LatestScoresResponse = await LeaderboardsService.Instance.GetScoresAsync(leaderboardServerID, scoreOptions);
            Debug.Log("Leaderboard Retrieve Completed.");
            ScoreObjects = new RetrievedScoreObject[numScoresToRetrieve];

            for (int i = 0; i < LatestScoresResponse.Results.Count; i++)
            {
                LeaderboardEntry score = LatestScoresResponse.Results[i];
                ScoreObjects[i] = new RetrievedScoreObject(score.PlayerId, score.Rank, score.PlayerName, (int)score.Score);
            }

            AddBlankScoreObjects(ScoreObjects, LatestScoresResponse.Results.Count);
        }

        private GetScoresOptions CreateScoreOptions(int numScores)
        {
            GetScoresOptions scoreOptions = new()
            {
                Limit = numScores,
                Offset = 0
            };
            return scoreOptions;
        }

        private void AddBlankScoreObjects(RetrievedScoreObject[] scoreObjectArray, int startIndex)
        {
            for (int i = startIndex; i < scoreObjectArray.Length; i++)
            {
                scoreObjectArray[i] = new RetrievedScoreObject("#", i, "", 0);
            } 
        }
    }

    [System.Serializable]
    public class RetrievedScoreObject
    {
        public readonly string playerID;
        public readonly int rank;
        public readonly string playerName;
        public readonly int score;

        public RetrievedScoreObject(string playerID, int rank, string playerName, int score)
        {
            this.playerID = playerID;
            this.rank = rank;
            this.playerName = playerName;
            this.score = score;
        }
    }

}
