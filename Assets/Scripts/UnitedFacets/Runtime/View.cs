// <copyright file="View.cs" company="Institute of Visual Computing / Bonn-Rhein-Sieg University of Applied Sciences">
// Copyright (c) 2016-2017 All Rights Reserved
// </copyright>
// <author>David Scherfgen, Anton Sigitov</author>
// <summary></summary>

using System;
using System.Collections.Generic;
using UnityEngine;

public class View : MonoBehaviour
{
    public enum SyncApproach
    {
        Naive,
        Cn_Adaptive,
        C1_Adaptive,
        None
    }

    [SerializeField]
    private SyncApproach approach;
    public SyncApproach Approach
    {
        get { return approach; }
        set { approach = value; }
    }

    public enum BorderOption
    {
        Ignore,
        CutOut
    }

    [SerializeField]
    private BorderOption borders;
    public BorderOption Borders
    {
        get { return borders; }
        set { borders = value; }
    }

    [SerializeField]
    private DisplaySetup activeDisplaySetup;
    public DisplaySetup ActiveDisplaySetup
    {
        get { return activeDisplaySetup; }
        set
        {
            if (activeDisplaySetup == value) return;
            activeDisplaySetup = value;

            SetupCameras();
        }
    }

    [SerializeField]
    private Vector3 cameraHeadPosition;
    public Vector3 CameraHeadPosition
    {
        get { return cameraHeadPosition; }
        set
        {
            if (cameraHeadPosition != value)
            {
                cameraHeadPosition = value;
                UpdateCameras();
            }
        }
    }

    [SerializeField]
    private bool mpiSync;
    public bool MPISync
    {
        get { return mpiSync; }
        set { mpiSync = value; }
    }

    [SerializeField]
    private bool useHeadTracking;
    public bool UseHeadTracking
    {
        get { return useHeadTracking; }
        set { useHeadTracking = value; }
    }

    [SerializeField]
    private float nearClippingPlane;
    public float NearClippingPlane
    {
        get { return nearClippingPlane; }
        set
        {
            value = Mathf.Clamp(value, 0, farClippingPlane);
            if (nearClippingPlane != value)
            {
                nearClippingPlane = value;
                UpdateCameras();
            }
        }
    }

    [SerializeField]
    private float farClippingPlane;
    public float FarClippingPlane
    {
        get { return farClippingPlane; }
        set
        {
            value = Mathf.Max(nearClippingPlane, value);
            if (farClippingPlane != value)
            {
                farClippingPlane = value;
                UpdateCameras();
            }
        }
    }

    [SerializeField]
    private bool useOcclusionCulling;
    public bool UseOcclusionCulling
    {
        get { return useOcclusionCulling; }
        set
        {
            if (useOcclusionCulling != value)
            {
                useOcclusionCulling = value;
                UpdateCameras();
            }
        }
    }

    [SerializeField]
    private GameObject cameraPrefab;
    public GameObject CameraPrefab
    {
        get { return cameraPrefab; }
        set { cameraPrefab = value; }
    }

    [HideInInspector]
    public Vector3 HeadOffset { get; set; }

    //private GameObject cam;
    private GameObject cameraHeadWrapper;
    private GameObject cameraHead;
    //private DisplayUnitInternal activeScreen;


    private void Awake()
    {
        // Prevent strange audio artifacts at the beginning.
        AudioListener.volume = 0;
        //DontDestroyOnLoad(transform.gameObject);
        // TODO yes or no???^^
    }

    private void Start()
    {
        SetupCameras();

        // Turn audio back on.
        AudioListener.volume = 1;
    }

    private void Update()
    {
        // With head tracking, the cameras have to be updated constantly.
        if (mpiSync) SyncCameras();
        if (useHeadTracking) UpdateCameras();
    }

    private void OnDrawGizmos()
    {
        if (activeDisplaySetup == null) return;

        Gizmos.matrix = transform.localToWorldMatrix;
        foreach (DisplayUnitInternal screen in activeDisplaySetup.Screens)
        {
            Vector3[] screenCorners = screen.ComputeOutterCornerPoints();

            // Draw the screen rectangle.
            // If the camera is behind the screen, draw it in red and strike it out.
            Color lineColor = screen.Enabled ? Color.white : Color.gray;
            bool cameraBehindScreen;
            if (cameraHead != null)
            {
                cameraBehindScreen = !screen.Plane.GetSide(cameraHead.transform.localPosition);
            }
            else
            {
                cameraBehindScreen = !screen.Plane.GetSide(cameraHeadPosition);
            }
            if (cameraBehindScreen) lineColor *= Color.red;

            Gizmos.color = lineColor;
            for (int i = 0; i < 4; ++i) Gizmos.DrawLine(screenCorners[i], screenCorners[(i + 1) % 4]);
            if (cameraBehindScreen)
            {
                Gizmos.DrawLine(screenCorners[0], screenCorners[2]);
                Gizmos.DrawLine(screenCorners[1], screenCorners[3]);
            }

            if (activeDisplaySetup != null && activeDisplaySetup.Projection == DisplaySetup.ProjectionType.Perspective)
                for (int i = 0; i < 4; ++i) Gizmos.DrawLine(screenCorners[i], cameraHeadPosition);

        }
    }

    private void SetupCameras()
    {
        // First clean up.
        if (cameraHeadWrapper != null)
        {
            DestroyImmediate(cameraHeadWrapper);
            DestroyImmediate(cameraHead);
            cameraHeadWrapper = null;
            cameraHead = null;
        }

        if (activeDisplaySetup == null) return;

        // We need an outer wrapper for camera effects such as shaking.
        cameraHeadWrapper = new GameObject("CameraHeadWrapper");
        cameraHeadWrapper.transform.parent = transform;
        cameraHeadWrapper.transform.localPosition = Vector3.zero;
        cameraHeadWrapper.transform.localRotation = Quaternion.identity;

        // Create a child game object that will contain all the cameras.
        // Also give it an audio listener.
        cameraHead = new GameObject("CameraHead", typeof(AudioListener));
        cameraHead.transform.parent = cameraHeadWrapper.transform;
        cameraHead.transform.localPosition = Vector3.zero;
        cameraHead.transform.localRotation = Quaternion.identity;

        if (approach == SyncApproach.Naive)
        {
            // Create one camera for the particular screen
            //DisplayUnitInternal du = activeScreenSetup.Screens.Find(x => x.MPIRank == MPIEnvironment.Rank);

            //du.ScreenCam = (GameObject)Instantiate(cameraPrefab);
            //du.ScreenCam.name = du.Name;
            //du.ScreenCam.transform.parent = cameraHead.transform;
            //du.ScreenCam.transform.localPosition = Vector3.zero;

            List<DisplayUnitInternal> dus = activeDisplaySetup.Screens.FindAll(x => x.MPIRank == MPIEnvironment.Rank);
            for (int i = 0; i < dus.Count; i++)
            {
                DisplayUnitInternal du = dus[i];
                du.ScreenCam = (GameObject)Instantiate(cameraPrefab);
                du.ScreenCam.name = du.Name;
                du.ScreenCam.transform.parent = cameraHead.transform;
                du.ScreenCam.transform.localPosition = Vector3.zero;
            }
        }
        else if (approach == SyncApproach.Cn_Adaptive)
        {
            // Create one camera per screen.
            // If current process is the manager, then create all cameras
            if (MPIEnvironment.Manager)
            {
                foreach (DisplayUnitInternal screen in activeDisplaySetup.Screens)
                {
                    screen.ScreenCam = (GameObject)Instantiate(cameraPrefab);
                    screen.ScreenCam.name = screen.Name;
                    screen.ScreenCam.transform.parent = cameraHead.transform;
                    screen.ScreenCam.transform.localPosition = Vector3.zero;
                    if (screen.MPIRank == MPIEnvironment.Rank)
                    {
                        screen.ScreenCam.GetComponent<Camera>().depth = 100;
                    }
                }
            }
            else
            {
                DisplayUnitInternal du = activeDisplaySetup.Screens.Find(x => x.MPIRank == MPIEnvironment.Rank);
                du.ScreenCam = (GameObject)Instantiate(cameraPrefab);
                du.ScreenCam.name = du.Name;
                du.ScreenCam.transform.parent = cameraHead.transform;
                du.ScreenCam.transform.localPosition = Vector3.zero;
            }
        }
        else if (approach == SyncApproach.C1_Adaptive)
        {
            if (MPIEnvironment.Manager)
            {
                foreach (DisplayUnitInternal screen in activeDisplaySetup.Screens)
                {
                    screen.ScreenCam = (GameObject)Instantiate(cameraPrefab);
                    screen.ScreenCam.name = screen.Name;
                    screen.ScreenCam.transform.parent = cameraHead.transform;
                    screen.ScreenCam.transform.localPosition = Vector3.zero;
                    if (screen.MPIRank == MPIEnvironment.Rank)
                    {
                        screen.ScreenCam.GetComponent<Camera>().depth = 100;
                    }
                    else
                    {
                        screen.ScreenCam.SetActive(false);
                    }
                }
            }
            else
            {
                DisplayUnitInternal du = activeDisplaySetup.Screens.Find(x => x.MPIRank == MPIEnvironment.Rank);
                du.ScreenCam = (GameObject)Instantiate(cameraPrefab);
                du.ScreenCam.name = du.Name;
                du.ScreenCam.transform.parent = cameraHead.transform;
                du.ScreenCam.transform.localPosition = Vector3.zero;

            }
        }

        UpdateCameras();
    }

    private void SyncCameras()
    {
        if (MPIEnvironment.Initialized)
        {
            MPIEnvironment.BroadcastTransform(gameObject, MPIEnvironment.ManagerRank);
        }
    }

    //public Tracker tracker;
    private void UpdateCameras()
    {
        if (activeDisplaySetup == null) return;

        // Set the camera head position.
        cameraHead.transform.localPosition = cameraHeadPosition;

        // Handle head tracking.
        if (useHeadTracking && mpiSync)
        {
            if (MPIEnvironment.Manager)
            {
                //                Vector3 bp = tracker.GetBodyPosition(0) * 0.001f;
                //                bp = new Vector3(bp.x, bp.y, -bp.z);
                //                cameraHead.transform.localPosition += bp;
                cameraHead.transform.localPosition += HeadOffset;
            }

            MPIEnvironment.BroadcastLocalPosition(cameraHead, MPIEnvironment.ManagerRank);
        }

        foreach (DisplayUnitInternal du in ActiveDisplaySetup.Screens)
        {
            if (du.ScreenCam != null)
            {
                Camera theCamera = du.ScreenCam.GetComponent<Camera>();

                // Enable or disable the camera based on the status of the screen.
                //theCamera.enabled = activeScreen.Enabled;

                // The camera has the same orientation as the screen.
                du.ScreenCam.transform.localRotation = du.Orientation;

                // Set the camera's viewport to match the screen's viewport.
                theCamera.rect = du.ViewportRect;

                // Enable/disable occlusion culling.
                theCamera.useOcclusionCulling = useOcclusionCulling;

                // Update the camera's perspective.
                // UpdateCameraPerspective(cam, activeScreen);
                UpdateCameraPerspective(du.ScreenCam, du);
            }
        }
    }

    private void UpdateCameraPerspective(GameObject camera, DisplayUnitInternal screen)
    {

        Vector3 cameraPosition = Vector3.zero;
        if (activeDisplaySetup.Projection == DisplaySetup.ProjectionType.Perspective)
        {
            cameraPosition = cameraHead.transform.localPosition;
        }
        else if (activeDisplaySetup.Projection == DisplaySetup.ProjectionType.Orthogonal)
        {
            cameraPosition = screen.Center + (screen.Normal);
        }

        // Make sure that the camera is not too close to the screen or even behind it.
        if (screen.Plane.GetDistanceToPoint(cameraPosition) < 0.01f) return;

        // Transform two opposing corner points of the screen into the camera's coordinate system.
        Quaternion invCameraRotation = Quaternion.Inverse(camera.transform.localRotation);
        Vector3[] screenCorners;
        if (borders == BorderOption.Ignore)
        {
            screenCorners = screen.ComputeOutterCornerPoints();
        }
        else
        {
            screenCorners = screen.ComputeInnerCornerPoints();
        }
        Vector3 toTopLeft = invCameraRotation * (screenCorners[0] - cameraPosition);
        Vector3 toBottomRight = invCameraRotation * (screenCorners[2] - cameraPosition);

        // Compute where the rays from the camera to the screen corner points would hit
        // the camera's image plane at distance 1.
        float left = toTopLeft.x / toTopLeft.z;
        float right = toBottomRight.x / toBottomRight.z;
        float top = toTopLeft.y / toTopLeft.z;
        float bottom = toBottomRight.y / toBottomRight.z;

        // For some things, Unity needs the standard camera properties set.
        // We try to derive them.
        float width = right - left;
        float height = top - bottom;
        Camera theCamera = camera.GetComponent<Camera>();

        if (activeDisplaySetup.Projection == DisplaySetup.ProjectionType.Perspective)
        {
            theCamera.aspect = width / height;
            theCamera.fieldOfView = Mathf.Rad2Deg * 2 * Mathf.Max(Mathf.Atan(Mathf.Abs(top)), Mathf.Atan(Mathf.Abs(bottom)));
        }
        else if (activeDisplaySetup.Projection == DisplaySetup.ProjectionType.Orthogonal)
        {
            theCamera.transform.position = cameraPosition;
            theCamera.orthographicSize = screen.Height * 0.5f;
        }

        // TODO: we need to recalculate near clipping plane for each camera
        theCamera.nearClipPlane = nearClippingPlane; // Vector3.Distance(activeScreen.Center, cameraPosition);
        theCamera.farClipPlane = farClippingPlane;

        // Now set the actual projection matrix.
        if (activeDisplaySetup.Projection == DisplaySetup.ProjectionType.Perspective)
        {
            theCamera.projectionMatrix = Utils.PerspectiveOffCenter(left, right, bottom, top, nearClippingPlane, farClippingPlane);
        }
        else if (activeDisplaySetup.Projection == DisplaySetup.ProjectionType.Orthogonal)
        {
            theCamera.projectionMatrix = Utils.OrthogonalOffCenter(left, right, bottom, top, nearClippingPlane, farClippingPlane);
        }

    }



}