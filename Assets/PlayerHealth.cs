using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using Random = UnityEngine.Random;

public class PlayerHealth : MonoBehaviour
{

	public bool hit = false, healing = false;

	[SerializeField]
	private float timer, maxTime;

	private Camera cam;

	[SerializeField]
	private int nHits = 0;

	private PlayerMovement player;
	
	private int lightIndex = 0;
	
	[SerializeField]
	private Color[] buttonColors = {new Color(57, 186, 75), new Color(42, 54, 55, 255), new Color(255, 176, 52, 255), new Color(207, 33, 39, 255)};
	private String[] buttonNames = {"btnA", "btnX", "btnY", "btnB"};
	[SerializeField]
	private string btnToPress = "";

	public string BtnToPress
	{
		get { return btnToPress; }
		set { btnToPress = value; }
	}

	[SerializeField]
	private Light healLight;


	// Use this for initialization
	void Start () {
		cam = Camera.main;
		player = GetComponent<PlayerMovement>();
		healLight = transform.GetChild(2).gameObject.GetComponent<Light>();
		healLight.enabled = false;
		//healLight.enabled = false;
	}

	public void Hit()
	{
		
	}

	public void Heal()
	{
		healLight.enabled = true;
		newKeyToPress();
	}

	private void newKeyToPress()
	{
		int newIndex = 0;
		do
		{
			
			newIndex = Random.Range(0, 3);
			
		} while (newIndex == lightIndex);
		
		lightIndex = newIndex;
		healLight.color = buttonColors[lightIndex];
		btnToPress = buttonNames[lightIndex];
		
	}

	public void removeEffect()
	{
		switch (nHits)
		{
			case 3:
				player.Speed *= 2;
				break;
			case 4:
				cam.gameObject.GetComponent<BlurOptimized>().enabled = false;
				break;
			case 5:
				player.Speed = player.Original_Speed/2;
				player.Shock = false;
				break;
		}

		nHits--;
		healing = false;

		if (nHits == 0)
		{
			healLight.enabled = false;
		}
		else
		{
			newKeyToPress();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (hit && nHits < 5)
		{
			nHits++;
			switch (nHits)
			{
				case 3:
					player.Speed /= 2;
					break;
				case 4:
					cam.gameObject.GetComponent<BlurOptimized>().enabled = true;
					break;
				case 5:
					player.Speed = 0;
					player.Shock = true;
					healLight.enabled = true;
					newKeyToPress();
					break;
				
				
				
			}

			hit = false;
			
		}
		
	}
}
