using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GliderServices 
{
    [CreateAssetMenu(fileName = "Leaderboard Info", menuName = "LeaderboardInfoObject", order = 0)]
    public class LeaderboardInfo : ScriptableObject
    {
        [field: SerializeField] public int LocalID {get; private set;}
        [field: SerializeField] public string ServerID {get; private set;} = "Main_Leaderboard";
        [field: SerializeField] public int ScoreFloor {get; private set;}
        [field: SerializeField] public int ScoreCap {get; private set;}
        [field: SerializeField] public bool UseHighscoreMode {get; private set;} = true;
        [field: SerializeField] public int MaxScoreSubmissionPerPlayer {get; private set;} = 100;
        [field: SerializeField] public int NumScoresToRetrieve {get; private set;} = 10;
        public HighscoreRetriever highscoreRetriever;  
    }
}
