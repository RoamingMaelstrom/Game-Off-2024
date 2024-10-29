using System.Collections.Generic;
using UnityEngine;
using GliderSave;
using System.Linq;
using System;

namespace GliderAudio
{

    public class SfxSystem : MonoBehaviour
    {
        [SerializeField] FloatSaveObject baseSfxVolumeSaveObject;

        [SerializeField] AudioListener listener;
        [SerializeField] GameObject baseAudioSourcePrefab;
        [SerializeField] int numberOfSources = 32;
        [SerializeField] ClipInfoContainer clipContainer;
        [SerializeField] List<SfxAudioSourceInfo> audioSourceInfos = new();
        [SerializeField] int[] audioSourceInfoIDs;
        [SerializeField] [Range(0f, 1f)] float baseVolume;

        [SerializeField] string[] clipsPlaying;


        public static SfxAudioSourceInfo NullSourceInfo{get => new(-1, null); private set {}}

        private void Awake() 
        {
            if (!listener) listener = FindObjectOfType<AudioListener>();
            clipsPlaying = new string[numberOfSources];
            CreateAudioSources();
            if (!SFX.IsSetup) SFX.Setup(this); 
            ChangeSfxBaseVolume(baseSfxVolumeSaveObject.GetValue());
        }



        private void CreateAudioSources()
        {

            for (int i = 0; i < numberOfSources; i++) 
            {
                AudioSource source = Instantiate(baseAudioSourcePrefab, transform).GetComponent<AudioSource>();
                audioSourceInfos.Add(new(i, source));
            }

            audioSourceInfoIDs = new int[numberOfSources * 4];
            for (int i = 0; i < audioSourceInfoIDs.Length; i++)
            {
                if (i < numberOfSources) audioSourceInfoIDs[i] = i;
                else audioSourceInfoIDs[i] = -1;
            }
        }

        private void FixedUpdate() 
        {
            clipContainer.UpdateClipInfosCooldown(Time.fixedDeltaTime);
            UpdateAudioSourcePositions();
            UpdatePlayingClipsData();
        }

        private void UpdatePlayingClipsData() {
            for (int i = 0; i < audioSourceInfos.Count; i++)
            {
                clipsPlaying[i] = audioSourceInfos[i].source.isPlaying ? audioSourceInfos[i].clipName : "";
            }
        }

        public void ChangeSfxBaseVolume(float newVolume)
        {
            float oldBaseVolume = baseVolume;
            baseVolume = Mathf.Clamp(newVolume, 0f, 1f);
            baseSfxVolumeSaveObject.SetValue(baseVolume);
            UpdatePlayingSourcesVolume(baseVolume, oldBaseVolume);
        }

        private void UpdatePlayingSourcesVolume(float newVolume, float oldVolume) => audioSourceInfos.ForEach(sourceInfo => sourceInfo.source.volume *= newVolume / oldVolume);

        public int PlayClip(SFX.PlayClipCall call)
        {
            ClipInfoEntry clipInfoEntry = clipContainer.GetSfxClipInfoEntry(call.clipName);
            if (clipInfoEntry.clipInfo == null) return -1;
            if (clipInfoEntry.currentCooldown > 0) return -1;

            SfxAudioSourceInfo sourceInfo = GetAudioSourceInfoObject(clipInfoEntry);
            ConfigureAudioSource(sourceInfo, clipInfoEntry.clipInfo, call);
            clipInfoEntry.currentCooldown = clipInfoEntry.clipInfo.cooldownOnPlay;

            if (!call.trackingTransform) sourceInfo.source.transform.position = GetAudioSourcePosition(call.position, call.relativePos);
            else sourceInfo.source.transform.position = sourceInfo.trackingTransform.position + sourceInfo.relativePos;
            
            sourceInfo.source.Play();

            return sourceInfo.ID;
        }

        private SfxAudioSourceInfo GetAudioSourceInfoObject(ClipInfoEntry clipInfoEntry)
        {
            if (clipInfoEntry.currentCooldown <= 0) return GetLowestPrioritySourceInfoObject();
            
            int sameClipSourceIndex = audioSourceInfos.FindIndex(info => info.clipName == clipInfoEntry.clipInfo.clipName);
            return sameClipSourceIndex > -1 ? audioSourceInfos[sameClipSourceIndex]: NullSourceInfo;
        }

        private SfxAudioSourceInfo GetLowestPrioritySourceInfoObject()
        {
            int freeSourceIndex = audioSourceInfos.FindIndex(info => !info.source.isPlaying);
            if (freeSourceIndex > -1) return audioSourceInfos[freeSourceIndex];

            int lowestPriority = audioSourceInfos.Min(info => info.source.priority);
            SfxAudioSourceInfo sourceInfo = audioSourceInfos.Find(info => info.source.priority == lowestPriority);

            audioSourceInfoIDs[sourceInfo.ID] = -1;
            sourceInfo.ID = (sourceInfo.ID + numberOfSources) % (numberOfSources * 4);
            audioSourceInfoIDs[sourceInfo.ID] = sourceInfo.ID % numberOfSources;

            return sourceInfo;
        }

        public void ConfigureAudioSource(SfxAudioSourceInfo sourceInfo, SfxClipInfo clipInfo, SFX.PlayClipCall call)
        {   
            sourceInfo.source.Stop();

            sourceInfo.source.clip = clipInfo.clip;
            sourceInfo.source.volume = baseVolume * clipInfo.volume * Mathf.Clamp(call.volumeMultiplier, 0f, 1f);
            sourceInfo.source.priority = clipInfo.priority;
            sourceInfo.source.spatialBlend = Mathf.Clamp(call.spatialBlend, 0f, 1f);

            sourceInfo.clipName = clipInfo.clipName;
            sourceInfo.volumeModifier = call.volumeMultiplier;
            sourceInfo.dynamicMovement = call.dynamicMovement;
            sourceInfo.isRelativePos = call.relativePos;
            sourceInfo.relativePos = call.relativePos ? call.position: Vector3.zero;
            sourceInfo.trackingTransform = call.trackingTransform;
        }

        public SfxAudioSourceInfo GetAudioSourceInfoByID(int sourceID) 
        {
            if (sourceID < 0 || sourceID > audioSourceInfoIDs.Length) return NullSourceInfo;

            int index = audioSourceInfoIDs[sourceID];
            return index == -1 ? NullSourceInfo : audioSourceInfos[index];
        }

        private Vector3 GetAudioSourcePosition(Vector3 inputPosition, bool isRelativePos) => isRelativePos ? listener.transform.position + inputPosition : inputPosition;

        private void UpdateAudioSourcePositions()
        {
            foreach (var sourceInfo in audioSourceInfos)
            {
                if (!sourceInfo.dynamicMovement) continue;
                if (!sourceInfo.isRelativePos) return;

                if (sourceInfo.trackingTransform != null) sourceInfo.source.transform.position = sourceInfo.trackingTransform.position + sourceInfo.relativePos;
                else sourceInfo.source.transform.position = GetAudioSourcePosition(sourceInfo.relativePos, true);
            }
        }
    }



    [System.Serializable]
    public class SfxAudioSourceInfo
    {
        public int ID;
        public AudioSource source;
        public string clipName;
        public float volumeModifier;
        public bool dynamicMovement; 
        public bool isRelativePos;
        public Vector3 relativePos;
        public Transform trackingTransform;

        public SfxAudioSourceInfo(int id, AudioSource source)
        {
            ID = id;
            this.source = source;
            clipName = "";
            volumeModifier = 1.0f;
            dynamicMovement = false;
            isRelativePos = false;
            relativePos = Vector3.zero;
            trackingTransform = null;
        }
    }
}
