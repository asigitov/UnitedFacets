// <copyright file="SimpleServer.cs" company="Institute of Visual Computing / Bonn-Rhein-Sieg University of Applied Sciences">
// Copyright (c) 2016-2017 All Rights Reserved
// </copyright>
// <author>Anton Sigitov</author>
// <summary></summary>

using UnityEngine;
using System.Collections;

public class SimpleServer : MonoBehaviour
{

    public string GameTypeName;
    public string GameName;

    private static SimpleServer instance = null;

    void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(transform.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(transform.gameObject);
        }
    }

    void Start()
    {

        NetworkConnectionError err = Network.InitializeServer(1, 250000, !Network.HavePublicAddress());
        if (err != NetworkConnectionError.NoError)
        {
            Debug.Log("Failed to initialize server!");
            Debug.Log(err.ToString());
        }

    }

    void OnServerInitialized()
    {
        Debug.Log("Server was successfully initialized!");
        MasterServer.RegisterHost(GameTypeName, GameName);
    }

    void OnMasterServerEvent(MasterServerEvent mse)
    {
        if (mse == MasterServerEvent.RegistrationSucceeded)
        {
            Debug.Log("Server registered!");
        }
    }

}
