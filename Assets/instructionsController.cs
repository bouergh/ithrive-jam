using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class instructionsController : MonoBehaviour
{

	[SerializeField]
	private Image image;

	// Use this for initialization
	void Start ()
	{
		image = this.GetComponent<Image>();
	}

	public void makeImageAppear()
	{
		image.enabled = !image.enabled;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
