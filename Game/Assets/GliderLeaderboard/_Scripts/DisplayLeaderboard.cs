using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace GliderServices
{
    public class DisplayLeaderboard : MonoBehaviour
    {
        [SerializeField] LeaderboardInfo leaderboardInfo;
        [SerializeField] GameObject leaderboardContainer;
        [SerializeField] List<GameObject> highscoreRowPrefabs = new();
        [SerializeField] GameObject leaderboardNotAvailablePanel;

        private GameObject[] highscoreDisplays;

        private bool rowsCreated = false;
        private bool checkForLeaderboardReply = false;
        private HighscoreRetriever highscoreRetriever;

        private void Start() {
            highscoreRetriever = leaderboardInfo.highscoreRetriever;
        }

        private void OnEnable() 
        {
            highscoreRetriever ??= leaderboardInfo.highscoreRetriever;
            if (highscoreRetriever == null || highscoreRetriever.ScoreObjects.Length == 0) SetLeaderboardNotAvailable();
            else SetupLeaderboard();
        }

        private void SetupLeaderboard()
        {
            leaderboardNotAvailablePanel.SetActive(false);

            CreateLeaderboardRows();
            PopulateLeaderboard();
        }

        private void SetLeaderboardNotAvailable()
        {
            leaderboardNotAvailablePanel.SetActive(true);
            checkForLeaderboardReply = true;
        }

        private void FixedUpdate() 
        {
            if (checkForLeaderboardReply)
            {
                if (highscoreRetriever == null) return;
                if (highscoreRetriever.ScoreObjects == null) return;
                if (highscoreRetriever.ScoreObjects.Length == 0) return;

                checkForLeaderboardReply = false;
                SetupLeaderboard();
            }
        }

        private void CreateLeaderboardRows()
        {
            if (rowsCreated) return;
            if (highscoreRetriever.ScoreObjects.Length == 0) return;

            highscoreDisplays = new GameObject[highscoreRetriever.ScoreObjects.Length];
            for (int i = 0; i < highscoreDisplays.Length; i++)
            {
                highscoreDisplays[i] = Instantiate(highscoreRowPrefabs[i % 2], leaderboardContainer.transform);
            }
            rowsCreated = true;
        }

        private void PopulateLeaderboard()
        {
            Debug.Log("Populating leaderboard");

            for (int i = 0; i < highscoreRetriever.ScoreObjects.Length; i++)
            {
                RetrievedScoreObject scoreObject = highscoreRetriever.ScoreObjects[i];
                
                TextMeshProUGUI[] textArray = highscoreDisplays[i].transform.GetComponentsInChildren<TextMeshProUGUI>();

                int rank = scoreObject.rank + 1;
                string name = scoreObject.playerName;
                name = name.Length > 0 ? name[..name.IndexOf("#")] : "";
                int score = scoreObject.score;

                textArray[0].SetText(string.Format("#{0}", rank));
                textArray[1].SetText(name);
                textArray[2].SetText(score.ToString());
            }
        }
    }
}
