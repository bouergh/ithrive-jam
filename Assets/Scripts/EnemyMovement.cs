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

    Vector3 TargetPosition;

    Vector3 GetTargetPos()
    {
        //get layer
        int layer = this.gameObject.layer;
        Vector3 pos = new Vector3(Random.Range(MIN_X,MAX_X), 0, Random.Range(MIN_Z, MAX_Z));
        foreach (GameObject cur in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (layer == 10 && cur.GetComponent<PlayerMovementNetwork>().objectColor == Color.red ||
                layer == 9 && cur.GetComponent<PlayerMovementNetwork>().objectColor == Color.blue)
            {
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
        transform.rotation = Quaternion.LookRotation(newDir, new Vector3(0, 1, 0));
    }
}
