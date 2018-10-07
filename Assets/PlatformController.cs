using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlatformController : MonoBehaviour {
	
	[SerializeField]
	private GameObject[] platforms = new GameObject[3];

	private GameObject currentPlatform;
	private int indexOfPlatform = 0;
	
	[SerializeField]
	private bool lowePrevious = false;
	

	[SerializeField]
	private float speed, initialHeight;

	
	public bool LowerPrevious
	{
		get { return lowePrevious; }
		set { lowePrevious = value; }
	}

	// Use this for initialization
	void Start ()
	{
		//platforms = GameObject.FindGameObjectsWithTag("Platform");
		currentPlatform = platforms[0];
		platforms[1].transform.position = new Vector3(platforms[1].transform.position.x, -initialHeight, platforms[1].transform.position.z);
		platforms[2].transform.position = new Vector3(platforms[2].transform.position.x, -initialHeight, platforms[2].transform.position.z);
		platforms[1].active = false;
		platforms[2].active = false;
		
		
	}

	public void changePlatform()
	{
		indexOfPlatform++;
		platforms[indexOfPlatform].active = true;
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (currentPlatform != platforms[indexOfPlatform])
		{
			platforms[indexOfPlatform].transform.Translate(0, 0, speed);
			if (platforms[indexOfPlatform].transform.position.y >= 0)
				currentPlatform = platforms[indexOfPlatform];
		}

		if (lowePrevious)
		{
			platforms[indexOfPlatform - 1].transform.Translate(0, 0, -speed);
			if (platforms[indexOfPlatform-1].transform.position.y <= -initialHeight){
				platforms[indexOfPlatform-1].active = false;
				lowePrevious = false;
			}
		}
	}
}
