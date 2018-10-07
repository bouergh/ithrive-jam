using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HealthNet : NetworkBehaviour
{
    float dmgDelayTime;

    public bool destroyOnDeath;

    public int maxHealth;
    [SyncVar] float currentHealth;
    [SyncVar] public string originLayer;


    public void OnEnable(){
        currentHealth = maxHealth;
        //set "color"
        originLayer = LayerMask.LayerToName(gameObject.layer);
    }

    [ClientRpc]
	public void RpcShowEnemy(){
        Debug.Log("RPC showing enemy "+gameObject.name);
        gameObject.layer = LayerMask.NameToLayer("VisibleEnemy");
	}
    [ClientRpc]
	public void RpcKillEnemy(){
        Debug.Log("RPC killing enemy "+gameObject.name);
        gameObject.SetActive(false); //a changer car degat depend du framerate ici
	}

    void OnMouseOver()
    {
        TakeDamage(Time.deltaTime * 25);
    }

    public void TakeDamage(float amount)
    {
        Debug.Log(gameObject.name+" took "+amount+" damage !");
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            if (destroyOnDeath)
            {
                RpcKillEnemy();
                gameObject.SetActive(false);
            }
        }
    }


}