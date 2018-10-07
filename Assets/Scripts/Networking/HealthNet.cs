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
    [SyncVar] public int originLayer;
    [SerializeField] private float invisBackDelay;
    [SyncVar] public bool visible = false;


    public void OnEnable(){
        currentHealth = maxHealth;
        //set "color"
        originLayer = gameObject.layer;
    }

    [ClientRpc]
	public void RpcShowEnemy(){
        Debug.Log("RPC SHOWING enemy "+gameObject.name);
        visible = true;
        GetComponent<EnemyMovement>().RpcSetLayerOnlyRecursively(LayerMask.NameToLayer("VisibleEnemy"));
        StartCoroutine(BackInvisible());
	}
    [ClientRpc]
	public void RpcHideEnemy(){
        Debug.Log("RPC HIDING enemy "+gameObject.name);
        //gameObject.layer = LayerMask.NameToLayer(originLayer);
        GetComponent<EnemyMovement>().RpcSetLayerOnlyRecursively(originLayer);
        visible = false;
	}

    IEnumerator BackInvisible(){
        visible = false;
        yield return new WaitForSeconds(invisBackDelay); 
        //if during this delay the player doesn't make "visible" again we can definitely hide it
        if(visible == false) RpcHideEnemy();
    }

    [ClientRpc]
	public void RpcKillEnemy(){
        Debug.Log("RPC killing enemy "+gameObject.name);
        gameObject.SetActive(false); //a changer car degat depend du framerate ici
	}

    // void OnMouseOver()
    // {
    //     TakeDamage(Time.deltaTime * 25);
    // }

    [Command]
    public void CmdTakeDamage(float amount)
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