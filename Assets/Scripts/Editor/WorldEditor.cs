using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(World))]
public class WorldEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);

        if (GUILayout.Button("Regen"))
        {
            ((World)target).RegenWorld();

            Undo.RegisterFullObjectHierarchyUndo(target, "Regen World");

            //foreach(Transform child in transform)
            //{
            //    Undo.RecordObject(child, "Regen World");
            //}

            //set scene dirty
            //using UnityEditor.SceneManagement;
            //EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
}
