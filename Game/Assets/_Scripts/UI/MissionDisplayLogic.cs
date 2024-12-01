using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SOEvents;

public class MissionDisplayLogic : MonoBehaviour
{
    [SerializeField] IntSOEvent startMissionEvent;

    [SerializeField] EarthSimulation earthSimulation;
    [SerializeField] MissionDisplayPanel earthMDP;

    [SerializeField] ReverseEngineerMissionLogic reverseEngineerMission;
    [SerializeField] MissionDisplayPanel reverseEngineerMDP;
    
    [SerializeField] ScanningMission scanningMission;
    [SerializeField] MissionDisplayPanel scanningMDP;

    [SerializeField] ProbeMission probeMission;
    [SerializeField] MissionDisplayPanel probeMDP;

    [SerializeField] PrototypeLaser prototypeLaser;
    [SerializeField] MissionDisplayPanel prototypeMDP; 
    [SerializeField] EnemySpawner enemySpawner;

    [SerializeField] RectTransform content;
    [SerializeField] Sprite hideSprite;
    [SerializeField] Sprite unhideSprite;
    [SerializeField] Image hideButtonIcon;
    [SerializeField] float hideDistance = 340f;
    [SerializeField] float hideDuration = 0.75f;

    private float timer;
    private bool hidden;
    private Coroutine toggleHiddenCoroutine;

    private void Awake() {
        startMissionEvent.AddListener(UnHide);
    }

    private void UnHide(int arg0) {
        if (hidden) ToggleHide();
    }

    private void FixedUpdate() {
        UpdateDefendEarthDisplay(earthMDP);
        if (reverseEngineerMission.missionActive) UpdateReverseEngineerDisplay(reverseEngineerMDP);
        else reverseEngineerMDP.gameObject.SetActive(false);
        if (scanningMission.missionActive) UpdateScanningDisplay(scanningMDP);
        else scanningMDP.gameObject.SetActive(false);
        if (probeMission.missionActive) UpdateProbeDisplay(probeMDP);
        else probeMDP.gameObject.SetActive(false);
        if (prototypeLaser.numUpgrades > 0 && prototypeLaser.numUpgrades < 7) FillPrototypeDisplay(prototypeMDP);
        else prototypeMDP.gameObject.SetActive(false);
    }

    public void UpdateDefendEarthDisplay(MissionDisplayPanel mdp) {
        float maxPopulation = earthSimulation.MaxPopulationReached;
        float currentPopulation = earthSimulation.currentPopulation;
        mdp.UpdateObjectiveText(string.Format("Earth Population\n{0:n2} billion", currentPopulation / 1000f)); 
        mdp.UpdateSliderValue(currentPopulation / maxPopulation);
    }

    public void UpdateReverseEngineerDisplay(MissionDisplayPanel mdp) {
        mdp.gameObject.SetActive(true);
        int parts = reverseEngineerMission.currentPickups;
        int needed = reverseEngineerMission.pickupsRequired;
        string objectiveText = parts < needed ? string.Format("Parts Collected: {0}/{1}", parts, needed) : "Return to the Earth with the Alien Ship Parts";
        mdp.UpdateObjectiveText(objectiveText);
        mdp.UpdateSliderValue(parts / (float)needed);
    }

    public void UpdateScanningDisplay(MissionDisplayPanel mdp) {
        mdp.gameObject.SetActive(true);
        float remaining = scanningMission.timeRemaining;
        float duration = scanningMission.scanDuration;
        float percentComplete = (duration - remaining) / duration;
        mdp.UpdateObjectiveText(string.Format("Scanning Progress\n{0:n0}% Complete", percentComplete * 100f));
        mdp.UpdateSliderValue(percentComplete);
    }

    public void UpdateProbeDisplay(MissionDisplayPanel mdp) {
        mdp.gameObject.SetActive(true);
        float speed = probeMission.probeBody.velocity.magnitude;
        float health = probeMission.probeHealth.GetCurrentHp();
        float maxHealth = probeMission.probeHealth.maxHp;
        float percentHealth = health / maxHealth;
        mdp.UpdateObjectiveText(string.Format("Escort Probe away from Earth\nProbe Speed: {0:n1}km/s\nProbe Health: {1:n0}%", speed, percentHealth * 100f));
        mdp.UpdateSliderValue(percentHealth);
    }

    public void FillPrototypeDisplay(MissionDisplayPanel mdp) {
        mdp.gameObject.SetActive(true);
        int count = prototypeLaser.numUpgrades;
        int max = prototypeLaser.maxUpgrades;

        float finaleTime = enemySpawner.finaleTimer;
        float finaleDuration = enemySpawner.finaleDuration;

        int finaleRemainingMinutes =  (int)(finaleDuration - finaleTime) / 60;
        int finaleRemainingSeconds =  (int)(finaleDuration - finaleTime) % 60;

        if (enemySpawner.finale) {
            mdp.UpdateHeadingText("Survive the Alien Onslaught");
            if (finaleRemainingSeconds < 10) mdp.UpdateObjectiveText(string.Format("Survive\nTime remaining: {0}:0{1}", finaleRemainingMinutes, finaleRemainingSeconds));
            else mdp.UpdateObjectiveText(string.Format("Survive\nTime remaining: {0}:{1}", finaleRemainingMinutes, finaleRemainingSeconds));
            mdp.UpdateSliderValue(finaleTime/finaleDuration);
        }
        else {
            mdp.UpdateHeadingText("Construct the Prototype");
            mdp.UpdateObjectiveText(string.Format("Complete the Prototype\nParts Remaining: {0}", max - count));
            mdp.UpdateSliderValue(count / (float)max);
        }
    }

    public void ToggleHide() {
        if (timer > 0) return;
        if (toggleHiddenCoroutine != null) StopCoroutine(toggleHiddenCoroutine);
        StartCoroutine(ToggleHidden());
    }

    private IEnumerator ToggleHidden() {
        timer = hideDuration;
        Vector2 start = content.anchoredPosition;
        Vector2 end = content.anchoredPosition + new Vector2(hideDistance * (hidden ? 1 : -1), 0);
        float deflator = 0.5f / hideDuration;
        hidden = !hidden;

        if (hidden) GliderAudio.SFX.PlayRelativeToListener("resume_2", Vector3.zero);
        else GliderAudio.SFX.PlayRelativeToListener("resume_1", Vector3.zero);

        EventSystem.current.SetSelectedGameObject(null);

        while (timer > 0) {
            timer -= Time.deltaTime;
            content.anchoredPosition = Vector2.Lerp(start, end, Mathf.Cos(timer * deflator * Mathf.PI));
            yield return new WaitForEndOfFrame();
        }
        content.anchoredPosition = end;
        hideButtonIcon.sprite = hidden ? unhideSprite : hideSprite;
        timer = 0;
        
    }

}
