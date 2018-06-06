using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

    Animator animator;
    int horizontalHash;
    int verticalHash;
    int attackHash;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        horizontalHash = Animator.StringToHash("Horizontal");
        verticalHash = Animator.StringToHash("Vertical");
        attackHash = Animator.StringToHash("Attack");
    }
	
	// Update is called once per frame
	void Update () {
        float xAxis = Input.GetAxis("Horizontal") * 2.32f;
        float yAxis = Input.GetAxis("Vertical") * 5.66f;

        animator.SetFloat(horizontalHash, xAxis , 0.1f, Time.deltaTime);
        animator.SetFloat(verticalHash, yAxis, 1f, Time.deltaTime);

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger(attackHash);
        }
	}
}
