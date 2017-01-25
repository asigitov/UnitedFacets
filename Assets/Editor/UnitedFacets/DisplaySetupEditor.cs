// <copyright file="DisplaySetupEditor.cs" company="Institute of Visual Computing / Bonn-Rhein-Sieg University of Applied Sciences">
// Copyright (c) 2016-2017 All Rights Reserved
// </copyright>
// <author>Anton Sigitov</author>
// <summary></summary>

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.Callbacks;
using UnityEngine;

[CustomEditor(typeof(DisplaySetup))]
public class DisplaySetupEditor : Editor
{
    private static Material uniformColorMaterial;

    private int cameraDistance = 350;
    private int orbitX;
    private int orbitY = 3600;
    private Vector3 moveAllScreens;
    private Vector3 rotateSelectedDUs;

    private int alignIndex1;
    private int alignIndex2;

    [MenuItem("Assets/Create/Display Setup")]
    public static void CreateAsset()
    {
        EditorUtils.CreateAsset<DisplaySetup>();
    }

    Vector2 scrollPos;
    
    void OnEnable()
    {
        
    }

    public override void OnInspectorGUI()
    {
        DisplaySetup me = (DisplaySetup)target;
        List<DisplayUnitInternal> screens = me.Screens;

        switch (me.DialogStep)
        {
            case 0:
                {
                    if (GUILayout.Button("Create empty display setup"))
                    {
                        EditorUtility.SetDirty(me);
                        me.DialogStep = 2;
                    }
                    if (GUILayout.Button("Create display setup from matrix"))
                    {
                        EditorUtility.SetDirty(me);
                        me.DialogStep = 1;
                    }
                    break;
                }
            case 1:
                {
                    me.NumDUH = EditorGUILayout.IntField("Num of Display Units (horizontal)", me.NumDUH);
                    me.NumDUV = EditorGUILayout.IntField("Num of Display Units (vertical)", me.NumDUV);
                    me.DU = (DisplayUnit)EditorGUILayout.ObjectField("Display Unit Type", me.DU, typeof(DisplayUnit), false);
                    if (GUILayout.Button("Create display setup from matrix"))
                    {
                        if (me.NumDUH < 1 || me.NumDUV < 1)
                        {
                            EditorUtility.DisplayDialog("Bad Matrix", "Number of display units must be greater than Zero!", "OK");
                            break;
                        }

                        if (me.DU == null)
                        {
                            EditorUtility.DisplayDialog("Display Unit Type Error", "Display unit type is not set!", "OK");
                            break;
                        }

                        Vector3 currentCenter = new Vector3((me.DU.Width * -0.5f) * (me.NumDUH - 1), (me.DU.Height * -0.5f) * (me.NumDUV - 1), 0.0f);
                        Debug.Log(currentCenter);
                        for (int v = 0; v < me.NumDUV; v++)
                        {
                            for (int h = 0; h < me.NumDUH; h++)
                            {
                                screens.Add(new DisplayUnitInternal
                                {
                                    Prototype = me.DU,
                                    Name = string.Format("H{0}V{1}", h, v),
                                    TypeName = me.DU.TypeName,
                                    Enabled = true,
                                    ViewportRect = new Rect(0, 0, 1, 1),
                                    Width = me.DU.Width,
                                    Height = me.DU.Height,
                                    LeftBorder = me.DU.LeftBorder,
                                    RightBorder = me.DU.RightBorder,
                                    TopBorder = me.DU.TopBorder,
                                    BottomBorder = me.DU.BottomBorder,
                                    Center = currentCenter
                                });

                                currentCenter += new Vector3(me.DU.Width, 0, 0);
                            }
                            currentCenter.x = (me.DU.Width * -0.5f) * (me.NumDUH - 1);
                            currentCenter += new Vector3(0, me.DU.Height, 0);
                        }
                        me.DialogStep = 2;
                        EditorUtility.SetDirty(me);
                    }
                    break;
                }
            case 2:
                {
                    // General info
                    me.Description = EditorGUILayout.TextField("Description", me.Description);
                    me.MainScreenIndex = EditorGUILayout.IntField("Main screen index", me.MainScreenIndex);
                    me.Projection = (DisplaySetup.ProjectionType)EditorGUILayout.EnumPopup("Projection", me.Projection);

                    scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true);

                    // Display Units
                    for (int i = 0; i < screens.Count; ++i)
                    {
                        DisplayUnitInternal screen = screens[i];

                        if (screen == null)
                        {
                            continue;
                        }


                        GUIStyle gs = GUI.skin.GetStyle("Label");
                        gs.fontStyle = FontStyle.Bold;

                        EditorGUI.BeginChangeCheck();
                        if (screen.Name != "")
                        {
                            screen.Selected = EditorGUILayout.ToggleLeft(screen.Name, screen.Selected, gs);
                        }
                        else
                        {
                            screen.Selected = EditorGUILayout.ToggleLeft(string.Format("Display Unit {0}", i), screen.Selected, gs);
                        }
                        if (EditorGUI.EndChangeCheck())
                        {
                            EditorUtility.SetDirty(me);
                        }

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.BeginVertical();

                        screen.Prototype = (DisplayUnit)EditorGUILayout.ObjectField("Display Unit Type", screen.Prototype, typeof(DisplayUnit), false);
                        if (screen.Prototype != null)
                        {
                            screen.Name = EditorGUILayout.TextField("Name", screen.Name);
                            screen.Host = EditorGUILayout.TextField("Host", screen.Host);
                            screen.XScreen = EditorGUILayout.IntField("X Screen", screen.XScreen);
                            screen.MPIRank = EditorGUILayout.IntField("MPI Rank", screen.MPIRank);
                            screen.ViewportRect = EditorGUILayout.RectField("Viewport rectangle", screen.ViewportRect);

                            screen.Center = EditorGUILayout.Vector3Field("Center", screen.Center);
                            screen.OrientationEuler = EditorGUILayout.Vector3Field("Orientation", screen.OrientationEuler);
                        }


                        EditorGUILayout.EndVertical();
                        EditorGUILayout.BeginVertical(GUILayout.Width(50));

                        if (GUILayout.Button("Delete", EditorStyles.miniButton))
                        {
                            // Delete this display unit.
                            screens.RemoveAt(i--);
                            EditorUtility.SetDirty(me);
                        }

                        if (screen.Prototype != null)
                        {
                            
                            if (GUILayout.Button("Clone", EditorStyles.miniButton))
                            {
                                // Clone this screen.
                                screens.Add(new DisplayUnitInternal { Name = screens[i].Name + " Clone", Enabled = screens[i].Enabled, ViewportRect = screens[i].ViewportRect, Width = screens[i].Width, Height = screens[i].Height });
                                EditorUtility.SetDirty(me);
                            }
                            
                        }



                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                        
                        EditorGUILayout.Separator();
                        
                    }

                    EditorGUILayout.EndScrollView();
                    EditorGUILayout.Separator();

                    if (GUILayout.Button("Add display unit"))
                    {
                        screens.Add(new DisplayUnitInternal { Name = "New Display Unit", Enabled = true, ViewportRect = new Rect(0, 0, 1, 1) });
                        EditorUtility.SetDirty(me);
                    }
                    
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Select all"))
                    {
                        foreach(DisplayUnitInternal du in screens)
                        {
                            du.Selected = true;
                        }
                        EditorUtility.SetDirty(me);
                    }

                    if (GUILayout.Button("Deselect all"))
                    {
                        foreach (DisplayUnitInternal du in screens)
                        {
                            du.Selected = false;
                        }
                        EditorUtility.SetDirty(me);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Separator();

                    // Allow the user to move selected display units at the same time.
                    moveAllScreens = EditorGUILayout.Vector3Field("Move selected by", moveAllScreens);
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Move"))
                    {
                        MoveSelected(screens, moveAllScreens);
                        EditorUtility.SetDirty(me);
                    }
                    EditorGUILayout.EndHorizontal();

                    // Allow the user to rotate selected display units at the same time.
                    rotateSelectedDUs = EditorGUILayout.Vector3Field("Rotate selected by", rotateSelectedDUs);
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Rotate"))
                    {
                        RotateSelected(screens, rotateSelectedDUs);
                        EditorUtility.SetDirty(me);
                    }
                    EditorGUILayout.EndHorizontal();

                    
                    string[] names = new string[screens.Count+1];
                    names[0] = "choose display unit";
                    for(int i=0; i<screens.Count; i++)
                    {
                        names[i+1] = screens[i].Name;
                    }
                    if(alignIndex1 >= screens.Count)
                    {
                        alignIndex1 = 0;
                    }
                    if (alignIndex2 >= screens.Count)
                    {
                        alignIndex2 = 0;
                    }
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField("Align display units");
                    EditorGUI.indentLevel++;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("align");
                    alignIndex1 = EditorGUILayout.Popup(alignIndex1, names);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("with");
                    alignIndex2 = EditorGUILayout.Popup(alignIndex2, names);
                    EditorGUILayout.EndHorizontal();
                    EditorGUI.indentLevel--;
                    if (GUILayout.Button("Align"))
                    {
                        SnapSelectedDisplayUnits(screens[alignIndex1-1], screens[alignIndex2-1]);
                        alignIndex1 = 0;
                        alignIndex2 = 0;
                        EditorUtility.SetDirty(me);
                    }
                    EditorGUILayout.EndVertical();
                    

                    if (GUI.changed) EditorUtility.SetDirty(me);

                    break;
                }
            default:
                {
                    break;
                }
        }

        return;

        // #####################################
    }

    public override bool HasPreviewGUI()
    {
        return true;
    }

    public override void OnPreviewSettings()
    {
        base.OnPreviewSettings();
    }

    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        // Strange Unity behavior: These GUI elements will be shown in the actual editor (not the preview) as well,
        // therefore we need this awkward check.
        if (r.width < 255 && r.width != 1) return;

        DisplaySetup me = (DisplaySetup)target;

        Rect glRect = new Rect(6, 94, r.width - 5, r.height - 5);
        GL.Viewport(glRect);

        Quaternion orbit = Quaternion.Euler(orbitX / 10.0f, orbitY / 10.0f, 0);
        Matrix4x4 modelViewMatrix = Matrix4x4.TRS(new Vector3(0, 0, cameraDistance / 10.0f), Quaternion.identity, Vector3.one) * Matrix4x4.TRS(Vector3.zero, orbit, Vector3.one);
        Matrix4x4 projectionMatrix = Matrix4x4.Perspective(60, glRect.width / glRect.height, 0, 10);

        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();

        // Draw the x axis arrow.
        vertices.Clear();
        indices.Clear();
        vertices.Add(Vector3.zero);
        vertices.Add(0.5f * Vector3.right);
        vertices.Add(0.45f * Vector3.right + 0.05f * Vector3.up);
        vertices.Add(0.45f * Vector3.right - 0.05f * Vector3.up);
        vertices.Add(0.45f * Vector3.right + 0.05f * Vector3.forward);
        vertices.Add(0.45f * Vector3.right - 0.05f * Vector3.forward);
        indices.AddRange(new int[] { 0, 1, 1, 2, 1, 3, 1, 4, 1, 5 });
        Draw(vertices, indices, GL.LINES, Color.red, modelViewMatrix, projectionMatrix);

        // Draw the y axis arrow.
        vertices.Clear();
        indices.Clear();
        vertices.Add(Vector3.zero);
        vertices.Add(0.5f * Vector3.up);
        vertices.Add(0.45f * Vector3.up + 0.05f * Vector3.right);
        vertices.Add(0.45f * Vector3.up - 0.05f * Vector3.right);
        vertices.Add(0.45f * Vector3.up + 0.05f * Vector3.forward);
        vertices.Add(0.45f * Vector3.up - 0.05f * Vector3.forward);
        indices.AddRange(new int[] { 0, 1, 1, 2, 1, 3, 1, 4, 1, 5 });
        Draw(vertices, indices, GL.LINES, Color.green, modelViewMatrix, projectionMatrix);

        // Draw the z axis arrow.
        vertices.Clear();
        indices.Clear();
        vertices.Add(Vector3.zero);
        vertices.Add(0.5f * Vector3.forward);
        vertices.Add(0.45f * Vector3.forward + 0.05f * Vector3.right);
        vertices.Add(0.45f * Vector3.forward - 0.05f * Vector3.right);
        vertices.Add(0.45f * Vector3.forward + 0.05f * Vector3.up);
        vertices.Add(0.45f * Vector3.forward - 0.05f * Vector3.up);
        indices.AddRange(new int[] { 0, 1, 1, 2, 1, 3, 1, 4, 1, 5 });
        Draw(vertices, indices, GL.LINES, Color.cyan, modelViewMatrix, projectionMatrix);


        // Draw the screens.
        for (int i = 0; i < me.Screens.Count; i++)
        {
            DisplayUnitInternal screen = me.Screens[i];
            if (screen == null || screen.Selected)
            {
                continue;
            }
            vertices.Clear();
            indices.Clear();
            vertices.AddRange(screen.ComputeOutterCornerPoints());
            indices.AddRange(new int[] { 0, 1, 1, 2, 2, 3, 3, 0 });
            vertices.Add(screen.Center);
            vertices.Add(screen.Center + 0.5f * screen.Normal);
            vertices.Add(screen.Center + 0.45f * screen.Normal + 0.05f * screen.Right);
            vertices.Add(screen.Center + 0.45f * screen.Normal - 0.05f * screen.Right);
            vertices.Add(screen.Center + 0.45f * screen.Normal + 0.05f * screen.Up);
            vertices.Add(screen.Center + 0.45f * screen.Normal - 0.05f * screen.Up);
            vertices.Add(screen.Center - 0.5f * screen.Width * screen.Right + (0.5f * screen.Height - 0.1f) * screen.Up);
            vertices.Add(screen.Center - (0.5f * screen.Width - 0.1f) * screen.Right + 0.5f * screen.Height * screen.Up);
            indices.AddRange(new int[] { 4, 5, 5, 6, 5, 7, 5, 8, 5, 9, 10, 11 });
            Draw(vertices, indices, GL.LINES, screen.Enabled ? Color.white : Color.gray, modelViewMatrix, projectionMatrix);
        }


        // Draw the selected screens.
        for (int i = 0; i < me.Screens.Count; i++)
        {
            DisplayUnitInternal screen = me.Screens[i];
            if (screen == null || !screen.Selected)
            {
                continue;
            }
            vertices.Clear();
            indices.Clear();
            vertices.AddRange(screen.ComputeOutterCornerPoints());
            indices.AddRange(new int[] { 0, 1, 1, 2, 2, 3, 3, 0, 0, 2, 1, 3 });
            vertices.Add(screen.Center);
            vertices.Add(screen.Center + 0.5f * screen.Normal);
            vertices.Add(screen.Center + 0.45f * screen.Normal + 0.05f * screen.Right);
            vertices.Add(screen.Center + 0.45f * screen.Normal - 0.05f * screen.Right);
            vertices.Add(screen.Center + 0.45f * screen.Normal + 0.05f * screen.Up);
            vertices.Add(screen.Center + 0.45f * screen.Normal - 0.05f * screen.Up);
            vertices.Add(screen.Center - 0.5f * screen.Width * screen.Right + (0.5f * screen.Height - 0.1f) * screen.Up);
            vertices.Add(screen.Center - (0.5f * screen.Width - 0.1f) * screen.Right + 0.5f * screen.Height * screen.Up);
            indices.AddRange(new int[] { 4, 5, 5, 6, 5, 7, 5, 8, 5, 9, 10, 11 });
            Draw(vertices, indices, GL.LINES, Color.yellow, modelViewMatrix, projectionMatrix);
        }

        // Draw align display units
        if(alignIndex1 != 0 && me.Screens.Count > (alignIndex1-1))
        {
            DisplayUnitInternal screen = me.Screens[alignIndex1-1];
            if (screen != null)
            {
                vertices.Clear();
                indices.Clear();
                vertices.AddRange(screen.ComputeOutterCornerPoints());
                indices.AddRange(new int[] { 0, 1, 1, 2, 2, 3, 3, 0, 0, 2, 1, 3 });
                vertices.Add(screen.Center);
                vertices.Add(screen.Center + 0.5f * screen.Normal);
                vertices.Add(screen.Center + 0.45f * screen.Normal + 0.05f * screen.Right);
                vertices.Add(screen.Center + 0.45f * screen.Normal - 0.05f * screen.Right);
                vertices.Add(screen.Center + 0.45f * screen.Normal + 0.05f * screen.Up);
                vertices.Add(screen.Center + 0.45f * screen.Normal - 0.05f * screen.Up);
                vertices.Add(screen.Center - 0.5f * screen.Width * screen.Right + (0.5f * screen.Height - 0.1f) * screen.Up);
                vertices.Add(screen.Center - (0.5f * screen.Width - 0.1f) * screen.Right + 0.5f * screen.Height * screen.Up);
                indices.AddRange(new int[] { 4, 5, 5, 6, 5, 7, 5, 8, 5, 9, 10, 11 });
                Draw(vertices, indices, GL.LINES, Color.green, modelViewMatrix, projectionMatrix);
            }
        }

        if (alignIndex2 != 0 && me.Screens.Count > (alignIndex2 - 1))
        {
            DisplayUnitInternal screen = me.Screens[alignIndex2 - 1];
            if (screen != null)
            {
                vertices.Clear();
                indices.Clear();
                vertices.AddRange(screen.ComputeOutterCornerPoints());
                indices.AddRange(new int[] { 0, 1, 1, 2, 2, 3, 3, 0, 0, 2, 1, 3 });
                vertices.Add(screen.Center);
                vertices.Add(screen.Center + 0.5f * screen.Normal);
                vertices.Add(screen.Center + 0.45f * screen.Normal + 0.05f * screen.Right);
                vertices.Add(screen.Center + 0.45f * screen.Normal - 0.05f * screen.Right);
                vertices.Add(screen.Center + 0.45f * screen.Normal + 0.05f * screen.Up);
                vertices.Add(screen.Center + 0.45f * screen.Normal - 0.05f * screen.Up);
                vertices.Add(screen.Center - 0.5f * screen.Width * screen.Right + (0.5f * screen.Height - 0.1f) * screen.Up);
                vertices.Add(screen.Center - (0.5f * screen.Width - 0.1f) * screen.Right + 0.5f * screen.Height * screen.Up);
                indices.AddRange(new int[] { 4, 5, 5, 6, 5, 7, 5, 8, 5, 9, 10, 11 });
                Draw(vertices, indices, GL.LINES, Color.green, modelViewMatrix, projectionMatrix);
            }
        }

        GL.Viewport(new Rect(0, 0, Screen.width, Screen.height));

        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Box("Rotate (Y)", GUILayout.Width(100));
        orbitY = (int)GUILayout.HorizontalSlider(orbitY, 3600, 0);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Box("Rotate (X)", GUILayout.Width(100));
        orbitX = (int)GUILayout.HorizontalSlider(orbitX, 0, 3600);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Box("Zoom", GUILayout.Width(100));
        cameraDistance = (int)GUILayout.HorizontalSlider(cameraDistance, 350, 10);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Separator();

        if (GUILayout.Button("Reset")) { orbitX = 0; orbitY = 0; cameraDistance = 350; }

        EditorGUILayout.EndVertical();


        //EditorGUILayout.BeginHorizontal(GUILayout.Width(200));
        //EditorGUILayout.BeginVertical();
        //if (GUILayout.RepeatButton("Up")) orbitX += 75;
        //EditorGUILayout.BeginHorizontal();
        //if (GUILayout.RepeatButton("Left")) orbitY += 75;
        //if (GUILayout.RepeatButton("Right")) orbitY -= 75;
        //EditorGUILayout.EndHorizontal();
        //if (GUILayout.RepeatButton("Down")) orbitX -= 75;
        //EditorGUILayout.EndVertical();
        //EditorGUILayout.BeginVertical();
        //if (GUILayout.RepeatButton("Zoom in") && cameraDistance > 10) cameraDistance -= 5;
        //if (GUILayout.RepeatButton("Zoom out")) ++cameraDistance;
        //if (GUILayout.Button("Reset")) { orbitX = 0; orbitY = 0; cameraDistance = 50; }


        //EditorGUILayout.EndVertical();
        //EditorGUILayout.EndHorizontal();
    }

    private static void Draw(IEnumerable<Vector3> vertices, IEnumerable<int> indices, int type, Color color, Matrix4x4 modelViewMatrix, Matrix4x4 projectionMatrix)
    {
        // Pre-transform the vertices.
        List<Vector3> transformedVertices = new List<Vector3>();
        foreach (Vector3 v in vertices)
        {
            Vector3 transformed = modelViewMatrix.MultiplyPoint(v);
            transformed.z = -transformed.z;
            transformedVertices.Add(transformed);
        }

        // TODO: If there is no UniformColorMaterial, an exception will be thrown
        // Draw the geometry.
        if (uniformColorMaterial == null) uniformColorMaterial = (Material)Resources.Load("UnitedFacets/UniformColorMaterial");
        uniformColorMaterial.color = color;
        uniformColorMaterial.SetPass(0);
        GL.PushMatrix();
        GL.modelview = projectionMatrix;
        GL.LoadProjectionMatrix(Matrix4x4.identity);
        GL.Begin(type);
        foreach (int i in indices) GL.Vertex(transformedVertices[i]);
        GL.End();
        GL.PopMatrix();
    }

    private static void SnapScreen(DisplayUnitInternal screen, List<DisplayUnitInternal> screens)
    {
        // Get the corner points of the other screens.
        List<Vector3> snapDestPoints = new List<Vector3>();
        foreach (DisplayUnitInternal destScreen in screens)
        {
            if (destScreen == screen) continue;
            snapDestPoints.AddRange(destScreen.ComputeOutterCornerPoints());
        }

        if (snapDestPoints.Count == 0) return;

        // Get the corner points of the screen to be snapped.
        Vector3[] snapSrcPoints = screen.ComputeOutterCornerPoints();

        // Find the closest point pair.
        float closestDistSq = float.PositiveInfinity;
        int closestSrcIndex = 0;
        int closestDestIndex = 0;
        for (int i = 0; i < snapSrcPoints.Length; ++i)
        {
            for (int j = 0; j < snapDestPoints.Count; ++j)
            {
                float distSq = (snapSrcPoints[i] - snapDestPoints[j]).sqrMagnitude;
                if (distSq < closestDistSq)
                {
                    closestDistSq = distSq;
                    closestSrcIndex = i;
                    closestDestIndex = j;
                }
            }
        }

        // Move the screen so that the closest points will have the same position.
        screen.Center += snapDestPoints[closestDestIndex] - snapSrcPoints[closestSrcIndex];
    }

    private static void SnapSelectedDisplayUnits(DisplayUnitInternal du1, DisplayUnitInternal du2)
    {
        // Get the corner points of the other screens.
        List<Vector3> snapDestPoints = new List<Vector3>();
        snapDestPoints.AddRange(du2.ComputeOutterCornerPoints());

        // Get the corner points of the screen to be snapped.
        Vector3[] snapSrcPoints = du1.ComputeOutterCornerPoints();

        // Find the closest point pair.
        float closestDistSq = float.PositiveInfinity;
        int closestSrcIndex = 0;
        int closestDestIndex = 0;
        for (int i = 0; i < snapSrcPoints.Length; ++i)
        {
            for (int j = 0; j < snapDestPoints.Count; ++j)
            {
                float distSq = (snapSrcPoints[i] - snapDestPoints[j]).sqrMagnitude;
                if (distSq < closestDistSq)
                {
                    closestDistSq = distSq;
                    closestSrcIndex = i;
                    closestDestIndex = j;
                }
            }
        }

        // Move the screen so that the closest points will have the same position.
        du1.Center += snapDestPoints[closestDestIndex] - snapSrcPoints[closestSrcIndex];
    }

    private static void MoveSelected(List<DisplayUnitInternal> screens, Vector3 offset)
    {
        foreach (DisplayUnitInternal screen in screens)
        {
            if (screen.Selected)
                screen.Center += offset;
        }
    }

    private static void RotateSelected(List<DisplayUnitInternal> screens, Vector3 offset)
    {
        foreach (DisplayUnitInternal screen in screens)
        {
            if (screen.Selected)
                screen.OrientationEuler += offset;
        }
    }
}