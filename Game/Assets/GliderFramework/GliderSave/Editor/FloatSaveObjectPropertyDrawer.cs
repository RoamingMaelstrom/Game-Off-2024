using UnityEngine;
using UnityEditor;
using GliderSave;
using Common = GliderSave.CommonSaveObjectPropertyFields;

[CustomEditor(typeof(FloatSaveObject)), CanEditMultipleObjects]
public class FloatSaveObjectPropertyDrawer : Editor {

    public string saveNameTemp;
    public float valueToUpdate;

    // Define Property Fields
    public SerializedProperty
        saveName_Prop,
        defaultValue_Prop,
        SaveExists_Prop,

        lowerBound_Prop,
        upperBound_Prop,

        saveUpdateRuleNumeric_Prop;

    void OnEnable () 
    {
        saveName_Prop = serializedObject.FindProperty("SaveName");
        defaultValue_Prop = serializedObject.FindProperty("defaultValue");
        SaveExists_Prop = serializedObject.FindProperty("SaveExists");

        lowerBound_Prop = serializedObject.FindProperty("lowerBound");
        upperBound_Prop = serializedObject.FindProperty("upperBound");

        saveUpdateRuleNumeric_Prop = serializedObject.FindProperty("saveUpdateRuleNumeric");

        FloatSaveObject saveObject = (FloatSaveObject)target;
        saveNameTemp = saveObject.GetPrefsName();
    }

    private void OnDisable() 
    {
        FloatSaveObject saveObject = (FloatSaveObject)target;
        saveObject.ChangeSaveName(saveNameTemp);
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        FloatSaveObject saveObject = (FloatSaveObject)target;

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

    private void DrawOverrideSavedValueField(FloatSaveObject saveObject)
    {
        EditorGUILayout.BeginHorizontal();
        if (saveObject.SaveExists)
        {
            EditorGUILayout.LabelField("Override Saved Value", GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * 0.37f));
            valueToUpdate = EditorGUILayout.FloatField(saveObject.GetValue(), GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * 0.63f));
            if (valueToUpdate != saveObject.GetValue()) saveObject.SetValue(valueToUpdate);
        }
        else EditorGUILayout.LabelField("");
        EditorGUILayout.EndHorizontal();
    }
}
