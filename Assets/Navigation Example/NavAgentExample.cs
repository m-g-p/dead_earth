using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavAgentExample : MonoBehaviour {

    [SerializeField] AIWaypointNetwork waypointNetwork = null;
    [SerializeField] NavMeshPathStatus pathStatus = NavMeshPathStatus.PathInvalid;
    [SerializeField] int currentIndex = 0;
    [SerializeField] bool hasPath = false;
    [SerializeField] bool pathPending = false;
    [SerializeField] bool isPathStale = false;

    NavMeshAgent agent;

    // Use this for initialization
    void Start () {
        agent = GetComponent<NavMeshAgent>();

        if (waypointNetwork == null) { return; }

        SetNextDestination(false);

    }
	
    void SetNextDestination (bool increment)
    {
        if (!waypointNetwork) { return; }

        int incrementStep = increment ? 1 : 0;
        
        int nextWaypoint = (currentIndex + incrementStep >= waypointNetwork.waypoints.Count) ? 0 : currentIndex + incrementStep;
        Transform nextWaypointTransform = waypointNetwork.waypoints[nextWaypoint];

        if(nextWaypointTransform != null)
        {
            currentIndex = nextWaypoint;
            agent.destination = nextWaypointTransform.position;
            return;
        }
        
        currentIndex++;
    }

	// Update is called once per frame
	void Update () {

        hasPath     = agent.hasPath;
        pathPending = agent.pathPending;
        isPathStale = agent.isPathStale;
        pathStatus = agent.pathStatus;

        if((!hasPath && !pathPending) || pathStatus == NavMeshPathStatus.PathInvalid /*|| pathStatus==NavMeshPathStatus.PathPartial*/)
        {
            SetNextDestination(true);
        }
        else if (agent.isPathStale)
        {
            SetNextDestination(false);
        }
	}
}
