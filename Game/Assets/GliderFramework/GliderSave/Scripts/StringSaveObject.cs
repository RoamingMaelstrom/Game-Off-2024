using UnityEngine;


namespace GliderSave
{
    [CreateAssetMenu(fileName = "StringSaveObject", menuName = "GliderFramework/StringSaveObject", order = 3)]
    public class StringSaveObject : ScriptableObject 
    {
        [field: SerializeField] public string SaveName {get; private set;}
        public string defaultValue;
        [field: SerializeField] public bool SaveExists {get; private set;}

        [Header("Save Update Rules")]
        [SerializeField] int maxLength;
        [SerializeField] UpdateRuleString saveUpdateRuleString;

        [Header("Local Copy Information")]
        [SerializeField] bool localCopyUpToDate = false;
        [SerializeField] string localCopyValue;

        public void ChangeSaveName(string newName)
        {
            if (SaveName == newName) return;
            SaveName = newName;
            string tempValue = GetValue();
            DeleteSave();
            OverrideValue(tempValue);
        }

        public int GetMaxLength() => maxLength;
        public string GetPrefsName() => SaveName;

        public void DeleteSave() 
        {
            SaveExists = false;
            PlayerPrefs.DeleteKey(GetPrefsName());
        }

        public string GetValue()
        {
            if (!localCopyUpToDate) return localCopyValue;

            ForceUpdateLocalCopy();
            return localCopyValue;
        }

        private void ForceUpdateLocalCopy()
        {
            localCopyUpToDate = true;
            localCopyValue = PlayerPrefs.GetString(GetPrefsName(), "");
        }

        private void OverrideValue(string newValue) 
        {
            localCopyUpToDate = false;
            SaveExists = true;
            PlayerPrefs.SetString(GetPrefsName(), (newValue.Length <= maxLength || maxLength <= 0) ? newValue : newValue[..Mathf.Min(newValue.Length, maxLength + 1)]);
        }

        public void ResetValue() => OverrideValue(defaultValue);

        public bool SetValue(string newValue)
        {
            string oldValue = GetValue();
            if (!SatifiesStringRule(saveUpdateRuleString, newValue, oldValue)) return false;
            OverrideValue(newValue);
            return true;
        }

        private static bool SatifiesStringRule(UpdateRuleString saveUpdateRuleString, string newValue, string oldValue)
        {
            return saveUpdateRuleString switch
            {
                UpdateRuleString.NONE => true,
                UpdateRuleString.LONGEST => newValue.Length > oldValue.Length,
                UpdateRuleString.SHORTEST => newValue.Length < oldValue.Length,
                _ => true
            };
        }
    }

    public enum UpdateRuleString
    {
        NONE,
        LONGEST,
        SHORTEST
    }
}