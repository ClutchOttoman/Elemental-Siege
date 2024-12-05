using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanishmentTower : BaseTower
{
    //Spawn projectile then tell it where to move towards -- target is not the enemy its the collider in the top half of the enemy to aim at. target.transform.parent is the enemy
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
}