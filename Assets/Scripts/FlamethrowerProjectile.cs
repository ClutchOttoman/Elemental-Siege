using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerProjectile : BaseProjectile
{
    public float damageConeHalfAngle = 15;
    public Vector3 damageConeForwardVector;
    protected float damageConeHalfAngleCosine;
    protected float damageOverTime = 3f;
    [SerializeField] public float attackInterval = 1f;

    override public void Awake()
    {
        base.Awake();
        this.damageConeHalfAngleCosine = Mathf.Cos(damageConeHalfAngle);
    }

    override protected void OnHitEnemy(BaseEnemy enemy)
    {
        if (enemy.isFlying)
        {
            //treat flyers different as they dont have nav agents
            return;
        }

        var targetEnemyNavAgent = EnemyManager.Instance.GetPath(enemy.transform);
        if (targetEnemyNavAgent == null)
        {
            return;
        }
        //all non-flying enemies in range take damage
        if (!IsInDamageCone(enemy)) { return; }
        // Debug.Log("Flamethrower Hitting " + enemy.transform.gameObject);
        enemy.TakeDamage(this.baseDamage);
        // Debug.Log("Flamethrower hit enemy: " + enemy.gameObject);
    }

    protected bool IsInDamageCone(BaseEnemy enemy)
    {
        Vector3 targetLocation = enemy.transform.position;
        targetLocation.y = 0;
        Vector3 thisLocation = this.transform.position;
        thisLocation.y = 0;
        Vector3 offset = targetLocation - thisLocation;

        // Debug.Log("Checking debug cone; \ntarget is at: " + targetLocation + "\ntower  is at: " + thisLocation + "\noffset is: " + offset + "");


        float cosineValue = Vector3.Dot(damageConeForwardVector, offset) / offset.magnitude;

        if (cosineValue >= this.damageConeHalfAngleCosine)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected override void OnTriggerEnter(Collider obj)
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
            this.damageOverTime = 0;
        }

    }

    public void SetDamageOverTime(float damageOverTime)
    {
        this.damageOverTime = damageOverTime;
    }
}
