// <copyright file="RotateY.cs" company="Institute of Visual Computing / Bonn-Rhein-Sieg University of Applied Sciences">
// Copyright (c) 2016-2017 All Rights Reserved
// </copyright>
// <author>Anton Sigitov</author>
// <summary></summary>

using UnityEngine;
using System.Collections;

public class RotateY : MonoBehaviour {

    public float speed = 1.0f;
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.Rotate(0, Time.deltaTime * speed, 0);
	}
}
