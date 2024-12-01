using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DifficultySlider : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Difficulty difficultyObject;
    [SerializeField] TextMeshProUGUI difficultyText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] Color defaultColour;
    [SerializeField] Color easyColour;
    [SerializeField] Color hardColour;

    private void OnEnable() {
        slider.SetValueWithoutNotify(difficultyObject.selectedDifficulty);
        SetDifficultyText(difficultyObject.selectedDifficulty);
        SetScoreText(difficultyObject.selectedDifficulty);
    }

    public void OnValueChange() {
        difficultyObject.selectedDifficulty = (int)slider.value;
        SetDifficultyText(difficultyObject.selectedDifficulty);
        SetScoreText(difficultyObject.selectedDifficulty);
    }

    private void SetScoreText(int selectedDifficulty) {
        string content = "";
        //if (selectedDifficulty > 2) content += string.Format("<color=#{0}>", easyColour.ToHexString());
        ////else if (selectedDifficulty < 2) content += string.Format("<color=#{0}>", hardColour.ToHexString());
        //else content += string.Format("<color=#{0}>", defaultColour.ToHexString());

        content += string.Format("Score Modifier\n{0}%", difficultyObject.scoreMultipliers[selectedDifficulty] * 100f);
        scoreText.SetText(content);
    }

    public void SetDifficultyText(int difficulty){
        string content = "";
        
        if (difficulty < 2) content += string.Format("<color=#{0}>", easyColour.ToHexString());
        else if (difficulty > 2) content += string.Format("<color=#{0}>", hardColour.ToHexString());
        else content += string.Format("<color=#{0}>", defaultColour.ToHexString());


        foreach (var specifier in difficultyObject.specifiers)
        {
            string line = "";
            switch (specifier.type)
            {
                case DifficultySpecifierType.DAMAGE: line = "Alien Damage"; break;
                case DifficultySpecifierType.HEALTH: line = "Alien Health"; break;
                case DifficultySpecifierType.SPAWN_RATE: line = "Alien Spawn Rate"; break;
                case DifficultySpecifierType.SPEED: line = "Alien Speed"; break;
                case DifficultySpecifierType.XP: line = "XP Earnt"; break;
                default: break;
            }

            if (specifier.strength[difficulty] < 1) line += string.Format(" {0:n1}%\n", specifier.strength[difficulty] * 100f);
            else if (specifier.strength[difficulty] > 1) line += string.Format(" {0:n1}%\n", specifier.strength[difficulty] * 100f);
            else line += " 100.0%\n";

            content += line;
        }

        difficultyText.SetText(content);
    }

}
