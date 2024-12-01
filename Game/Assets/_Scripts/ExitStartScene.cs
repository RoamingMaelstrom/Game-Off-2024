using GliderAudio;
using GliderServices;
using SOEvents;
using UnityEngine;

public class ExitStartScene : MonoBehaviour
{
    [SerializeField] StringSOEvent playSfxAtCameraEvent;
    [SerializeField] WorldInfo worldInfo;

    private void Awake() {
        SfxSystem sfxSystem = FindObjectOfType<SfxSystem>();
        sfxSystem?.FindAudioListener();
        worldInfo.CallOnMainSceneLoad();
        playSfxAtCameraEvent.AddListener(PlaySfxMenu);
    }

    private void PlaySfxMenu(string sfx)
    {
        GliderAudio.SFX.PlayRelativeToListener(sfx, Vector3.zero);
    }

    private void Start() {
        Music.SwitchTrackContainer(0);
        Music.ChangeVolume(1);
    }

    public void OnExitWindow() {
        Music.ChangeVolumeFaded(0f, 2f);
        AccountSystem accountSystem = FindObjectOfType<AccountSystem>();
        accountSystem?.RenameUser(PlayerLocalInfo.PlayerName);
    }
}
