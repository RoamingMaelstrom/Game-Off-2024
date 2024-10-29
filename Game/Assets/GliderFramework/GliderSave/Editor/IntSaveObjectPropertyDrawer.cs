using UnityEngine;
using UnityEditor;
using GliderSave;
using Common = GliderSave.CommonSaveObjectPropertyFields;

[CustomEditor(typeof(IntSaveObject)), CanEditMultipleObjects]
public class IntSaveObjectPropertyDrawer : Editor {

    public int valueToUpdate;
    public string saveNameTemp;
    public bool saveExists;

    // Define Property Fields
    public SerializedProperty
        saveName_Prop,
        defaultValue_Prop,
        SaveExists_Prop,

        lowerBound_Prop,
        upperBound_Prop,

        saveUpdateRuleNumeric_Prop;

    // Link Property fields to Object Properties
    void OnEnable () 
    {
        saveName_Prop = serializedObject.FindProperty("saveName");
        defaultValue_Prop = serializedObject.FindProperty("defaultValue");
        SaveExists_Prop = serializedObject.FindProperty("SaveExists");

        lowerBound_Prop = serializedObject.FindProperty("lowerBound");
        upperBound_Prop = serializedObject.FindProperty("upperBound");

        saveUpdateRuleNumeric_Prop = serializedObject.FindProperty("saveUpdateRuleNumeric");
        IntSaveObject saveObject = (IntSaveObject)target;
        saveNameTemp = saveObject.GetPrefsName();
    }

    private void OnDisable() 
    {
        IntSaveObject saveObject = (IntSaveObject)target;
        saveObject.ChangeSaveName(saveNameTemp);
    }

    public override void OnInspectorGUI() 
    {
        serializedObject.Update();
        IntSaveObject saveObject = (IntSaveObject)target;

        saveNameTemp = Common.DrawSaveNameField(saveNameTemp);
        if (Common.DrawChangeNameButton(saveObject.SaveName, saveNameTemp)) saveObject.ChangeSaveName(saveNameTemp);
        EditorGUILayout.PropertyField(defaultValue_Prop);

        Common.DrawUILine();

        Common.DrawSavedValueField(saveObject.SaveExists, saveObject.GetValue());
        DrawOverrideSavedValueField(saveObject);
        if (Common.DrawResetButton(saveObject.SaveExists, saveObject.GetPrefsName())) saveObject.ResetValue();
        if (Common.DrawDeleteButton(saveObject.GetPrefsName())) saveObject.DeleteSave();

        Common.DrawUILine();

        EditorGUILayout.PropertyField(lowerBound_Prop);
        EditorGUILayout.PropertyField(upperBound_Prop);
        Common.DrawBoundsDisabledText(saveObject.BoundsEnabled(), saveObject.lowerBound, saveObject.upperBound);
        GUILayout.Space(5);
        EditorGUILayout.PropertyField(saveUpdateRuleNumeric_Prop);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawOverrideSavedValueField(IntSaveObject saveObject)
    {
        EditorGUILayout.BeginHorizontal();
        if (saveObject.SaveExists)
        {
            EditorGUILayout.LabelField("Override Saved Value", GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * 0.37f));
            valueToUpdate = EditorGUILayout.IntField(saveObject.GetValue(), GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * 0.63f));
            if (valueToUpdate != saveObject.GetValue()) saveObject.SetValue(valueToUpdate);
        }
        else EditorGUILayout.LabelField("");
        EditorGUILayout.EndHorizontal();
    }
}
