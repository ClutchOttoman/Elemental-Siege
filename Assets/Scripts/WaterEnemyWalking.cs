using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterEnemyWalking : BaseEnemy
{
    [SerializeField] protected float baseRegenFactor = 0.2f;
    protected float regenFactor;
    protected float maxHealth;
    // Start is called before the first frame update
    protected override void Start()
    {
        this.maxHealth = this.health;
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        //takes damage in dry tiles, heals up to max health when in wetter tiles
        if (this.health + regenFactor > this.maxHealth)
        {
            var regen = this.maxHealth - this.health;
            this.TakeDamage(-regen);
        }
        else { 
            this.TakeDamage(-this.regenFactor);
        }

    }

    public override void ApplyTileModifiers(float elevation, float wetness)
    {
        base.ApplyTileModifiers(elevation, wetness);
       
        this.regenFactor = this.baseRegenFactor * (wetness);
        //Debug.Log("Enemy: " + this.transform.gameObject + " regen factor =" + this.regenFactor);

    }
}