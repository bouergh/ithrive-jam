using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	private Transform target;

	private Vector3 offset;

	// Use this for initialization
	void Start ()
	{
		target = GameObject.FindGameObjectWithTag("Player").transform;
		offset = transform.position - target.position;
	}
	
	// Update is called once per frame
	void Update () {
		
		transform.position = target.position + offset;
		transform.LookAt(target);
	}
}
