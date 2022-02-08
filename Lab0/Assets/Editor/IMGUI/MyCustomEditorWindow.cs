using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class MyCustomEditorWindow : EditorWindow
{
    private string objectName = "";
    private bool autoRename = false;

    // TODO: create a menu item in the Editor to show our custom EditorWindow
    //       set the minSize for the window to be (600, 400) pixels

    [MenuItem("Lab0/My Custom Window")]
    static void ShowWindow()
    {
        EditorWindow window = GetWindow<MyCustomEditorWindow>();
        window.minSize = new Vector2(600, 400);
        window.Show();
    }
    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();

        Color oldContentColor = GUI.contentColor;

        GUI.contentColor = Color.cyan;
        GUILayout.Label("Welcome to your very own editor!", EditorStyles.largeLabel);
        GUI.contentColor = oldContentColor;

        GUILayout.Label("This is a label with no styles at all. \nFeel free to experiment with me and learn how to use the IMGUI system.");

        GUILayout.Space(20);

        GUILayout.Label("Name your object(s):", EditorStyles.boldLabel);
        objectName = EditorGUILayout.TextField("Name", objectName);

        GUILayout.Space(20);

        GUILayout.BeginHorizontal();
        autoRename = EditorGUILayout.Toggle(
            new GUIContent("Auto Rename?", "Automatically rename selected game objects without having to press the button."), 
            autoRename, 
            GUILayout.MaxWidth(180)
        );

        if (!autoRename && GUILayout.Button("Rename Selected GameObjects", GUILayout.MaxWidth(200)))
        {
            GameObjectRename(Selection.gameObjects);
        }
        GUILayout.EndHorizontal();

        if (autoRename && EditorGUI.EndChangeCheck())
        {
            GameObjectRename(Selection.gameObjects);
        }
    }

    private void GameObjectRename(GameObject[] objects)
    {
        foreach (var obj in objects)
        {
            if (obj.name != objectName)
                obj.name = objectName;
        }
    }
}
