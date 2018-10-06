using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;

public class CameraController : MonoBehaviour
{

	[SerializeField]
	private Transform[] cameraPositions = new Transform[3];

	[SerializeField]
	private Vector3 targetPosition;

	private int indexOfPosition = 0;

	[SerializeField]
	private float speed;
	// Use this for initialization
	void Start ()
	{
		GameObject[] positionObjects = GameObject.FindGameObjectsWithTag("CameraPosition");
		Debug.Log(positionObjects);
		for (int i = 0; i < positionObjects.Length; i++)
		{
			cameraPositions[i] = positionObjects[i].transform;
		}

		transform.position = cameraPositions[0].position;
		targetPosition = cameraPositions[indexOfPosition].position;
	}

	public void changeTarget()
	{
		indexOfPosition++;
		targetPosition = cameraPositions[indexOfPosition].position;
	}
	
	
	// Update is called once per frame
	void Update () {
		if (transform.position != cameraPositions[indexOfPosition].position)
		{
			float step = speed * Time.deltaTime;
			transform.position = Vector3.MoveTowards(transform.position, cameraPositions[indexOfPosition].position, step);
		}
	}
}
