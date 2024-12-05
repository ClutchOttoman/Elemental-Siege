using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FountainTower : BaseTower
{
    protected int towerLayerAsMask = 1 << 12;
    [SerializeField] protected float attackIntervalMultiplier = 1.3F;
    //Apply fountain buff to all towers in range every attack interval
    protected override bool AttemptAttack()
    {
        if ((this.timeAtLastAttack + this.GetAttackInterval()) <= TowerManager.Instance.towerTimer)
        {
            //Debug.Log("Fountain Tower Attempting To buff");
            //Get colliders of tower's projectile spawn point : tower -> hitCollider[i].transform.parent
            Collider[] hitColliders;
            hitColliders = Physics.OverlapCapsule(this.aimingLowerBound, this.aimingUpperBound, this.GetRange(), this.towerLayerAsMask, QueryTriggerInteraction.Collide);
           
            //tower will always hit itself but cannot buff itself
            if (hitColliders.Length == 1)
            {
                Debug.Log("No Targets in Range");
                return false;
            }
            Debug.Log("Towers to fountain buff: " + (hitColliders.Length - 1));
            // Spawn the firing VFX
            this.firingParticleSystem.Play();
            this.timeAtLastAttack = TowerManager.Instance.towerTimer;
            //increment fountain buff for each tower in range

            //Debug.Log("Buffing Towers");
            for (int i = 0; i < hitColliders.Length; i++)
            {
                //Debug.Log("Buffing tower" + i);
                BaseTower tower = TowerManager.Instance.GetTowerScript(hitColliders[i].transform.parent);
                if(tower == null) { continue; }
                //tower cant buff itself
                if(tower == this) { continue; }
                tower.AddFountainBuff();
            }
            return true;
        }
        else return false;
    }
    public override void InitializeTileBuff(Tile tile)
    {
        base.InitializeTileBuff(tile);
        float wetness = tile.wetness * (1 + (this.fountainBuff * this.fountainWetnessMultiplier));
        this.attackInterval = this.baseAttackInterval - this.attackIntervalMultiplier * (wetness - 1.25f);
    }


}
