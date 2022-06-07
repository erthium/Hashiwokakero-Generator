using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Manager))]
public class ManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Manager manager = (Manager)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Initialize"))
        {
            manager.Initialize();
        }

        if (GUILayout.Button("Generate"))
        {
            manager.Generate();
        }
    }
}
