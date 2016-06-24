using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MPIView : MonoBehaviour
{
    // TODO make not editable
    public int MPIViewID = -1;
    public int NumEchos = 2;
    public Component[] ComponentsToRemove;
    public MonoBehaviour scriptToUpdate;

    private Dictionary<int, int> ranks = new Dictionary<int, int>();
    //private List<int> toUpdate = new List<int>();
    private View vc;
    //private MPIView[] objs;
    private Plane[] planes;
    //private int[] TransformPackage = new int[2] { MessageCodes.UpdateTransform, 100 };

    private List<DisplayUnitInternal> screens;

    bool[] dests;
    int[] rs;

    void Start()
    {
        // If simulation is ON, do nothing
        if (MPIEnvironment.Simulate || !MPIEnvironment.Manager)
            return;

        if (!MPIEnvironment.Manager)
        {
            foreach (Component c in ComponentsToRemove)
            {
                if (c != null)
                    DestroyImmediate(c);
            }
        }

        // Get View instance
        vc = GameObject.FindObjectOfType<View>();
        screens = GameObject.FindObjectOfType<View>().ActiveScreenSetup.Screens;
        dests = new bool[MPIEnvironment.WorldSize];
        rs = new int[MPIEnvironment.WorldSize];

        for (int i = 0; i < MPIEnvironment.WorldSize; i++)
        {
            if (i == MPIEnvironment.Rank)
            {
                continue;
            }
            ranks.Add(i, NumEchos);
        }


    }

    //private List<int> destinations = new List<int>();

    void Update()
    {
        if (MPIEnvironment.Simulate || !MPIEnvironment.Manager)
            return;

        if (MPIEnvironment.Initialized)
        {
            if (vc.Approach == View.SyncApproach.None)
            {
                return;
            }
            // Naive broadcast
            // Distribute states of all objects, which were updated, to all nodes
            else if (vc.Approach == View.SyncApproach.Naive)
            {
                // TODO: check if object was updated
                // This function is probably slow, keep an eye on it
                if (scriptToUpdate == null || scriptToUpdate.GetType() == typeof(Transform))
                {
                    MPIEnvironment.Send2AllTransform(gameObject, MPIViewID);
                }
                // TODO: add other scripts

            }
            // C1 approach
            // Sumbits the state of an objects, if it lays within the frustum of the camera
            // Problems with shadows.
            else if (vc.Approach == View.SyncApproach.C1_Adaptive)
            {
                foreach (DisplayUnitInternal screen in screens)
                {
                    dests[screen.MPIRank] = false;
                    if (screen.MPIRank == MPIEnvironment.ManagerRank)
                    {
                        continue;
                    }

                    // calculate frustum planes of a camera
                    planes = GeometryUtility.CalculateFrustumPlanes(screen.ScreenCam.camera);
                    if (gameObject.renderer != null)
                    {
                        if (GeometryUtility.TestPlanesAABB(planes, gameObject.renderer.bounds))
                        {
                            ranks[screen.MPIRank] = NumEchos;

                            if (scriptToUpdate == null || scriptToUpdate.GetType() == typeof(Transform))
                            {
                                dests[screen.MPIRank] = true;
                            }
                            // TODO: add other scripts
                        }
                        else
                        {
                            if (ranks[screen.MPIRank] > 0)
                            {
                                ranks[screen.MPIRank]--;
                                if (scriptToUpdate == null || scriptToUpdate.GetType() == typeof(Transform))
                                {
                                    dests[screen.MPIRank] = true;
                                }
                                // TODO: add other scripts
                            }
                        }

                    }

                }

                MPIEnvironment.Send2IdsTransform(gameObject, MPIViewID, ref dests);
            }
            // Cn approach
            // Conveys object's state to a node, only if the node's camera sees the object
            // Problem: manager node has to render all cameras; Shadows
            else if (vc.Approach == View.SyncApproach.Cn_Adaptive)
            {
                for (int d = 0; d < dests.Length; d++)
                {
                    dests[d] = false;
                }

                for (int i = 0; i < rs.Length; i++)
                {
                    if (rs[i] > 0)
                    {
                        rs[i] = rs[i] - 1;
                        dests[i] = true;
                    }
                }
            }

        }
    }

    void OnWillRenderObject()
    {
        // This callback is used only with Cn approach
        if (MPIEnvironment.Simulate || vc.Approach != View.SyncApproach.Cn_Adaptive || !MPIEnvironment.Manager)
            return;

        if (MPIEnvironment.Initialized)
        {
            DisplayUnitInternal screen = vc.ActiveScreenSetup.Screens.Find(x => x.Name == Camera.current.name);
            if (screen.MPIRank == MPIEnvironment.Rank)
                return;

            if (scriptToUpdate == null || scriptToUpdate.GetType() == typeof(Transform))
            {
                dests[screen.MPIRank] = true;
                rs[screen.MPIRank] = NumEchos;
            }
        }
    }

    public void Dispatch()
    {
        MPIEnvironment.Send2IdsTransform(gameObject, MPIViewID, ref dests);
    }
}
