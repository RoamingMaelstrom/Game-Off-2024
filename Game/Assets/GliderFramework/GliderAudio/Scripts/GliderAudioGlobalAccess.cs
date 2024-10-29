using UnityEngine;

namespace GliderAudio
{
    public static class SFX {
        private static SfxSystem sfxSystemGlobal;
        public static bool IsSetup{get; private set;} = false;

        public static void Setup(SfxSystem sfxMain){
            sfxSystemGlobal = sfxMain;
            IsSetup = true;
        }

        public static SfxAudioSourceInfo GetSfxAudioSourceInfo(int sourceID) => sfxSystemGlobal ? sfxSystemGlobal.GetAudioSourceInfoByID(sourceID) : SfxSystem.NullSourceInfo;

        public static int ChangeBaseSfxVolume(float newVolume){
            if (!sfxSystemGlobal) return -1;
            sfxSystemGlobal.ChangeSfxBaseVolume(newVolume);
            PlayStandard("change_volume_blip");
            return 1;
        }

        public static int ChangeSingleSfxVolume(int sourceID, float newVolumeMultiplier){
            if (!sfxSystemGlobal) return -1;
            SfxAudioSourceInfo sourceInfo = sfxSystemGlobal.GetAudioSourceInfoByID(sourceID);
            if (sourceInfo.ID == -1) return -1;
            sourceInfo.source.volume = sourceInfo.source.volume * newVolumeMultiplier / sourceInfo.volumeModifier;
            sourceInfo.volumeModifier = newVolumeMultiplier;
            return sourceID;
        }

        public static int StopPlaying(int sourceID){
            if (!sfxSystemGlobal) return -1;
            SfxAudioSourceInfo sourceInfo = sfxSystemGlobal.GetAudioSourceInfoByID(sourceID);
            if (sourceInfo.ID == -1) return -1;
            sourceInfo.source.Stop();
            return sourceID;
        }



        public static int PlayStandard(string clipName, float volume = 1f){
            if (!sfxSystemGlobal) return -1;
            return sfxSystemGlobal.PlayClip(new(clipName, Vector3.zero, volume, 0, false, false, null));
        }

        public static int PlayRelativeToListener(string clipName, Vector3 relativePos, float volume = 1f){
            if (!sfxSystemGlobal) return -1;
            return sfxSystemGlobal.PlayClip(new(clipName, relativePos, volume, 1, true, true, null));         
        }

        public static int PlayAtPoint(string clipName, Vector2 position, float volume = 1f){
            if (!sfxSystemGlobal) return -1;
            return sfxSystemGlobal.PlayClip(new(clipName, position, volume, 1, false, false, null));
        }

        public static int PlayAtRelativePoint(string clipName, Vector2 relativePos, float volume = 1f){
            if (!sfxSystemGlobal) return -1;
            return sfxSystemGlobal.PlayClip(new(clipName, relativePos, volume, 1, true, false, null));
        }

        public static int PlayRelativeToTransform(string clipName, Transform objectTransform, Vector3 offset, float volume = 1f){
            if (!sfxSystemGlobal) return -1;
            return sfxSystemGlobal.PlayClip(new(clipName, offset, volume, 1, true, false, objectTransform));
        }

        public static int PlayRelativeToTransform(string clipName, Transform objectTransform, float volume = 1f){
            if (!sfxSystemGlobal) return -1;
            return sfxSystemGlobal.PlayClip(new(clipName, Vector3.zero, volume, 1, true, false, objectTransform));
        }

        public static int PlayRandomStandard(float volume, params string[] clipNames) => PlayStandard(clipNames[Random.Range(0, clipNames.Length)], volume);
        public static int PlayRandomAtCamera(float volume, params string[] clipNames) => PlayRelativeToListener(clipNames[Random.Range(0, clipNames.Length)], Vector3.zero, volume);
        public static int PlayRandomAtPoint(Vector2 position, float volume, params string[] clipNames) => PlayAtPoint(clipNames[Random.Range(0, clipNames.Length)], position, volume);
        public static int PlayRandomAtRelativePoint(Vector2 position, float volume, params string[] clipNames) => PlayAtRelativePoint(clipNames[Random.Range(0, clipNames.Length)], position, volume);
        
        public static int PlayRandomStandard(params string[] clipNames) => PlayStandard(clipNames[Random.Range(0, clipNames.Length)]);
        public static int PlayRandomAtCamera(params string[] clipNames) => PlayRelativeToListener(clipNames[Random.Range(0, clipNames.Length)], Vector3.zero);
        public static int PlayRandomAtPoint(Vector2 position, params string[] clipNames) => PlayAtPoint(clipNames[Random.Range(0, clipNames.Length)], position);
        public static int PlayRandomAtRelativePoint(Vector2 position, params string[] clipNames) => PlayAtRelativePoint(clipNames[Random.Range(0, clipNames.Length)], position);


        public struct PlayClipCall
        {
            public string clipName; 
            public Vector3 position; 
            public float volumeMultiplier; 
            public float spatialBlend; 
            public bool relativePos; 
            public bool dynamicMovement; 
            public Transform trackingTransform;

            public PlayClipCall(string clipName, Vector3 position, float volumeMultiplier, float spatialBlend, bool relativePos, bool dynamicMovement, Transform trackingTransform) {
                this.clipName = clipName;
                this.position = position;
                this.volumeMultiplier = volumeMultiplier;
                this.spatialBlend = spatialBlend; 
                this.relativePos = relativePos;
                this.dynamicMovement = dynamicMovement;
                this.trackingTransform = trackingTransform;
            }
        }
    }



    public static class Music{
        private static MusicSystem musicSystemGlobal;
        public static bool IsSetup{get; private set;} = false;

        public static void Setup(MusicSystem musicSystem){
            musicSystemGlobal = musicSystem;
            IsSetup = true;
        }

        public static int ChangeBaseVolume(float newVolume){
            if (!musicSystemGlobal) return -1;
            musicSystemGlobal.ChangeBaseVolume(newVolume);
            return 1;
        }

        public static int ChangeVolume(float newVolume){
            if (!musicSystemGlobal) return -1;
            musicSystemGlobal.ChangeVolume(newVolume);
            return 1;
        }

        public static int ChangeVolumeFaded(float targetVolume, float fadeDuration){
            if (!musicSystemGlobal) return -1;
            musicSystemGlobal.ChangeVolumeFaded(targetVolume, fadeDuration);
            return 1;
        }

        public static int FadeToNextTrack(){
            if (!musicSystemGlobal) return -1;
            musicSystemGlobal.FadeToNextTrack();
            return 1;
        }

        public static int SetMusicPaused(bool isPaused){
            if (!musicSystemGlobal) return -1;
            musicSystemGlobal.SetPaused(isPaused);
            return 1;
        }

        public static int SetOnGamePauseMute(bool isPaused){
            if (!musicSystemGlobal) return -1;
            musicSystemGlobal.SetOnGamePauseMute(isPaused);
            return 1;
        }

        public static int SwitchTrackContainer(int index) => musicSystemGlobal ? musicSystemGlobal.SwitchTrackContainer(index) : -1;
    }
}
