using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildfireTower : BaseTower
{
    [SerializeField] protected float attackIntervalMultiplier = 2F;

    protected override Collider GetTarget(Collider[] hitColliders)
    {
        return hitColliders[0];
    }
    protected override bool Attack(Collider target)
    {
        Debug.Log("Wildfire Attacking");
        var projectileToLaunch = Instantiate(this.projectileTemplate, this.projectileSpawnPoint.transform);
        var projectileScript = projectileToLaunch.GetComponent<BaseProjectile>();
        // Fire!
        projectileScript.LaunchProjectile(Vector3.zero, buff);
        // Spawn the firing VFX
        firingParticleSystem.Play();
        return true;
    }

    public override void InitializeTileBuff(Tile tile)
    {
        base.InitializeTileBuff(tile);
        float wetness = tile.wetness * (1 - (this.fountainBuff * this.fountainWetnessMultiplier));
        this.attackInterval = this.baseAttackInterval - (this.attackIntervalMultiplier * this.baseAttackInterval) * (1.00F - wetness);
    }

}
