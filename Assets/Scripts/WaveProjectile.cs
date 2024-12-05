using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveProjectile : BaseProjectile
{

    [SerializeField] protected float minimumVelocity = 0.1f;

    override protected void OnHitEnemy(BaseEnemy enemy)
    {
        // Debug.Log("Hitting enemy");
        var targetEnemyNavAgent = EnemyManager.Instance.GetPath(enemy.transform);
        Vector3 targetVelocity = targetEnemyNavAgent.velocity;

        float componentAlongWave = Vector3.Dot(targetVelocity, this.projectileRigidbody.velocity) / this.projectileRigidbody.velocity.magnitude;

        float deltaV = componentAlongWave - this.projectileRigidbody.velocity.magnitude;

        float scalingFactor = -0.5f * 10;
        // Maybe give enemies some immunity frames!
        enemy.TakeDamage(this.buffedDamage);

        Vector3 aimingVelocity = this.projectileRigidbody.velocity;
        aimingVelocity.y = 0.0f;

        // Stun the enemy.
        enemy.Stun(40f);
        enemy.ApplyForce(deltaV * scalingFactor * aimingVelocity);
    }

    override protected void OnHitTerrain(GameObject obj)
    {
    }

    override protected bool ShouldDestroy()
    {
        return base.ShouldDestroy() || this.projectileRigidbody.velocity.magnitude < this.minimumVelocity;
    }
}
