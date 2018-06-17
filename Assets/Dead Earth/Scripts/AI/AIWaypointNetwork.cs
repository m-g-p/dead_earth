using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PathDisplayMode { None, Connections, Paths}  

public class AIWaypointNetwork : MonoBehaviour 
{
    [HideInInspector] public PathDisplayMode displayMode = PathDisplayMode.Connections;
    [HideInInspector] public int uiStart  = 0;
    [HideInInspector] public int uiEnd    = 0;
	public List<Transform> Waypoints = new List<Transform>();
}
