using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VortexProjectile : BaseProjectile
{
    [SerializeField] protected float pull = 1;

    override protected void OnHitEnemy(BaseEnemy enemy)
    {
        if (enemy.isFlying)
        {
            //treat flyers different as they dont have nav agents
            OnHitFlyingEnemy(enemy);
        }
        // Debug.Log("Hitting enemy");
        var targetEnemyNavAgent = EnemyManager.Instance.GetPath(enemy.transform);
        if (targetEnemyNavAgent == null)
        {
            return;
        }
        Vector3 targetPosition = targetEnemyNavAgent.transform.position;

        Vector3 displacement = targetPosition - this.projectileRigidbody.position;
        Vector3 unitVector = displacement.normalized;

        float distance = displacement.magnitude;

        float scalingFactor = -1000f * pull;
        // enemy.TakeDamage(this.buffedDamage);

        // Stun the enemy.
        enemy.Stun(10f);
        enemy.ApplyForce(unitVector * scalingFactor * 1 / (distance));
    }
    protected void OnHitFlyingEnemy(BaseEnemy enemy)
    {
        Vector3 targetPosition = this.transform.position;

        Vector3 displacement = targetPosition - this.projectileRigidbody.position;
        displacement.y = 0;
        Vector3 unitVector = displacement.normalized;

        float distance = displacement.magnitude;

        float scalingFactor = -1000f * pull;
        // enemy.TakeDamage(this.buffedDamage);

        // Stun the enemy.
        enemy.ApplyForce(unitVector * scalingFactor * 1 / (distance));
    }

    override protected void OnHitTerrain(GameObject obj)
    {
    }
}
