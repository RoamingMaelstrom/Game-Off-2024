using UnityEngine;
using System.Collections;
using GliderAudio;
using SOEvents;
using System;

public class TriggeredSfxPlayer : MonoBehaviour
{
    [SerializeField] StringSOEvent playSfxOnCameraEvent;
    [SerializeField] IntSOEvent levelUpEvent;
    [SerializeField] FloatSOEvent playerDamagedEvent;
    [SerializeField] TechObjectDisplaySOEvent unlockTechEvent;

    [SerializeField] IntSOEvent missionCompleteEvent;
    [SerializeField] GameObjectFloatSOEvent probeDestroyedEvent;
    [SerializeField] IntSOEvent startMissionEvent;

    [SerializeField] string levelUpSfx;
    [SerializeField] string[] playerDamagedSfx;
    [SerializeField] string[] unlockSfx;
    [SerializeField] string missionSuccessSfx;
    [SerializeField] string probeDestroyedSfx;

    [SerializeField] int mainMusicTrackContainerID;
    [SerializeField] int reverseEngineerTrackContainerID;
    [SerializeField] int scanningTrackContainerID;
    [SerializeField] int probeTrackContainerID;
    [SerializeField] int finaleTrackContainerID;
    [SerializeField] float musicFadeDuration;

    private void Awake() {
        missionCompleteEvent.AddListener(FadeToMainMusic);
        missionCompleteEvent.AddListener(PlayMissionSuccessSfx);
        probeDestroyedEvent.AddListener(FadeToMainMusic);
        probeDestroyedEvent.AddListener(PlayProbeDestroyedSfx);
        startMissionEvent.AddListener(FadeToMissionMusic);

        playSfxOnCameraEvent.AddListener(PlaySfxOnCamera);
        levelUpEvent.AddListener(PlayLevelUpSfx);
        playerDamagedEvent.AddListener(PlayPlayerDamagedSfx);
        unlockTechEvent.AddListener(PlayUnlockSFX);
    }

    private void PlayProbeDestroyedSfx(GameObject arg0, float arg1) {
        SFX.PlayRelativeToListener(probeDestroyedSfx, Vector3.zero);
    }

    private void PlayMissionSuccessSfx(int arg0) => SFX.PlayRelativeToListener(missionSuccessSfx, Vector3.zero);

    private void FadeToMissionMusic(int missionID) {
        int containerID = missionID switch
        {
            0 => reverseEngineerTrackContainerID,
            1 => scanningTrackContainerID,
            2 => probeTrackContainerID,
            3 => finaleTrackContainerID,
            _ => mainMusicTrackContainerID,
        };
        StartCoroutine(SwitchTrackContainersFaded(containerID, musicFadeDuration));
    }

    private void FadeToMainMusic(GameObject arg0, float arg1) {
        StartCoroutine(SwitchTrackContainersFaded(mainMusicTrackContainerID, musicFadeDuration));
    }

    private void FadeToMainMusic(int arg0) {
        StartCoroutine(SwitchTrackContainersFaded(mainMusicTrackContainerID, musicFadeDuration));
    }

    private IEnumerator SwitchTrackContainersFaded(int trackContainerID, float duration) {
        Music.ChangeVolumeFaded(0.01f, duration);
        yield return new WaitForSecondsRealtime(duration);

        Music.SwitchTrackContainer(trackContainerID);
        Music.ChangeVolumeFaded(1f, duration / 2f);
    }

    private void PlaySfxOnCamera(string sfxName)
    {
        SFX.PlayRelativeToListener(sfxName, Vector2.zero);
    }

    private void PlayPlayerDamagedSfx(float arg0)
    {
        SFX.PlayRandomAtCamera(playerDamagedSfx);
    }

    private void PlayLevelUpSfx(int arg0)
    {
        SFX.PlayRelativeToListener(levelUpSfx, Vector3.zero);
    }

    private void PlayUnlockSFX(TechObjectDisplay arg0)
    {
        SFX.PlayRandomAtCamera(unlockSfx);
    }
}
