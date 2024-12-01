using UnityEngine;

public class TechTree : MonoBehaviour
{
    [SerializeField] int unlockCount = 0;
    private int numTechs;
    public TechTreeBand[] bands;
    [SerializeField] TechType techTreeType;
    [SerializeField] TechLevelLines techLevelLines;

    public void CalculateNumTechs() {
        numTechs = 0;
        foreach (var band in bands) {
            band.GetChildTechDisplays();
            numTechs += band.techDisplays.Length;
        }
    }

    public int UnlockCount {get => unlockCount; private set {}}
    public int NumTechs {get => numTechs; private set {}}

    public void UnlockTech(TechObjectDisplay tech) {
        if (tech.techObject.techType != techTreeType) return;
        tech.techUnlockStatusEncoded = 64;
        unlockCount ++;
    }

    public void Refresh(TechDisplayFormatter techDisplayFormatter, int unlockPoints, int level) {
        foreach (var band in bands)
        {
            ZeroTechStatesIfNotUnlocked(band);
            SetZeroedInteractableStates(band, unlockPoints, level);
            UpdateTechDisplays(band);
            SetTODTooltips(techDisplayFormatter, band);
        }

        techLevelLines.SetLineColours(level);
    }

    public void SetTODTooltips(TechDisplayFormatter techDisplayFormatter, TechTreeBand band) {
        foreach (var tech in band.techDisplays)
        {
            techDisplayFormatter.SetTooltip(tech, band);
        }
    }

    private void UpdateTechDisplays(TechTreeBand band) {
        foreach (TechObjectDisplay techDisplay in band.techDisplays)
        {
            techDisplay.UpdateUIElements();
        }
    }

    private void SetZeroedInteractableStates(TechTreeBand band, int unlockPoints, int level)
    {
        foreach (TechObjectDisplay techDisplay in band.techDisplays)
        {
            if (techDisplay.techUnlockStatusEncoded > 0) continue;
            if (unlockPoints <= 0) techDisplay.techUnlockStatusEncoded += 1;
            if (level < band.minLevel) techDisplay.techUnlockStatusEncoded += 2;
            foreach (var tech in techDisplay.dependentTechs)
            {
                if ((tech.techUnlockStatusEncoded & 64) == 0) {
                    techDisplay.techUnlockStatusEncoded += 4;
                    break;
                }
            }
        }

    }

    private void ZeroTechStatesIfNotUnlocked(TechTreeBand band) {
        foreach (TechObjectDisplay techDisplay in band.techDisplays) techDisplay.techUnlockStatusEncoded &= 64;
    }
}
