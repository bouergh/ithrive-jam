using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

public class EnemyGenerator : MonoBehaviour {

    const int MIN_X = -10;
    const int MAX_X = 10;
    const int MIN_Z = -10;
    const int MAX_Z = 10;
    const int SPAWN_TIME = 1;

    public List<Wave> waves;

    float timeSinceSpawn = 0;
    int currWave = 0;
    int currSpawn = 0;

    string[] spawnL1;
    string[] spawnL2;

    void Update()
    {
        //if still a wave to go through
        if (spawnL1 != null && currSpawn < spawnL1.Length && currWave <= waves.Count)
        {
            if (timeSinceSpawn >= SPAWN_TIME) {
                // spawn a random enemy from the lot
                Spawn(spawnL1[currSpawn], 1);

                // Uncomment to spawn on second layer (and uncomment as well in spawn method)
                // Spawn(spawnL2[currSpawn], 2);
                currSpawn++;
                timeSinceSpawn = 0;
            }

            else {
                timeSinceSpawn += Time.deltaTime;
            }
        }

        else
        {
            spawnL1 = null;
            spawnL2 = null;
            if (currWave < waves.Count) { InitWave(); }
            currSpawn = 0;
            currWave += 1;
        }
    }

    void AddEnemyType(string type, int curr, int count)
    {
        for(int i=0;i<count; i++)
        {
            spawnL1[i + curr] = type;
            spawnL2[i + curr] = type;
        }
    }

    void InitWave()
    {
        spawnL1 = new string[waves[currWave].GetNbEnemies()];
        spawnL2 = new string[waves[currWave].GetNbEnemies()];

        AddEnemyType("fastEnemy", 0, waves[currWave].countFast);
        AddEnemyType("tankEnemy", waves[currWave].countFast, waves[currWave].countTank);
        AddEnemyType("normalEnemy", waves[currWave].countTank + waves[currWave].countFast, waves[currWave].countNorm);

        // random order
        Reshuffle(spawnL1);
        Reshuffle(spawnL2);
    }

    void Reshuffle(string[] ar)
    {
        // Knuth shuffle algorithm :: courtesy of Wikipedia :)
        for (int t = 0; t < ar.Length; t++)
        {
            string tmp = ar[t];
            int r = Random.Range(t, ar.Length);
            ar[t] = ar[r];
            ar[r] = tmp;
        }
    }

    void Spawn(string tag, int layer)
    {
        //spawn at a random edge
        Vector3 position;
        Quaternion rotation;

        int edge = UnityEngine.Random.Range(0, 4);
        switch (edge)
        {
            case 0:
                position = new Vector3(MIN_X, 1, UnityEngine.Random.Range(MIN_Z, MAX_Z + 1));
                rotation = Quaternion.identity;
                break;
            case 1:
                position = new Vector3(MAX_X, 1, UnityEngine.Random.Range(MIN_Z, MAX_Z + 1));
                rotation = Quaternion.identity;
                break;
            case 2:
                position = new Vector3(UnityEngine.Random.Range(MIN_X, MAX_X + 1), 1, MIN_Z);
                rotation = Quaternion.identity;
                break;
            default:
                position = new Vector3(UnityEngine.Random.Range(MIN_X, MAX_X + 1), 1, MAX_Z);
                rotation = Quaternion.identity;
                break;
        }

        // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.

        GameObject enemy = ObjectPooler.SharedInstance.GetPooledObject(tag);
        if (enemy != null)
        {
            // uncomment to set layer;
            // enemy.layer = layer;
            enemy.transform.position = position;
            enemy.transform.rotation = rotation;
            enemy.SetActive(true);
        }
    }
}
