using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthquakeTower : BaseTower
{
    [SerializeField] float percentageMultiplier = 0.5f;
    //projectile hits all enemeies in range
    protected override Collider GetTarget(Collider[] hitColliders)
    {
        return hitColliders[0];
    }
    protected override bool Attack(Collider target)
    {
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
        this.elementalMultiplier = this.tile.elevation * (1 - this.fountainElevationMultiplier);
        this.buff = -(this.elementalMultiplier * percentageMultiplier);
        //Debug.Log(this.gameObject + " buff: " + this.buff);
    }
    

}
