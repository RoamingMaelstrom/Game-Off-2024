using GliderServices;
using TMPro;
using UnityEngine;

public class LocalHighscoreDisplay : MonoBehaviour
{
    [SerializeField] LeaderboardInfo leaderboardInfo;
    [SerializeField] TextMeshProUGUI highscoreText;
    [SerializeField] TextMeshProUGUI tagText;

    private void Update() {
        highscoreText.SetText(string.Format("Highscore:    {0:n0}", (PlayerLocalInfo.GetBestScore(leaderboardInfo.LocalID) / 10) * 10));
        tagText.SetText("Tag:    " + PlayerLocalInfo.PlayerName);
    }
}
