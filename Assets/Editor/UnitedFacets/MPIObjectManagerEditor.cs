using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MPIObjectManager))]
public class MPIObjectManagerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        MPIObjectManager me = (MPIObjectManager)target;

        if (GUILayout.Button("Enumerate MPIViews"))
        {
            me.RegisterMPIViews();
        }
    }
}
