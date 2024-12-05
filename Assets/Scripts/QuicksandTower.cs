using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuicksandTower : BaseTower
{
    [SerializeField] float percentageMultiplier = 0.25f;
    protected bool hasAttacked = false;
    protected QuicksandProjectile projectileScript;
    //projectile hits all enemeies in range
    protected override bool AttemptAttack()
    {
        if (((this.timeAtLastAttack + this.GetAttackInterval()) <= TowerManager.Instance.towerTimer) || !hasAttacked)
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
    protected override Collider GetTarget(Collider[] hitColliders)
    {
        return hitColliders[0];
    }
    protected override bool Attack(Collider target)
    {
        var projectileToLaunch = Instantiate(this.projectileTemplate, this.projectileSpawnPoint.transform);
        this.projectileScript = projectileToLaunch.GetComponent<QuicksandProjectile>();
        // Fire!
        this.projectileScript.LaunchProjectile(Vector3.zero, buff);
        this.projectileScript.ModifyRange(this.buff);
        // Spawn the firing VFX
        firingParticleSystem.Play();
        return true;
    }
    public override void InitializeTileBuff(Tile tile)
    {
        base.InitializeTileBuff(tile);
        this.elementalMultiplier = this.tile.elevation * (1 - this.fountainElevationMultiplier);
        this.buff = -(this.elementalMultiplier * percentageMultiplier);
        if (this.projectileScript != null)
        {
            this.projectileScript.ModifyRange(this.buff);
        }

        Debug.Log(this.gameObject + " buff: " + this.buff);

    }
}
