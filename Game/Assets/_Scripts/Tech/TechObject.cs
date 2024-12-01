using UnityEngine;

[CreateAssetMenu(menuName = "Jam/TechObject", fileName = "Tech Object", order = 0)]
public class TechObject : ScriptableObject
{
    public string techName;
    [TextArea(5, 4)] public string description;
    public TechEffect[] effects;
    public TechType techType;
    public TechUpgradeHandler[] techUpgradeHandlers;

    public string GetEffectText() => "Placeholder\nEffect Text\n(list)"; 
    public bool ContainsHandler(TechUpgradeHandler otherHandler) {
        foreach (var handler in techUpgradeHandlers)
        {
            if (otherHandler == handler) return true;
        }
        return false;
    }
}

public enum TechType
{
    SHIP,
    ORBIT,
    CIVIC,
    VICTORY
}

public enum TechUpgradeHandler
{
    ___NONE___ = 0,

    ___SHIP___ = 100,//
    PLAYER_CONTROLLER = 101,//
    PLAYER_WEAPON_MANAGER = 102,//
    WEAPON_1 = 103,//
    WEAPON_2 = 104,//
    WEAPON_3 = 105,//
    WEAPON_4 = 106,//

    ___ORBIT___ = 200,
    CAMERA_LOGIC = 201,//
    ENEMY_MANAGER = 202,//
    SATELLITE_1 = 203,//
    SATELLITE_2 = 204,//
    SATELLITE_3 = 205,//
    SATELLITE_MANAGER = 206,//
    SPACE_STATION = 207,

    ___CIVIC___ = 300,
    EARTH_DEFENCE = 301,//
    EARTH_SIMULATION = 302,//
    PLAYER_LEVEL = 303,//

    ___VICTORY___ = 500
}
