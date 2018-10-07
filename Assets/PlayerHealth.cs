using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.ImageEffects;
using Random = UnityEngine.Random;

public class PlayerHealth : NetworkBehaviour
{

	public bool hit = false, healing = false;

	[SerializeField]
	private float timer, maxTime;

	private Camera cam;

	[SerializeField][SyncVar]
	public int nHits = 0;

	private PlayerMovement player;
	
	private int lightIndex = 0;
	
	[SerializeField]
	private Color[] buttonColors = {new Color(57, 186, 75), new Color(42, 54, 55, 255), new Color(255, 176, 52, 255), new Color(207, 33, 39, 255)};
	private String[] buttonNames = {"btnA", "btnX", "btnY", "btnB"};
	[SerializeField]
    [SyncVar]
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
	}

    void OnCollisionEnter(Collision col)
    {
        if (!isLocalPlayer) return;
        if (col.gameObject.tag.EndsWith("Enemy"))
        {
            if(( //color is SAME
				(( col.gameObject.GetComponent<HealthNet>().originLayer == LayerMask.NameToLayer("BlueEnemy")) && (GetComponent<PlayerMovementNetwork>().objectColor == Color.blue))
				|| (( col.gameObject.GetComponent<HealthNet>().originLayer == LayerMask.NameToLayer("RedEnemy")) && (GetComponent<PlayerMovementNetwork>().objectColor == Color.red)))
			){
                CmdHit();
                col.gameObject.GetComponent<HealthNet>().CmdTakeDamage(1000);
            }
            
        }
    }

    [Command]
	public void CmdHit()
	{
        
        Debug.Log("cmd hit");
        //TODO : BRUIT QUAND ON SE FAIT HIT
        if (nHits < 5)
        {
            nHits++;
            switch (nHits)
            {
                case 1:
                    GetComponent<PlayerMovementNetwork>().speed = 0;
                    GetComponent<PlayerMovementNetwork>().shock = true;
                    healLight.enabled = true;
                    return;

                // case 3:
                //     GetComponent<PlayerMovementNetwork>().Speed /= 2;
                //     break;
                // case 4:
                //     cam.gameObject.GetComponent<BlurOptimized>().enabled = true;
                //     break;
                // case 5:
                //     GetComponent<PlayerMovementNetwork>().Speed = 0;
                //     GetComponent<PlayerMovementNetwork>().Shock = true;
                //     healLight.enabled = true;
                //     newKeyToPress();
                //     break;
            }
        }
        RpcHit();
    }

    [ClientRpc]
    public void RpcHit(){
        Debug.Log("rpc hit");
        if (nHits < 5)
        {
            nHits++;
            switch (nHits)
            {
                case 1:
                    GetComponent<PlayerMovementNetwork>().speed = 0;
                    GetComponent<PlayerMovementNetwork>().shock = true;
                    healLight.enabled = true;
                    return;

                // case 3:
                //     GetComponent<PlayerMovementNetwork>().Speed /= 2;
                //     break;
                // case 4:
                //     cam.gameObject.GetComponent<BlurOptimized>().enabled = true;
                //     break;
                // case 5:
                //     GetComponent<PlayerMovementNetwork>().Speed = 0;
                //     GetComponent<PlayerMovementNetwork>().Shock = true;
                //     healLight.enabled = true;
                //     newKeyToPress();
                //     break;
            }
        }
    }

	private void newKeyToPress()
	{
        //int newIndex = 0;
        //do
        //{
        //	newIndex = Random.Range(0, 3);

        //} while (newIndex == lightIndex);

        // lightIndex = newIndex;
        // healLight.color = buttonColors[lightIndex];
        // btnToPress = buttonNames[lightIndex];
        healLight.color = buttonColors[2];
        Debug.Log("SET BTN TO PRESS TO" + buttonNames[2]);
        btnToPress = buttonNames[2];

    }

    [Command]
	public void CmdRemoveEffect()
	{
        //switch (nHits)
        //{
        //	case 3:
        //              GetComponent<PlayerMovementNetwork>().Speed *= 2;
        //		break;
        //	case 4:
        //		cam.gameObject.GetComponent<BlurOptimized>().enabled = false;
        //		break;
        //	case 5:
        //              GetComponent<PlayerMovementNetwork>().Speed = GetComponent<PlayerMovementNetwork>().Original_Speed / 2;
        //              GetComponent<PlayerMovementNetwork>().Shock = false;
        //              break;
        //}

        //GetComponent<PlayerMovementNetwork>().Speed *= 2;
        //cam.gameObject.GetComponent<BlurOptimized>().enabled = false;
        //GetComponent<PlayerMovementNetwork>().Speed = GetComponent<PlayerMovementNetwork>().Original_Speed / 2;
        Debug.Log("cmd remove effect00 "+transform.position);
        GetComponent<PlayerMovementNetwork>().shock = false;
        //Debug.Log("original speed is "+GetComponent<PlayerMovementNetwork>().Original_Speed);
        GetComponent<PlayerMovementNetwork>().speed= 50;//GetComponent<PlayerMovementNetwork>().Original_Speed;
        healLight.enabled = false;
        nHits = 0;

        // nHits--;
		// healing = false;

		// if (nHits == 0)
		// {
		// 	healLight.enabled = false;
		// }
		// else
		// {
		// 	newKeyToPress();
		// }

        RpcRemoveEffet();
	}

    [ClientRpc]
    public void RpcRemoveEffet(){
        Debug.Log("rpc remove effect00 "+transform.position);
        GetComponent<PlayerMovementNetwork>().shock = false;
        //Debug.Log("original speed is "+GetComponent<PlayerMovementNetwork>().Original_Speed);
        GetComponent<PlayerMovementNetwork>().speed = 50;//GetComponent<PlayerMovementNetwork>().Original_Speed;
        healLight.enabled = false;
        nHits = 0;
        Debug.Log("rpc remove effect DOOOOOOOONE "+transform.position);

        // nHits--;
		// healing = false;

		// if (nHits == 0)
		// {
		// 	healLight.enabled = false;
		// }
		// else
		// {
		// 	newKeyToPress();
		// }
    }
}
