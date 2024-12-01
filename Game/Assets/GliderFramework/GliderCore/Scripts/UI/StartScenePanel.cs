using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartScenePanel : MonoBehaviour
{
    [SerializeField] List<GameObject> menuPanels = new();
    [SerializeField] StartSceneCameraLogic startSceneCameraLogic;


    private void Awake() {
        foreach (var panel in menuPanels)
        {
            panel.SetActive(false);
        }
    }

    public void OpenMenu(int index) {
        for (int i = 0; i < menuPanels.Count; i++) 
        {
            if (i == index) continue;
            menuPanels[i].SetActive(false);
        }
        bool newState = !menuPanels[index].activeInHierarchy;
        menuPanels[index].SetActive(newState);
        if (newState && index == 1) startSceneCameraLogic.FocusPlayer();
        else startSceneCameraLogic.FocusEarth();

        

        if (!newState) EventSystem.current.SetSelectedGameObject(null);
    }
}
