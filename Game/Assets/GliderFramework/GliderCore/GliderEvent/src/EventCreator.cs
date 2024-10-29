using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class EventCreator : EditorWindow
{
    private readonly string scriptFolderPath = "Assets/GliderFramework/GliderCore/GliderEvent/_Event_Scripts/";
    private readonly string defaultEventObjectFolderPath = "Assets/GliderFramework/GliderCore/GliderEvent/Events/";
    private string eventObjectFolderPath = "Assets/GliderFramework/GliderCore/GliderEvent/Events/";
    private string eventName = "";
    private string eventClassName = "";
    private string eventObjectName = "";
    private int numArgs = 0;
    private List<EventArgumentField> arguments = new();

    private bool useDefaultEventObjectPath = true;

 
    [MenuItem("Window/Event Creator")]
    public static void OnWindowOpen()
    {
        EventCreator window = GetWindow<EventCreator>("Event Creator");
        window.minSize = new Vector2(600, 400);
    }
 
    private void OnGUI()
    {
        GUILayout.Space(10);
        DrawUILine(2);
        GUILayout.Space(10);

        GUILayout.Label("Create Event Script");
        eventName = EditorGUILayout.TextField(" Event Name:", eventName);
        GUILayout.Space(10);

        DisplayArgumentFields();

        if (numArgs < 3) {
            if (GUILayout.Button("+ Add Argument")) AddArgument();
        }
        GUILayout.Space(40);
        


        if (GUILayout.Button("Create Event Script")) CreateEventScript();

        GUILayout.Space(10);
        DrawUILine(2);
        GUILayout.Space(10);

        GUILayout.Label("Create Event Object");
        eventObjectName = EditorGUILayout.TextField(" Event Name: ", eventObjectName);
        GUILayout.Space(10);
        if (GUILayout.Button("Create Event Object")) CreateEventObject();

        GUILayout.Space(20);
        DrawUILine(2);
        GUILayout.Space(10);

        GUILayout.Label("Event Object Path Location");
        GUILayout.Space(10);
        useDefaultEventObjectPath = GUILayout.Toggle(useDefaultEventObjectPath, "Use Default Path");
        GUI.enabled = !useDefaultEventObjectPath;
        eventObjectFolderPath = EditorGUILayout.TextField("Path", useDefaultEventObjectPath ? defaultEventObjectFolderPath : eventObjectFolderPath);
        GUI.enabled = true;

    }

    private void DisplayArgumentFields()
    {
        for (int i = 0; i < arguments.Count; i++)
        {
            DrawUILine();
            GUILayout.Label(string.Format("Argument Type {0} (Optional):", i + 1));
            arguments[i].argumentType = EditorGUILayout.TextField("", arguments[i].argumentType);
            GUILayout.Label(string.Format("Description (Optional):", i + 1));
            arguments[i].argumentDescription = EditorGUILayout.TextField("", arguments[i].argumentDescription);
            DrawUILine();   
        }
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

    private void AddArgument()
    {
        if (numArgs >= 3) return;
        numArgs ++;
        arguments.Add(new());
    }

    public void CreateEventScript()
    {
        // make sure script has a name
        if (eventName.Length == 0)
        {
            Debug.Log("Error - Script name cannot be empty.");
            return;
        }
 
        // remove spaces and capitalize the first letter
        eventName = eventName.Replace(" ", "");
        eventName = char.ToUpper(eventName[0]) + eventName[1..];
        eventClassName = eventName + "Event";


        string assetPath = scriptFolderPath + eventClassName + ".cs";
 
        // make sure this script doesn't already exist
        if (File.Exists(assetPath))
        {
            Debug.Log("Error - File with that name already exists.");
            return;
        }

        string args = "";
        int argBits = 0;

        

        for (int i = 0; i < arguments.Count; i++)
        {
            arguments[i].argumentType = arguments[i].argumentType.Replace(" ", "");
            if (arguments[i].argumentType.Length > 0) {
                args += args.Length == 0 ? arguments[i].argumentType : ", " + arguments[i].argumentType;
                argBits += (int)Mathf.Pow(2, i);
            }
        }

        string typeInfo = argBits > 0 ? "<" + args + ">" : "";
 
        // create the script
        using StreamWriter outfile = new(assetPath);
        outfile.WriteLine("using UnityEngine;");
        outfile.WriteLine("");
        outfile.WriteLine("namespace GliderEvents");
        outfile.WriteLine("{");
        outfile.WriteLine("\t");
        outfile.WriteLine("public class " + eventClassName + " : SOEvent" + typeInfo);
        outfile.WriteLine("{");
        outfile.WriteLine("\t");

        string startAwake = "private void Awake() { ";
        string endAwake = " }";
        string middleAwake = "";

        int argDescNum = 1;
        if ((argBits & 1) > 0) {
            middleAwake += string.Format("argument{0}Description = \"{1}\";", argDescNum, arguments[0].argumentDescription);
            argDescNum ++;
        }
        if ((argBits & 2) > 0) {
            middleAwake += string.Format("argument{0}Description = \"{1}\";", argDescNum, arguments[1].argumentDescription);
            argDescNum ++;
        }
        if ((argBits & 4) > 0) {
            middleAwake += string.Format("argument{0}Description = \"{1}\";", argDescNum, arguments[2].argumentDescription);
            argDescNum ++;
        }

        outfile.WriteLine(startAwake + middleAwake + endAwake);

        outfile.WriteLine("}");
        outfile.WriteLine("}");
        outfile.Close();
 
        Debug.Log(string.Format("Creating Event Script {0}.cs ", eventClassName));

        AssetDatabase.Refresh();
        arguments = new();
        numArgs = 0;
        argBits = 0;

        eventObjectName = eventClassName;
    }

    private void CreateEventObject() {
        string assetPath = defaultEventObjectFolderPath + eventClassName + ".asset";
        Debug.Log(string.Format("Creating {0} Event Object", eventClassName));
        AssetDatabase.CreateAsset(CreateInstance(eventClassName), assetPath); 
        AssetDatabase.SaveAssets();
    }
}


public class EventArgumentField 
{

    public string argumentType;
    public string argumentDescription;
}