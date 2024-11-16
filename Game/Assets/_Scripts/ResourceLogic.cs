using UnityEngine;
using TMPro;
using System.Text;

public class ResourcesLogic : MonoBehaviour
{
    public Resource production;
    public Resource science;
    public Resource population;

    private void Start() {
        production.Setup(Time.fixedDeltaTime);
        science.Setup(Time.fixedDeltaTime);
        population.Setup(Time.fixedDeltaTime);
        UpdateModifiers();
    }

    private void FixedUpdate() {
        Tick();
        UpdateModifiers();
        UpdateDisplays();
    }

    private void UpdateModifiers() {
        population.modifier = Mathf.Clamp(population.value / 10000f, 0.5f, 2f);
        production.modifier = population.modifier;
        science.modifier = population.modifier;
    }

    private void Tick() {
        production.Tick(Time.fixedDeltaTime);
        science.Tick(Time.fixedDeltaTime);
        population.Tick(Time.fixedDeltaTime);
    }

    private void UpdateDisplays() {
        production.UpdateDisplay();
        science.UpdateDisplay();
        population.UpdateDisplay();
    }
}

[System.Serializable]
public class Resource {
    public string displayName;
    [TextArea(3, 8)]
    private StringBuilder tooltipBuilder;
    public Tooltip tooltip;
    public TextMeshProUGUI displayValueText;
    public float value;
    public float cap;
    public float modifier;
    public float growthValue;
    public float growthInterval;
    private float timer;

    public void Setup(float dt) {
        tooltipBuilder = new StringBuilder();
        Tick(dt);
        UpdateDisplay();
    }


    public void Tick(float dt) {
        timer += dt;
        if (timer > growthInterval) {
            timer -= growthInterval;
            value += growthValue * modifier;
            value = Mathf.Clamp(value, 0, cap);
        }
    }

    public void UpdateDisplay() {
        tooltipBuilder.Clear();
        tooltipBuilder.Append(string.Format("Current {0} - {1:n0} \nCapped at - {2:n0}\n", displayName, value, cap));
        tooltipBuilder.Append(string.Format("{0} grows by {1:n1} every {2:n2} seconds", displayName, growthValue, growthInterval));
        tooltip.content = tooltipBuilder.ToString();

        displayValueText.SetText(string.Format("{0:n0}", value));
    }
}
