using UnityEngine;
using System.Collections;

public class SimpleClient : MonoBehaviour
{

    void Awake()
    {
        MasterServer.RequestHostList("HornetABD");
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
                if (d.gameName.Equals("IVC"))
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
            MasterServer.RequestHostList("HornetABD");
        }
    }

    void OnDestroy()
    {
        Network.Disconnect();
    }

}
