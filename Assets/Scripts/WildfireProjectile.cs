using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildfireProjectile : BaseProjectile
{
    override protected void OnHitEnemy(BaseEnemy enemy)
    {
        //all non-flying enemies in range take damage
        enemy.TakeDamage(this.baseDamage);
        Debug.Log("Wildfire hit enemy: " + enemy.gameObject);
    }

    override protected void OnTriggerEnter(Collider other)
    {
        int collisionLayerAsMask = 1 << other.transform.gameObject.layer;

        if ((this.enemyLayerAsMask & collisionLayerAsMask) > 0)
        {
            BaseEnemy target = EnemyManager.Instance.GetEnemy(other.transform);
            if (target == null) { return; }
            OnHitEnemy(target);

        }
        else
        {
            //Debug.Log("AOE hit non Enemy");
        }
    }
}


