using UnityEngine;


[CreateAssetMenu(fileName = "Difficulty", menuName = "Jam/Difficulty", order = 0)]
public class Difficulty : ScriptableObject 
{
    public int selectedDifficulty = 2;
    public string[] difficultyNames = new string[5]; 
    public float[] scoreMultipliers = new float[5];
    public DifficultySpecifier[] specifiers;

    public float GetDifficultyStrengthForType(DifficultySpecifierType type) {
        foreach (var specifier in specifiers)
        {
            if (specifier.type == type) return specifier.strength[selectedDifficulty];
        }

        return 1;
    }

    public float GetDifficultyScoreModifier() => scoreMultipliers[selectedDifficulty];

    // Order
    // Spawn Rate
    // Health
    // Damage
    // Speed
    // XP
}

[System.Serializable]
public class DifficultySpecifier
{
    public DifficultySpecifierType type;
    public float[] strength = new float[5];
}


public enum DifficultySpecifierType
{
    SPAWN_RATE,
    HEALTH,
    DAMAGE,
    SPEED,
    XP
}

