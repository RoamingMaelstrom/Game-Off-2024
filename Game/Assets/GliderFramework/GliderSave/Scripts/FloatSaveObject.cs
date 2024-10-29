using UnityEngine;

namespace GliderSave
{
    [CreateAssetMenu(fileName = "FloatSaveObject", menuName = "GliderFramework/FloatSaveObject", order = 2)]
    public class FloatSaveObject : ScriptableObject 
    {
        [field: SerializeField] public string SaveName {get; private set;}
        [SerializeField] float defaultValue;
        public bool SaveExists;
        
        [Header("Save Update Rules")]
        public float lowerBound;
        public float upperBound;
        [SerializeField] UpdateRuleNumeric saveUpdateRuleNumeric;

        [Header("Local Copy Information")]
        [SerializeField] bool localCopyUpToDate = false;
        [SerializeField] float localCopyValue;

        public bool BoundsEnabled() => lowerBound < upperBound;
        public string GetPrefsName() => SaveName;

        public void ChangeSaveName(string newName)
        {
            if (SaveName == newName) return;
            SaveName = newName;
            float tempValue = GetValue();
            DeleteSave();
            OverrideValue(tempValue);
        }

        public void DeleteSave() 
        {
            SaveExists = false;
            PlayerPrefs.DeleteKey(GetPrefsName());
            localCopyValue = 0;
        }

        public float GetValue()
        {
            if (!SaveExists) ResetValue();
            if (localCopyUpToDate) return localCopyValue;

            ForceUpdateLocalCopy();
            return localCopyValue;
        }

        public void ForceUpdateLocalCopy()
        {
            localCopyUpToDate = true;
            localCopyValue = PlayerPrefs.GetFloat(GetPrefsName(), defaultValue);
        }

        public void OverrideValue(float newValue) 
        {
            localCopyUpToDate = false;
            SaveExists = true;
            PlayerPrefs.SetFloat(GetPrefsName(), newValue);
        }

        public void ResetValue() => OverrideValue(defaultValue);

        public bool SetValue(float newValue)
        {
            float oldValue = GetValue();
            if (!SatifiesNumericRule(saveUpdateRuleNumeric, newValue, oldValue)) return false;
            OverrideValue(newValue);
            return true;
        }

        private bool SatifiesNumericRule(UpdateRuleNumeric saveUpdateRuleNumeric, float newValue, float oldValue)
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
}