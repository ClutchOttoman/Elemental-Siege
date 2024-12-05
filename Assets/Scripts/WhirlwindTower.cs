using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhirlwindTower : BaseTower
{
    protected override bool AttemptAttack()
    {
        if (((this.timeAtLastAttack + this.GetAttackInterval()) <= TowerManager.Instance.towerTimer))
        {
            //Debug.Log("time: " +  (timeAtLastAttack+attackInterval) + "  time2: " + TowerManager.Instance.towerTimer);
            //get all colliders in Target Layer(10) that overlap
            Collider[] hitColliders;
            if (this.canTargetFlyers)
            {
                hitColliders = Physics.OverlapCapsule(this.aimingLowerBound, this.aimingUpperBound, this.GetRange(), (this.targetLayerAsMask | this.flyingLayerAsMask));
            }
            else
            {
                hitColliders = Physics.OverlapCapsule(this.aimingLowerBound, this.aimingUpperBound, this.GetRange(), this.targetLayerAsMask);
            }
            hitColliders = MathHelpers.FilteredArray(hitColliders, (hitCollider) => this.IsInFiringAngle(hitCollider.transform.position));
            //hitColliders = MathHelpers.FilteredArray(hitColliders, (hitColliders) => this.CheckIfTargetIsVisiblie(hitColliders));
            if (hitColliders.Length == 0)
            {
                return false;
                /*Debug.Log("No Targets in Range")*/
            }
            else
            {
                var target = GetTarget(hitColliders);
                if (Attack(target))
                {
                    timeAtLastAttack = TowerManager.Instance.towerTimer; ;
                    return true;
                }
                else { return false; }
            }
        }
        else
        {
            return false;
        }
    }
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

        var projectileToLaunch = Instantiate(this.projectileTemplate, this.projectileSpawnPoint.transform);
        var projectileScript = projectileToLaunch.GetComponent<BaseProjectile>();
        projectileScript.LaunchProjectile(new Vector3(0, 0, 0), buff);

        firingParticleSystem.Play();

        return true;
    }
    //saves elevation as buff to be passed to projectiles to improve their life spans. life span++ as elevation increases
    public override void InitializeTileBuff(Tile tile)
    {
        base.InitializeTileBuff(tile);
        this.buff = tile.elevation * (1 + (this.fountainElevationMultiplier * this.fountainBuff));
    }
}