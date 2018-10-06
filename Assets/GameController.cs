using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

	private CameraController _cameraController;
	[SerializeField]
	private PlatformController _platformController;

	[SerializeField]
	private bool change = false;

	// Use this for initialization
	void Start ()
	{
		_cameraController = Camera.main.gameObject.GetComponent<CameraController>();

	}
	
	// Update is called once per frame
	void Update () {
		if (change)
		{
			_cameraController.changeTarget();
			_platformController.changePlatform();
			change = false;
		}
	}
}
