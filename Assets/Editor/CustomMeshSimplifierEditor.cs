using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CustomMeshSimplifier))]
public class CustomMeshSimplifierEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CustomMeshSimplifier myTarget = (CustomMeshSimplifier)target;

        if (GUILayout.Button("Simplify Mesh"))
        {
            myTarget.SimplifyMesh();
        }

        if (GUILayout.Button("Revert Mesh"))
        {
            myTarget.RevertMesh();
          
        }

        EditorGUILayout.LabelField(Selection.activeGameObject.name);
        EditorGUILayout.LabelField("Vertices: ", myTarget.vertexCount.ToString());
        EditorGUILayout.LabelField("Triangles: ", myTarget.triangleCount.ToString());
        EditorGUILayout.LabelField("SubMeshes: ", myTarget.submeshCount.ToString());

        this.Repaint();
    }
}