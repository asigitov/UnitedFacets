// <copyright file="DisplaySetup.cs" company="Institute of Visual Computing / Bonn-Rhein-Sieg University of Applied Sciences">
// Copyright (c) 2016-2017 All Rights Reserved
// </copyright>
// <author>Anton Sigitov</author>
// <summary>Contains an example of how to use MPIEvent</summary>

using UnityEngine;
using System.Collections;
using System;

public class MPIEventExample : MonoBehaviour
{

    public GameObject prototype;
    private GameObject clone;

    // Use this for initialization
    void Start()
    {
        if (!MPIEnvironment.Manager)
        {
            // Subsribe to events
            MPIReceiver.SubscribeToEvent(47, OnCreateObject);
            MPIReceiver.SubscribeToEvent(48, OnDestroyObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (MPIEnvironment.Manager)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if(prototype != null && clone == null)
                {
                    MPIEnvironment.FireEvent(47);
                    OnCreateObject();
                    StartCoroutine(DestroyAfterSeconds());
                }
            }
        }
    }

    private IEnumerator DestroyAfterSeconds()
    {
        yield return new WaitForSeconds(2.0f);
        MPIEnvironment.FireEvent(48);
        OnDestroyObject();
    }

    void OnCreateObject()
    {
        clone = Instantiate(prototype) as GameObject;
        MPIView mv = clone.AddComponent<MPIView>();
        FindObjectOfType<MPIObjectManager>().RegisterMPIView(mv);
    }

    void OnDestroyObject()
    {
        FindObjectOfType<MPIObjectManager>().UnregisterMPIView(clone.GetComponent<MPIView>());
        DestroyImmediate(clone);
    }
}
