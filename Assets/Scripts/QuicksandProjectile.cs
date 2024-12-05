using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuicksandProjectile : BaseProjectile
{
    [SerializeField] float quicksandSlowModifier = 0.5f;
    SphereCollider rangeCollider;
    protected float baseRange = -100;
    protected override void Start()
    {
        base.Start();
        this.rangeCollider = this.transform.GetComponent<SphereCollider>();
        if(this.rangeCollider != null)
        {
            this.baseRange = this.rangeCollider.radius;
        }
    }
    protected override void Update()
    {
        base.Update();
        if (this.baseRange==-100 && this.rangeCollider != null)
        {
            this.baseRange = this.rangeCollider.radius;
        }
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

            target.SlowAgent(this.quicksandSlowModifier);
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

            target.SpeedUpAgent(this.quicksandSlowModifier);
        }

    }

    public void ModifyRange(float buff)
    {
        //buff is negative -> decrease radius as elevation increases
        if (this.rangeCollider != null) {
            this.rangeCollider.radius = (this.baseRange + buff);
            Debug.Log(this.transform.gameObject + " modded range: " +  this.rangeCollider.radius);
        }
        else
        {
            Debug.Log(this.transform.gameObject + " collider is null");
        }
    }
}
