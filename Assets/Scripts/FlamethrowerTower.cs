using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FlamethrowerTower : BaseTower
{
    [SerializeField] protected float attackIntervalMultiplier = 3F;
    [SerializeField] VisualEffect flamethrowerAttackVfx;
    [SerializeField] float fireRange = 7f;
    protected Vector3 flameSpawnPoint;
    protected Vector3 flameEndPoint;
    [SerializeField] protected float flameRadius = 1.5f;
    [SerializeField] protected float baseDamage = 9f;
    protected float timeAtLastSuccessfulAttack;
    protected override void Update()
    {
        if (this.tile == null)
        {
            this.tile = this.defaultTile;
        }

    }
    protected void FixedUpdate()
    {
        //Attacks can happen every interval
        AttemptAttack();
        //this.timeAtLastSuccessfulAttack = this.timeAtLastAttack;
        //this.timeAtLastAttack = TowerManager.Instance.towerTimer;

    }

    protected override void Awake()
    {
        this.flameSpawnPoint = this.projectileSpawnPoint.transform.position - new Vector3(0,0,0.5f);
        this.flameEndPoint -= new Vector3(0, 0, fireRange);
    }
    protected override bool AttemptAttack()
    {
        // Debug.Log("Trying to attack at " + this.transform.position);
        if ((this.timeAtLastAttack + this.attackInterval) <= TowerManager.Instance.towerTimer)
        {
            //Debug.Log("time: " +  (timeAtLastAttack+attackInterval) + "  time2: " + TowerManager.Instance.towerTimer);
            //get all colliders in Target Layer(10) that overlap
            Collider[] hitColliders;
            hitColliders = Physics.OverlapCapsule(this.projectileSpawnPoint.transform.position,this.flameEndPoint , this.flameRadius, (this.targetLayerAsMask));
            hitColliders = MathHelpers.FilteredArray(hitColliders, (hitColliders) => this.CheckIfTargetIsVisiblie(hitColliders));
            if (hitColliders.Length == 0)
            {
                //this.timeAtLastAttack = this.timeAtLastSuccessfulAttack;
                return false;
                /*Debug.Log("No Targets in Range")*/
            }
            else
            {

                firingParticleSystem.Play();
                if (flamethrowerAttackVfx != null)
                {
                    // Angle the flamethrower in the same angle as the attack first.
                    flamethrowerAttackVfx.transform.Rotate(Vector3.zero, Space.Self);
                    flamethrowerAttackVfx.Play();
                }
                Damage(hitColliders);
                this.timeAtLastAttack = TowerManager.Instance.towerTimer;
                return true;
            }
        }
        else
        {
            return false;
        }
    }
    protected void Damage(Collider[] hitColliders)
    {
        foreach (Collider target in hitColliders) {
            BaseEnemy enemy = EnemyManager.Instance.GetEnemy(target.transform.parent.transform);
            if ((enemy == null))
            {
                break;
            }
            enemy.TakeDamage(this.baseDamage);
        }
    }
    protected override bool Attack(Collider target)
    {
        return true;
    }

    public override void InitializeTileBuff(Tile tile)
    {
        base.InitializeTileBuff(tile);
        float wetness = tile.wetness * (1 - (this.fountainBuff * this.fountainWetnessMultiplier));
        this.attackInterval = this.baseAttackInterval - (this.attackIntervalMultiplier * this.baseAttackInterval) * (1.00F - wetness);
    }
    
}
