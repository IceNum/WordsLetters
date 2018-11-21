using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;

[CustomEditor(typeof(Cell))]
[CanEditMultipleObjects]
public class CellEditor : Editor
{
    private ReorderableList _alphabet;


    void OnEnable()
    {
        _alphabet = new ReorderableList(serializedObject, serializedObject.FindProperty("Alphabet"), true, true, true, true);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("cells"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("back"),new GUIContent("Background"));

        EditorGUILayout.LabelField("Value: " + ((((Cell)target).value.letter == "") ? "None" : ((Cell)target).value.letter));
        
        ((Cell)target).index= EditorGUILayout.Vector2Field("Index: ",((Cell)target).index);

        _alphabet.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
          {
              var element = _alphabet.serializedProperty.GetArrayElementAtIndex(index);
              rect.y += 2;
              EditorGUI.PropertyField(new Rect(rect.x, rect.y, 130, EditorGUIUtility.singleLineHeight),
                  element.FindPropertyRelative("icon"), GUIContent.none);
              EditorGUI.PropertyField(new Rect(rect.x+140, rect.y, 25, EditorGUIUtility.singleLineHeight),
                  element.FindPropertyRelative("letter"), GUIContent.none);
          };

        _alphabet.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Alphabet");
        };
        _alphabet.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
    }
}
