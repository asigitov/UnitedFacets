// <copyright file="FPSDisplay.cs" company="Institute of Visual Computing / Bonn-Rhein-Sieg University of Applied Sciences">
// Copyright (c) 2016-2017 All Rights Reserved
// </copyright>
// <author>Anton Sigitov</author>
// <summary></summary>

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FPSdisplay : MonoBehaviour
{

    public Text label;
    float deltaTime;
    bool visible;
    float fps;

    void Start()
    {
        deltaTime = 0.0f;
        visible = label.gameObject.activeSelf;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.F))
        {
            visible = !visible;
            label.gameObject.SetActive(visible);
        }

        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        if (visible & Time.frameCount % 1 == 0)
        {
            fps = (1.0f / deltaTime);
            label.text = string.Format("FPS: {0}", fps.ToString());
        }
    }
}
