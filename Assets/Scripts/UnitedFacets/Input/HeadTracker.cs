using UnityEngine;
using System.Collections;

public class HeadTracker : MonoBehaviour
{

    public Tracker tracker;
    public View view;
    public int trackerHeadID;
    public float scaleFactor;

    public bool invertX;
    public bool invertY;
    public bool invertZ;

    private Vector3 values;

    // Update is called once per frame
    void Update()
    {
        if (tracker != null && view != null)
        {
            values = tracker.GetBodyPosition(trackerHeadID) * scaleFactor;

            if(invertX)
            {
                values.x *= -1;
            }

            if (invertY)
            {
                values.y *= -1;
            }

            if (invertZ)
            {
                values.z *= -1;
            }

            view.HeadOffset = values;
        }
    }
}
