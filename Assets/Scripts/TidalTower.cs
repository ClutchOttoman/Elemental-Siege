using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TidalTower : BaseTower
{
    [SerializeField] protected float attackIntervalMultiplier = 2F;
    //Spawn projectile then tell it where to move towards -- target is not the enemy its the collider in the top half of the enemy to aim at. target.transform.parent is the enemy
    protected override bool Attack(Collider target)
    {
        // Debug.Log("Firing");
        var templateScript = this.projectileTemplate.GetComponent<BaseProjectile>();
        var targetEnemyNavAgent = EnemyManager.Instance.GetPath(target.transform.parent);

        if (targetEnemyNavAgent == null)
        {
            Debug.LogWarning("Unable to locate enemy NavAgent");
            return false;
        }
        else if (templateScript == null)
        {
            Debug.LogWarning("Unable to locate projectile script. Enemy is at " + target.transform.position);
            return false;
        }

        Vector3 shooterPosition = this.projectileSpawnPoint.transform.position;
        Vector3 targetPosition = target.transform.position;
        Vector3 targetVelocity = targetEnemyNavAgent.velocity;
        float projectileSpeed = templateScript.getSpeed();
        Vector3 firingDirection = new Vector3(0, 0, 0);

        // If there's a shot to be taken, take it.
        bool hasShot = PredictiveAiming.CalculateFiringSolution(shooterPosition, targetPosition, targetVelocity, projectileSpeed, out firingDirection);

        if (hasShot && (Vector3.Dot(firingDirection, targetPosition - shooterPosition) > 0))
        {
            var projectileToLaunch = Instantiate(this.projectileTemplate, this.projectileSpawnPoint.transform);
            var projectileScript = projectileToLaunch.GetComponent<BaseProjectile>();
            // Fire!
            projectileScript.LaunchProjectile(firingDirection.normalized, buff);
            // Spawn the firing VFX
            firingParticleSystem.Play();
            return true;
        }

        return false;
    }

    //high wetness improves attack interval
    public override void InitializeTileBuff(Tile tile)
    {
        base.InitializeTileBuff(tile);
        float wetness = tile.wetness * (1 + (this.fountainBuff * this.fountainWetnessMultiplier));
        this.attackInterval = this.baseAttackInterval - this.attackIntervalMultiplier * (wetness - 1.15F);
    }
}