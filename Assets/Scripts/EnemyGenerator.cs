using System.Collections;
using UnityEngine;
using System.Collections.Generic;

// en charge de gen des ennemis
// pour chaque joueur

public class EnemyGenerator : MonoBehaviour {

    const int MIN_X = -10;
    const int MAX_X = 10;
    const int MIN_Z = -10;
    const int MAX_Z = 10;

    public float spawnTime=1f;
    public List<string> tags;

    // Use this for initialization
    void Start()
    {
        InvokeRepeating("Spawn", spawnTime, spawnTime);
    }

    IEnumerable Spawn()
    {
        yield return new WaitForSeconds(Random.Range(0f, 2f));
        //spawn at a random edge
        Vector3 position;
        Quaternion rotation;

        int edge = Random.Range(0, 4);
        switch (edge)
        {
            case 0:
                position = new Vector3(MIN_X, 1, Random.Range(MIN_Z, MAX_Z + 1));
                rotation = Quaternion.identity;
                break;
            case 1:
                position = new Vector3(MAX_X, 1, Random.Range(MIN_Z, MAX_Z + 1));
                rotation = Quaternion.identity;
                break;
            case 2:
                position = new Vector3(Random.Range(MIN_X, MAX_X + 1), 1, MIN_Z);
                rotation = Quaternion.identity;
                break;
            default:
                position = new Vector3(Random.Range(MIN_X, MAX_X + 1), 1, MAX_Z);
                rotation = Quaternion.identity;
                break;
        }

        // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.

        string currTag = tags[Random.Range(0, tags.Count)];
        GameObject enemy = ObjectPooler.SharedInstance.GetPooledObject(currTag);
        if (enemy != null)
        {
            enemy.transform.position = position;
            enemy.transform.rotation = rotation;
            enemy.SetActive(true);
        }
    }
}
