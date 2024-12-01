using UnityEngine;
using UnityEngine.UI;

public class TutorialPanelLogic : MonoBehaviour
{
    [SerializeField] Button nextButton;
    [SerializeField] Button previousButton;
    [SerializeField] GameObject[] panels;
    [SerializeField] PlayerFiring playerFiring;
    [SerializeField] Weapon chaingun;

    [SerializeField] Button tech1Button;
    [SerializeField] Image tech1Icon;
    [SerializeField] Image tech1Border;
    [SerializeField] Tooltip tech1Tooltip;

    [SerializeField] Button tech2Button;
    [SerializeField] Image tech2Icon;
    [SerializeField] Image tech2Border;
    [SerializeField] Tooltip tech2Tooltip;
    [SerializeField] GameObject exampleTechContainer;

    public int index;
    private bool addedWeapon = false;
    private bool addedUpgrade = false;

    private void OnEnable() {
        index = 0;
        foreach (var panel in panels)
        {
            panel.SetActive(false);
        }

        panels[0].SetActive(true);
        previousButton.interactable = false;
        nextButton.interactable = true;
        ResetUnlocks();
        exampleTechContainer.SetActive(true);
    }

    public void Next() {
        panels[index].SetActive(false);
        index++;
        panels[index].SetActive(true);
        if (index >= panels.Length - 1) nextButton.interactable = false;
        previousButton.interactable = true;
    }

    public void Previous() {
        panels[index].SetActive(false);
        index--;
        panels[index].SetActive(true);
        if (index <= 0) previousButton.interactable = false;
        nextButton.interactable = true;
    }

    public void ResetUnlocks() {
        if (addedWeapon) playerFiring.RemoveWeaponAt(1);
        if (addedUpgrade) chaingun.fireRate -= 4.5f;
        SetTech1GreyedOut();
        SetTech2GreyedOut();
    }

    public void SetTech1GreyedOut() {
        tech1Border.color = Color.white * 0.66f;
        tech1Icon.color = Color.white;
        tech1Button.interactable = true;
        SetTech1TooltipUnlockable();
    }
    public void SetTech2GreyedOut() {
        tech2Border.color = Color.white * 0.33f;
        tech2Icon.color = Color.white * 0.5f;
        tech2Button.interactable = false;
        SetTech2TooltipCannotUnlock();
    }

    public void UnlockTech1() {
        playerFiring.AddWeapon(chaingun);
        tech1Border.color = Color.white;
        tech1Icon.color = Color.white;
        tech2Border.color = Color.white * 0.66f;
        tech2Icon.color = Color.white;
        tech2Button.interactable = true;
        tech1Button.interactable = false;
        SetTech1TooltipUnlocked();
        SetTech2TooltipUnlockable();
        addedWeapon = true;
    }

    public void UnlockTech2() {
        tech2Border.color = Color.white;
        tech2Icon.color = Color.white;
        chaingun.fireRate += 4.5f;
        tech2Button.interactable = false;
        SetTech2TooltipUnlocked();
        addedUpgrade = true;
        exampleTechContainer.SetActive(false);
    }

    private void SetTech1TooltipUnlockable() {
        string content = @"<b><align=center><size=+8>Chain Gun<size=100%></align></b>

<align=center>Add rapid-fire Chain Gun to Ship</align>
<align=center><size=-4>
<color=#2980B9FF>Adds Chaingun to Ship<color=#FFFFFFFF>

<size=+8>Click to Unlock<size=100%></align>";

        tech1Tooltip.content = content;
    }

    private void SetTech1TooltipUnlocked() {
        string content = @"<b><align=center><size=+8>Chain Gun<size=100%></align></b>

<align=center>Add rapid-fire Chain Gun to Ship</align>
<align=center><size=-4>
<color=#2980B9FF>Adds Chaingun to Ship<color=#FFFFFFFF>

<size=+8>Already Unlocked<size=100%></align>";

        tech1Tooltip.content = content;
    }

    private void SetTech2TooltipCannotUnlock() {
        string content = @"<size=+8><align=center><b>Laser Munitions</b><color=#C0392BFF>

Cannot Unlock

<color=#FFFFFFFF><b><color=#C0392BFF>Requires
<color=#FFFFFFFF></b><i><color=#C0392BFF><size=-2>Chain Gun<size=100%><color=#FFFFFFFF></i>
</align><size=100%>";

        tech2Tooltip.content = content;
    }

    private void SetTech2TooltipUnlockable() {
        string content = @"<b><align=center><size=+8>Increase Caliber<size=100%></align></b>

<align=center>Increase fire rate of Chain Gun</align>
<align=center><size=-4>
<color=#27AE60FF>+<b>50</b>% Weapon Fire Rate <color=#FFFFFFFF>
<size=+8>
Click to Unlock<size=100%></align>";

        tech2Tooltip.content = content;
    }

    private void SetTech2TooltipUnlocked() {
        string content = @"<b><align=center><size=+8>Increase Caliber<size=100%></align></b>

<align=center>Increase fire rate of Chain Gun</align>
<align=center><size=-4>
<color=#27AE60FF>+<b>50</b>% Weapon Fire Rate <color=#FFFFFFFF>
<size=+8>
Already Unlocked<size=100%></align>";

        tech2Tooltip.content = content;
    }
}
