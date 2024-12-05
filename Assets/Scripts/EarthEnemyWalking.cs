using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthEnemyWalking : BaseEnemy
{
    // Start is called before the first frame update
    protected override void Start()
    {
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
    }

    public override void ApplyTileModifiers(float elevation, float wetness)
    {
        //base.ApplyTileModifiers(elevation, wetness);

        //Earth Enemy Damage resistance capped at 
        this.damageResistance = this.baseDamageResistance * (1 + (this.maxDamageResistance / elevation));
        //Debug.Log("Enemy: " + this.transform.gameObject + " damage resistance =" + this.damageResistance);
    }
}
