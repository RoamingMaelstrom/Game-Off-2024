using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TechTreeHeaderPanel : MonoBehaviour
{
    [SerializeField] GameObject headerPanel;
    [SerializeField] TextMeshProUGUI treeTitleText;
    [SerializeField] TextMeshProUGUI unlockCountText;


    public void Show(string treeTitle, int unlockCount, int numTechs) {
        headerPanel.SetActive(true);
        treeTitleText.SetText(treeTitle);
        unlockCountText.SetText(string.Format("Unlocked\n{0}/{1}", unlockCount, numTechs));
    }

    public void Hide() => headerPanel.SetActive(false);
}
