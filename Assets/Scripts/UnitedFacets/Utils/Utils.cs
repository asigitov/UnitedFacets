// <copyright file="Utils.cs" company="Institute of Visual Computing / Bonn-Rhein-Sieg University of Applied Sciences">
// Copyright (c) 2016-2017 All Rights Reserved
// </copyright>
// <author>David Scherfgen, Anton Sigitov</author>
// <summary></summary>

using System;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{

    public static Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
    {
        float x = 2 / (right - left);
        float y = 2 / (top - bottom);
        float a = (right + left) / (right - left);
        float b = (top + bottom) / (top - bottom);
        float c = -(far + near) / (far - near);
        float d = -(2 * far * near) / (far - near);
        float e = -1;
        Matrix4x4 m = new Matrix4x4();
        m[0, 0] = x;
        m[0, 1] = 0;
        m[0, 2] = a;
        m[0, 3] = 0;
        m[1, 0] = 0;
        m[1, 1] = y;
        m[1, 2] = b;
        m[1, 3] = 0;
        m[2, 0] = 0;
        m[2, 1] = 0;
        m[2, 2] = c;
        m[2, 3] = d;
        m[3, 0] = 0;
        m[3, 1] = 0;
        m[3, 2] = e;
        m[3, 3] = 0;
        return m;
    }

    public static Matrix4x4 OrthogonalOffCenter(float left, float right, float bottom, float top, float near, float far)
    {
        float x = 2.0F / (right - left);
        float y = 2.0F / (top - bottom);
        float z = -2.0F / (far - near);
        float a = -((right + left) / (right - left));
        float b = -((top + bottom) / (top - bottom));
        float c = -((far + near) / (far - near));
        float e = 1.0F;
        Matrix4x4 m = new Matrix4x4();
        m[0, 0] = x;
        m[0, 1] = 0;
        m[0, 2] = 0;
        m[0, 3] = a;
        m[1, 0] = 0;
        m[1, 1] = y;
        m[1, 2] = 0;
        m[1, 3] = b;
        m[2, 0] = 0;
        m[2, 1] = 0;
        m[2, 2] = z;
        m[2, 3] = c;
        m[3, 0] = 0;
        m[3, 1] = 0;
        m[3, 2] = 0;
        m[3, 3] = e;
        return m;
    }

}