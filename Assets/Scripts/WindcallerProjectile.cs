using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WindcallerProjectile : BaseProjectile
{
    //protected Dictionary<Transform,BaseEnemy> enemiesToHit = new Dictionary<Transform,BaseEnemy>();
    protected float damageOverTime = 0;

    protected WindcallerTower parentTower;

    private int maxTicksHoldingStill = 100;
    private int ticksHoldingStill = 0;

    protected override void Update()
    {
        /*foreach(BaseEnemy enemy in enemiesToHit.Values)
        {
            Debug.Log("Windcaller Attacking: " + enemy.transform.gameObject);
            if (enemy != null && !enemy.ShouldBeImmune())
            {
                OnHitEnemy(enemy);
            }
            else if (enemy == null)
            {
                enemiesToHit.Remove(enemy.transform);
            }
        }*/
    }
    override protected void OnHitEnemy(BaseEnemy enemy)
    {
        /*
        Debug.Log("Inflicting damage at " + this.projectileRigidbody.transform.position);
        var enemyHealth = enemy.TakeDamage(this.buffedDamage);
        if(enemyHealth <= 0)
        {
            enemiesToHit.Remove(enemy.transform);
        }
        */
    }

    override protected void FixedUpdate()
    {
        base.FixedUpdate();

        if (this.projectileRigidbody.velocity.magnitude < 0.01f)
        {
            Debug.Log("Holding still.");
            this.ticksHoldingStill++;
        }
    }

    override protected void OnHitTerrain(GameObject obj)
    {
    }

    override protected bool ShouldDestroy()
    {
        return base.ShouldDestroy() || this.ticksHoldingStill > this.maxTicksHoldingStill;
    }

    override protected void OnDestroy()
    {
        if (this.parentTower != null)
        {
            this.parentTower.resetCooldown();
        }
    
        Collider[] hitCollider = Physics.OverlapSphere(this.transform.position, 0.6f, this.enemyLayerAsMask);
        foreach (Collider c in hitCollider)
        {
            BaseEnemy enemy = EnemyManager.Instance.GetEnemy(c.transform);
            enemy.StopTakingDamageOverTime(this.damageOverTime);
        }
     
        base.OnDestroy();
    }


    public override void LaunchProjectile(Vector3 direction, float buff)
    {
        this.buffedDamage = this.baseDamage * (1 + buff);
        Physics.SyncTransforms();
    }
    protected override void OnTriggerEnter(Collider obj)
    {

        int collisionLayerAsMask = 1 << obj.transform.gameObject.layer;
        // Debug.Log(collisionLayerAsMask + " vs " + this.terrainLayerAsMask);
        //check if what we hit is an enemy
        if ((this.enemyLayerAsMask & collisionLayerAsMask) > 0)
        {
            Debug.Log("HIT " + obj.transform.gameObject);
            BaseEnemy target = EnemyManager.Instance.GetEnemy(obj.transform);
            if (target == null) { return; }
            //this.enemiesToHit.Add(obj.transform,target);
            this.damageOverTime = this.buffedDamage;
            target.StartTakingDamageOverTime(this.damageOverTime);
        }
    }

    protected void OnTriggerExit(Collider obj)
    {
        int collisionLayerAsMask = 1 << obj.transform.gameObject.layer;
        // Debug.Log(collisionLayerAsMask + " vs " + this.terrainLayerAsMask);
        //check if what we hit is an enemy
        if ((this.enemyLayerAsMask & collisionLayerAsMask) > 0)
        {
            BaseEnemy target = EnemyManager.Instance.GetEnemy(obj.transform);
            if (target == null) { return; }
            //this.enemiesToHit.Remove(obj.transform);
            target.StopTakingDamageOverTime(this.damageOverTime);
        }
    }

    public void UpdateBuffedDamage(float buff)
    {
        this.buffedDamage = this.baseDamage * (1 + buff);
    }

}
