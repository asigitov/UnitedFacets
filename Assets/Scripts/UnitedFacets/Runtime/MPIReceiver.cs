// <copyright file="MPIReceiver.cs" company="Institute of Visual Computing / Bonn-Rhein-Sieg University of Applied Sciences">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Anton Sigitov</author>
// <summary>Class for receiving and processing of MPI messages from the Manager</summary>

using UnityEngine;
using System.Collections.Generic;

public class MPIReceiver : MonoBehaviour
{
    private MPIObjectManager mom;
    byte[] buffer = new byte[128];

    void Start()
    {
        if (MPIEnvironment.Manager)
        {
            DestroyImmediate(this);
        }

        mom = FindObjectOfType<MPIObjectManager>();
    }

    void Update()
    {
        if (MPIEnvironment.Simulate || MPIEnvironment.Manager)
            return;

        bool done = false;

        while (!done)
        {

            int code;
            int vid;
            MPIEnvironment.ReceiveByteArray(ref buffer, MPIEnvironment.ManagerRank);
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer))
            {
                using (System.IO.BinaryReader br = new System.IO.BinaryReader(ms))
                {
                    code = br.ReadInt32();
                    vid = br.ReadInt32();

                    if (code == MessageCodes.EndOfFrame)
                    {
                        done = true;
                        break;
                    }
                    else if (code == MessageCodes.UpdateTransform)
                    {
                        GameObject go = mom.GetGameObjectWithID(vid);
                        go.transform.position = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                        go.transform.rotation = new Quaternion(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                        go.transform.localScale = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    }
                    else if (code == MessageCodes.FireEvent)
                    {
                        FireEvent(vid);
                    }
                }
            }
        }
    }

    public delegate void EventCallback();
    private static Dictionary<int, List<EventCallback>> callbacks = new Dictionary<int, List<EventCallback>>();

    public static void SubscribeToEvent(int eventID, EventCallback callback)
    {
        if (callbacks.ContainsKey(eventID))
        {
            if (!callbacks[eventID].Contains(callback))
                callbacks[eventID].Add(callback);
        }
        else
        {
            callbacks[eventID] = new List<EventCallback>();
            callbacks[eventID].Add(callback);
        }
    }

    public static void UnsubscribeFromEvent(int eventID, EventCallback callback)
    {
        if (callbacks.ContainsKey(eventID))
        {
            callbacks[eventID].Remove(callback);
            if (callbacks[eventID].Count == 0)
            {
                callbacks.Remove(eventID);
            }
        }
    }

    private static void FireEvent(int eventID)
    {
        if (!callbacks.ContainsKey(eventID))
            return;

        foreach (EventCallback ec in callbacks[eventID])
        {
            ec();
        }
    }
}
