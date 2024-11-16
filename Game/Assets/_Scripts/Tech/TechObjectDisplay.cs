using UnityEngine;
using UnityEngine.UI;
using SOEvents;
using UnityEditor;

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
            focusButton.interactable = false;
        }
        else if (techUnlockStatusEncoded == 0) {
            border.color = Color.white * 0.5f;
            icon.color = Color.white;
            focusButton.interactable = true;
        }
        else {
            border.color = Color.white * 0.25f;
            icon.color = Color.white * 0.25f;
            focusButton.interactable = false;
        }
    }

    public void SelectTech() => selectTechDisplayEvent.Invoke(this);

    
    private void OnDrawGizmosSelected() {
        DrawDependencyLines(this, 0);
    }

    private static void DrawDependencyLines(TechObjectDisplay tOD, int c=0) {
        if (c >= 20) return;
        if (EditorApplication.isPlaying) return;
        if (Application.platform == RuntimePlatform.WindowsEditor) {
            foreach (var dependent in tOD.dependentTechs)
            {
                Vector3 pA = tOD.border.rectTransform.position;
                pA.x -= 50;
                Vector3 pB = dependent.border.rectTransform.position;
                pB.x += 25;


                Debug.DrawLine(pB, pA, new Color(0.8f, 0.1f, 0.1f, 1));
                DrawDependencyLines(dependent, c++);
            }
        }
    }
}

public enum TechUnlockStatus
{
    NO_TECH_POINTS = 1,
    NOT_ENOUGH_UNLOCKS = 2,
    REQUIRES_PREVIOUS = 4,
    UNLOCKED = 64
}
