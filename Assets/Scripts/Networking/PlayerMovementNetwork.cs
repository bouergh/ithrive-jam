using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


// //IMPORTANT :
// R = joueur rouge (serveur)
// B = joueur bleu (client)
// b = ennemi bleu
// r = ennemi rouge

// tag = rendre visible les monstres qu'on voit pour son allie
// kill = tuer les monstres qu'on ne voit pas ou qui ont ete rendu visibles par l'allie

// B voit r, B tag r avec lumiere rouge (LT)
// B ne voit pas b, B tue b avec lumiere bleue (RT) et peut etre tue par b

// R voit b, R tag b avec lumiere bleue (LT)
// R ne voit pas r, R tue r avec lumiere rouge (RT) et peut etre tue par r


public class PlayerMovementNetwork : NetworkBehaviour {

	
	[SyncVar] public Color objectColor, otherColor;
	[SyncVar] public bool isRedPlayer;


	[SerializeField] [SyncVar] public float speed;
	
	[SerializeField] private float lightIntensity;

 	[SyncVar] private float x, z, xAim, zAim;
	[SerializeField] private GameObject flashLight, tagLight;
	[SyncVar] private Vector3 turnedAim;

	private Animator anim;
	
	private Rigidbody rb;

	private Vector3 facingVect = new Vector3(1,1,1);

	[SerializeField] private SkinnedMeshRenderer coat;

    [SerializeField] [SyncVar] public bool shock = false;

    [SerializeField]
    private const float ORIGINAL_SPEED = 13;

    public float Original_Speed
    {
        get { return ORIGINAL_SPEED; }
    }


    // Use this for initialization
    void Start () {
        speed = ORIGINAL_SPEED;
        rb = this.GetComponent<Rigidbody>();
		//flashLight = transform.GetChild(0).gameObject; ///ATTENTION NE PAS CHANGER L'ORDRE
		//tagLight = transform.GetChild(1).gameObject;
		flashLight = transform.FindDeepChild("FlashLight").gameObject; ///ATTENTION NE PAS CHANGER LES NOMS
		tagLight = transform.FindDeepChild("tagLight").gameObject;
		anim = GetComponent<Animator>();
		//if(isLocalPlayer)
		{
			coat = transform.FindDeepChild("coat").GetComponent<SkinnedMeshRenderer>();
			Debug.Log("coat is "+coat.gameObject.name);
		}
		//set color + camera culling mask to be red and see only red for server, VS blue for player
		ChangeColor();
	}

	public void ChangeColor(){
		if(isLocalPlayer){
			if(isServer){//joueur ROUGE
				Camera.main.cullingMask &=  ~(1 << LayerMask.NameToLayer("RedEnemy")); //to remove ONLY this layer from the culling mask
				Debug.Log("is server local player");
				objectColor = Color.red;
				otherColor = Color.blue;
				//GetComponent<MeshRenderer>().material.color = objectColor; 
				coat.material.color = objectColor;//change it on server
				flashLight.GetComponent<Light>().color = objectColor;
				tagLight.GetComponent<Light>().color = otherColor;

				RpcChangeColor(gameObject, objectColor, otherColor); //change it on client

			}else //need this because server is BOTH 
			if(isClient){ //will be called by second instance because CLIENT //joueur BLEU
				Camera.main.cullingMask &=  ~(1 << LayerMask.NameToLayer("BlueEnemy")); //to remove ONLY this layer from the culling mask
				Debug.Log("is client local player");
				objectColor = Color.blue;
				otherColor = Color.red;
				//GetComponent<MeshRenderer>().material.color = objectColor; 
				coat.material.color = objectColor;//change it on client
				flashLight.GetComponent<Light>().color = objectColor;
				tagLight.GetComponent<Light>().color = otherColor;
				CmdChangeServerColor(gameObject, objectColor, otherColor); //change it on server + call ChangeColor AGAIN on server
			}
		}
		
	}
	
	[Command]
	public void CmdChangeServerColor(GameObject obj, Color col, Color otherCol){
		//receive color change from client and change it on server
		Debug.Log("received a command");
		//obj.GetComponent<MeshRenderer>().material.color = col;
		obj.GetComponent<PlayerMovementNetwork>().objectColor = col;
		obj.GetComponent<PlayerMovementNetwork>().otherColor = otherCol;
		obj.GetComponent<PlayerMovementNetwork>().coat.material.color = col;
		obj.GetComponent<PlayerMovementNetwork>().flashLight.GetComponent<Light>().color = col;
		obj.GetComponent<PlayerMovementNetwork>().tagLight.GetComponent<Light>().color = otherCol;
			
		//send the server player color to the client to change it
		foreach(GameObject servObj in GameObject.FindGameObjectsWithTag("Player")){
			servObj.GetComponent<PlayerMovementNetwork>().ChangeColor();
		}
	}
	
	[ClientRpc]
	public void RpcChangeColor(GameObject obj, Color col, Color otherCol){
		if(!isClient) return;
		//obj.GetComponent<MeshRenderer>().material.color = col;
		obj.GetComponent<PlayerMovementNetwork>().objectColor = col;
		obj.GetComponent<PlayerMovementNetwork>().otherColor = otherCol;
		obj.GetComponent<PlayerMovementNetwork>().coat.material.color = col;
		obj.GetComponent<PlayerMovementNetwork>().flashLight.GetComponent<Light>().color = col;
		obj.GetComponent<PlayerMovementNetwork>().tagLight.GetComponent<Light>().color = otherCol;
	}

	
	void Update()
	{
		if(!isLocalPlayer) return;
		if(!isClient) return;
        if (shock) return;


		if (Input.GetAxis("Fire1") > 0 && Input.GetAxis("Fire2") <= 0)
		{
			float intensityMult = Input.GetAxis("Fire1");
			//flashLight.active = true;
			//flashLight.GetComponent<Light>().intensity = intensityMult * lightIntensity;
			CmdOnFlashLight(gameObject, intensityMult);
		}
		else
		{
			//flashLight.active = false;
			//if(flashLight.activeInHierarchy) 
			CmdOffFlashLight(gameObject);
		}

		if (Input.GetAxis("Fire2") > 0 && Input.GetAxis("Fire1") <= 0)
		{
			float intensityMult = Input.GetAxis("Fire2");
			//tagLight.active = true;
			//tagLight.GetComponent<Light>().intensity = Input.GetAxis("Fire2") * lightIntensity;
			CmdOnTagLight(gameObject, intensityMult);
		}
		else
		{
			//tagLight.active = false;
			//if(tagLight.activeInHierarchy) 
			CmdOffTagLight(gameObject);
		}

	}

	[Command]
	public void CmdOnFlashLight(GameObject obj,float intensityMult){
		obj.GetComponent<PlayerMovementNetwork>().flashLight.GetComponent<Light>().enabled = true;
		obj.GetComponent<PlayerMovementNetwork>().flashLight.GetComponent<CapsuleCollider>().enabled = true;
		obj.GetComponent<PlayerMovementNetwork>().flashLight.GetComponent<Light>().intensity = intensityMult * lightIntensity;	
		if(isServer) RpcOnFlashLight(obj, intensityMult);
	}
	[ClientRpc]
	public void RpcOnFlashLight(GameObject obj, float intensityMult){
		if(!isClient) return;
		//Debug.Log("change intensity of light");
		//obj.SetActive(true);
		obj.GetComponent<PlayerMovementNetwork>().flashLight.GetComponent<Light>().enabled = true;
		obj.GetComponent<PlayerMovementNetwork>().flashLight.GetComponent<CapsuleCollider>().enabled = true;
		obj.GetComponent<PlayerMovementNetwork>().flashLight.GetComponent<Light>().intensity = intensityMult * lightIntensity;	
	}
	[Command]
	public void CmdOffFlashLight(GameObject obj){
		obj.GetComponent<PlayerMovementNetwork>().flashLight.GetComponent<Light>().enabled = false;
		obj.GetComponent<PlayerMovementNetwork>().flashLight.GetComponent<CapsuleCollider>().enabled = false;
		if(isServer) RpcOffFlashLight(obj);
	}
	[ClientRpc]
	public void RpcOffFlashLight(GameObject obj){
		if(!isClient) return;
		//Debug.Log("turn off light");
		//obj.SetActive(false);	
		obj.GetComponent<PlayerMovementNetwork>().flashLight.GetComponent<Light>().enabled = false;
		obj.GetComponent<PlayerMovementNetwork>().flashLight.GetComponent<CapsuleCollider>().enabled = false;
	}
	[Command]
	public void CmdOnTagLight(GameObject obj, float intensityMult){
		obj.GetComponent<PlayerMovementNetwork>().tagLight.GetComponent<Light>().enabled = true;
		obj.GetComponent<PlayerMovementNetwork>().tagLight.GetComponent<CapsuleCollider>().enabled = true;
		obj.GetComponent<PlayerMovementNetwork>().tagLight.GetComponent<Light>().intensity = intensityMult * lightIntensity;	
		if(isServer) RpcOnTagLight(obj, intensityMult);
	}
	[ClientRpc]
	public void RpcOnTagLight(GameObject obj, float intensityMult){
		if(!isClient) return;
		//Debug.Log("change intensity of light");
		//obj.SetActive(true);
		obj.GetComponent<PlayerMovementNetwork>().tagLight.GetComponent<Light>().enabled = true;
		obj.GetComponent<PlayerMovementNetwork>().tagLight.GetComponent<CapsuleCollider>().enabled = true;
		obj.GetComponent<PlayerMovementNetwork>().tagLight.GetComponent<Light>().intensity = intensityMult * lightIntensity;	
	}
	[Command]
	public void CmdOffTagLight(GameObject obj){
		obj.GetComponent<PlayerMovementNetwork>().tagLight.GetComponent<Light>().enabled = false;
		obj.GetComponent<PlayerMovementNetwork>().tagLight.GetComponent<CapsuleCollider>().enabled = false;
		if(isServer) RpcOffTagLight(obj);
	}
	[ClientRpc]
	public void RpcOffTagLight(GameObject obj){
		if(!isClient) return;
		//Debug.Log("turn off light");
		//obj.SetActive(false);	
		obj.GetComponent<PlayerMovementNetwork>().tagLight.GetComponent<Light>().enabled = false;
		obj.GetComponent<PlayerMovementNetwork>().tagLight.GetComponent<CapsuleCollider>().enabled = false;
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
			anim.SetBool("Walking", true);
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

	void OnTriggerStay(Collider other){

		if(!isLocalPlayer) return;
		if(!isClient) return;

		if (other.gameObject.CompareTag("Player"))
        {
			Debug.Log("players colliding");
            //PlayerHealth otherHealth = other.gameObject.GetComponent<PlayerHealth>();
            //Debug.Log(otherHealth.BtnToPress);
            //if (Input.GetButtonDown(otherHealth.BtnToPress))
			if (Input.GetButtonDown("btnY")) //ok
            {
				if(gameObject.GetComponent<PlayerHealth>().nHits > 0){
					Debug.Log( transform.position + " healing "+ other.transform.position);
                	other.gameObject.GetComponent<PlayerHealth>().CmdRemoveEffect();
				}
            }
        }


        
        //Debug.Log("a) player detected trigger");
        if (other.tag.EndsWith("Enemy")){
			//Debug.Log("b) player detected enemy " + other.gameObject.name);

			// if(tagLight && ( //color is the same
			// 	((LayerMask.LayerToName(other.gameObject.layer) == "RedEnemy") && (objectColor == Color.red)
			// 	|| (LayerMask.LayerToName(other.gameObject.layer) == "BlueEnemy")) && (objectColor == Color.blue)
			// )) { //show enemy to friendo
			// 	Debug.Log("c) player will show enemy !");
			// 	other.GetComponent<HealthNet>().CmdShowEnemy();	
			if(tagLight.GetComponent<Light>().enabled){
				//Debug.Log("c) tagging");
				if(objectColor == Color.red){
					//Debug.Log("d) player is Red");
					if(LayerMask.LayerToName(other.gameObject.layer) == "BlueEnemy"){
						//Debug.Log("e) blue enemy spotted by red player red light too ! player will SHOW enemy !");
						CmdShowEnemy(other.gameObject);
					}
				}else{
					if(objectColor == Color.blue){
					//Debug.Log("d) player is Blue");
						if(LayerMask.LayerToName(other.gameObject.layer) == "RedEnemy"){
							Debug.Log("e) red enemy spotted by blue player blue light too ! player will SHOW enemy !");
							CmdShowEnemy(other.gameObject);
						}
					}
				}
			}
			else if(flashLight.GetComponent<Light>().enabled && ( //color is different
				(((other.GetComponent<HealthNet>().originLayer == LayerMask.NameToLayer("BlueEnemy")) && (objectColor == Color.blue))
				|| ((other.GetComponent<HealthNet>().originLayer == LayerMask.NameToLayer("RedEnemy")) && (objectColor == Color.red)))
			)){ //kill enemy
				Debug.Log("c) player will DAMAGE enemy !");
				CmdDmgEnemy(other.gameObject);

			}
		}
	}


	[Command]
	void CmdShowEnemy(GameObject obj){
		if(isServer) obj.GetComponent<HealthNet>().RpcShowEnemy();
	}
	[Command]
	void CmdDmgEnemy(GameObject obj){
		if(isServer) obj.GetComponent<HealthNet>().CmdTakeDamage(1f);
	}


	
}
