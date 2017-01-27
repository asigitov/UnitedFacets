// <copyright file="MPIObjectManager.cs" company="Institute of Visual Computing / Bonn-Rhein-Sieg University of Applied Sciences">
// Copyright (c) 2016-2017 All Rights Reserved
// </copyright>
// <author>Anton Sigitov</author>
// <summary></summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MPIObjectManager : MonoBehaviour
{

    [SerializeField]
    private List<MPIView> views = new List<MPIView>();
    [SerializeField]
    private int objectCounter = 0;

    View v;

    void Start()
    {
        v = GameObject.FindObjectOfType<View>();
    }

    public void RegisterMPIViews()
    {
        objectCounter = 0;
        views.Clear();
        MPIView[] objs = GameObject.FindObjectsOfType<MPIView>();
        foreach (MPIView o in objs)
        {
            o.MPIViewID = objectCounter;
            views.Add(o);
            objectCounter++;
        }
    }

    public void RegisterMPIView(MPIView mv)
    {
        if (!views.Contains(mv))
        {
            mv.MPIViewID = objectCounter++;
            views.Add(mv);
        }
    }

    public void UnregisterMPIView(MPIView mv)
    {
        views.Remove(mv);
    }

    public void RemoveMPIView(int id)
    {
        views.RemoveAll(x => x.MPIViewID == id);
    }

    public MPIView GetMPIViewWithID(int id)
    {
        return views.Find(x => x.MPIViewID == id);
    }

    public GameObject GetGameObjectWithID(int id)
    {
        MPIView v = views.Find(x => x.MPIViewID == id);
        if (v != null)
        {
            return v.gameObject;
        }

        return null;
    }

    void Update()
    {
        if (!MPIEnvironment.Manager)
            return;

        if (v.Approach == View.SyncApproach.Cn_Adaptive)
        {
            foreach (MPIView mv in views)
            {
                if (mv.gameObject.activeInHierarchy)
                    mv.Dispatch();
            }
        }

    }

    // TODO: add a method for a single object registration (react to events leading to object creation)
}
