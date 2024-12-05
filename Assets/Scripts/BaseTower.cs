using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BaseTower : MonoBehaviour
{

    [SerializeField]
    protected float attackInterval = 10F; //in seconds
    protected float baseAttackInterval;
    [SerializeField]
    protected float degreesFiringArc = 180F;
    protected float degreesFiringArcCosine;
    protected float timeAtLastAttack;
    public int health = 100;
    public enum Elements
    {
        None,
        Earth,
        Fire,
        Water,
        Air
    }
    [SerializeField] public Elements element;

    //To store relavant value for buff to damage only! keep zero if buff does not affect damage
    protected float buff = 0;

    //Stores multiplier to set relevant elemental buffs
    protected float elementalMultiplier;
    protected int fountainBuff = 0;
    [SerializeField] protected float fountainWetnessMultiplier = 0.01f;
    [SerializeField] protected float fountainElevationMultiplier = 0.01f;


    //Pts for top and bottom of capsule collider for range finding
    protected Vector3 aimingLowerBound;
    protected Vector3 aimingUpperBound;
    [SerializeField]
    protected float range = 10f;
    protected int targetLayerAsMask = 1 << 10;
    protected int flyingLayerAsMask = 1 << 11;
    protected int defaultLayerAsMask = 1 << 1;
    protected int tileLayerAsMask = 1 << 7;
    protected int visiblityCheckLayersAsMask;
    [SerializeField] protected bool canTargetFlyers = true;
    [SerializeField] protected GameObject projectileSpawnPoint;
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] public float volumeAudioValue = 0.5f;

    protected enum TargetPriorities
    {
        First,
        Last,
        Strong,
        Close
    }
    [SerializeField]
    protected TargetPriorities targetPriority = TargetPriorities.First; // 0:first,1:last,2:strong,3:close

    [SerializeField]
    protected GameObject projectileTemplate;
    [SerializeField]
    protected ParticleSystem firingParticleSystem;

    public Tile tile { get; set; }

    [SerializeField] public Tile defaultTile;

    [SerializeField] public int cost = 250;

    // Awake is called on initialization.
    protected virtual void Awake()
    {
        visiblityCheckLayersAsMask = this.tileLayerAsMask;
        this.aimingLowerBound = new Vector3(this.transform.position.x, this.transform.position.y - 10, this.transform.position.z);
        this.aimingUpperBound = new Vector3(this.transform.position.x, this.transform.position.y + 10, this.transform.position.z);
        this.firingParticleSystem = this.GetComponent<ParticleSystem>();
        this.baseAttackInterval = this.attackInterval;
        
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        this.timeAtLastAttack = TowerManager.Instance.towerTimer;
        TowerManager.Instance.RegisterTower(this.transform, this);

        //foreach (Transform child in this.gameObject.transform)
        //{
        //    Debug.Log("Checking if " + child.name + " is ProjectileSpawnPoint");
        //    if (child.name == "ProjectileSpawnPoint")
        //    {
        //        Debug.Log("Setting the projectile spawn point!");
        //        this.projectileSpawnPoint = child.gameObject;
        //    }
        //}
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        //Attacks can happen every interval
        if (AttemptAttack())
        {
            // Play the firing sound
            if (this.audioSource != null){
                AudioSource.PlayClipAtPoint(this.audioSource.clip, this.transform.position + new Vector3(0.0f, -30.0f, 0.0f), this.volumeAudioValue);
            } 
        }

        if(this.tile == null)
        {
            this.tile = this.defaultTile;
        }

    }

    public Elements GetTowerTyping(){
        return this.element;
    }

    //setters for tile info in subclass

    //for subclass to set its tile info and get the correct info for its buff
    public virtual void InitializeTileBuff(Tile tile)
    {
        this.tile = tile;
    }

    //for subclass to add its approiate buff from fountain tower
    public void AddFountainBuff()
    {
        this.fountainBuff++;
        // Debug.Log("Buffing " + this.gameObject + " got its fountain buff incremented to " + fountainBuff);
        this.InitializeTileBuff(this.tile);
    }

    protected virtual bool IsInFiringAngle(Vector3 position)
    {

        bool b = (Vector3.Angle(this.transform.position - position, this.transform.forward) < this.degreesFiringArc);
        return b;
        
    }

    protected virtual bool AttemptAttack()
    {
        // Debug.Log("Trying to attack at " + this.transform.position);
        if ((this.timeAtLastAttack + this.GetAttackInterval()) <= TowerManager.Instance.towerTimer)
        {
            //Debug.Log("time: " +  (timeAtLastAttack+attackInterval) + "  time2: " + TowerManager.Instance.towerTimer);
            //get all colliders in Target Layer(10) that overlap
            Collider[] hitColliders;
            if (this.canTargetFlyers)
            {
                hitColliders = Physics.OverlapCapsule(this.aimingLowerBound, this.aimingUpperBound, this.GetRange(), (this.targetLayerAsMask|this.flyingLayerAsMask));
            }
            else
            {
                hitColliders = Physics.OverlapCapsule(this.aimingLowerBound, this.aimingUpperBound, this.GetRange(), this.targetLayerAsMask);
            }
            hitColliders = MathHelpers.FilteredArray(hitColliders, (hitCollider) => this.IsInFiringAngle(hitCollider.transform.position));
            hitColliders = MathHelpers.FilteredArray(hitColliders, (hitColliders) => this.CheckIfTargetIsVisiblie(hitColliders));
            if (hitColliders.Length == 0)
            {
                return false;
                /*Debug.Log("No Targets in Range")*/
            }
            else
            {
                var target = GetTarget(hitColliders);
                if (Attack(target))
                {
                    timeAtLastAttack = TowerManager.Instance.towerTimer; ;
                    return true;
                }
                else { return false; }
            }
        }
        else
        {
            return false;
        }
    }


    // So it's easy to override in subclasses.
    // And easy to edit later, in case buffs affect it.
    protected float GetAttackInterval()
    {
        return this.attackInterval;
    }

    // So it's easy to override in subclasses.
    // And easy to edit later, in case buffs affect it.
    protected float GetRange()
    {
        return this.range;
    }

    // Get offset for bullet spawn point.
    protected Vector3 GetBulletEmitterLocation()
    {
        return new Vector3(0, 1, 0);
    }

    //Raycast to see if target can be hit uninterrupted by terrain
    protected bool CheckIfTargetIsVisiblie(Collider target)
    {
        var shooterPositon = this.projectileSpawnPoint.transform.position;
        var targetDirection = target.transform.position - shooterPositon;
        bool isNotVisible = Physics.Raycast(shooterPositon, targetDirection.normalized, targetDirection.magnitude, visiblityCheckLayersAsMask);
        if (!isNotVisible) {
            // Debug.Log(target.transform.parent + " is visible");
            return true;
        }
        else
        {
            // Debug.Log(target.transform.parent + " is not visible");
            return false;
        }
    }

    protected virtual Collider GetTarget(Collider[] hitColliders)
    {
        var target = hitColliders[0];
        var targetParent = EnemyManager.Instance.GetEnemy(hitColliders[0].transform.parent);
        //var targetPath = EnemyManager.Instance.GetPath(hitColliders[0].transform.parent);
        //UnityEngine.AI.NavMeshAgent parentPath;


        // Debug.Log(target.transform.parent + " : " + targetParent);

        switch (targetPriority)
        {
            case TargetPriorities.First: //First
                for (int i = 1; i < hitColliders.Length; i++)
                {
                    
                    if (GetRemainingDistance(hitColliders[i]) < GetRemainingDistance(target))
                    {
                      
                        //targetPath = parentPath;
                        target = hitColliders[i];
                    }
                }
                break;
            case TargetPriorities.Last: //Last
                for (int i = 1; i < hitColliders.Length; i++)
                {
                    //parentPath = EnemyManager.Instance.GetPath(hitColliders[i].transform.parent);
                    if (GetRemainingDistance(hitColliders[i]) > GetRemainingDistance(target))
                    {
                        
                        //targetPath = parentPath;
                        target = hitColliders[i];
                    }
                }
                break;
            case TargetPriorities.Strong: //Strong
                for (int i = 1; i < hitColliders.Length; i++)
                {
                    var parent = EnemyManager.Instance.GetEnemy(hitColliders[i].transform.parent);
                    //parentPath = EnemyManager.Instance.GetPath(hitColliders[i].transform.parent);
                    if (parent.health > targetParent.health)
                    {
                        targetParent = parent;
                        target = hitColliders[i];
                    }
                    else if (parent.health == targetParent.health && GetRemainingDistance(hitColliders[i]) < GetRemainingDistance(target))
                    {
                      
                        //Target First Strong
                        target = hitColliders[i];
                        //targetPath = parentPath;
                    }
                }
                break;
            case TargetPriorities.Close: //Close
                var targetDistance = Vector3.Distance(target.transform.position, this.transform.position);
                for (int i = 1; i < hitColliders.Length; i++)
                {
                    //parentPath = EnemyManager.Instance.GetPath(hitColliders[i].transform.parent);
                    var distance = Vector3.Distance(hitColliders[i].transform.position, this.transform.position);
                    if (distance < targetDistance)
                    {
                        
                        targetDistance = distance;
                        target = hitColliders[i];
                    }
                    else if (distance == targetDistance && GetRemainingDistance(hitColliders[i]) < GetRemainingDistance(target))
                    {
                       
                        //Target First Close
                        target = hitColliders[i];
                        //targetPath = parentPath;
                    }
                }
                break;
            default:
                break;
        }
        return target;
    }

    protected virtual float GetRemainingDistance(Collider target)
    {
        BaseEnemy enemy =  EnemyManager.Instance.GetEnemy(target.transform.parent);
        return enemy.GetDistanceToStronghold();
    }

    //Spawn projectile then tell it where to move towards -- target is not the enemy its the collider in the top half of the enemy to aim at. target.transform.parent is the enemy
    protected virtual bool Attack(Collider target)
    {
        var templateScript = this.projectileTemplate.GetComponent<BaseProjectile>();
        var targetEnemyNavAgent = EnemyManager.Instance.GetPath(target.transform.parent);

        //Flyers dont have nav agents -> redirect
        if (targetEnemyNavAgent == null && this.canTargetFlyers)
        {
            return AttackFlyingEnemy(target);
        }
        else if (targetEnemyNavAgent == null)
        {
            Debug.LogWarning("Unable to locate enemy NavAgent");
            return false;
        }
        else if (templateScript == null)
        {
            Debug.LogWarning("Unable to locate projectile script. Enemy is at " + target.transform.position);
            return false;
        }

        //Debug.Log("object: " + this.projectileSpawnPoint);
        //Debug.Log("transform: " + this.projectileSpawnPoint.transform);
        //Debug.Log("position: " + this.projectileSpawnPoint.transform.position);
        Vector3 shooterPosition = this.projectileSpawnPoint.transform.position;
        Vector3 targetPosition = target.transform.position;
        Vector3 targetVelocity = targetEnemyNavAgent.velocity;
        float projectileSpeed = templateScript.getSpeed();
        Vector3 gravity = templateScript.getGravity();
        Vector3 firingDirection = new Vector3(0, 0, 0);

        // If there's a shot to be taken, take it.
        bool hasShot = PredictiveAiming.CalculateFiringSolutionWithGravity(shooterPosition, targetPosition, targetVelocity, projectileSpeed, gravity, out firingDirection);

        if (hasShot && (Vector3.Dot(firingDirection, targetPosition - shooterPosition) > 0))
        {
            var projectileToLaunch = Instantiate(this.projectileTemplate, this.projectileSpawnPoint.transform);
            projectileToLaunch.transform.SetParent(null);
            var projectileScript = projectileToLaunch.GetComponent<BaseProjectile>();
            // Fire!
            projectileScript.LaunchProjectile(firingDirection.normalized, buff);
            // Spawn the firing VFX
            firingParticleSystem.Play();
            return true;
        }

        return false;
    }
    protected virtual bool AttackFlyingEnemy(Collider target)
    {
        var templateScript = this.projectileTemplate.GetComponent<BaseProjectile>();
        BaseEnemy enemy = EnemyManager.Instance.GetEnemy(target.transform.parent);
        //Debug.Log(this.transform.gameObject + " Attacking Flyer");
        if(enemy == null)
        {
            Debug.Log("Unable to get enemy script for attacking flying enemy");
            return false;
        }
        if (templateScript == null)
        {
            Debug.LogWarning("Unable to locate projectile script. Enemy is at " + target.transform.position);
            return false;
        }

        //Debug.Log("object: " + this.projectileSpawnPoint);
        //Debug.Log("transform: " + this.projectileSpawnPoint.transform);
        //Debug.Log("position: " + this.projectileSpawnPoint.transform.position);
        Vector3 shooterPosition = this.projectileSpawnPoint.transform.position;
        Vector3 targetPosition = target.transform.position;
        Vector3 targetVelocity = enemy.GetVelocity();
        float projectileSpeed = templateScript.getSpeed();
        Vector3 gravity = templateScript.getGravity();
        Vector3 firingDirection = new Vector3(0, 0, 0);

        // If there's a shot to be taken, take it.
        bool hasShot = PredictiveAiming.CalculateFiringSolutionWithGravity(shooterPosition, targetPosition, targetVelocity, projectileSpeed, gravity, out firingDirection);

        if (hasShot && (Vector3.Dot(firingDirection, targetPosition - shooterPosition) > 0))
        {
            var projectileToLaunch = Instantiate(this.projectileTemplate, this.projectileSpawnPoint.transform);
            projectileToLaunch.transform.SetParent(null);
            var projectileScript = projectileToLaunch.GetComponent<BaseProjectile>();
            // Fire!
            projectileScript.LaunchProjectile(firingDirection.normalized, buff);
            // Spawn the firing VFX
            firingParticleSystem.Play();
            return true;
        }

        return false;

    }

    // By ChatGPT 4o
    public static double CalculateTimeOfIntercept(
        double Ex0, double Ey0, // Enemy's initial position
        double Evx, double Evy, // Enemy's velocity components
        double Bx0, double By0, // Turret's initial position
        double Bv               // Bullet's total velocity (magnitude)
    )
    {
        // Step 1: Calculate the coefficients of the quadratic equation
        double EvSquared = Evx * Evx + Evy * Evy; // Total velocity squared of the enemy
        double a = EvSquared - Bv * Bv;

        // Coefficients for the linear term (b)
        double b = 2 * (Evx * (Ex0 - Bx0) + Evy * (Ey0 - By0));

        // Coefficient for the constant term (c)
        double c = (Ex0 - Bx0) * (Ex0 - Bx0) + (Ey0 - By0) * (Ey0 - By0);

        // Step 2: Compute the discriminant (b^2 - 4ac)
        double discriminant = b * b - 4 * a * c;

        // Step 3: Check if a valid solution exists (discriminant must be non-negative)
        if (discriminant < 0)
        {
            throw new Exception("No valid intercept found (discriminant < 0).");
        }

        // Step 4: Calculate the two possible values for time (t)
        double sqrtDiscriminant = Math.Sqrt(discriminant);
        double t1 = (-b - sqrtDiscriminant) / (2 * a);
        double t2 = (-b + sqrtDiscriminant) / (2 * a);

        // Step 5: Return the positive time value (t must be greater than 0)
        if (t1 > 0) return t1;
        if (t2 > 0) return t2;

        throw new Exception("No positive intercept time found.");
    }

    // By ChatGPT 40
    public static double CalculateFiringAngle(
        double Ex0, double Ey0, // Enemy's initial position
        double Evx, double Evy, // Enemy's velocity components
        double Bx0, double By0, // Turret's initial position
        double Bv,              // Bullet's total velocity
        double t                // The time of intercept
    )
    {
        // Step 1: Calculate the enemy's position at time t
        double ExIntercept = Ex0 + Evx * t;
        double EyIntercept = Ey0 + Evy * t;

        // Step 2: Calculate the delta x and delta y between the turret and enemy's intercept position
        double deltaX = ExIntercept - Bx0;
        double deltaY = EyIntercept - By0;

        // Step 3: Calculate the angle using the arctangent of (deltaY / deltaX)
        // atan2 is used to handle the correct quadrant
        double firingAngle = Math.Atan2(deltaY, deltaX);

        return firingAngle;
    }

    // By ChatGPT 4o
    public static (double, double) CalculateInterceptPosition(
        double Ex0, double Ey0, // Enemy's initial position
        double Evx, double Evy, // Enemy's velocity components
        double Bx0, double By0, // Turret's initial position
        double Bv,              // Bullet's total velocity
        double t                // The time of intercept
    )
    {
        // Calculate the enemy's intercept position at time t
        double ExIntercept = Ex0 + Evx * t;
        double EyIntercept = Ey0 + Evy * t;

        // Return the intercept position as a tuple (x, y)
        return (ExIntercept, EyIntercept);
    }


    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(this.transform.position, range);
    }


}
