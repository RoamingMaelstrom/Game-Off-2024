using SOEvents;
using TMPro;
using UnityEngine;

public class TechPanelLogic : MonoBehaviour
{
    [SerializeField] IntSOEvent levelUpEvent;
    [SerializeField] TechObjectDisplaySOEvent unlockTechEvent;
    [SerializeField] TechDisplayFormatter techDisplayFormatter;
    [SerializeField] GameObject contentPanel;
    [SerializeField] GameObject mainScreenPanel;
    [SerializeField] TechUnlockTabLogic techUnlockTabLogic;
    [SerializeField] TextMeshProUGUI unlockPointsText;
    [SerializeField] TechTree shipTechTree;
    [SerializeField] TechTree orbitTechTree;
    [SerializeField] TechTree civicTechTree;
    [SerializeField] TechTree victoryTechTree;
    [SerializeField] int unlockPoints;
    [SerializeField] int level;    

    private void Awake() {
        levelUpEvent.AddListener(OnLevelUp);
        unlockTechEvent.AddListener(UnlockTech);
    }

    private void Start() {
        SetupTechTree(shipTechTree);
        SetupTechTree(orbitTechTree);
        SetupTechTree(civicTechTree);
        SetupTechTree(victoryTechTree);
    }

    public void ToggleTechPanel() {
        if (contentPanel.activeInHierarchy) ExitTechTreePanel();
        else EnterTechTreePanel();
    }

    public void EnterTechTreePanel() {
        contentPanel.SetActive(true);
        shipTechTree.gameObject.SetActive(false);
        orbitTechTree.gameObject.SetActive(false);
        civicTechTree.gameObject.SetActive(false);
        victoryTechTree.gameObject.SetActive(false);
        mainScreenPanel.SetActive(true);
        unlockPointsText.SetText("Unlock Points\n{0}", unlockPoints);
    }

    public void ExitTechTreePanel() {
        shipTechTree.gameObject.SetActive(false);
        orbitTechTree.gameObject.SetActive(false);
        civicTechTree.gameObject.SetActive(false);
        victoryTechTree.gameObject.SetActive(false);
        contentPanel.SetActive(false);
        mainScreenPanel.SetActive(false);
        techUnlockTabLogic.ClosePanel();
    }

    public void SelectTechTree(int techTypeID) {
        mainScreenPanel.SetActive(false);
        TechType techType = techTypeID switch
        {
            0 => TechType.SHIP,
            1 => TechType.ORBIT,
            2 => TechType.CIVIC,
            _ => TechType.VICTORY,
        };
        TechTree tree = GetTechTree(techType);
        tree.gameObject.SetActive(true);
        tree.Refresh(techDisplayFormatter, unlockPoints, level);
    }

    public void DeselectTechTree() {
        shipTechTree.gameObject.SetActive(false);
        orbitTechTree.gameObject.SetActive(false);
        civicTechTree.gameObject.SetActive(false);
        victoryTechTree.gameObject.SetActive(false);
        mainScreenPanel.SetActive(true);
        techUnlockTabLogic.ClosePanel();
    }

    private void SetupTechTree(TechTree tree) {
        tree.gameObject.SetActive(true);
        tree.CalculateNumTechs();
        
        tree.gameObject.SetActive(false);
    }

    private void OnLevelUp(int newLevel) {
        level = newLevel;
        unlockPoints++;
    }


    private void UnlockTech(TechObjectDisplay tOD)
    {
        TechTree tree = GetTechTree(tOD.techObject.techType);
        if (tree == null) return;

        tree.UnlockTech(tOD);
        unlockPoints--;
        tree.Refresh(techDisplayFormatter, unlockPoints, level);
        unlockPointsText.SetText("Unlock Points\n{0}", unlockPoints);
    }

    private TechTree GetTechTree(TechType techType) {
        return techType switch
        {
            TechType.SHIP => shipTechTree,
            TechType.ORBIT => orbitTechTree,
            TechType.CIVIC => civicTechTree,
            TechType.VICTORY => victoryTechTree,
            _ => null,
        };
    }
}
