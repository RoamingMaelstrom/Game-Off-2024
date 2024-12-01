using SOEvents;
using TMPro;
using UnityEngine;

public class TechPanelLogic : MonoBehaviour
{
    [SerializeField] IntSOEvent levelUpEvent;
    [SerializeField] TechObjectDisplaySOEvent unlockTechEvent;
    [SerializeField] TechDisplayFormatter techDisplayFormatter;
    [SerializeField] TogglePause togglePause;
    [SerializeField] GameObject contentPanel;
    [SerializeField] GameObject mainScreenPanel;
    [SerializeField] GameObject mainMenuPopupPanel;
    [SerializeField] TechTreeHeaderPanel techTreeHeaderPanel;
    [SerializeField] PlayerInfoPanel playerInfoPanel;
    [SerializeField] TechUnlockTabLogic techUnlockTabLogic;
    [SerializeField] TextMeshProUGUI unlockPointsText;
    [SerializeField] TechTree shipTechTree;
    [SerializeField] TechTree orbitTechTree;
    [SerializeField] TechTree civicTechTree;
    [SerializeField] TechTree victoryTechTree;
    [SerializeField] SpriteMask earthSpriteMask;
    [SerializeField] int unlockPoints;
    [SerializeField] int level;    

    [SerializeField] TMP_Dropdown popupRuleDropdown;
    private TechPanelPopupRule popupRule = TechPanelPopupRule.EVERY_LEVEL;
    private bool autoClose = true;

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

    public int GetUnlockPointCount() => unlockPoints;

    public void SetPopupRule() => popupRule = (TechPanelPopupRule)popupRuleDropdown.value;
    public void ToggleAutoClose() => autoClose = !autoClose;

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
        togglePause.Toggle();
        playerInfoPanel.gameObject.SetActive(false);
        techTreeHeaderPanel.Hide();
        earthSpriteMask.enabled = false;
        mainMenuPopupPanel.SetActive(false);
    }

    public void ExitTechTreePanel() {
        shipTechTree.gameObject.SetActive(false);
        orbitTechTree.gameObject.SetActive(false);
        civicTechTree.gameObject.SetActive(false);
        victoryTechTree.gameObject.SetActive(false);
        contentPanel.SetActive(false);
        mainScreenPanel.SetActive(false);
        techUnlockTabLogic.ClosePanel();
        togglePause.Toggle();
        earthSpriteMask.enabled = true;
        mainMenuPopupPanel.SetActive(false);
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
        playerInfoPanel.gameObject.SetActive(true);
        playerInfoPanel.UpdateText(level, unlockPoints);

        techTreeHeaderPanel.Show(GetTreeName(techType), tree.UnlockCount, tree.NumTechs);
    }

    private string GetTreeName(TechType techType) {
        string treeName = techType switch
        {
            TechType.SHIP => "Ship",
            TechType.ORBIT => "Orbit",
            TechType.CIVIC => "Civic",
            _ => "Victory",
        };

        return treeName;
    }

    public void DeselectTechTree() {
        shipTechTree.gameObject.SetActive(false);
        orbitTechTree.gameObject.SetActive(false);
        civicTechTree.gameObject.SetActive(false);
        victoryTechTree.gameObject.SetActive(false);
        mainScreenPanel.SetActive(true);
        techUnlockTabLogic.ClosePanel();
        playerInfoPanel.gameObject.SetActive(false);
        techTreeHeaderPanel.Hide();
    }

    private void SetupTechTree(TechTree tree) {
        tree.gameObject.SetActive(true);
        tree.CalculateNumTechs();
        
        tree.gameObject.SetActive(false);
    }

    private void OnLevelUp(int newLevel) {
        level = newLevel;
        unlockPoints++;
        if (popupRule == TechPanelPopupRule.EVERY_LEVEL) EnterTechTreePanel();
        else if (popupRule == TechPanelPopupRule.EVERY_5_LEVELS && level % 5 == 0) EnterTechTreePanel();
    }


    private void UnlockTech(TechObjectDisplay tOD)
    {
        TechTree tree = GetTechTree(tOD.techObject.techType);
        if (tree == null) return;

        tree.UnlockTech(tOD);
        unlockPoints--;
        unlockPointsText.SetText("Unlock Points\n{0}", unlockPoints);

        if (unlockPoints == 0 && autoClose) ExitTechTreePanel();
        else {
            tree.Refresh(techDisplayFormatter, unlockPoints, level);
            playerInfoPanel.UpdateText(level, unlockPoints);
            techTreeHeaderPanel.Show(GetTreeName(tOD.techObject.techType), tree.UnlockCount, tree.NumTechs);
        }
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

public enum TechPanelPopupRule
{
    EVERY_LEVEL = 0,
    EVERY_5_LEVELS = 1,
    NO_POPUP = 2
}
