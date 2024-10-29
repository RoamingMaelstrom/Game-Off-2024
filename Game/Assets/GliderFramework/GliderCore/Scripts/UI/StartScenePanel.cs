using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartScenePanel : MonoBehaviour
{
    [SerializeField] List<GameObject> menuPanels = new();


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

        if (!newState) EventSystem.current.SetSelectedGameObject(null);
    }
}
