// <copyright file="DisplayUnit.cs" company="Institute of Visual Computing / Bonn-Rhein-Sieg University of Applied Sciences">
// Copyright (c) 2016-2017 All Rights Reserved
// </copyright>
// <author>Anton Sigitov</author>
// <summary></summary>

using UnityEngine;
using System.Collections;

public class DisplayUnit : ScriptableObject
{
    [SerializeField]
    private string displayTypeName;
    public string TypeName
    {
        get { return displayTypeName; }
        set { displayTypeName = value; }
    }

    [SerializeField]
    private float width;
    public float Width
    {
        get { return width; }
        set { width = Mathf.Max(0, value); }
    }

    [SerializeField]
    private float height;
    public float Height
    {
        get { return height; }
        set { height = Mathf.Max(0, value); }
    }

    [SerializeField]
    private float leftBorder;
    public float LeftBorder
    {
        get { return leftBorder; }
        set { leftBorder = Mathf.Max(0, value); }
    }

    [SerializeField]
    private float rightBorder;
    public float RightBorder
    {
        get { return rightBorder; }
        set { rightBorder = Mathf.Max(0, value); }
    }

    [SerializeField]
    private float topBorder;
    public float TopBorder
    {
        get { return topBorder; }
        set { topBorder = Mathf.Max(0, value); }
    }

    [SerializeField]
    private float bottomBorder;
    public float BottomBorder
    {
        get { return bottomBorder; }
        set { bottomBorder = Mathf.Max(0, value); }
    }

    
}
