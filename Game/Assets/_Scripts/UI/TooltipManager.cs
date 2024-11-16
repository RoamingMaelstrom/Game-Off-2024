using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    [SerializeField] MouseInfo mouseInfo;
    [SerializeField] Image textBox;
    [SerializeField] TextMeshProUGUI textBoxText;
    [SerializeField] float displayDelay = 0.06f;
    private float displayDelayTimer;

    private void HideBox() {
        textBox.color = Color.clear;
        textBoxText.color = Color.clear;
    }

    private void UnHideBox() {
        textBox.color = new Color(0, 0, 0, 0.9f);
        textBoxText.color = Color.white;
    }

    private void LateUpdate() {
        if (!mouseInfo.MouseOverUI) {
            textBox.gameObject.SetActive(false);
            displayDelayTimer = 0;
            // Todo: Store previously displayed tooltip object so that bug does not occur when immediately switching from one tooltip to another.
            return;
        }

        List<RaycastResult> uiElements = RaycastMouse();
        if (uiElements.Count == 0) {
            textBox.gameObject.SetActive(false);
            displayDelayTimer = 0;
            return;
        }

        RaycastResult result = uiElements[0];

        if (result.gameObject.TryGetComponent(out Tooltip tooltip)) {
            displayDelayTimer += Time.deltaTime;
            textBox.gameObject.SetActive(true);
            if (displayDelayTimer < displayDelay) HideBox();
            else UnHideBox();
            
            Vector2 textBoxSize = new Vector2(tooltip.suggestedWidth, tooltip.suggestedHeight);
            if (textBoxSize.x < 50) textBoxSize.x = 250;
            if (textBoxSize.y < 50) textBoxSize.y = 100;
            
            textBoxText.rectTransform.sizeDelta = textBoxSize;
            textBoxText.SetText(tooltip.content);
            textBox.rectTransform.position = result.gameObject.GetComponent<RectTransform>().position;
            textBox.rectTransform.sizeDelta = textBoxText.textBounds.extents * 2.25f;
            textBox.rectTransform.sizeDelta += new Vector2(16, 16);

            Vector3 offset = textBoxText.textBounds.size * 0.5f;
            offset.x += 75f;
            offset.y += result.gameObject.GetComponent<RectTransform>().rect.height;
            float xMult = (textBox.rectTransform.position.x - (Screen.width / 2f)) / Screen.width;
            float yMult = (textBox.rectTransform.position.y - (Screen.height / 2f)) / Screen.height;
            xMult = Mathf.Abs(xMult) >= Mathf.Abs(yMult) ? Mathf.Sign(xMult) : xMult;
            yMult = Mathf.Abs(yMult) > Mathf.Abs(xMult) ? Mathf.Sign(yMult) : yMult;
            textBox.transform.position -= new Vector3(offset.x * xMult, offset.y * yMult, 0);
            
            return;
        }
        else textBox.gameObject.SetActive(false);  
    }

    public List<RaycastResult> RaycastMouse(){
            
            PointerEventData pointerData = new(EventSystem.current)
            {
                pointerId = -1,
                position = mouseInfo.MousePosScreen
            };
            
            List<RaycastResult> output = new();
            EventSystem.current.RaycastAll(pointerData, output);
            return output;
        }

}
