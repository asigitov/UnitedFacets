using UnityEngine;

public class MPIReceiver : MonoBehaviour
{
    private MPIObjectManager mom;
    //private View vc;
    //private int[] package = new int[2];
    byte[] buffer = new byte[128];

    void Start()
    {
        if (MPIEnvironment.Manager)
        {
            DestroyImmediate(this);
        }

        mom = FindObjectOfType<MPIObjectManager>();
        //vc = FindObjectOfType<View>();
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

                    if (code == MessageCodes.UpdateTransform)
                    {
                        GameObject go = mom.GetGameObjectWithID(vid);
                        go.transform.position = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                        go.transform.rotation = new Quaternion(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                        go.transform.localScale = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    }
                }
            }
        }
    }

    void receiveTransform(int id)
    {
        GameObject go = mom.GetGameObjectWithID(id);
        MPIEnvironment.ReceiveTransform(go, MPIEnvironment.ManagerRank);
    }
}
