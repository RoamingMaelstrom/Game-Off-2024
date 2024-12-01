using UnityEngine;
using UnityEngine.UI;
using SOEvents;

[ExecuteInEditMode]
public class TechObjectDisplay : MonoBehaviour
{
    [SerializeField] TechObjectDisplaySOEvent selectTechDisplayEvent;
    public Image border;
    public Image icon;
    public Button focusButton;
    public TechObject techObject;
    public TechObjectDisplay[] dependentTechs;
    public Tooltip tooltip;
    public int techUnlockStatusEncoded;

    public void UpdateUIElements() {
        if ((techUnlockStatusEncoded & 64) == 64) {
            border.color = Color.white;
            icon.color = Color.white;
            focusButton.interactable = true;
        }
        else if (techUnlockStatusEncoded <= 1) {
            border.color = Color.white * 0.5f;
            icon.color = Color.white;
            focusButton.interactable = true;
        }
        else {
            border.color = Color.white * 0.33f;
            icon.color = Color.white * 0.33f;
            focusButton.interactable = false;
        }
    }

    public void SelectTech() => selectTechDisplayEvent.Invoke(this);
}

public enum TechUnlockStatus
{
    NO_TECH_POINTS = 1,
    NOT_ENOUGH_UNLOCKS = 2,
    REQUIRES_PREVIOUS = 4,
    UNLOCKED = 64
}
