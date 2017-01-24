// <copyright file="MPIEnvironment.cs" company="Institute of Visual Computing / Bonn-Rhein-Sieg University of Applied Sciences">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Anton Sigitov</author>
// <summary>Class for management of MPI environment and for sending of MPI messages</summary>

using UnityEngine;
using UnityEngine.UI;
using MPI;

public class MPIEnvironment : MonoBehaviour
{
    // global MPI environment
    private static Environment environment;

    public static bool Initialized = false;
    public static bool Manager = false;
    public static int Rank = -1;
    public static bool Simulate = false;
    public static int ManagerRank = -1;
    public static int WorldSize = -1;

    public Text RankLabel; 

    // GUI
    public int expectedWorldSize = 1;
    public int managerRank = 0;
    public bool simulate = false;

    private const int bufferSize = 128;

    // The MPI environment will be initialized here
    void Awake()
    {
        // Ensure there is only one MPI environment
        if (environment != null)
        {
            DestroyImmediate(this);
            return;
        }

        if (simulate)
        {
            Initialized = true;
            Manager = true;
            Rank = 0;
            WorldSize = 0;
            Simulate = true;
        }
        else
        {
            string[] args = System.Environment.GetCommandLineArgs();
            environment = new Environment(ref args);
            if (Communicator.world != null && Communicator.world.Size == expectedWorldSize)
            {
                // We have to set anyTag, since it is not initialized properly otherwise
                Communicator.anyTag = 1000;
                Initialized = true;
                ManagerRank = managerRank;
                WorldSize = Communicator.world.Size;
                Rank = Communicator.world.Rank;

                if(RankLabel != null)
                {
                    RankLabel.text = string.Format("Rank: {0}", Rank);
                }

                if (Rank == managerRank)
                {
                    Manager = true;
                }
            }
            // TODO: If world.Size != expectedWorldSize -> dispose environment
        }

        // We need this in order to preserve the MPI environemnt on scene change
        DontDestroyOnLoad(this.gameObject);
    }

    #region Transform distribution

    //##############################
    // TRANSFORM DISTRIBUTION
    //##############################

    public static void BroadcastTransform(GameObject go, int managerRank)
    {
        if (Simulate)
            return;

        if (Initialized)
        {
            float[] values = new float[10];
            if (Communicator.world.Rank == managerRank)
            {
                values[0] = go.transform.position.x;
                values[1] = go.transform.position.y;
                values[2] = go.transform.position.z;

                values[3] = go.transform.rotation.x;
                values[4] = go.transform.rotation.y;
                values[5] = go.transform.rotation.z;
                values[6] = go.transform.rotation.w;

                values[7] = go.transform.localScale.x;
                values[8] = go.transform.localScale.y;
                values[9] = go.transform.localScale.z;

                Communicator.world.Broadcast(ref values, managerRank);
            }
            else
            {
                Communicator.world.Broadcast(ref values, managerRank);
                go.transform.position = new Vector3(values[0], values[1], values[2]);
                go.transform.rotation = new Quaternion(values[3], values[4], values[5], values[6]);
                go.transform.localScale = new Vector3(values[7], values[8], values[9]);
            }
        }
    }

    public static void BroadcastLocalPosition(GameObject go, int managerRank)
    {
        if (Simulate)
            return;

        if (Initialized)
        {
            float[] values = new float[3];
            if (Communicator.world.Rank == managerRank)
            {
                values[0] = go.transform.localPosition.x;
                values[1] = go.transform.localPosition.y;
                values[2] = go.transform.localPosition.z;
                
                Communicator.world.Broadcast(ref values, managerRank);
            }
            else
            {
                Communicator.world.Broadcast(ref values, managerRank);
                go.transform.localPosition = new Vector3(values[0], values[1], values[2]);
            }
        }
    }

    public static void SendTransform(GameObject go, int destination)
    {
        if (Simulate)
            return;

        if (Initialized)
        {
            float[] values = new float[10];
            values[0] = go.transform.position.x;
            values[1] = go.transform.position.y;
            values[2] = go.transform.position.z;

            values[3] = go.transform.rotation.x;
            values[4] = go.transform.rotation.y;
            values[5] = go.transform.rotation.z;
            values[6] = go.transform.rotation.w;

            values[7] = go.transform.localScale.x;
            values[8] = go.transform.localScale.y;
            values[9] = go.transform.localScale.z;
            Communicator.world.Send<float>(values, destination, Communicator.anyTag);
        }
    }

    public static void ReceiveTransform(GameObject go, int source)
    {
        if (Simulate)
            return;

        if (Initialized)
        {
            float[] values = new float[10];
            Communicator.world.Receive<float>(source, Communicator.anyTag, ref values);
            go.transform.position = new Vector3(values[0], values[1], values[2]);
            go.transform.rotation = new Quaternion(values[3], values[4], values[5], values[6]);
            go.transform.localScale = new Vector3(values[7], values[8], values[9]);
        }
    }

    public static void Send2AllTransform(GameObject go, int viewID)
    {
        if (Simulate)
            return;

        if (Initialized)
        {
            byte[] buffer = new byte[128];
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer))
            {
                using (System.IO.BinaryWriter bw = new System.IO.BinaryWriter(ms))
                {
                    bw.Write(MessageCodes.UpdateTransform);
                    bw.Write(viewID);
                    bw.Write(go.transform.position.x);
                    bw.Write(go.transform.position.y);
                    bw.Write(go.transform.position.z);
                    bw.Write(go.transform.rotation.x);
                    bw.Write(go.transform.rotation.y);
                    bw.Write(go.transform.rotation.z);
                    bw.Write(go.transform.rotation.w);
                    bw.Write(go.transform.localScale.x);
                    bw.Write(go.transform.localScale.y);
                    bw.Write(go.transform.localScale.z);
                }
            }

            for (int i = 0; i < MPIEnvironment.WorldSize; i++)
            {
                if (i == MPIEnvironment.ManagerRank)
                    continue;

                Communicator.world.Send<byte>(buffer, i, Communicator.anyTag);
            }
        }
    }

    public static void Send2IdTransform(GameObject go, ref int[] TransformPackage, int dest)
    {
        if (Simulate)
            return;

        if (Initialized)
        {
            byte[] buffer = new byte[bufferSize];
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer))
            {
                using (System.IO.BinaryWriter bw = new System.IO.BinaryWriter(ms))
                {
                    bw.Write(MessageCodes.UpdateTransform);
                    bw.Write(TransformPackage[1]);
                    bw.Write(go.transform.position.x);
                    bw.Write(go.transform.position.y);
                    bw.Write(go.transform.position.z);
                    bw.Write(go.transform.rotation.x);
                    bw.Write(go.transform.rotation.y);
                    bw.Write(go.transform.rotation.z);
                    bw.Write(go.transform.rotation.w);
                    bw.Write(go.transform.localScale.x);
                    bw.Write(go.transform.localScale.y);
                    bw.Write(go.transform.localScale.z);
                }
            }


            Communicator.world.Send<byte>(buffer, dest, Communicator.anyTag);

        }
    }

    public static void Send2IdsTransform(GameObject go, int viewID, int[] dests)
    {
        if (Simulate)
            return;

        if (Initialized)
        {
            byte[] buffer = new byte[bufferSize];
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer))
            {
                using (System.IO.BinaryWriter bw = new System.IO.BinaryWriter(ms))
                {
                    bw.Write(MessageCodes.UpdateTransform);
                    bw.Write(viewID);
                    bw.Write(go.transform.position.x);
                    bw.Write(go.transform.position.y);
                    bw.Write(go.transform.position.z);
                    bw.Write(go.transform.rotation.x);
                    bw.Write(go.transform.rotation.y);
                    bw.Write(go.transform.rotation.z);
                    bw.Write(go.transform.rotation.w);
                    bw.Write(go.transform.localScale.x);
                    bw.Write(go.transform.localScale.y);
                    bw.Write(go.transform.localScale.z);
                }
            }

            foreach (int dest in dests)
            {
                Communicator.world.Send<byte>(buffer, dest, Communicator.anyTag);
            }

        }
    }

    public static void Send2IdsTransform(GameObject go, int viewID, ref bool[] dests)
    {
        if (Simulate)
            return;

        if (Initialized)
        {
            byte[] buffer = new byte[bufferSize];
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer))
            {
                using (System.IO.BinaryWriter bw = new System.IO.BinaryWriter(ms))
                {
                    bw.Write(MessageCodes.UpdateTransform);
                    bw.Write(viewID);
                    bw.Write(go.transform.position.x);
                    bw.Write(go.transform.position.y);
                    bw.Write(go.transform.position.z);
                    bw.Write(go.transform.rotation.x);
                    bw.Write(go.transform.rotation.y);
                    bw.Write(go.transform.rotation.z);
                    bw.Write(go.transform.rotation.w);
                    bw.Write(go.transform.localScale.x);
                    bw.Write(go.transform.localScale.y);
                    bw.Write(go.transform.localScale.z);
                }
            }

            for (int i = 0; i < dests.Length; i++)
            {
                if (dests[i])
                    Communicator.world.Send<byte>(buffer, i, Communicator.anyTag);
            }

        }
    }

    #endregion

    #region Code distribution

    public static void Send2AllCode(int code, int managerRank)
    {
        if (Simulate)
            return;

        if (Initialized)
        {
            byte[] buffer = new byte[bufferSize];
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer))
            {
                using (System.IO.BinaryWriter bw = new System.IO.BinaryWriter(ms))
                {
                    bw.Write(code);

                }
            }

            for (int i = 0; i < MPIEnvironment.WorldSize; i++)
            {
                if (i == MPIEnvironment.ManagerRank)
                    continue;

                Communicator.world.Send<byte>(buffer, i, Communicator.anyTag);
            }
            //}

        }
    }

    #endregion

    #region Linerenderer distribution

    //##############################
    // LINERENDERER DISTRIBUTION
    //##############################

    public static void SendLineRenderer(LineRendererExtended lr, int destination)
    {
        if (Simulate || !Initialized)
        {
            return;
        }

        // send num points first
        int numPositions = lr.GetVertexCount();
        Communicator.world.Send<int>(numPositions, destination, Communicator.anyTag);
        if (numPositions == 0)
            return;

        // send positions next
        // temp structure -> only points
        // n x 3 floats - points
        float[] points = new float[numPositions * 3];
        int index = 0;
        for (int i = 0; i < numPositions; i++)
        {
            Vector3 v = Vector3.zero;
            lr.GetPosition(i, ref v);
            points[index] = v.x;
            index++;
            points[index] = v.y;
            index++;
            points[index] = v.z;
            index++;
        }

        Communicator.world.Send<float>(points, destination, Communicator.anyTag);
    }

    public static void ReceiveLineRenderer(LineRendererExtended lr, int source)
    {
        if (Simulate || !Initialized)
        {
            return;
        }

        // get num positions first
        int numPositions = 0;
        Communicator.world.Receive<int>(source, 1000, out numPositions);
        lr.SetVertexCount(numPositions);
        if (numPositions == 0)
            return;

        float[] positions = new float[numPositions * 3];
        Communicator.world.Receive<float>(source, Communicator.anyTag, ref positions);
        int index = 0;
        for (int i = 0; i < positions.Length; i = i + 3)
        {
            Vector3 v = new Vector3(positions[i], positions[i + 1], positions[i + 2]);
            lr.SetPosition(index, v);
            index++;
        }
    }

    #endregion

    #region Integer distribution

    //##############################
    // INTEGER DISTRIBUTION
    //##############################

    public static void SendInt(int value, int destination)
    {
        if (Simulate)
            return;

        if (Initialized)
        {
            Communicator.world.Send<int>(value, destination, Communicator.anyTag);
        }
    }

    public static void SendInt2(ref int[] values, int destination)
    {
        if (Simulate)
            return;

        if (Initialized)
        {
            Communicator.world.Send<int>(values, destination, Communicator.anyTag);
        }
    }

    public static void ReceiveInt(out int value, int source)
    {
        if (Simulate)
        {
            value = -1;
            return;
        }

        if (Initialized)
        {
            Communicator.world.Receive<int>(source, Communicator.anyTag, out value);
        }
        else
        {
            value = -1;
        }
    }

    public static void ReceiveInt2(ref int[] values, int source)
    {
        if (Simulate)
        {
            return;
        }

        if (Initialized)
        {
            Communicator.world.Receive<int>(source, Communicator.anyTag, ref values);
        }
    }

    public static void BroadcastInt(ref int value, int managerRank)
    {
        if (Simulate)
            return;

        if (Initialized)
        {
            if (Communicator.world.Rank == managerRank)
            {
                Communicator.world.Broadcast(ref value, managerRank);
            }
            else
            {
                Communicator.world.Broadcast(ref value, managerRank);
            }
        }
    }

    public static void BroadcastInt2(ref int[] values, int managerRank)
    {
        if (Simulate)
            return;

        if (Initialized)
        {
            if (Communicator.world.Rank == managerRank)
            {
                Communicator.world.Broadcast<int>(ref values, managerRank);
            }
            else
            {
                Communicator.world.Broadcast<int>(ref values, managerRank);
            }
        }
    }

    #endregion

    #region Bytes distribution

    // ***********************
    // BYTES
    // ***********************

    public static void SendByteArray(ref byte[] buffer, int dest)
    {
        if (Simulate)
            return;

        if (Initialized)
        {

            Communicator.world.Send<byte>(buffer, dest, Communicator.anyTag);

        }
    }

    public static void ReceiveByteArray(ref byte[] buf, int source)
    {
        if (Simulate)
            return;

        if (Initialized)
        {
            Communicator.world.Receive<byte>(source, Communicator.anyTag, ref buf);

        }
    }

    #endregion

    #region Event distribution

    //*********************
    // Event
    //*********************

    public static void FireEvent(int eventID)
    {
        if (Simulate)
            return;

        if (Initialized)
        {
            byte[] buffer = new byte[sizeof(int)*2];
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer))
            {
                using (System.IO.BinaryWriter bw = new System.IO.BinaryWriter(ms))
                {
                    bw.Write(MessageCodes.FireEvent);
                    bw.Write(eventID);
                }
            }

            for (int i = 0; i < MPIEnvironment.WorldSize; i++)
            {
                if (i == MPIEnvironment.ManagerRank)
                    continue;

                Communicator.world.Send<byte>(buffer, i, Communicator.anyTag);
            }
        }
    }

    public static void FireEvent(int eventID, int dest)
    {
        if (Simulate)
            return;

        if (Initialized)
        {
            byte[] buffer = new byte[sizeof(int) * 2];
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer))
            {
                using (System.IO.BinaryWriter bw = new System.IO.BinaryWriter(ms))
                {
                    bw.Write(MessageCodes.FireEvent);
                    bw.Write(eventID);
                }
            }

            Communicator.world.Send<byte>(buffer, dest, Communicator.anyTag);
            
        }
    }

    public static void FireEvent(int eventID, int[] dest)
    {
        if (Simulate)
            return;

        if (Initialized)
        {
            byte[] buffer = new byte[sizeof(int) * 2];
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer))
            {
                using (System.IO.BinaryWriter bw = new System.IO.BinaryWriter(ms))
                {
                    bw.Write(MessageCodes.FireEvent);
                    bw.Write(eventID);
                }
            }

            foreach(int n in dest)
            {
                Communicator.world.Send<byte>(buffer, n, Communicator.anyTag);
            }
        }
    }

    #endregion
}
