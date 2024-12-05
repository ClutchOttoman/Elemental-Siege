using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Stronghold : MonoBehaviour
{ 
    public float health = 100; // starting ad=nd current health
    public float healthMaxScale; // the maxinum value the stronghold's health can be.
    // Start is called before the first frame update
    public GameObject rippleShield; 

    void Start()
    {
        if (LevelManager.Instance.stronghold == null)
        {
            LevelManager.Instance.stronghold = this;
        }
        if (EnemyManager.Instance.stronghold == null)
        {
            EnemyManager.Instance.stronghold = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            StrongholdDestroyed();
        }
    }

    public float TakeDamage(float damage)
    {
        float actualDamage = damage;
        if(health >= damage)
        {
            this.health -= damage;
        }
        else
        {
            actualDamage = this.health;
            this.health = 0;
            StrongholdDestroyed();
        }

        // Debug.Log("Damage: " + actualDamage + "Stronghold Health: " + this.health);
        return actualDamage;
    }

    public void StrongholdDestroyed()
    {
        // Debug.Log("Stronghold Destroyed");
        LevelManager.Instance.LoseLevel();
        if (rippleShield != null) rippleShield.SetActive(false);
    }
}
