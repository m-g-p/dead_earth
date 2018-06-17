using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

[CustomEditor(typeof(AIWaypointNetwork))]
public class AIWaypointNetworkEditor : Editor
{
    GUIStyle style = new GUIStyle();

    public override void OnInspectorGUI()
    {
        AIWaypointNetwork network = (AIWaypointNetwork)target;

        network.displayMode = (PathDisplayMode)EditorGUILayout.EnumPopup("Display Mode", network.displayMode);

        if (network.displayMode == PathDisplayMode.Paths)
        {
            network.uiStart = EditorGUILayout.IntSlider("Waypoint Start", network.uiStart, 0, network.Waypoints.Count - 1);
            network.uiEnd = EditorGUILayout.IntSlider("Waypoint End", network.uiEnd, 0, network.Waypoints.Count - 1);
        }

        DrawDefaultInspector();
    }

    private void OnSceneGUI()
    {
        style.normal.textColor = Color.white;

        AIWaypointNetwork network = (AIWaypointNetwork)target;

        for (int i = 0; i < network.Waypoints.Count; i++)
        {
            if (network.Waypoints[i] != null)
            {
                Handles.Label(network.Waypoints[i].position, "Waypoint " + i.ToString(), style);
            }
        }

        if (network.displayMode == PathDisplayMode.Connections)
        {
            Vector3[] linePoints = new Vector3[network.Waypoints.Count + 1];

            for (int i = 0; i < linePoints.Length; i++)
            {
                int index = i != network.Waypoints.Count ? i : 0;

                if(network.Waypoints.Count > 0)
                {
                    if (network.Waypoints[index] != null)
                    {
                        linePoints[i] = network.Waypoints[index].position;
                    }
                    else
                    {
                        Debug.Log("waypoint not set!!");
                    }
                }
            }
            Handles.color = Color.cyan;
            Handles.DrawPolyLine(linePoints);
        }
        else if (network.displayMode == PathDisplayMode.Paths)
        {
            NavMeshPath path    = new NavMeshPath();

            if(network.Waypoints[network.uiStart] != null && network.Waypoints[network.uiEnd] != null)
            {
                Vector3 from        = network.Waypoints[network.uiStart].position;
                Vector3 to          = network.Waypoints[network.uiEnd].position;

                NavMesh.CalculatePath(from, to, NavMesh.AllAreas, path);

                Handles.color = Color.yellow;
                Handles.DrawPolyLine(path.corners);
            }
        }
    }
}
