using UnityEngine;
using UnityEditor;
using GliderSave;

[CustomEditor(typeof(SaveObjectContainer)), CanEditMultipleObjects]
public class SaveObjectContainerPropertyDrawer : Editor
{
    public SerializedProperty
        loadFromFolderPath_Prob,
        boolSaveObjectArray_Prob,
        floatSaveObjectArray_Prob,
        intSaveObjectArray_Prob,
        stringSaveObjectArray_Prob;

    void OnEnable () {
        loadFromFolderPath_Prob = serializedObject.FindProperty("loadFromFolderPath");
        boolSaveObjectArray_Prob = serializedObject.FindProperty("boolSaveObjectArray");
        floatSaveObjectArray_Prob = serializedObject.FindProperty("floatSaveObjectArray");
        intSaveObjectArray_Prob = serializedObject.FindProperty("intSaveObjectArray");
        stringSaveObjectArray_Prob = serializedObject.FindProperty("stringSaveObjectArray");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        SaveObjectContainer saveObjectContainer = (SaveObjectContainer)target;

        EditorGUILayout.PropertyField(loadFromFolderPath_Prob);
        EditorGUILayout.PropertyField(boolSaveObjectArray_Prob);
        EditorGUILayout.PropertyField(floatSaveObjectArray_Prob);
        EditorGUILayout.PropertyField(intSaveObjectArray_Prob);
        EditorGUILayout.PropertyField(stringSaveObjectArray_Prob);

        DrawDeleteSavesButton(saveObjectContainer);

         serializedObject.ApplyModifiedProperties();
    }


    public static bool DrawDeleteSavesButton(SaveObjectContainer saveObjectContainer) {
        if (GUILayout.Button("Delete All Saved Data")) 
        {
            saveObjectContainer.DeleteAllSavedData();
            Debug.Log(string.Format("Deleted All Saved Data."));
            return true;
        }
        return false;

    }
}
