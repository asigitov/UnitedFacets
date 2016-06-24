using UnityEngine;
using System.Collections;

public class SimpleClientMPI : MonoBehaviour {

    public string GameTypeName;
    public string GameName;

    void Awake()
    {
        if(!MPIEnvironment.Manager)
        {
            DestroyImmediate(this);
            return;
        }
        MasterServer.RequestHostList(GameTypeName);
    }

    void OnMasterServerEvent(MasterServerEvent mse)
    {
        if (mse == MasterServerEvent.HostListReceived)
        {
            Debug.Log("Host list received!");
            HostData[] data = MasterServer.PollHostList();
            Debug.Log("Num hosts: " + data.Length);
            foreach (HostData d in data)
            {
                if (d.gameName.Equals(GameName))
                {
                    NetworkConnectionError err = Network.Connect(d);
                    if (err != NetworkConnectionError.NoError)
                    {
                        Debug.Log("Failed to connect to server!");
                    }
                    else
                    {
                        Debug.Log("Successfully connected to server!");
                    }
                    break;
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            MasterServer.RequestHostList(GameTypeName);
        }
    }

    void OnDestroy()
    {
        Network.Disconnect();
    }
}
