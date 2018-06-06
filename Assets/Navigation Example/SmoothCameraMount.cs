﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCameraMount : MonoBehaviour {

    [SerializeField] Transform mount;
    [SerializeField] float speed = 5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
        transform.position = Vector3.Lerp(transform.position, mount.position, Time.deltaTime * speed);
        transform.rotation = Quaternion.Slerp(transform.rotation, mount.rotation, Time.deltaTime * speed);
	}
}
