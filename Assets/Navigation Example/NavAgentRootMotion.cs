using UnityEngine;
using System.Collections;
using UnityEngine.AI;

// ----------------------------------------------------------
// CLASS	:	NavAgentRootMotion
// DESC		:	Behaviour to test Unity's NavMeshAgent with
//				Animator component using root motion
// ----------------------------------------------------------
[RequireComponent(typeof(NavMeshAgent))]
public class NavAgentRootMotion : MonoBehaviour
{
    // Inspector Assigned Variable
    [SerializeField] AIWaypointNetwork waypointNetwork = null;
    [SerializeField] NavMeshPathStatus pathStatus = NavMeshPathStatus.PathInvalid;
    [SerializeField] AnimationCurve jumpCurve = new AnimationCurve();
    [SerializeField] int currentIndex = 0;
    [SerializeField] bool hasPath = false;
    [SerializeField] bool pathPending = false;
    [SerializeField] bool pathStale = false;
    [SerializeField] bool mixedMode = true;

    // Private Members
    private NavMeshAgent agent = null;
    private Animator animator = null;
    private float smoothangle = 0.0f;

    // -----------------------------------------------------
    // Name :	Start
    // Desc	:	Cache MavMeshAgent and set initial 
    //			destination.
    // -----------------------------------------------------
    void Start()
    {
        // Cache NavMeshAgent Reference
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Turn off auto-update of rotation
        /*_navAgent.updatePosition = false;*/
        agent.updateRotation = false;

        // If not valid Waypoint Network has been assigned then return
        if (waypointNetwork == null) return;

        // Set first waypoint
        SetNextDestination(false);
    }

    // -----------------------------------------------------
    // Name	:	SetNextDestination
    // Desc	:	Optionally increments the current waypoint
    //			index and then sets the next destination
    //			for the agent to head towards.
    // -----------------------------------------------------
    void SetNextDestination(bool increment)
    {
        // If no network return
        if (!waypointNetwork) return;

        // Calculatehow much the current waypoint index needs to be incremented
        int incStep = increment ? 1 : 0;
        Transform nextWaypointTransform = null;

        // Calculate index of next waypoint factoring in the increment with wrap-around and fetch waypoint 
        int nextWaypoint = (currentIndex + incStep >= waypointNetwork.Waypoints.Count) ? 0 : currentIndex + incStep;
        nextWaypointTransform = waypointNetwork.Waypoints[nextWaypoint];

        // Assuming we have a valid waypoint transform
        if (nextWaypointTransform != null)
        {
            // Update the current waypoint index, assign its position as the NavMeshAgents
            // Destination and then return
            currentIndex = nextWaypoint;
            agent.destination = nextWaypointTransform.position;
            return;
        }

        // We did not find a valid waypoint in the list for this iteration
        currentIndex = nextWaypoint;
    }

    // ---------------------------------------------------------
    // Name	:	Update
    // Desc	:	Called each frame by Unity
    // ---------------------------------------------------------
    void Update()
    {

        // Copy NavMeshAgents state into inspector visible variables
        hasPath = agent.hasPath;
        pathPending = agent.pathPending;
        pathStale = agent.isPathStale;
        pathStatus = agent.pathStatus;

        // Transform agents desired velocity into local space
        Vector3 localDesiredVelocity = transform.InverseTransformVector(agent.desiredVelocity);

        // Get angle in degrees we need to turn to reach the desired velocity direction
        float angle = Mathf.Atan2(localDesiredVelocity.x, localDesiredVelocity.z) * Mathf.Rad2Deg;

        // Smoothly interpolate towards the new angle
        smoothangle = Mathf.MoveTowardsAngle(smoothangle, angle, 80.0f * Time.deltaTime);

        // Speed is simply the amount of desired velocity projected onto our own forward vector
        float speed = localDesiredVelocity.z;

        // Set animator parameters
        animator.SetFloat("Angle", smoothangle);
        animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);

        if (agent.desiredVelocity.sqrMagnitude > Mathf.Epsilon)
        {
            if (!mixedMode ||
                (mixedMode && Mathf.Abs(angle) < 80.0f && animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Locomotion")))
            {
                Quaternion lookRotation = Quaternion.LookRotation(agent.desiredVelocity, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 5.0f * Time.deltaTime);
            }
        }

        // If agent is on an offmesh link then perform a jump
        /*if (_navAgent.isOnOffMeshLink)
		{
			StartCoroutine( Jump( 1.0f) );
			return;
		}*/

        // If we don't have a path and one isn't pending then set the next
        // waypoint as the target, otherwise if path is stale regenerate path
        if ((agent.remainingDistance <= agent.stoppingDistance && !pathPending) || pathStatus == NavMeshPathStatus.PathInvalid /*|| PathStatus==NavMeshPathStatus.PathPartial*/)
        {
            SetNextDestination(true);
        }
        else
        if (agent.isPathStale)
            SetNextDestination(false);

    }

    // ----------------------------------------------------------
    // Name	:	OnAnimatorMove
    // Desc	:	Called by Unity to allow application to process
    //			and apply root motion
    // ----------------------------------------------------------
    void OnAnimatorMove()
    {
        // If we are in mixed mode and we are not in the Locomotion state then apply rooy rotation
        if (mixedMode && !animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Locomotion"))
            transform.rotation = animator.rootRotation;

        // Override Agent's velocity with the velocity of the root motion
        agent.velocity = animator.deltaPosition / Time.deltaTime;
    }

    // ---------------------------------------------------------
    // Name	:	Jump
    // Desc	:	Manual OffMeshLInk traversal using an Animation
    //			Curve to control agent height.
    // ---------------------------------------------------------
    IEnumerator Jump(float duration)
    {
        // Get the current OffMeshLink data
        OffMeshLinkData data = agent.currentOffMeshLinkData;

        // Start Position is agent current position
        Vector3 startPos = agent.transform.position;

        // End position is fetched from OffMeshLink data and adjusted for baseoffset of agent
        Vector3 endPos = data.endPos + (agent.baseOffset * Vector3.up);

        // Used to keep track of time
        float time = 0.0f;

        // Keeo iterating for the passed duration
        while (time <= duration)
        {
            // Calculate normalized time
            float t = time / duration;

            // Lerp between start position and end position and adjust height based on evaluation of t on Jump Curve
            agent.transform.position = Vector3.Lerp(startPos, endPos, t) + (jumpCurve.Evaluate(t) * Vector3.up);

            // Accumulate time and yield each frame
            time += Time.deltaTime;
            yield return null;
        }

        // NOTE : Added this for a bit of stability to make sure the
        //        Agent is EXACTLY on the end position of the off mesh
        //		  link before completeing the link.
        agent.transform.position = endPos;

        // All done so inform the agent it can resume control
        agent.CompleteOffMeshLink();
    }
}
