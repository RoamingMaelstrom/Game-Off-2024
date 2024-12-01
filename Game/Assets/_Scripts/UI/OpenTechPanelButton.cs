using UnityEngine;
using UnityEngine.UI;

public class OpenTechPanelButton : MonoBehaviour
{
    [SerializeField] TechPanelLogic techPanelLogic;
    [SerializeField] Button button;

    private void FixedUpdate() {
        if (techPanelLogic.GetUnlockPointCount() <= 0) button.interactable = false;
        else button.interactable = true;
    }
}
