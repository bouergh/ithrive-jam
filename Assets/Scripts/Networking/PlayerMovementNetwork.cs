using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovementNetwork : NetworkBehaviour {

	
	[SyncVar] private Color objectColor;

	[SerializeField]
	private float speed;
	public float Speed
	{
		get
		{
			return speed;
		}
		set { speed = value; }
	}
	
	[SerializeField] private float lightIntensity;

 	[SyncVar] private float x, z, xAim, zAim;
	private GameObject flashLight, tagLight;
	[SyncVar] private Vector3 turnedAim;

	private Animator anim;
	
	private Rigidbody rb;

	private Vector3 facingVect = new Vector3(1,1,1);

	[SerializeField] private MeshRenderer coat;

	// Use this for initialization
	void Start () {
		rb = this.GetComponent<Rigidbody>();
		flashLight = transform.GetChild(0).gameObject;
		tagLight = transform.GetChild(1).gameObject;
		anim = GetComponent<Animator>();
		//if(isLocalPlayer)
		{
			coat = transform.FindDeepChild("coat").GetComponent<MeshRenderer>();
			Debug.Log("coat is "+coat.gameObject.name);
		}
		//set color + camera culling mask to be red and see only red for server, VS blue for player
		ChangeColor();
	}

	public void ChangeColor(){
		if(isLocalPlayer){
			if(isServer){
				Camera.main.cullingMask &=  ~(1 << LayerMask.NameToLayer("BlueEnemy")); //to remove ONLY this layer from the culling mask
				Debug.Log("is server local player");
				objectColor = Color.red;
				//GetComponent<MeshRenderer>().material.color = objectColor; 
				coat.material.color = objectColor;//change it on server
				RpcChangeColor(gameObject,objectColor); //change it on client

			}else //need this because server is BOTH
			if(isClient){ //will be called by second instance because CLIENT
			Camera.main.cullingMask &=  ~(1 << LayerMask.NameToLayer("RedEnemy")); //to remove ONLY this layer from the culling mask
				Debug.Log("is client local player");
				objectColor = Color.blue;
				//GetComponent<MeshRenderer>().material.color = objectColor; 
				coat.material.color = objectColor;//change it on client
				CmdChangeServerColor(gameObject, objectColor); //change it on server + call ChangeColor AGAIN on server
			}
		}
		
	}
	

	[Command]
	public void CmdChangeServerColor(GameObject obj, Color col){
		//receive color change from client and change it on server
		Debug.Log("received a command");
		//obj.GetComponent<MeshRenderer>().material.color = col;
		obj.GetComponent<PlayerMovementNetwork>().coat.material.color = col;
			
		//send the server player color to the client to change it
		foreach(GameObject servObj in GameObject.FindGameObjectsWithTag("Player")){
			servObj.GetComponent<PlayerMovementNetwork>().ChangeColor();
		}
	}
	

	[ClientRpc]
	public void RpcChangeColor(GameObject obj, Color col){
		//obj.GetComponent<MeshRenderer>().material.color = col;
		obj.GetComponent<PlayerMovementNetwork>().coat.material.color = col;
	}

 
	void Update()
	{
		if(!isLocalPlayer) return;
		
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

	// Update is called once per frame
	void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            // exit from update if this is not the local player
            return;
        }

        //facing will be used to make the character move relative to the camera
		float facing = Camera.main.transform.eulerAngles.y;

		//Set the variables according to the imput. They will be used for the movement of the player
		x = Input.GetAxis("Horizontal");
		z = Input.GetAxis("Vertical");  //We use z cause unity doesn't understand standard axis

		if (x != 0 || z != 0)
		{
			anim.SetBool("walking", true);
		}
		else
		{
			anim.SetBool("walking", false);
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
		//Debug.Log(xAim + " " + zAim);
		if (xAim != 0 || zAim != 0)
		{
			turnedAim = Quaternion.Euler(0, facing, 0) * aim;
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