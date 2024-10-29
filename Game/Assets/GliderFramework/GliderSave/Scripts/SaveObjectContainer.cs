using UnityEngine;

namespace GliderSave
{
    public class SaveObjectContainer : MonoBehaviour
    {
        [SerializeField] string loadFromFolderPath = "SaveObjects";
        public BoolSaveObject[] boolSaveObjectArray;
        public FloatSaveObject[] floatSaveObjectArray;
        public IntSaveObject[] intSaveObjectArray;
        public StringSaveObject[] stringSaveObjectArray;

        private bool gameRunning = false;

        private void Awake() 
        {
            gameRunning = true;
        }

        private void LoadAllSaveObjects() {
            boolSaveObjectArray = Resources.LoadAll<BoolSaveObject>(loadFromFolderPath);
            floatSaveObjectArray = Resources.LoadAll<FloatSaveObject>(loadFromFolderPath);
            intSaveObjectArray = Resources.LoadAll<IntSaveObject>(loadFromFolderPath);
            stringSaveObjectArray = Resources.LoadAll<StringSaveObject>(loadFromFolderPath);

            int creationCounter = 0;

            foreach (var saveObject in boolSaveObjectArray)
            {
                creationCounter++;
                if (saveObject.SaveExists) continue;
                saveObject.ResetValue();
            }

            if (creationCounter > 0) Debug.Log(string.Format("Loaded {0} BoolSaveObjects into Application", creationCounter));

            creationCounter = 0;

            foreach (var saveObject in floatSaveObjectArray)
            {
                creationCounter++;
                if (saveObject.SaveExists) continue;
                saveObject.ResetValue();
            }

            if (creationCounter > 0) Debug.Log(string.Format("Loaded {0} FloatSaveObjects into Application", creationCounter));

            creationCounter = 0;

            foreach (var saveObject in intSaveObjectArray)
            {
                creationCounter++;
                if (saveObject.SaveExists) continue;
                saveObject.ResetValue();
            }

            if (creationCounter > 0) Debug.Log(string.Format("Loaded {0} IntSaveObjects into Application", creationCounter));

            creationCounter = 0;

            foreach (var saveObject in stringSaveObjectArray)
            {
                creationCounter++;
                if (saveObject.SaveExists) continue;
                saveObject.ResetValue();
            }

            if (creationCounter > 0) Debug.Log(string.Format("Loaded {0} StringSaveObjects into Application", creationCounter));
        }

        public bool DeleteAllSavedData() {
            if (!gameRunning) LoadAllSaveObjects();

            foreach (var saveObject in boolSaveObjectArray) saveObject.DeleteSave();
            foreach (var saveObject in floatSaveObjectArray) saveObject.DeleteSave();
            foreach (var saveObject in intSaveObjectArray) saveObject.DeleteSave();
            foreach (var saveObject in stringSaveObjectArray) saveObject.DeleteSave();
            PlayerPrefs.Save();
            return true;
        }
    }
}
