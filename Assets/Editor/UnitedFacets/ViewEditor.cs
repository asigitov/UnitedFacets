// <copyright file="ViewEditor.cs" company="Institute of Visual Computing / Bonn-Rhein-Sieg University of Applied Sciences">
// Copyright (c) 2016-2017 All Rights Reserved
// </copyright>
// <author>Anton Sigitov</author>
// <summary></summary>

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System.IO;

[CustomEditor(typeof(View))]
public class ViewEditor : Editor
{
    

    public override void OnInspectorGUI()
    {
        View me = (View)target;

        base.OnInspectorGUI();
        
        if (GUILayout.Button("Generate MPI app file"))
        {

            DisplaySetup screenSetup = me.ActiveDisplaySetup;
            List<string> commands = new List<string>(screenSetup.Screens.Count);
            for (int i = 0; i < screenSetup.Screens.Count; i++)
            {
                commands.Add("");
            }

                foreach (DisplayUnitInternal s in screenSetup.Screens)
                {
                    string command = string.Format("-np 1 --oversubscribe -x DISPLAY=:{0} --prefix {1} -x LD_LIBRARY_PATH -x PATH -host {2} {3}",
                        s.XScreen,
                        "<target path to openmpi binaries>",
                        s.Host,
                        "<your executable name>");
                    commands[s.MPIRank] = command;
                }

            string filename = Application.dataPath + "/../mpi.app";
            File.WriteAllLines(filename, commands.ToArray());
            
            Debug.Log("MPI app file has been written to: " + Path.GetFullPath(filename));
        }

        
    }

}
