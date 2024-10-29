using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GliderSave;

namespace GliderAudio
{
    public class MusicSystem : MonoBehaviour
    {
        [Tooltip("FloatSaveObject that Stores Base Volume. Manipulated by volume sliders. For effects such as fading, set runtimeVolumeMultiplier")]
        [SerializeField] FloatSaveObject musicVolumeSaveObject;


        [Header("Volume")]

        [Tooltip("Maximum volume that AudioSources can play at. Set in Inspector. DO NOT SET AT RUNTIME")]
        [SerializeField] float MAX_AUDIO_SOURCE_VOLUME = 0.6f;

        [Tooltip("Manipulate this Volume multiplier at runtime to produce effects e.g. Fading, Music Muffle.")]
        [SerializeField] [Range(0f, 1f)] float runtimeVolumeMultiplier = 1.0f;
        [SerializeField] [Range(0f, 1f)] float pauseMuffleMultiplier = 0.25f;
        private bool usePauseMuffle = false;
        public float VolumeActual {get => MAX_AUDIO_SOURCE_VOLUME * VolumeVirtual; private set {}}
        public float VolumeBaseVirtual {get => musicVolumeSaveObject.GetValue(); private set {}}
        public float VolumeVirtual {get => VolumeBaseVirtual * runtimeVolumeMultiplier * (usePauseMuffle ? pauseMuffleMultiplier : 1.0f); private set {}}

        [Header("Internals")]
        [SerializeField] AudioSource playingSource;
        [SerializeField] List<TrackContainer> trackContainers = new();
        [SerializeField] int startTrackContainerIndex = 0;
        private TrackContainer currentTrackContainer;
        private Coroutine fadingRoutine;
        private Coroutine nextTrackFadingRoutine;
        [SerializeField] bool isPaused = false;

        private bool fadeInOutRunning = false;



        private void Awake(){
            if (!Music.IsSetup) Music.Setup(this); 
        }

        void Start(){
            UpdateSourceVolume();
            currentTrackContainer = trackContainers[startTrackContainerIndex];
            PlayNextTrack();
        }

        void Update(){
            if (isPaused && playingSource.isPlaying) playingSource.Pause();
            if (!isPaused && !playingSource.isPlaying) playingSource.UnPause();

            if (fadeInOutRunning) return;

            if (playingSource.clip.length - (playingSource.timeSamples / (float)playingSource.clip.frequency) <= currentTrackContainer.defaultFadeOutDuration) FadeToNextTrack();
        }



        public void ChangeBaseVolume(float newValue){
            musicVolumeSaveObject.SetValue(Mathf.Clamp(newValue, 0f, 1f));
            UpdateSourceVolume();
        }

        public void ChangeVolume(float newValue){ 
            runtimeVolumeMultiplier = Mathf.Clamp(newValue, 0f, 1f);
            UpdateSourceVolume();
        }

        public void ChangeVolumeFaded(float targetVolume, float duration){
            if (fadingRoutine != null) StopCoroutine(fadingRoutine);
            fadingRoutine = StartCoroutine(FadeVolumeRoutine(targetVolume, duration));
        }

        public void FadeToNextTrack(){
            if (nextTrackFadingRoutine != null) StopCoroutine(nextTrackFadingRoutine);
            nextTrackFadingRoutine = StartCoroutine(TrackChangeFadeRoutine(currentTrackContainer.defaultFadeOutDuration, currentTrackContainer.defaultFadeInDuration));
        }

        public void SetOnGamePauseMute(bool pauseMuffle){
            usePauseMuffle = pauseMuffle;
            UpdateSourceVolume();
        }

        public void SetPaused(bool isPaused) => this.isPaused = isPaused;

        public int SwitchTrackContainer(int index){
            if (!ValidTrackContainerIndex(index)) return -2;
            if (currentTrackContainer == trackContainers[index]) return -3;
            currentTrackContainer = trackContainers[index];
            PlayNextTrack();
            return 1;
        }



        /* 
        Be careful about calling this while another instance of the routine is currently playing. 
        Will likely lead to unexpected results as the existing routine is interrupted.
        */
        private IEnumerator FadeVolumeRoutine(float targetVolume, float duration){
            targetVolume = Mathf.Clamp(targetVolume, 0f, 1f);
            float time = 0;
            float startVolume = runtimeVolumeMultiplier;
            float deltaVolume = runtimeVolumeMultiplier - targetVolume;
            float deltaT = 0.01f;

            float m = Mathf.PI * 0.5f / duration;
            while (time < duration){
                time += deltaT;
                ChangeVolume(startVolume - (Mathf.Sin(m * time) * deltaVolume));
                yield return new WaitForSecondsRealtime(deltaT);
            }

            ChangeVolume(targetVolume);
        }

        private bool ValidTrackContainerIndex(int index) => index < trackContainers.Count && index >= 0;

        private void PlayNextTrack(){
            playingSource.clip = currentTrackContainer.GetNextTrack();
            playingSource.Play();
        }

        private IEnumerator TrackChangeFadeRoutine(float fadeOutTime, float fadeInTime){
            fadeInOutRunning = true;
            float startVolume = runtimeVolumeMultiplier;
            ChangeVolumeFaded(0, fadeOutTime);
            yield return new WaitForSecondsRealtime(fadeOutTime);

            PlayNextTrack();

            ChangeVolumeFaded(startVolume, fadeInTime);
            yield return new WaitForSecondsRealtime(fadeInTime);
            fadeInOutRunning = false;
        }

        private void UpdateSourceVolume() => playingSource.volume = VolumeActual;
    }
}
