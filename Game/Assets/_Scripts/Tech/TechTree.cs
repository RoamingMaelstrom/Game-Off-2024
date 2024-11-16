using SOEvents;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class TechTree : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI unlockCountText;
    [SerializeField] int unlockCount = 0;
    private int numTechs;
    public TechTreeBand[] bands;
    [SerializeField] TechType techTreeType;

    public void CalculateNumTechs() {
        numTechs = 0;
        foreach (var band in bands) {
            band.GetChildTechDisplays();
            numTechs += band.techDisplays.Length;
        }
    }

    public void UnlockTech(TechObjectDisplay tech) {
        Debug.Log(tech.techObject);
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

        unlockCountText.SetText(string.Format("Unlocked\n{0}/{1}", unlockCount, numTechs));
    }

    public void SetTODTooltips(TechDisplayFormatter techDisplayFormatter, TechTreeBand band) {
        foreach (var tech in band.techDisplays)
        {
            techDisplayFormatter.SetTooltip(tech);
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
            if (level < band.minUnlocks) techDisplay.techUnlockStatusEncoded += 2;
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
