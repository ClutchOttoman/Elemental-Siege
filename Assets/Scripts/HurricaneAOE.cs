using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurricaneAOE : MonoBehaviour
{
    [SerializeField] protected CapsuleCollider hurricaneTrigger;
    [SerializeField] protected float lifeTime = 10f;
    protected float endTime;

    protected float damageOverTime = 3f;

    protected int enemyLayerAsMask = 1 << 3;
    protected void Awake()
    {
        Debug.Log("Starting Hurriane AOE");
        this.endTime = TowerManager.Instance.towerTimer + this.lifeTime;
    }
    protected void Update() {
        if(this.endTime < TowerManager.Instance.towerTimer)
        {
            Debug.Log("Ending Hurriane AOE");
            Destroy(this.gameObject);
        }
    }
    protected void OnDestroy()
    {
        Collider[] hitCollider = Physics.OverlapSphere(this.transform.position, 7f, this.enemyLayerAsMask);
        foreach (Collider c in hitCollider)
        {
            BaseEnemy enemy = EnemyManager.Instance.GetEnemy(c.transform);
            enemy.StopTakingDamageOverTime(this.damageOverTime);
        }
    }
    protected void OnTriggerEnter(Collider obj)
    {
        int collisionLayerAsMask = 1 << obj.transform.gameObject.layer;
        // Debug.Log(collisionLayerAsMask + " vs " + this.terrainLayerAsMask);
        //check if what we hit is an enemy
        if ((this.enemyLayerAsMask & collisionLayerAsMask) > 0)
        {
            Debug.Log("Hurricane HIT " + obj.transform.gameObject);
            BaseEnemy target = EnemyManager.Instance.GetEnemy(obj.transform);
            if (target == null) { return; }
            //this.enemiesToHit.Add(obj.transform,target);
            target.StartTakingDamageOverTime(this.damageOverTime);
        }
        else
        {
            Debug.Log("AOE hit non Enemy");
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

    public void SetDamageOverTime(float damageOverTime)
    {
        this.damageOverTime = damageOverTime;
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.gray;
        Gizmos.DrawSphere(this.transform.position, hurricaneTrigger.radius);
    }
}

