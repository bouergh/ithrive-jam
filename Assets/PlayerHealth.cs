﻿using System;
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

	private Animator anim;

	public string BtnToPress
	{
		get { return btnToPress; }
		set { btnToPress = value; }
	}

	[SerializeField]
	public GameObject healLight;


	// Use this for initialization
	void Start () {
		cam = Camera.main;
		player = GetComponent<PlayerMovement>();
		healLight = transform.FindDeepChild("Point Light").gameObject;
		healLight.GetComponent<Light>().enabled = false;
		anim = GetComponent<Animator>();
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
                CmdHit(gameObject);
                col.gameObject.GetComponent<HealthNet>().CmdTakeDamage(1000);
            }
            
        }
    }

    [Command]
	public void CmdHit(GameObject go)
	{

        
        // go.GetComponent<PlayerMovementNetwork>().speed = 1f;
        // go.GetComponent<PlayerMovementNetwork>().shock = true;
        // go.GetComponent<PlayerHealth>().healLight.GetComponent<Light>().enabled = true;

        // Debug.Log("rpc hit");
        // if (nHits < 5)
        // {
        //     nHits++;
        //     switch (nHits)
        //     {
        //         // case 1:
        //         //     go.GetComponent<PlayerMovementNetwork>().speed = 1f;
        //         //     go.GetComponent<PlayerMovementNetwork>().shock = true;
        //         //     go.GetComponent<PlayerHealth>().healLight.GetComponent<Light>().enabled = true;
        //         //     return;

        //         case 3:
        //             GetComponent<PlayerMovementNetwork>().speed /= 2;
        //             break;
        //         case 4:
        //             cam.gameObject.GetComponent<BlurOptimized>().enabled = true;
        //             break;
        //         case 5:
        //             go.GetComponent<PlayerMovementNetwork>().speed = 1f;
        //             go.GetComponent<PlayerMovementNetwork>().shock = true;
        //             go.GetComponent<PlayerHealth>().healLight.GetComponent<Light>().enabled = true;
        //             newKeyToPress();
        //             break;
        //     }
        // }
        RpcHit(gameObject);
    }

    [ClientRpc]
    public void RpcHit(GameObject go){

        
        // go.GetComponent<PlayerMovementNetwork>().speed = 1f;
        // go.GetComponent<PlayerMovementNetwork>().shock = true;
        // go.GetComponent<PlayerHealth>().healLight.GetComponent<Light>().enabled = true;

        Debug.Log("rpc hit");
        if (go.GetComponent<PlayerHealth>().nHits < 5)
        {
            nHits++;
            switch (go.GetComponent<PlayerHealth>().nHits)
            {
                // case 1:
                //     go.GetComponent<PlayerMovementNetwork>().speed = 1f;
                //     go.GetComponent<PlayerMovementNetwork>().shock = true;
                //     go.GetComponent<PlayerHealth>().healLight.GetComponent<Light>().enabled = true;
                //     return;

                case 3:
                    GetComponent<PlayerMovementNetwork>().speed /= 2;
                    break;
                case 4:
                    //cam.gameObject.GetComponent<BlurOptimized>().enabled = true;
                    break;
                case 5:
                    go.GetComponent<PlayerMovementNetwork>().speed = 1f;
                    go.GetComponent<PlayerMovementNetwork>().shock = true;
                    go.GetComponent<PlayerHealth>().healLight.GetComponent<Light>().enabled = true;
	                anim.SetBool("shake", true);
                    newKeyToPress();
                    break;
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
        healLight.GetComponent<Light>().color = buttonColors[2];
        Debug.Log("SET BTN TO PRESS TO" + buttonNames[2]);
        btnToPress = buttonNames[2];

    }

    [Command]
	public void CmdRemoveEffect(GameObject go)
	{
        
        
        // switch (nHits)
        // {
        // 	case 3:
        //              GetComponent<PlayerMovementNetwork>().speed *= 2;
        // 		break;
        // 	case 4:
        // 		    //cam.gameObject.GetComponent<BlurOptimized>().enabled = false;
        // 		break;
        // 	case 5:
        //              GetComponent<PlayerMovementNetwork>().speed = GetComponent<PlayerMovementNetwork>().Original_Speed / 2;
        //              GetComponent<PlayerMovementNetwork>().shock = false;
        //              break;
        // }

        // Debug.Log("cmd remove effect00 "+transform.position);
        // //Debug.Log("original speed is "+GetComponent<PlayerMovementNetwork>().Original_Speed);
        // //go.GetComponent<PlayerMovementNetwork>().speed= go.GetComponent<PlayerMovementNetwork>().Original_Speed;
        // //nHits = 0;

        // nHits--;
		// healing = false;

		// if (nHits == 0)
		// {
		// 	go.GetComponent<PlayerHealth>().healLight.GetComponent<Light>().enabled = false;
		// }
		// else
		// {
		// 	//newKeyToPress();
		// }

        RpcRemoveEffet(go);
	}

    [ClientRpc]
    public void RpcRemoveEffet(GameObject go){
        
        switch (go.GetComponent<PlayerHealth>().nHits)
        {
        	case 3:
                     go.GetComponent<PlayerMovementNetwork>().speed *= 2;
        		break;
        	case 4:
        		    //cam.gameObject.GetComponent<BlurOptimized>().enabled = false;
        		break;
        	case 5:
                     go.GetComponent<PlayerMovementNetwork>().speed = go.GetComponent<PlayerMovementNetwork>().Original_Speed / 2;
                     go.GetComponent<PlayerMovementNetwork>().shock = false;
		        	anim.SetBool("shake", false);
                     break;
        }

        Debug.Log("cmd remove effect00 "+transform.position);
        //Debug.Log("original speed is "+GetComponent<PlayerMovementNetwork>().Original_Speed);
        //go.GetComponent<PlayerMovementNetwork>().speed= go.GetComponent<PlayerMovementNetwork>().Original_Speed;
        //nHits = 0;

        go.GetComponent<PlayerHealth>().nHits--;
		healing = false;

		if (go.GetComponent<PlayerHealth>().nHits == 0)
		{
			go.GetComponent<PlayerHealth>().healLight.GetComponent<Light>().enabled = false;
		}
		else
		{
			//newKeyToPress();
		}
    }
}