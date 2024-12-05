using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhirlwindProjectile : BaseProjectile
{
    List<BaseEnemy> enemyHitList = new List<BaseEnemy>();
    [SerializeField] protected float damageOverTime = 0;
    override protected void OnHitEnemy(BaseEnemy enemy)
    {
        // Debug.Log("Inflicting damage at " + this.projectileRigidbody.transform.position);
        enemy.TakeDamage(this.buffedDamage);
    }

    override protected bool ShouldDestroy()
    {
        return base.ShouldDestroy();
    }
    override protected void OnDestroy()
    {
        Collider[] hitCollider = Physics.OverlapSphere(this.transform.position, 0.75f, this.enemyLayerAsMask);
        foreach(Collider c in hitCollider)
        {
            BaseEnemy enemy = EnemyManager.Instance.GetEnemy(c.transform);
            enemy.StopTakingDamageOverTime(this.damageOverTime);
        }
        base.OnDestroy();
    }
//For interaction w/ elevation -> elevation value is passed in as buff and then buff is applied to improve lifetime of whirlwind projectiles
public override void LaunchProjectile(Vector3 direction, float buff)
    {
        this.lifeTime *= (1 + buff / 10);
        buff = 0;
        base.LaunchProjectile(direction, buff);
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
            target.StopTakingDamageOverTime(this.damageOverTime);
        }
    }
}
