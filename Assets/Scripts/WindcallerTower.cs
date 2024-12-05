using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindcallerTower : BaseTower
{
    protected bool hasAttacked = false;
    protected WindcallerProjectile projectileScript;
    protected override bool AttemptAttack()
    {
        if(((this.timeAtLastAttack + this.GetAttackInterval()) <= TowerManager.Instance.towerTimer) || !hasAttacked)
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
                    hasAttacked = true;
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
    protected override bool Attack(Collider target)
    {
        // Debug.Log("Firing");
        var templateScript = this.projectileTemplate.GetComponent<BaseProjectile>();
        var targetEnemyNavAgent = EnemyManager.Instance.GetPath(target.transform.parent);

        if (targetEnemyNavAgent == null)
        {
            //Debug.LogWarning("Unable to locate enemy NavAgent");
            return false;
        }
        else if (templateScript == null)
        {
            Debug.LogWarning("Unable to locate projectile script. Enemy is at " + target.transform.position);
            return false;
        }

        var projectileToLaunch = Instantiate(this.projectileTemplate, this.projectileSpawnPoint.transform);
        var windcallerNavController = projectileToLaunch.GetComponent<WindcallerNavController>();
        //Give projectile its parent tower for range limitation of projectile
        windcallerNavController.parentWindcallerTower = this;
        projectileScript = projectileToLaunch.GetComponent<WindcallerProjectile>();
        projectileScript.LaunchProjectile(new Vector3(0, 0, 0), buff);

        firingParticleSystem.Play();

        return true;
    }
    //saves elevation as buff to be passed to projectiles to improve their damage
    public override void InitializeTileBuff(Tile tile)
    {
        base.InitializeTileBuff(tile);
        this.elementalMultiplier = tile.elevation * 0.1f;
        this.buff = this.elementalMultiplier * (1 + (this.fountainElevationMultiplier * this.fountainBuff)) ;
        if(this.projectileScript != null)
        {
            this.projectileScript.UpdateBuffedDamage(buff);
        }
    }

    public void resetCooldown()
    {
        this.hasAttacked = false;
    }
}
