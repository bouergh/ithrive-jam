using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Policy;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

	
	[SerializeField]
	private float speed;

	private bool shock = false;

	public bool Shock
	{
		get { return shock; }
		set { shock = value; }
	}

	[SerializeField]
	private const float ORIGINAL_SPEED = 13;

	public float Original_Speed
	{
		get { return ORIGINAL_SPEED; }
	}

	public float Speed
	{
		get
		{
			return speed;
		}
		set { speed = value; }
	}

	[SerializeField]
	private float lightIntensity;

	private float x, z;

	private float xAim, zAim;
	
	private Rigidbody rb;

	private GameObject flashLight, tagLight;
	private Vector3 turnedAim;

	private Animator anim;
	
	// Use this for initialization
	void Start ()
	{
		speed = ORIGINAL_SPEED;
		rb = GetComponent<Rigidbody>();
		flashLight = transform.GetChild(0).gameObject;
		tagLight = transform.GetChild(1).gameObject;
		anim = GetComponent<Animator>();
		
	}
	
	// Update is called once per frame
	void Update()
	{
		if (!shock)
		{
			if (Input.GetAxis("Fire1") > 0 && Input.GetAxis("Fire2") <= 0)
			{
				flashLight.active = true;
				flashLight.GetComponent<Light>().intensity = Input.GetAxis("Fire1") * lightIntensity;
			}
			else
			{
				flashLight.active = false;
			}

			if (Input.GetAxis("Fire2") > 0 && Input.GetAxis("Fire1") <= 0)
			{
				tagLight.active = true;
				tagLight.GetComponent<Light>().intensity = Input.GetAxis("Fire2") * lightIntensity;
			}
			else
			{
				tagLight.active = false;

			}
		}

	}
	
	private void onTriggerStay(Collider other)
	{
		Debug.Log("Cunt");
		if (other.CompareTag("Player"))
		{
			PlayerHealth otherHealth = other.GetComponent<PlayerHealth>();
			Debug.Log(otherHealth.BtnToPress);
			if (Input.GetButtonDown(otherHealth.BtnToPress))
			{
				otherHealth.removeEffect();
			}
		}
	}


	private void FixedUpdate()
	{
		//facing will be used to make the character move relative to the camera
		float facing = Camera.main.transform.eulerAngles.y;
		float facingAim = Camera.main.transform.eulerAngles.z;

		//Set the variables according to the imput. They will be used for the movement of the player
		x = Input.GetAxis("Horizontal");
		z = Input.GetAxis("Vertical");  //We use z cause unity doesn't understand standard axis

		if (x != 0 || z != 0)
		{
			anim.SetBool("Walking", true);
			anim.speed = Mathf.Sqrt((x * x) + (z * z));
		}
		else
		{
			anim.SetBool("Walking", false);
		}

		xAim = Input.GetAxis("Aim X");
		zAim = Input.GetAxis("Aim Y");
		
		
		//We set the new velocity without the camera facing. We keep the normal y speed of the rigidbody because at 0 the character will float in the air instead of falling
		Vector3 inputs = new Vector3(x * speed, this.GetComponent<Rigidbody>().velocity.y, z * speed);
		
		Vector3 aim = new Vector3(xAim + transform.position.x, transform.position.y, zAim + transform.position.z);

		//We change our inputs vector so it rotates so the player can move relative to how the camera is positionned
		Vector3 myTurnedInputs = Quaternion.Euler(0, facing, 0) * inputs;
		
		//We set the velocity of the player with our new inputs
		this.GetComponent<Rigidbody>().velocity = myTurnedInputs;
		
		if (xAim != 0 || zAim != 0)
		{
			
			turnedAim = Quaternion.Euler(0, 0, facingAim) * aim;
			Debug.Log(facing);
		}
		else
		{
			turnedAim = new Vector3(myTurnedInputs.x + transform.position.x, transform.position.y , myTurnedInputs.z + transform.position.z);
		}
		

		//We rotate our character so turns to the direction he is moving towards
		//facingVect = new Vector3(myTurnedInputs.x + transform.position.x, transform.position.y , myTurnedInputs.z + transform.position.z);
        

		transform.LookAt(turnedAim);

	}
}
