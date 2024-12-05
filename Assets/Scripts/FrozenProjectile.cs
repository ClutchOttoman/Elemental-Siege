using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenProjectile : BaseProjectile
{
    [SerializeField] protected float minimumVelocity = 0.1f;
    protected bool hitEnemy = false;
    [SerializeField] protected float freezeTime = 60f;

    override protected void OnHitEnemy(BaseEnemy enemy)
    {
        //Debug.Log("Hitting enemy");
        // Freeze the enemy
        enemy.FreezeEnemy(this.freezeTime);
        //each projectile hits one enemy
        hitEnemy = true;
    }

    override protected void OnHitTerrain(GameObject obj)
    {

    }

    public override void LaunchProjectile(Vector3 direction, float buff)
    {
        freezeTime *= buff;
        buff = 0;
        base.LaunchProjectile(direction, buff);
    }

    override protected bool ShouldDestroy()
    {
        return base.ShouldDestroy() || this.hitEnemy || this.projectileRigidbody.velocity.magnitude < this.minimumVelocity;
    }
}
