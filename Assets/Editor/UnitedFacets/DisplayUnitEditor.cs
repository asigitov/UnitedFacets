// <copyright file="DisplayUnitEditor.cs" company="Institute of Visual Computing / Bonn-Rhein-Sieg University of Applied Sciences">
// Copyright (c) 2016-2017 All Rights Reserved
// </copyright>
// <author>Anton Sigitov</author>
// <summary></summary>

using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(DisplayUnit))]
public class DisplayUnitEditor : Editor
{
    [MenuItem("Assets/Create/Display Unit")]
    public static void CreateAsset()
    {
        EditorUtils.CreateAsset<DisplayUnit>();
    }

    public override void OnInspectorGUI()
    {
        DisplayUnit me = (DisplayUnit)target;

        me.TypeName = EditorGUILayout.TextField("Type Name", me.TypeName);
        me.Width = EditorGUILayout.FloatField("Width", me.Width);
        me.Height = EditorGUILayout.FloatField("Height", me.Height);
        me.LeftBorder = EditorGUILayout.FloatField("Left Border", me.LeftBorder);
        me.RightBorder = EditorGUILayout.FloatField("Right Border", me.RightBorder);
        me.TopBorder = EditorGUILayout.FloatField("Top Border", me.TopBorder);
        me.BottomBorder = EditorGUILayout.FloatField("Bottom Border", me.BottomBorder);
    }
}
