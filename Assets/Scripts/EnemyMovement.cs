using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnemyMovement : NetworkBehaviour
{
    const int MIN_X = -50;
    const int MAX_X = 10;
    const int MIN_Z = -13;
    const int MAX_Z = 50;

    public float vitesse;

    [SyncVar] Vector3 TargetPosition;

    Vector3 GetTargetPos()
    {
        //get layer
        //( 9->rouge)
        //10->bleu
        int layer = this.gameObject.layer;
        Vector3 pos = transform.position;
        //new Vector3(Random.Range(MIN_X,MAX_X), 0, Random.Range(MIN_Z, MAX_Z));
        foreach (GameObject cur in GameObject.FindGameObjectsWithTag("Player"))
        {
            //Debug.Log(cur.GetComponent<PlayerMovementNetwork>().objectColor);
            if ((layer == 9 && cur.GetComponent<PlayerMovementNetwork>().objectColor == Color.red) ||
                (layer == 10 && cur.GetComponent<PlayerMovementNetwork>().objectColor == Color.blue))
            {
                //Debug.Log("found player "+layer);
                pos = cur.GetComponent<Transform>().position;
            }
        }
        pos.y = this.gameObject.transform.position.y;
        return pos;
    }

    // Update is called once per frame
    void Update()
    {
        TargetPosition = GetTargetPos();

        Vector3 newPos = transform.position;
        Vector3 newDir;

        //towards the player
        newPos += Vector3.Normalize(TargetPosition - newPos) * vitesse;
        newDir = TargetPosition - newPos;

        //Check for bounds
        if (newPos.x > MAX_X) { newPos.x = MAX_X; }
        if (newPos.z > MAX_Z) { newPos.z = MAX_Z; }
        if (newPos.x < MIN_X) { newPos.x = MIN_X; }
        if (newPos.z < MIN_Z) { newPos.z = MIN_Z; }

        transform.position = newPos;
        if(newDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(newDir, new Vector3(0, 1, 0));
    }

    [ClientRpc]
    public void RpcSetLayerRecursively(int newLayer, Color color)
    {
        //Debug.Log("RPC setting layer recu");
        gameObject.layer = newLayer;
        if(GetComponent<SkinnedMeshRenderer>() != null){
           GetComponent<SkinnedMeshRenderer>().material.color = color;
           Debug.Log("RPC setting layer recu");
        }
        foreach (Transform child in transform)
        {
            SetLayerRecursively(child.gameObject, newLayer, color);
        }
    }
    public void SetLayerRecursively(GameObject obj, int newLayer, Color color)
    {
        //Debug.Log("setting layer recu");
        obj.layer = newLayer;
        if(obj.GetComponent<SkinnedMeshRenderer>() != null){
            obj.GetComponent<SkinnedMeshRenderer>().material.color = color;
            Debug.Log("RPC setting layer recu");
        }
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer, color);
        }
    }
}
