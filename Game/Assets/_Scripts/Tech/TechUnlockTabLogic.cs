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
    [SerializeField] TextMeshProUGUI unlockButtonText;

    private TechObjectDisplay selectedTOD;

    private void Awake() {
        selectTechDisplayEvent.AddListener(SetupTechPanel);
        techUnlockTabPanel.SetActive(false);
    }

    public void SetupTechPanel(TechObjectDisplay tOD) {
        techUnlockTabPanel.SetActive(true);
        techNameText.SetText(techDisplayFormatter.GetNameText(tOD));
        if (techNameText.text.Length < 20) techNameText.SetText(TechDisplayFormatter.UpSize(techNameText.text, 6));

        techDescriptionText.SetText(techDisplayFormatter.GetDescriptionText(tOD));
        if (techDescriptionText.text.Length < 250) techDescriptionText.SetText(TechDisplayFormatter.UpSize(techDescriptionText.text, 6));

        techEffectText.SetText(techDisplayFormatter.GetEffectText(tOD, false));
        if (techEffectText.text.Length < 50) techEffectText.SetText(TechDisplayFormatter.UpSize(techEffectText.text, 6));

        selectedTOD = tOD;
        unlockButton.interactable = (tOD.techUnlockStatusEncoded & 255) == 0;
        if ((tOD.techUnlockStatusEncoded & 64) != 0) unlockButtonText.SetText(TechDisplayFormatter.DownSize("Already Unlocked", 8));
        else if ((tOD.techUnlockStatusEncoded & 1) != 0) unlockButtonText.SetText(TechDisplayFormatter.DownSize("No Unlock Points", 8));
        else unlockButtonText.SetText("Unlock");
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
