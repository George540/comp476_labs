using UnityEditor;

using UnityEngine;

// A custom editor for the type "Player"
[CustomEditor(typeof(Player))]
public class PlayerInspector : Editor
{
    private SerializedProperty m_Script;
    private SerializedProperty catchPhrase;
    private SerializedProperty health;
    private SerializedProperty maxHealth;
    private SerializedProperty withShield;
    private SerializedProperty shield;

    // fired when the inpector first needs to display this editor
    private void OnEnable()
    {
        //Debug.Log("PlayerInspector.OnEnable()");
        m_Script = serializedObject.FindProperty("m_Script");
        catchPhrase = serializedObject.FindProperty("catchPhrase");
        health = serializedObject.FindProperty("health");
        maxHealth = serializedObject.FindProperty("maxHealth");
        withShield = serializedObject.FindProperty("withShield");
        shield = serializedObject.FindProperty("shield");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        EditorGUILayout.PropertyField(m_Script);

        // using EditorGUILayout.TextArea() is not good enough, we want the custom label, the scrollbar, and we want to set the min and max lines
        // essentially, we want to display it how Unity would by default (taking into consideration any decorator attributes)
        //EditorGUILayout.TextArea(catchPhrase.stringValue, GUILayout.MaxHeight(80));
        EditorGUILayout.PropertyField(catchPhrase, new GUIContent("Special Catch Phrase!", "A phrase that's commonly used."));

        // create and show an IntSlider that allows the designer to modify the health property between the range [0, maxHealth] and has the label "Health"
        EditorGUILayout.IntSlider(health, 0, maxHealth.intValue, health.displayName);
        
        // indent and show maxHealth (use EditorGUI.indentLevel)
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(maxHealth);
        EditorGUI.indentLevel--;

        if (maxHealth.intValue <= health.intValue)
        {
            health.intValue = maxHealth.intValue;
        }

        // put some vertical space between maxHealth and withShield
        EditorGUILayout.Space();

        // show withShield
        EditorGUILayout.PropertyField(withShield);

        // if withShield's bool Value is true, indent and show shield
        if (withShield.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(shield);
            EditorGUI.indentLevel--;
        }
        else
        {
            shield.intValue = 0;
        }

        if (GUILayout.Button("My button"))
        {
            //custom code
        }

        serializedObject.ApplyModifiedProperties();
    }
}
