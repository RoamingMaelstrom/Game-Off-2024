using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;


namespace GliderServices
{
    public class AccountSystem : MonoBehaviour
    {
        [SerializeField] LeaderboardInfo[] leaderboardInfoObjects;
        private int[] leaderboardLocalIDs;
        public static bool Init {get; private set;} = false;

        private void Awake() {
            if (!Init) InitSystem();
            else Destroy(gameObject);
        }

        private void InitSystem() {
            leaderboardLocalIDs = new int[leaderboardInfoObjects.Length];

            for (int i = 0; i < leaderboardInfoObjects.Length; i++)
            {
                leaderboardLocalIDs[i] = leaderboardInfoObjects[i].LocalID;
                leaderboardInfoObjects[i].highscoreRetriever = new(leaderboardInfoObjects[i].LocalID, leaderboardInfoObjects[i].ServerID, leaderboardInfoObjects[i].NumScoresToRetrieve);
            }

            Init = true;
        }

        private async void Start() {
            await ServiceConnection.InitUnityServices();

            if (!ServiceConnection.IsConnectedToNetwork())
            {
                Debug.Log("Could not find local network. Aborting SignIn.");
                return;
            }

            if (!PlayerLocalInfo.IsSetup()) PlayerLocalInfo.SetupPlayerPrefs(leaderboardInfoObjects);

            await ServiceConnection.SignIn();
            foreach (var info in leaderboardInfoObjects)
            {
                await info.highscoreRetriever.LoadScores();
            }
        }

        private async void OnApplicationQuit() {
            RenameUser(PlayerLocalInfo.PlayerName);
            foreach (var info in leaderboardInfoObjects)
            {
                await TrySubmitScore(info.LocalID, PlayerLocalInfo.GetBestScore(info.LocalID));
            }
            ServiceConnection.SignOut();
        }

        public async void ResetAccount() {
            await ServiceConnection.DeleteAccount();
            await ServiceConnection.SignIn();  // Creates a new account automatically.
            PlayerLocalInfo.SetupPlayerPrefs(leaderboardInfoObjects);
        }

        public async Task<bool> TrySubmitScore(int localID, int newScore) {
            ServiceConnection.SyncPlayerNameServerToLocal(PlayerLocalInfo.PlayerName);
            
            bool isNewBest = PlayerLocalInfo.UpdateBestScore(localID, newScore);
            bool submitted = false;
            if (isNewBest) {
                LeaderboardInfo info = GetLeaderboardInfoByLocalID(localID);
                submitted = await SubmitHighscore.TrySubmitScore(info, newScore);
                await info.highscoreRetriever.LoadScores();
            }

            return isNewBest && submitted;
        }

        public void RenameUser(string newName) {
            PlayerLocalInfo.PlayerName = newName;
            ServiceConnection.SyncPlayerNameServerToLocal(PlayerLocalInfo.PlayerName);
        }

        public LeaderboardInfo GetLeaderboardInfoByLocalID(int localID) {
            foreach (var info in leaderboardInfoObjects)
            {
                if (info.LocalID == localID) return info;            
            }
            return null;
        }
    }
}



