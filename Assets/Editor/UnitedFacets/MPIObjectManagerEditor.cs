// <copyright file="MPIObjectManagerEditor.cs" company="Institute of Visual Computing / Bonn-Rhein-Sieg University of Applied Sciences">
// Copyright (c) 2016-2017 All Rights Reserved
// </copyright>
// <author>Anton Sigitov</author>
// <summary></summary>

using UnityEngine;
using UnityEditor;

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
