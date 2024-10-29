using UnityEngine;
using UnityEditor;
using GliderSave;
using Common = GliderSave.CommonSaveObjectPropertyFields;

[CustomEditor(typeof(StringSaveObject)), CanEditMultipleObjects]
public class StringSaveObjectPropertyDrawer : Editor {

    public string valueToUpdate;
    public string saveNameTemp;
    public bool saveExists;

    // Define Property Fields
    public SerializedProperty
        saveName_Prop,
        defaultValue_Prop,
        SaveExists_Prop,

        maxLength_Prop,

        saveUpdateRuleString_Prop;

    void OnEnable () 
    {
        saveName_Prop = serializedObject.FindProperty("saveName");
        defaultValue_Prop = serializedObject.FindProperty("defaultValue");
        SaveExists_Prop = serializedObject.FindProperty("SaveExists");

        maxLength_Prop = serializedObject.FindProperty("maxLength");

        saveUpdateRuleString_Prop = serializedObject.FindProperty("saveUpdateRuleString");
        StringSaveObject saveObject = (StringSaveObject)target;
        saveNameTemp = saveObject.GetPrefsName();
    }

    private void OnDisable() 
    {
        StringSaveObject saveObject = (StringSaveObject)target;
        saveObject.ChangeSaveName(saveNameTemp);
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        StringSaveObject saveObject = (StringSaveObject)target;

        saveNameTemp = Common.DrawSaveNameField(saveNameTemp);
        if (Common.DrawChangeNameButton(saveObject.SaveName, saveNameTemp)) saveObject.ChangeSaveName(saveNameTemp);
        EditorGUILayout.PropertyField(defaultValue_Prop);

        Common.DrawUILine();

        Common.DrawSavedValueField(saveObject.SaveExists, saveObject.GetValue());
        DrawOverrideSavedValueField(saveObject);
        if (Common.DrawResetButton(saveObject.SaveExists, saveObject.GetPrefsName())) saveObject.ResetValue();
        if (Common.DrawDeleteButton(saveObject.GetPrefsName())) saveObject.DeleteSave();

        Common.DrawUILine();

        EditorGUILayout.PropertyField(maxLength_Prop);
        DrawMaxLengthDisabledText(saveObject);
        GUILayout.Space(5);
        EditorGUILayout.PropertyField(saveUpdateRuleString_Prop);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawMaxLengthDisabledText(StringSaveObject saveObject)
    {
        if (saveObject.GetMaxLength() > 0)
            EditorGUILayout.LabelField(string.Format("save length must be less than {0}.", saveObject.GetMaxLength()));
        else 
            EditorGUILayout.LabelField("No length limit for Save Value. Set maxLength > 0 to enable Length limit.");
    }

    private void DrawOverrideSavedValueField(StringSaveObject saveObject)
    {
        EditorGUILayout.BeginHorizontal();
        if (saveObject.SaveExists)
        {
            EditorGUILayout.LabelField("Override Saved Value", GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * 0.37f));
            valueToUpdate = EditorGUILayout.TextField(saveObject.GetValue(), GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * 0.63f));
            if (valueToUpdate != saveObject.GetValue()) saveObject.SetValue(valueToUpdate);
        }
        else EditorGUILayout.LabelField("");
        EditorGUILayout.EndHorizontal();
    }
}
