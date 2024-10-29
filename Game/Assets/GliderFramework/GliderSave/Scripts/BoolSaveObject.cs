using UnityEngine;

namespace GliderSave
{
    [CreateAssetMenu(fileName = "BoolSaveObject", menuName = "GliderFramework/BoolSaveObject", order = 0)]
    public class BoolSaveObject : ScriptableObject 
    {
        [field: SerializeField] public string SaveName {get; private set;}
        [SerializeField] bool defaultValue;
        [field: SerializeField] public bool SaveExists {get; private set;}

        [Header("Local Copy Information")]
        [SerializeField] bool localCopyValue;
        [SerializeField] bool localCopyUpToDate = false;

        public void ChangeSaveName(string newName)
        {
            if (SaveName == newName) return;
            SaveName = newName;
            bool tempValue = GetValue();
            DeleteSave();
            OverrideValue(tempValue);
        }

        public string GetPrefsName() => SaveName;

        public void DeleteSave() 
        {
            SaveExists = false;
            PlayerPrefs.DeleteKey(GetPrefsName());
        }

        public bool GetValue() 
        {
            if (localCopyUpToDate) return localCopyValue;

            ForceUpdateLocalCopy();
            return localCopyValue;
        }

        public void OverrideValue(bool newValue) 
        {
            localCopyUpToDate = false;
            SaveExists = true;
            PlayerPrefs.SetInt(GetPrefsName(), newValue ? 1 : 0);
        }

        public void ResetValue() => OverrideValue(defaultValue);

        public bool SetValue(bool newValue)
        {
            OverrideValue(newValue);
            return true;
        }

        private void ForceUpdateLocalCopy()
        {
            localCopyUpToDate = true;
            localCopyValue = PlayerPrefs.GetInt(GetPrefsName(), 0) != 0;
        }
    }
}
