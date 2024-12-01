using GliderAudio;
using UnityEngine;

public class SetupMainScene : MonoBehaviour
{
    [SerializeField] WorldInfo worldInfo;
    [SerializeField] bool disableListenerSearch = false;
    [SerializeField] int mainSceneTrackContainerID = 1;

    private void Awake() {
        Music.SwitchTrackContainer(mainSceneTrackContainerID);
        if (!disableListenerSearch) FindObjectOfType<SfxSystem>().FindAudioListener();
        worldInfo.CallOnMainSceneLoad();
        Music.ChangeVolumeFaded(1f, 3f);
    }
}
