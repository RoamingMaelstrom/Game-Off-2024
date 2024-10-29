using UnityEngine;
using SOEvents;

public class TogglePauseScreen : MonoBehaviour
{
    [SerializeField] BoolSOEvent pauseEvent;
    [SerializeField] SOEvent togglePauseScreenEvent;
    [SerializeField] GameObject pauseScreenContent;

    public bool active {get; private set;} = false;

    private void Awake() 
    {
        togglePauseScreenEvent.AddListener(ToggleContent);
    }

    private void ToggleContent()
    {
        if (active)
        {
            pauseScreenContent.SetActive(false);
            pauseEvent.Invoke(false);
        }
        else
        {
            pauseScreenContent.SetActive(true);
            pauseEvent.Invoke(true);
        }

        active = !active;
    }
}
