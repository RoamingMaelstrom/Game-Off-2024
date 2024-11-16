using SOEvents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TechUnlockTabLogic : MonoBehaviour
{
    [SerializeField] TechObjectDisplaySOEvent selectTechDisplayEvent;
    [SerializeField] TechObjectDisplaySOEvent unlockTechEvent;
    [SerializeField] TechDisplayFormatter techDisplayFormatter;
    [SerializeField] GameObject techUnlockTabPanel;
    [SerializeField] TextMeshProUGUI techNameText;
    [SerializeField] TextMeshProUGUI techDescriptionText;
    [SerializeField] TextMeshProUGUI techEffectText;
    [SerializeField] Button unlockButton;

    private TechObjectDisplay selectedTOD;

    private void Awake() {
        selectTechDisplayEvent.AddListener(SetupTechPanel);
        techUnlockTabPanel.SetActive(false);
    }

    public void SetupTechPanel(TechObjectDisplay tOD) {
        techUnlockTabPanel.SetActive(true);
        techNameText.SetText(techDisplayFormatter.GetNameText(tOD));
        techDescriptionText.SetText(techDisplayFormatter.GetDescriptionText(tOD));
        techEffectText.SetText(techDisplayFormatter.GetEffectText(tOD, false));
        selectedTOD = tOD;
        unlockButton.enabled = (tOD.techUnlockStatusEncoded & 1) == 0;
    }

    public void UnlockTech() {
        unlockTechEvent.Invoke(selectedTOD);
        ClosePanel();
    }

    public void ClosePanel() {
        selectedTOD = null;
        techUnlockTabPanel.SetActive(false);
    }


}
