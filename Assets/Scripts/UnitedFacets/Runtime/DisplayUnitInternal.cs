// <copyright file="DisplayUnitInternal.cs" company="Institute of Visual Computing / Bonn-Rhein-Sieg University of Applied Sciences">
// Copyright (c) 2016-2017 All Rights Reserved
// </copyright>
// <author>Anton Sigitov</author>
// <summary></summary>

using UnityEngine;
using System;

[Serializable]
public class DisplayUnitInternal
{

    [SerializeField]
    private DisplayUnit prototype;
    public DisplayUnit Prototype
    {
        get { return prototype; }
        set
        {
            prototype = value;
            if (value != null)
            {
                TypeName = value.TypeName;
                Width = value.Width;
                Height = value.Height;
                LeftBorder = value.LeftBorder;
                RightBorder = value.RightBorder;
                TopBorder = value.TopBorder;
                BottomBorder = value.BottomBorder;
            }
        }
    }

    [SerializeField]
    private string displayTypeName;
    public string TypeName
    {
        get { return displayTypeName; }
        set { displayTypeName = value; }
    }

    [SerializeField]
    private string displayName;
    public string Name
    {
        get { return displayName; }
        set { displayName = value; }
    }

    [SerializeField]
    private string host;
    public string Host
    {
        get { return host; }
        set { host = value; }
    }

    [SerializeField]
    private int xscreen;
    public int XScreen
    {
        get { return xscreen; }
        set { xscreen = value; }
    }

    [SerializeField]
    private int mpiRank;
    public int MPIRank
    {
        get { return mpiRank; }
        set { mpiRank = value; }
    }

    [SerializeField]
    private bool enabled;
    public bool Enabled
    {
        get { return enabled; }
        set { enabled = value; }
    }

    [SerializeField]
    private bool selected;
    public bool Selected
    {
        get { return selected; }
        set { selected = value; }
    }

    [SerializeField]
    private Rect viewportRect;
    public Rect ViewportRect
    {
        get { return viewportRect; }
        set
        {
            viewportRect.x = Mathf.Clamp01(value.x);
            viewportRect.y = Mathf.Clamp01(value.y);
            viewportRect.width = Mathf.Clamp01(value.width);
            viewportRect.height = Mathf.Clamp01(value.height);
        }
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

    [SerializeField]
    private Vector3 center;
    public Vector3 Center
    {
        get { return center; }
        set { center = value; }
    }

    [SerializeField]
    private Vector3 orientationEuler;
    public Vector3 OrientationEuler
    {
        get { return orientationEuler; }
        set { orientationEuler = value; }
    }

    public Quaternion Orientation
    {
        get { return Quaternion.Euler(orientationEuler); }
        set { orientationEuler = value.eulerAngles; }
    }

    public Vector3 Right { get { return Orientation * Vector3.right; } }
    public Vector3 Left { get { return Orientation * Vector3.left; } }
    public Vector3 Up { get { return Orientation * Vector3.up; } }
    public Vector3 Down { get { return Orientation * Vector3.down; } }
    public Vector3 Normal { get { return Orientation * -Vector3.forward; } }
    public Plane Plane { get { return new Plane(Normal, Center); } }

    [SerializeField]
    private GameObject screenCam;
    public GameObject ScreenCam
    {
        get { return screenCam; }
        set { screenCam = value; }
    }

    public Vector3[] ComputeOutterCornerPoints()
    {
        Vector3[] corners = new Vector3[4];
        Vector3 right = 0.5f * width * Right;
        Vector3 up = 0.5f * height * Up;
        corners[0] = center - right + up;
        corners[1] = center + right + up;
        corners[2] = center + right - up;
        corners[3] = center - right - up;
        return corners;
    }

    public Vector3[] ComputeInnerCornerPoints()
    {
        Vector3[] corners = new Vector3[4];
        Vector3 right = (0.5f * width - rightBorder) * Right;
        Vector3 left = (0.5f * width - leftBorder) * Left;
        Vector3 up = (0.5f * height - topBorder) * Up;
        Vector3 down = (0.5f * height - bottomBorder) * Down;
        corners[0] = center + left + up;
        corners[1] = center + right + up;
        corners[2] = center + right + down;
        corners[3] = center + left + down;
        return corners;
    }


}
