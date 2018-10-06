using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    float dmgDelayTime;

    public bool destroyOnDeath;

    public int maxHealth;
    float currentHealth;

    public void Start()
    {
        currentHealth = maxHealth;
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