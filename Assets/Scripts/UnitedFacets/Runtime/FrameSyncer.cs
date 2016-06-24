using UnityEngine;
using System.Collections;
using MPI;

public class FrameSyncer : MonoBehaviour
{
    void Start()
    {
        if (MPIEnvironment.Simulate)
            return;

        if (MPIEnvironment.Initialized)
        {
            StartCoroutine(Sync());
        }
    }

    IEnumerator Sync()
    {
        while (MPIEnvironment.Initialized)
        {
            yield return new WaitForEndOfFrame();
            if (MPIEnvironment.Manager)
            {
                MPIEnvironment.Send2AllCode(MessageCodes.EndOfFrame, MPIEnvironment.Rank);
            }

            Communicator.world.Barrier();
        }
    }
}
