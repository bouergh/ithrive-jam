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
    [SyncVar] string originLayer;


    public void OnEnable(){
        currentHealth = maxHealth;
        //set "color"
        originLayer = LayerMask.LayerToName(gameObject.layer);
    }

    [Command]
	public void CmdShowEnemy(){
            Debug.Log("CMD showing enemy "+gameObject.name);
            gameObject.layer = LayerMask.NameToLayer("VisibleEnemy");
            RpcShowEnemy();
	}
    [ClientRpc]
	public void RpcShowEnemy(){
        Debug.Log("RPC showing enemy "+gameObject.name);
        gameObject.layer = LayerMask.NameToLayer("VisibleEnemy");
	}

    void OnMouseOver()
    {
        TakeDamage(Time.deltaTime * 25);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            if (destroyOnDeath)
            {
                gameObject.SetActive(false);
            }
        }
    }


}