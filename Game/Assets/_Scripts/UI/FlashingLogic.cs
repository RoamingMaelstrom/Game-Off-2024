using System;
using System.Collections;
using SOEvents;
using TMPro;
using UnityEngine;

public class FlashingLogic : MonoBehaviour
{
    [SerializeField] IntSOEvent levelUpEvent;
    [SerializeField] IntSOEvent missionCompleteEvent;
    [SerializeField] FloatSOEvent playerDamagedEvent;
    [SerializeField] TechObjectDisplaySOEvent unlockTechEvent;
    [SerializeField] SpriteRenderer earthMinimapIcon;

    [SerializeField] float numberOfFlashes = 3f;
    [SerializeField] float flashingDuration = 2f;
    [SerializeField] Color redFlashColour;

    [SerializeField] TextMeshProUGUI levelUpText;
    [SerializeField] Color levelUpFlashColour;
    [SerializeField] TextMeshProUGUI missionCompleteText;
    [SerializeField] Color missionCompleteFlashColour;

    [SerializeField] MissionDisplayPanel[] missionDisplayPanels;

    private Coroutine earthFlashingCoroutine;
    private bool flashRunning = false;

    private void Awake() {
        levelUpEvent.AddListener(FlashLevelUpText);
        missionCompleteEvent.AddListener(FlashMissionCompleteText);
        playerDamagedEvent.AddListener(FlashEarth);
        unlockTechEvent.AddListener(CheckForFlashing);
    }

    private void FlashMissionCompleteText(int arg0) {
        StartCoroutine(FlashText(missionCompleteText, Color.clear, missionCompleteFlashColour, 5, 2.2f));
    }

    private void FlashLevelUpText(int arg0) {
        StartCoroutine(FlashText(levelUpText, Color.clear, levelUpFlashColour, 5, 2.2f));
    }

    private void FlashEarth(float arg0) {
        if (flashRunning) return;
        earthFlashingCoroutine = StartCoroutine(FlashEarth());
    }

    private static IEnumerator FlashText(TextMeshProUGUI levelUpText, Color startColour, Color flashColour, int count, float duration) {
        float interval = duration / count;
        bool flash = false;
        float t = 0;
        while(t < duration) {
            flash = !flash;
            levelUpText.color = flash ? flashColour : startColour;
            t += interval / 2f;
            yield return new WaitForSeconds(interval / 2f);
        }

        levelUpText.color = startColour;
    }

    // Todo: Make Generic ones for sprites, text and images
    private IEnumerator FlashEarth() {
        flashRunning = true;
        float interval = flashingDuration / numberOfFlashes;
        Color startColour = earthMinimapIcon.color;
        bool flashColour = false;
        float t = 0;
        while(t < flashingDuration) {
            flashColour = !flashColour;
            earthMinimapIcon.color = flashColour ? redFlashColour : startColour;
            t += interval / 2f;
            yield return new WaitForSeconds(interval / 2f);
        }

        earthMinimapIcon.color = startColour;
        flashRunning = false;
    }

    private void CheckForFlashing(TechObjectDisplay tOD) {
        foreach (var effect in tOD.techObject.effects)
        {
            if (effect.effectType == EffectType.START_COLLECTION_MISSION) StartCoroutine(FlashCanvasGroup(missionDisplayPanels[1].canvasGroup, 0.25f));
            else if (effect.effectType == EffectType.START_SCANNING_MISSION) StartCoroutine(FlashCanvasGroup(missionDisplayPanels[2].canvasGroup, 0.25f));
            else if (effect.effectType == EffectType.START_PROBE_MISSION) StartCoroutine(FlashCanvasGroup(missionDisplayPanels[3].canvasGroup, 0.25f));
            else if (effect.effectType == EffectType.PROTOTYPE_WEAPON) StartCoroutine(FlashCanvasGroup(missionDisplayPanels[3].canvasGroup, 0.25f));
            else if (effect.effectType == EffectType.START_FINALE_MISSION) StartCoroutine(FlashCanvasGroup(missionDisplayPanels[4].canvasGroup, 0.25f));
        }
    }

    private IEnumerator FlashCanvasGroup(CanvasGroup canvasGroup, float alpha) {
        float interval = flashingDuration / numberOfFlashes;
        float startAlpha = canvasGroup.alpha;
        bool dimmed = false;
        float t = 0;
        while(t < flashingDuration) {
            dimmed = !dimmed;
            canvasGroup.alpha = dimmed ? alpha : startAlpha;
            t += interval / 2f;
            yield return new WaitForSeconds(interval / 2f);
        }

        canvasGroup.alpha = startAlpha;
    }
}
