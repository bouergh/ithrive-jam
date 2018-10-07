using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

// en charge de gen des ennemis
// pour chaque joueur
[System.Serializable]
public class Wave
{
    public int countFast;
    public int countNorm;
    public int countTank;

    public int GetNbEnemies() { return countFast + countNorm + countTank; }
}

public class Spawner : NetworkBehaviour {

    const int MIN_X = -50;
    const int MAX_X = 10;
    const int MIN_Z = -13;
    const int MAX_Z =50;

    const int SPAWN_TIME = 1;
    float timeSinceSpawn = 0;
    int currSpawn = 0;

    int[] toSpawn = new int[] {0, 0, 0, 0, 1, 1, 0, 1, 1, 1, 2, 1, 1, 1, 1, 0, 0, 1, 2, 0, 0, 1, 1, 1};
    //int[] toSpawn = new int[] {0, 1, 2, 0, 0};
    public GameObject[] prefabs = new GameObject[3];

    

    [Server]
    public void Update()
    {
        if(NetworkManager.singleton.numPlayers != 2) return;
        if (currSpawn < toSpawn.Length)
        {
            if (timeSinceSpawn >= SPAWN_TIME)
            {
                // spawn a random enemy from the lot
                Spawn(toSpawn[currSpawn], 9);
                Spawn(toSpawn[currSpawn], 10);
                currSpawn++;
                timeSinceSpawn = 0;
            }

            else
            {
                timeSinceSpawn += Time.deltaTime;
            }
        }
    }

    [Server]
    void Spawn(int tag, int layer)
    {
        //spawn at a random edge
        Vector3 position;
        Quaternion rotation;

        int edge = UnityEngine.Random.Range(0, 4);
        switch (edge)
        {
            case 0:
                position = new Vector3(MIN_X, prefabs[tag].transform.position.y, UnityEngine.Random.Range(MIN_Z, MAX_Z + 1));
                rotation = Quaternion.identity;
                break;
            case 1:
                position = new Vector3(MAX_X, prefabs[tag].transform.position.y, UnityEngine.Random.Range(MIN_Z, MAX_Z + 1));
                rotation = Quaternion.identity;
                break;
            case 2:
                position = new Vector3(UnityEngine.Random.Range(MIN_X, MAX_X + 1), prefabs[tag].transform.position.y, MIN_Z);
                rotation = Quaternion.identity;
                break;
            default:
                position = new Vector3(UnityEngine.Random.Range(MIN_X, MAX_X + 1), prefabs[tag].transform.position.y, MAX_Z);
                rotation = Quaternion.identity;
                break;
        }

        // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
        GameObject enemy = Instantiate(prefabs[tag], position, rotation);
        Color col = (layer == 10) ? Color.blue : Color.red;
        enemy.GetComponent<EnemyMovement>().SetLayerRecursively(enemy, layer, col);   
        NetworkServer.Spawn(enemy);  
        enemy.GetComponent<EnemyMovement>().RpcSetLayerRecursively(layer, col); 
        //Debug.Log("net spawned an enemy");
        }


    // [Server]
    // public void SetLayerRecursively(GameObject obj, int newLayer, Color color)
    // {
    //     obj.layer = newLayer;
    //     if(obj.GetComponent<MeshRenderer>() != null){
    //         obj.GetComponent<MeshRenderer>().material.color = color;
    //     }
    //     foreach (Transform child in obj.transform)
    //     {
    //         SetLayerRecursively(child.gameObject, newLayer, color);
    //     }
    // }


    // [ClientRpc]
    // void RpcSetLayerRecursively(GameObject obj, int newLayer, Color color)
    // {
    //     obj.layer = newLayer;
    //     if(obj.GetComponent<MeshRenderer>() != null){
    //         obj.GetComponent<MeshRenderer>().material.color = color;
    //     }
    //     foreach (Transform child in obj.transform)
    //     {
    //         RpcSetLayerRecursively(child.gameObject, newLayer, color);
    //     }
    // }
}
