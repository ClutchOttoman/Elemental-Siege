using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthquakeProjectile : BaseProjectile
{
    //Hit everyting in range of earthquake tower
    override protected void OnHitEnemy(BaseEnemy enemy)
    {
        if (enemy.isFlying)
        {
            //treat flyers different as they dont have nav agents
            Debug.Log("isFlying");
            return;
        }
        //Debug.Log("Hitting enemy");
       

        //all non-flying enemies in range take damage
        enemy.TakeDamage(this.buffedDamage);
        //Debug.Log("Earthquake hit enemy: " + enemy.gameObject);
    }
    protected override void OnTriggerEnter(Collider obj)
    {

        //Debug.Log("HIT " + obj.transform.parent.gameObject);
        int collisionLayerAsMask = 1 << obj.transform.gameObject.layer;
        //Debug.Log(collisionLayerAsMask + " vs " + this.terrainLayerAsMask + " & " + this.enemyLayerAsMask);
        //check if what we hit is an enemy
        if ((this.enemyLayerAsMask & collisionLayerAsMask) > 0)
        {
            var enemy = EnemyManager.Instance.GetEnemy(obj.transform);
            if (enemy != null)
            {
                this.OnHitEnemy(enemy);
            }
        }
        else if ((this.terrainLayerAsMask & collisionLayerAsMask) > 0)
        {
            this.OnHitTerrain(null);
        }
    }
}

