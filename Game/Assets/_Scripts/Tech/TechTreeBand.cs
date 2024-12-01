using UnityEngine;

public class TechTreeBand : MonoBehaviour
{
    public int minLevel;
    public TechObjectDisplay[] techDisplays;
    public bool isSetup = false;

    private void Awake() {
        GetChildTechDisplays();
    }

    public void GetChildTechDisplays() {
        if (isSetup) return;
        techDisplays = GetComponentsInChildren<TechObjectDisplay>();
        isSetup = true;
    }
}
