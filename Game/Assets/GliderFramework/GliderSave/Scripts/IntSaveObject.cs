using UnityEngine;

namespace GliderSave
{
    [CreateAssetMenu(fileName = "IntSaveObject", menuName = "GliderFramework/IntSaveObject", order = 1)]
    public class IntSaveObject : ScriptableObject 
    {
        [field: SerializeField] public string SaveName {get; private set;}
        [SerializeField] int defaultValue;
        [field: SerializeField] public bool SaveExists {get; private set;}

       [Header("Save Update Rules")]
        public int lowerBound;
        public int upperBound;
        [SerializeField] UpdateRuleNumeric saveUpdateRuleNumeric;

        [Header("Local Copy Information")]
        [SerializeField] int localCopyValue;
        [SerializeField] bool localCopyUpToDate = false;

        public bool BoundsEnabled() => lowerBound < upperBound;

        public void ChangeSaveName(string newName)
        {
            if (SaveName == newName) return;
            SaveName = newName;
            int tempValue = GetValue();
            DeleteSave();
            OverrideValue(tempValue);
        }

        public string GetPrefsName() => SaveName;

        public void DeleteSave() 
        {
            SaveExists = false;
            PlayerPrefs.DeleteKey(GetPrefsName());
        }

        public int GetValue() 
        {
            if (localCopyUpToDate) return localCopyValue;

            ForceUpdateLocalCopy();
            return localCopyValue;
        }

        public void OverrideValue(int newValue) 
        {
            localCopyUpToDate = false;
            SaveExists = true;
            PlayerPrefs.SetInt(GetPrefsName(), lowerBound == upperBound ? newValue : Mathf.Clamp(newValue, lowerBound, upperBound));
        }

        public void ResetValue() => OverrideValue(defaultValue);

        public bool SetValue(int newValue)
        {
            int oldValue = GetValue();
            if (!SatifiesNumericRule(saveUpdateRuleNumeric, newValue, oldValue)) return false;
            OverrideValue(newValue);
            return true;
        }

        private void ForceUpdateLocalCopy()
        {
            localCopyUpToDate = true;
            localCopyValue = PlayerPrefs.GetInt(GetPrefsName(), 0);
        }

        private bool SatifiesNumericRule(UpdateRuleNumeric saveUpdateRuleNumeric, int newValue, int oldValue)
        {
            return saveUpdateRuleNumeric switch
            {
                UpdateRuleNumeric.NONE => true,
                UpdateRuleNumeric.NOT_EQUAL => newValue != oldValue,
                UpdateRuleNumeric.GREATER => newValue > oldValue,
                UpdateRuleNumeric.LESS_THAN => newValue < oldValue,
                _ => true,
            };
        }
    }

    public enum UpdateRuleNumeric
    {
        NONE,
        NOT_EQUAL,
        GREATER,
        LESS_THAN
    }
}