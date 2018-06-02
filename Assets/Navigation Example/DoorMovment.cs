using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DoorState{ Open, Animating, Closed };

public class DoorMovment : MonoBehaviour {

    [SerializeField] float slidingDistance = 3f;
    [SerializeField] float duration = 1.5f;
    [SerializeField] AnimationCurve slideCurve = new AnimationCurve();

    Vector3 openPos;
    Vector3 closedPos;
    DoorState doorstate = DoorState.Closed;

    // Use this for initialization
    void Start () {

        closedPos = transform.position;
        openPos = closedPos + (transform.right * slidingDistance);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space)&& doorstate != DoorState.Animating)
        {
            StartCoroutine(AnimateDoor(doorstate == DoorState.Open ? DoorState.Closed : DoorState.Open));
        }

	}

    IEnumerator AnimateDoor(DoorState newState)
    {
        doorstate = DoorState.Animating;
        float time = 0;
        Vector3 startPos = (newState == DoorState.Open) ? closedPos : openPos;
        Vector3 endPos = (newState == DoorState.Open) ? openPos : closedPos;

        while(time <= duration)
        {
            float t = time / duration;
            transform.position = Vector3.Lerp(startPos, endPos, slideCurve.Evaluate(t));
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        doorstate = newState;
    }
}
