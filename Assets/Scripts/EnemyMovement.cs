﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    const int MIN_X = -40;
    const int MAX_X = 40;
    const int MIN_Z = -40;
    const int MAX_Z = 40;

    public float vitesse;

    Vector3 PlayerPosition = new Vector3(4, 1, -5);

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = transform.position;
        Vector3 newDir;

        //towards the player
        newPos += Vector3.Normalize(PlayerPosition - newPos) * vitesse;
        newDir = PlayerPosition - newPos;

        //Check for bounds
        if (newPos.x > MAX_X) { newPos.x = MAX_X; }
        if (newPos.z > MAX_Z) { newPos.z = MAX_Z; }
        if (newPos.x < MIN_X) { newPos.x = MIN_X; }
        if (newPos.z < MIN_Z) { newPos.z = MIN_Z; }

        transform.position = newPos;
        transform.rotation = Quaternion.LookRotation(newDir, new Vector3(0, 1, 0));
    }
}