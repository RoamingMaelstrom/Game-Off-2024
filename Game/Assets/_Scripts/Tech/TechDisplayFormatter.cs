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

    public void SetTooltip(TechObjectDisplay tOD, TechTreeBand band) {
        if ((tOD.techUnlockStatusEncoded & 6) != 0) {
            SetToolTipCannotUnlock(tOD, band.minLevel);
            return;
        }

        SetTooltipAvailable(tOD);
    }

    private void SetTooltipAvailable(TechObjectDisplay tOD)
    {
        string content = GetNameText(tOD);
        content += "\n";
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

    public string GetEffectText(TechObjectDisplay tOD, bool centre = true) {
        string content = "";
        foreach (var effect in tOD.techObject.effects)
        {
            content += "\n";

            string valueText = effect.value.ToString("n0");
            valueText = Bold(valueText);

            switch (effect.effectType)
            {
                case EffectType.BOOST_THRUST: content += Green(string.Format("+{0:n0}% Boost Thrust", effect.value)); break;
                case EffectType.NORMAL_THRUST: content += Green(string.Format("+{0:n0}% Base Thrust", effect.value)); break;
                case EffectType.ROTATION_SPEED: content += Green(string.Format("+{0:n0}% Rotation Speed", effect.value)); break;
                case EffectType.WARP_JUMP_DISTANCE: content += Green(string.Format("+{0:n0}% Tunnel Boost Distance", effect.value)); break;

                case EffectType.ACCURACY: content += Green(string.Format("Improved Weapon Accuracy")); break;
                case EffectType.BURST_FIRE: content += Green(string.Format("Fires in Burst of {0:n0}", valueText)); break;
                case EffectType.DAMAGE: content += Green(string.Format("+{0:n0}% Weapon Damage", valueText)); break;
                case EffectType.DROP_OFF: content += Green(string.Format("-{0:n0}% Laser Damage Dropoff", valueText)); break;
                case EffectType.EXPLOSION_SIZE: content += Green(string.Format("+{0:n0}% Explosion Radius", valueText)); break;
                case EffectType.FIRE_RATE: content += Green(string.Format("+{0:n0}% Fire Rate", valueText)); break;
                case EffectType.MUNITION_COUNT: content += Green(string.Format("Fires {0:n0} Projectiles", valueText)); break;
                case EffectType.MUNITION_SIZE: content += Green(string.Format("+{0:n0}% Projectile Size", effect.value)); break;
                case EffectType.MUNITION_SPEED: content += Green(string.Format("+{0:n0}% Projectile Speed", effect.value)); break;
                case EffectType.PIERCING: content += Green(string.Format("+{0:n0} Projectile Pierce", effect.value)); break;
                case EffectType.PIERCE_PROBABILITY: content += Green(string.Format("+{0:n0}% Probability Pierce", effect.value)); break;
                case EffectType._ALL_WEAPONS_: content += "All Weapons Affected"; break;

                case EffectType.ALIEN_SPAWN_DISTANCE: content += Green(string.Format("+{0:n0}% Alien Spawn Distance", effect.value)); break;
                case EffectType.MAP_SIZE: content += Green(string.Format("+{0:n0}% Map Size", effect.value)); break;
                case EffectType.MINIMAP_ZOOM: content += Green(string.Format("Minimap Zooms Out +{0:n0}%", effect.value)); break;
                case EffectType.SATELLITE_COUNT: content += Green(string.Format("+{0:n0} Satellite Count", effect.value)); break;
                case EffectType.SATELLITE_RANGE: content += Green(string.Format("+{0:n0}% Satellite Range", effect.value)); break;
                case EffectType._ALL_SATELLITES_: content += "All Satellites Affected"; break;

                case EffectType.POPULATION_GROWTH_TICK_VALUE: content += Green(string.Format("+{0:n0}% Passive Population Growth", effect.value)); break;
                case EffectType.POPULATION_GROWTH_TICK_RATE: content += Green(string.Format("-{0:n0}% Time Between Population Ticks", effect.value)); break;
                case EffectType.POPULATION_GROWTH_RANDOM_PROB: content += Green(string.Format("+{0:n0}% Chance of Population Boost", effect.value)); break;
                case EffectType.POPULATION_GROWTH_RANDOM_VALUE: content += Green(string.Format("+{0:n0}% Random Population Boost Size", effect.value)); break;
                case EffectType.POPULATION_XP_GAIN_MODIFIER: content += Green(string.Format("+{0:n0}% XP Bonus from Population Size", effect.value)); break;
                case EffectType.XP_PASSIVE_GAIN: content += Green(string.Format("+{0:n0}% Passive XP", effect.value)); break;
                case EffectType.XP_KILL_GAIN: content += Green(string.Format("+{0:n0}% XP from Alien Kills by Ship", effect.value)); break;
                case EffectType.DAMAGE_REDUCTION: content += Green(string.Format("-{0:n0}% Population Loss from Alien Attacks", effect.value)); break;
                case EffectType.RETALIATION_COOLDOWN: content += Green(string.Format("-{0:n0}% Retaliation Cooldown", effect.value)); break;
                case EffectType.RETALIATION_COUNT: content += Green(string.Format("+{0:n0} Missiles Fired by Retaliation", effect.value)); break;
                case EffectType.RETALIATION_NUKES: content += Green(string.Format("Go Nuclear")); break;
                case EffectType.SATELLITE_XP: content += Green(string.Format("+{0:n0}% XP from Alien Kills by Satellites", effect.value)); break;
                case EffectType.EMERGENCY_REPOPULATION: content += Green(string.Format("+{0:n0}% Population Growth when below Maximum Population", effect.value)); break;

                case EffectType.UNLOCK_BOOST: content += Blue(string.Format("Unlocks Boost Ability")); break;
                case EffectType.UNLOCK_TUNNEL_WARP: content += Blue(string.Format("Unlocks Short-Range Warp Ability")); break;
                case EffectType.UNLOCK_EARTH_MISSILES: content += Blue(string.Format("Unlocks Extra-Terrestrial Missiles")); break;
                case EffectType.UNLOCK_ICONS: content += Blue(string.Format("Different Alien Ships Have Their Own Icons")); break;
                case EffectType.UNLOCK_RETALIATION: content += Blue(string.Format("Unlocks Retaliatory Strike. Cooldown {0} seconds", effect.value)); break;
                case EffectType.UNLOCK_SATELLITE_1: content += Blue(string.Format("Adds Six Basic Satellites to Earths Orbit")); break;
                case EffectType.UNLOCK_SATELLITE_2: content += Blue(string.Format("Adds Four High-Powered Cannons to Earths Orbit")); break;
                case EffectType.UNLOCK_SATELLITE_3: content += Blue(string.Format("Adds Eight Laserbeam Satellites to Earths Orbit")); break;
                case EffectType.UNLOCK_SPACE_STATION: content += Blue(string.Format("Adds Space Station to Earths Orbit")); break;
                case EffectType.UNLOCK_WEAPON_2: content += Blue(string.Format("Adds Chaingun to Ship")); break;
                case EffectType.UNLOCK_WEAPON_3: content += Blue(string.Format("Adds Laser Turret to Ship")); break;
                case EffectType.UNLOCK_WEAPON_4: content += Blue(string.Format("Add Rocket Pods to Ship")); break;

                case EffectType.START_MISSION: content += Gold(string.Format("Start Mission")); break;
                case EffectType.START_COLLECTION_MISSION: content += Gold(string.Format("Start Mission\n\"Reverse Engineer\"\nCollect {0} Parts", effect.value)); break;
                case EffectType.START_SCANNING_MISSION: content += Gold(string.Format("Start Mission\n\"Scan for Alien Base\"")); break;
                case EffectType.START_PROBE_MISSION: content += Gold(string.Format("Start Mission\n\"Send Probe\"")); break;
                case EffectType.START_FINALE_MISSION: content += Gold(string.Format("Start Mission\n\"Survive the Alien Onslaught\"")); break;

                case EffectType.ADD_SCORE: content += Gold(string.Format("+{0} to Final Score", effect.value)); break;
                case EffectType.WIN_GAME: content += UpSize(Gold(string.Format("Win the Game", effect.value)), 8); break;
                default: break;
            }
        }
        content = DownSize(content, 4);
        if (centre) content = Align(content, "center");
        return content;
    }

    private void SetToolTipCannotUnlock(TechObjectDisplay tOD, int minLevel) {
        string content = Bold(tOD.techObject.techName);
        content += Red("\n\nCannot Unlock\n\n");
        if ((tOD.techUnlockStatusEncoded & 2) != 0) content += Bold(Red(string.Format("Requires Level {0}\n", Italic(minLevel.ToString()))));
        if ((tOD.techUnlockStatusEncoded & 4) != 0) {
            content += Bold(Red("Requires\n"));
            foreach (var dependent in tOD.dependentTechs)
            {
                if (dependent.techUnlockStatusEncoded > 32) continue;
                content += Italic(Red(DownSize(dependent.techObject.techName, 2)));
                content += "\n";
            }
        }

        tOD.tooltip.content = UpSize(Align(content, "center"), 8);
    }

    public static string Bold(string content) => "<b>" + content + "</b>";
    public static string UpSize(string content, int value) => string.Format("<size=+{0}>", value) + content + "<size=100%>";
    public static string DownSize(string content, int value) => string.Format("<size=-{0}>", value) + content + "<size=100%>";
    public static string Indent(string content, int percent) => string.Format("<indent={0}%>", percent) + content + "</indent>";
    public static string Align(string content, string alignment) => string.Format("<align={0}>", alignment) + content + "</align>";
    public static string Italic(string content) => "<i>" + content + "</i>";

    public string Blue(string content) => cachedBlueColour + content + cachedBaseColour;
    public string Gold(string content) => cachedGoldColour + content + cachedBaseColour;
    public string Green(string content) => cachedGreenColour + content + cachedBaseColour;
    public string Red(string content) => cachedRedColour + content + cachedBaseColour;
}
