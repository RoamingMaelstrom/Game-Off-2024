using Unity.VisualScripting;
using UnityEngine;

public class TechDisplayFormatter : MonoBehaviour
{
    [SerializeField] Color baseColour;
    [SerializeField] Color gold;
    [SerializeField] Color red;
    [SerializeField] Color green;
    [SerializeField] Color blue;

    private string cachedBaseColour;
    private string cachedBlueColour;
    private string cachedGoldColour;
    private string cachedGreenColour;
    private string cachedRedColour;

    private void Start() {
        cachedBaseColour = string.Format("<color=#{0}>", baseColour.ToHexString());
        cachedBlueColour = string.Format("<color=#{0}>", blue.ToHexString());
        cachedGoldColour = string.Format("<color=#{0}>", gold.ToHexString());
        cachedGreenColour = string.Format("<color=#{0}>", green.ToHexString());
        cachedRedColour = string.Format("<color=#{0}>", red.ToHexString());
    }

    public void SetTooltip(TechObjectDisplay tOD) {
        if ((tOD.techUnlockStatusEncoded & 6) != 0) {
            SetToolTipCannotUnlock(tOD);
            return;
        }

        SetTooltipAvailable(tOD);
    }

    private void SetTooltipAvailable(TechObjectDisplay tOD)
    {
        string content = GetNameText(tOD);
        content += GetDescriptionText(tOD);
        content += GetEffectText(tOD);
        
        tOD.tooltip.content = content;
    }

    public string GetNameText(TechObjectDisplay tOD) {
        string content = Bold(Align(UpSize(tOD.techObject.techName, 8), "center"));
        content += "\n";
        return content;
    }

    public string GetDescriptionText(TechObjectDisplay tOD) {
        string content = Align(tOD.techObject.description, "center");
        content += "\n";
        return content;
    }

    public string GetEffectText(TechObjectDisplay tOD, bool indent = true) {
        string content = "\n";
        foreach (var effect in tOD.techObject.effects)
        {
            content = "\n";
            switch (effect.effectType)
            {
                case EffectType.BOOST_THRUST: content += string.Format("+{0:n0}% Boost Thrust", effect.value); break;
                case EffectType.NORMAL_THRUST: content += string.Format("+{0:n0}% Base Thrust", effect.value); break;
                case EffectType.ROTATION_SPEED: content += string.Format("+{0:n0}% Rotation Speed", effect.value); break;
                case EffectType.WARP_JUMP_DISTANCE: content += string.Format("+{0:n0}% Tunnel Boost Distance", effect.value); break;

                case EffectType.ACCURACY: content += string.Format("Improved Weapon Accuracy"); break;
                case EffectType.BURST_FIRE: content += string.Format("Fires in Burst of {0:n0}", effect.value); break;
                case EffectType.DAMAGE: content += string.Format("+{0:n0}% Weapon Damage", effect.value); break;
                case EffectType.DROP_OFF: content += string.Format("-{0:n0}% Laser Damage Dropoff", effect.value); break;
                case EffectType.EXPLOSION_SIZE: content += string.Format("+{0:n0}% Explosion Radius", effect.value); break;
                case EffectType.FIRE_RATE: content += string.Format("+{0:n0}% Fire Rate", effect.value); break;
                case EffectType.MUNITION_COUNT: content += string.Format("Fires {0:n0} Projectiles", effect.value); break;
                case EffectType.MUNITION_SIZE: content += string.Format("+{0:n0}% Projectile Size", effect.value); break;
                case EffectType.MUNITION_SPEED: content += string.Format("+{0:n0}% Projectile Speed", effect.value); break;
                case EffectType.PIERCING: content += string.Format("+{0:n0} Projectile Pierce", effect.value); break;
                case EffectType.PIERCE_PROBABILITY: content += string.Format("+{0:n0}% Probability Pierce", effect.value); break;
                case EffectType._ALL_WEAPONS_: content += "All Weapons Affected"; break;

                case EffectType.ALIEN_SPAWN_DISTANCE: content += string.Format("+{0:n0}% Alien Spawn Distance", effect.value); break;
                case EffectType.MAP_SIZE: content += string.Format("+{0:n0}% Map Size", effect.value); break;
                case EffectType.MINIMAP_ZOOM: content += string.Format("Minimap Zooms Out +{0:n0}%", effect.value); break;
                case EffectType.SATELLITE_COUNT: content += string.Format("+{0:n0} Satellite Count", effect.value); break;
                case EffectType.SATELLITE_RANGE: content += string.Format("+{0:n0}% Satellite Range", effect.value); break;
                case EffectType._ALL_SATELLITES_: content += "All Satellites Affected"; break;

                case EffectType.POPULATION_GROWTH_TICK_VALUE: content += string.Format("+{0:n0}% Passive Population Growth", effect.value); break;
                case EffectType.POPULATION_GROWTH_TICK_RATE: content += string.Format("-{0:n0}% Time Between Population Ticks", effect.value); break;
                case EffectType.POPULATION_GROWTH_RANDOM_PROB: content += string.Format("+{0:n0}% Chance of Population Boost", effect.value); break;
                case EffectType.POPULATION_GROWTH_RANDOM_VALUE: content += string.Format("+{0:n0}% Random Population Boost Size", effect.value); break;
                case EffectType.POPULATION_XP_GAIN_MODIFIER: content += string.Format("+{0:n0}% XP Bonus from Population Size", effect.value); break;
                case EffectType.XP_PASSIVE_GAIN: content += string.Format("+{0:n0}% Passive XP", effect.value); break;
                case EffectType.XP_KILL_GAIN: content += string.Format("+{0:n0}% XP from Alien Kill", effect.value); break;
                case EffectType.DAMAGE_REDUCTION: content += string.Format("-{0:n0}% Population Loss from Alien Attacks", effect.value); break;
                case EffectType.RETALIATION_COOLDOWN: content += string.Format("-{0:n0}% Retaliation Cooldown", effect.value); break;
                case EffectType.RETALIATION_TARGETING: content += string.Format("Smart Retaliation Targeting"); break;
                case EffectType.RETALIATION_NUKES: content += string.Format("Retaliation fires Nukes"); break;

                case EffectType.UNLOCK_BOOST: content += Blue(string.Format("Unlocks Boost Ability")); break;
                case EffectType.UNLOCK_TUNNEL_WARP: content += Blue(string.Format("Unlocks Tunnel Warp Ability")); break;
                case EffectType.UNLOCK_EARTH_MISSILES: content += Blue(string.Format("Unlocks Extra-Terrestrial Missiles")); break;
                case EffectType.UNLOCK_ICONS: content += Blue(string.Format("Different Alien Ships Have Their Own Icons")); break;
                case EffectType.UNLOCK_RETALIATION: content += Blue(string.Format("Unlocks Retaliatory Strike")); break;
                case EffectType.UNLOCK_SATELLITE_1: content += Blue(string.Format("Adds Four Basic Satellites to Earths Orbit")); break;
                case EffectType.UNLOCK_SATELLITE_2: content += Blue(string.Format("Adds Four High-Powered Cannons to Earths Orbit")); break;
                case EffectType.UNLOCK_SATELLITE_3: content += Blue(string.Format("Adds Four Laserbeam Satellites to Earths Orbit")); break;
                case EffectType.UNLOCK_SPACE_STATION: content += Blue(string.Format("Adds Space Station to Earths Orbit")); break;
                case EffectType.UNLOCK_WEAPON_2: content += Blue(string.Format("Adds Chaingun to Ship")); break;
                case EffectType.UNLOCK_WEAPON_3: content += Blue(string.Format("Adds Laser Turret to Ship")); break;
                case EffectType.UNLOCK_WEAPON_4: content += Blue(string.Format("Add Rocket Pods to Ship")); break;

                case EffectType.__START_MISSION_: content += UpSize(Gold(string.Format("Start Mission")), 8); break;
                default: break;
            }
        }

        content = DownSize(content, 4);
        if (indent) content = Indent(content, 15);
        return content;
    }

    private void SetToolTipCannotUnlock(TechObjectDisplay tOD) {
        string content = Bold(tOD.techObject.techName);
        content += Red("\n\nCannot Unlock\n\n");
        if ((tOD.techUnlockStatusEncoded & 2) != 0) content += Bold(Red("Level too Low\n"));
        if ((tOD.techUnlockStatusEncoded & 4) != 0) {
            content += Bold(Red("Requires\n"));
            foreach (var dependent in tOD.dependentTechs)
            {
                content += Italic(Red(DownSize(dependent.techObject.techName, 2)));
                content += "\n";
            }
        }

        tOD.tooltip.content = UpSize(Align(content, "center"), 8);
    }

    private static string Bold(string content) => "<b>" + content + "</b>";
    private static string UpSize(string content, int value) => string.Format("<size=+{0}>", value) + content + "<size=100%>";
    private static string DownSize(string content, int value) => string.Format("<size=-{0}>", value) + content + "<size=100%>";
    private static string Indent(string content, int percent) => string.Format("<indent={0}%>", percent) + content + "</indent>";
    private static string Align(string content, string alignment) => string.Format("<align={0}>", alignment) + content + "</align>";
    private static string Italic(string content) => "<i>" + content + "</i>";

    private string Blue(string content) => cachedBlueColour + content + cachedBaseColour;
    private string Gold(string content) => cachedGoldColour + content + cachedBaseColour;
    private string Green(string content) => cachedGreenColour + content + cachedBaseColour;
    private string Red(string content) => cachedRedColour + content + cachedBaseColour;

    // Todo: Add TechUnlockTabLogic Formatting.
}
