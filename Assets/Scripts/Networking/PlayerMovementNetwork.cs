using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovementNetwork : NetworkBehaviour {


	[SerializeField]
	private float speed;

	private float x, z;
	
	private Rigidbody rb;

	private Vector3 facingVect = new Vector3(1,1,1);

	// Use this for initialization
	void Start () {
			rb = this.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            // exit from update if this is not the local player
            return;
        }

        // handle player input for movement
		//facing will be used to make the character move relative to the camera
		float facing = Camera.main.transform.eulerAngles.y;

		//Set the variables according to the imput. They will be used for the movement of the player
		x = Input.GetAxis("Horizontal");
		z = Input.GetAxis("Vertical");  //We use z cause unity doesn't understand standard axis
		
		//We set the new velocity without the camera facing. We keep the normal y speed of the rigidbody because at 0 the character will float in the air instead of falling
		Vector3 inputs = new Vector3(x * speed, this.GetComponent<Rigidbody>().velocity.y, z * speed);

		//We change our inputs vector so it rotates so the player can move relative to how the camera is positionned
		Vector3 myTurnedInputs = Quaternion.Euler(0, facing, 0) * inputs;

		//We set the velocity of the player with our new inputs
		this.GetComponent<Rigidbody>().velocity = myTurnedInputs;

		//We rotate our character so turns to the direction he is moving towards
		facingVect = new Vector3(myTurnedInputs.x + transform.position.x, transform.position.y , myTurnedInputs.z + transform.position.z);
        

		transform.LookAt(facingVect);
    }
}