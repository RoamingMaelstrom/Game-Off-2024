using UnityEngine;
using UnityEditor;
using GliderSave;
using Common = GliderSave.CommonSaveObjectPropertyFields;

[CustomEditor(typeof(BoolSaveObject)), CanEditMultipleObjects]
public class BoolSaveObjectPropertyDrawer : Editor {

    public bool valueToUpdate;
    public string saveNameTemp;
    public bool saveExists;

    // Define Property Fields
    public SerializedProperty
        saveName_Prop,
        defaultValue_Prop,
        SaveExists_Prop;

    // Link Property fields to Object Properties
    void OnEnable () 
    {
        saveName_Prop = serializedObject.FindProperty("saveName");
        defaultValue_Prop = serializedObject.FindProperty("defaultValue");
        SaveExists_Prop = serializedObject.FindProperty("SaveExists");

        BoolSaveObject saveObject = (BoolSaveObject)target;
        saveNameTemp = saveObject.GetPrefsName();
    }

    private void OnDisable() 
    {
        BoolSaveObject saveObject = (BoolSaveObject)target;
        saveObject.ChangeSaveName(saveNameTemp);
    }

    public override void OnInspectorGUI() 
    {
        serializedObject.Update();
        BoolSaveObject saveObject = (BoolSaveObject)target;

        saveNameTemp = Common.DrawSaveNameField(saveNameTemp);
        if (Common.DrawChangeNameButton(saveObject.SaveName, saveNameTemp)) saveObject.ChangeSaveName(saveNameTemp);
        EditorGUILayout.PropertyField(defaultValue_Prop);

        Common.DrawUILine();

        Common.DrawSavedValueField(saveObject.SaveExists, saveObject.GetValue());
        DrawOverrideSavedValueField(saveObject);
        if (Common.DrawResetButton(saveObject.SaveExists, saveObject.GetPrefsName())) saveObject.ResetValue();
        if (Common.DrawDeleteButton(saveObject.GetPrefsName())) saveObject.DeleteSave();

        Common.DrawUILine();
        GUILayout.Space(5);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawOverrideSavedValueField(BoolSaveObject saveObject)
    {
        EditorGUILayout.BeginHorizontal();
        if (saveObject.SaveExists)
        {
            EditorGUILayout.LabelField("Override Saved Value", GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * 0.37f));
            valueToUpdate = EditorGUILayout.Toggle(saveObject.GetValue(), GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * 0.63f));
            if (valueToUpdate != saveObject.GetValue()) saveObject.SetValue(valueToUpdate);
        }
        else EditorGUILayout.LabelField("");
        EditorGUILayout.EndHorizontal();
    }
}
