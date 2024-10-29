using UnityEngine;
using UnityEditor;

namespace GliderSave
{
    public static class CommonSaveObjectPropertyFields
    {

        public static bool DrawChangeNameButton(string oldName, string newName)
        {
            EditorGUI.BeginDisabledGroup(oldName == newName);
            string changeNameText = oldName == newName ? "Save Name matches display name" : "Change Save Name";

            if (GUILayout.Button(changeNameText)) 
            {
                Debug.Log(string.Format("\"{0}\" Changed Save Name from {0} to {1}", oldName, newName));
                EditorGUI.EndDisabledGroup();
                return true;
            }
            EditorGUI.EndDisabledGroup();
            return false;

        }

        public static string DrawSaveNameField(string saveName)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Save Name", GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * 0.37f));
            string newSaveName = EditorGUILayout.TextField(saveName, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * 0.63f));
            //if (saveNameTemp != saveObject.SaveName) saveObject.ChangeSaveName(saveNameTemp);
            EditorGUILayout.EndHorizontal();
            return newSaveName;
        }

        public static void DrawBoundsDisabledText(bool boundsEnabled, float lowerBound, float upperBound)
        {
            if (boundsEnabled) 
                EditorGUILayout.LabelField(string.Format("Save Values must be between {0} and {1} (inclusive)", lowerBound, upperBound));
            else 
                EditorGUILayout.LabelField("Bounds Disabled. Set upper bound to be greater than lower bound to enable bounds.");
        }

        public static void DrawSavedValueField<T>(bool saveExists, T value)
        {
            if (saveExists) EditorGUILayout.LabelField(string.Format("Saved Value: {0}", value));
            else EditorGUILayout.LabelField("PlayerPrefs entry has not been created yet.");
        }

        public static bool DrawResetButton(bool saveExists, string savePrefsName)
        {
            string resetButtonText = saveExists ? "Reset to Default Value" : "Create Save Entry";
            resetButtonText = (savePrefsName == "") ? "Cannot Create Save. Save Name not set" : resetButtonText;
            
            GUILayout.Space(5);
            EditorGUI.BeginDisabledGroup(savePrefsName == "");
            if (GUILayout.Button(resetButtonText)) 
            {
                Debug.Log(string.Format("\"{0}\" reset to default value.", savePrefsName));
                return true;
            }
            EditorGUI.EndDisabledGroup();
            return false;
        }

        public static bool DrawDeleteButton(string savePrefsName)
        {
            GUILayout.Space(5);
            if (GUILayout.Button("Delete Saved Entry")) 
            {
                Debug.Log(string.Format("\"{0}\" PlayerPrefs entry deleted.", savePrefsName));
                return true;
            }
            return false;
        }

        public static void DrawUILine(int thickness = 1, int padding = 20)
        {
            Color color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding/2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }

    }
}
