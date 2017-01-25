// <copyright file="DisplaySetup.cs" company="Institute of Visual Computing / Bonn-Rhein-Sieg University of Applied Sciences">
// Copyright (c) 2016-2017 All Rights Reserved
// </copyright>
// <author>Anton Sigitov</author>
// <summary></summary>

using System;
using System.Collections.Generic;
using UnityEngine;

public class DisplaySetup : ScriptableObject
{
    [SerializeField]
    private int dialogStep;
    public int DialogStep
    {
        get { return dialogStep; }
        set { dialogStep = value; }
    }

    [SerializeField]
    private int numDUH;
    public int NumDUH
    {
        get { return numDUH; }
        set { numDUH = value; }
    }

    [SerializeField]
    private int numDUV;
    public int NumDUV
    {
        get { return numDUV; }
        set { numDUV = value; }
    }

    [SerializeField]
    private DisplayUnit du;
    public DisplayUnit DU
    {
        get { return du; }
        set { du = value; }
    }

    [SerializeField]
    private string description;
    public string Description
    {
        get { return description; }
        set { description = value; }
    }

    public enum ProjectionType
    {
        Perspective,
        Orthogonal
    }
    [SerializeField]
    private ProjectionType projection;
    public ProjectionType Projection
    {
        get { return projection; }
        set { projection = value; }
    }

    [SerializeField]
    private List<DisplayUnitInternal> screens = new List<DisplayUnitInternal>();
    public List<DisplayUnitInternal> Screens { get { return screens; } }

    [SerializeField]
    private int mainScreenIndex;
    public int MainScreenIndex
    {
        get { return mainScreenIndex; }
        set { mainScreenIndex = value; }
    }

    public DisplayUnitInternal MainScreen { get { return screens[mainScreenIndex]; } }
}