using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 10F;
    [SerializeField] public float baseDamage = 10;
    [SerializeField] private float gravity = -9.8F;
    [SerializeField] protected float lifeTime = 60F;
    [SerializeField] protected float aoeRadius = 0.2F;

    private Vector3 gravityVector;

    protected int enemyLayerAsMask;
    protected int terrainLayerAsMask;

    public float buffedDamage;
    protected Rigidbody projectileRigidbody;
    protected float spawnTime;

    // Awake is called on initialization
    public virtual void Awake()
    {
        this.enemyLayerAsMask = 1 << LayerMask.NameToLayer("Enemy");
        this.terrainLayerAsMask = 1 << LayerMask.NameToLayer("Tiles");
        this.buffedDamage = baseDamage;
        this.projectileRigidbody = this.GetComponent<Rigidbody>();
        this.projectileRigidbody.useGravity = false;
        this.getGravity();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        this.spawnTime = TowerManager.Instance.towerTimer;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
    }

    // Fixed update is called once per frame
    protected virtual void FixedUpdate()
    {
        if (this.getGravity() != null)
        {
            projectileRigidbody.AddForce(this.getGravity(), ForceMode.Acceleration);
        }
        if (this.ShouldDestroy())
        {
            Destroy(this.gameObject);
        }
    }

    protected virtual bool ShouldDestroy()
    {
        return this.spawnTime + this.lifeTime < TowerManager.Instance.towerTimer;
    }

    protected virtual void OnDestroy()
    {
        //Debug.Log( this.gameObject +" Destroyed");
    }

    //Handle when projectile collides with a gameObject
    protected virtual void OnTriggerEnter(Collider obj)
    {

        //Debug.Log("HIT " + obj.transform.parent.gameObject);
        int collisionLayerAsMask = 1 << obj.transform.gameObject.layer;
        // Debug.Log(collisionLayerAsMask + " vs " + this.terrainLayerAsMask + " & " + this.enemyLayerAsMask);
        //check if what we hit is an enemy
        if ((this.enemyLayerAsMask & collisionLayerAsMask) > 0)
        {
            var enemy = EnemyManager.Instance.GetEnemy(obj.transform);
            if (enemy != null)
            {
                this.DoAOE(this.enemyLayerAsMask, obj => { this.OnHitEnemy(EnemyManager.Instance.GetEnemy(obj.transform)); });
                // this.OnHitEnemy(enemy);
            }
        }
        else if ((this.terrainLayerAsMask & collisionLayerAsMask) > 0)
        {
            this.OnHitTerrain(null);
        }
    }

    // ChatGPT 4.0 helped with getting the command to get the colliders.
    protected virtual void DoAOE(int targetCollisionLayerAsMask, Action<GameObject> action)
    {
        // Get all colliders within the specified radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, this.aoeRadius);

        foreach (Collider collider in colliders)
        {
            // Get the GameObject the collider is attached to
            GameObject collidedWith = collider.gameObject;

            // Run the function on each object
            if (collidedWith != null)
            {
                int collisionLayerAsMask = 1 << collidedWith.transform.gameObject.layer;
                if ((targetCollisionLayerAsMask & collisionLayerAsMask) > 0)
                {
                    // Debug.Log("AOEing " + collidedWith);
                    action(collidedWith);

                }
            }
        }

    }

    protected virtual void OnHitEnemy(BaseEnemy enemy)
    {
        if (enemy != null)
        {
            enemy.TakeDamage(this.buffedDamage);
        }
        Destroy(this.gameObject);
    }

    protected virtual void OnHitTerrain(GameObject tile)
    {
        Destroy(this.gameObject);
    }

    public virtual void LaunchProjectile(Vector3 direction, float buff)
    {
        this.buffedDamage = this.baseDamage * (1 + buff);
        this.projectileRigidbody.velocity = direction * speed;
        Physics.SyncTransforms();
    }

    public virtual void SetPosition(Vector3 position)
    {
        this.projectileRigidbody.position = position;
    }

    public virtual float getSpeed()
    {
        return this.speed;
    }

    // Override for different results!
    public virtual Vector3 getGravity()
    {
        if (null == this.gravityVector || this.gravityVector.y != gravity)
        {
            this.gravityVector = new Vector3(0, gravity, 0);
        }
        return this.gravityVector;
    }
}
