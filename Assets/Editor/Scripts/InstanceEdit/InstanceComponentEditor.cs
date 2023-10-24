using Form;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InstanceComponent))]
public class InstanceComponentEditor : UnityEditor.Editor
{

    private void OnEnable()
    {
        EditorApplication.update += OnEditorUpdate;
        Debug.Log("add");
    }

    private void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
        Debug.Log("remove");
    }
    

    private void OnEditorUpdate()
    {
    }
}
