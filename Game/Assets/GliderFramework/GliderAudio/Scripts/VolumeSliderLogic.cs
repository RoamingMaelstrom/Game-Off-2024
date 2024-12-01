using UnityEngine;
using SOEvents;
using UnityEngine.UI;
using TMPro;
using GliderSave;

public class VolumeSliderLogic : MonoBehaviour
{
    [SerializeField] FloatSaveObject localSavedValue;
    [SerializeField] FloatSOEvent changeVolumeEvent;
    [SerializeField] Slider volumeSlider;
    [SerializeField] TextMeshProUGUI volumeTypeText;
    [SerializeField] Color defaultColour;
    [SerializeField] Color mutedColour;
    [SerializeField] string defaultText;
    [SerializeField] string mutedText;

    [SerializeField] bool isMuted = false;

    private void Awake() {
        volumeSlider.SetValueWithoutNotify(localSavedValue.GetValue());
    }

    private void Start() 
    {
        if (isMuted) SetMutedText();
        else SetDefaultText();
    }

    private void SetDefaultText() {
        volumeTypeText.SetText(defaultText);
        volumeTypeText.color = defaultColour;
    }

    private void SetMutedText() {
        volumeTypeText.SetText(mutedText);
        volumeTypeText.color = mutedColour;
    }


    public void OnValueChange()
    {
        changeVolumeEvent.Invoke(volumeSlider.value);

        if (isMuted && volumeSlider.value > 0)
        {
            SetDefaultText();
            isMuted = false;
            return;
        }

        if (volumeSlider.value <= 0 && !isMuted) 
        {
            SetMutedText();
            isMuted = true;
            return;
        }
    }
}
