using UnityEditor;

[CustomEditor(typeof(CustomConsole))]
public class CustomConsoleEditor : Editor 
{
	void OnEnable()
	{
		CustomConsole.instance=(CustomConsole)target;
	}
	public override void OnInspectorGUI()
	{		
		CustomConsole.instance.hideConsole=EditorGUILayout.Toggle("Hide console ",CustomConsole.instance.hideConsole);
		CustomConsole.instance.replaceInDebug=EditorGUILayout.Toggle("Replace in debug ",CustomConsole.instance.replaceInDebug);
		CustomConsole.instance.defaultMessageColor=EditorGUILayout.ColorField("Default color ",CustomConsole.instance.defaultMessageColor);

		CustomConsole.instance.duration=EditorGUILayout.FloatField("Duration ",CustomConsole.instance.duration);
		CustomConsole.instance.fontSize=EditorGUILayout.IntField("Font size ",CustomConsole.instance.fontSize);
	}

}
