using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseEnemy : MonoBehaviour
{

    [SerializeField]
    // Don't change this normally.
    protected float immunityFrames;
    [SerializeField]
    public float health = 100;
    [SerializeField]
    public float speed = 1.0f;
    [SerializeField] protected float attackInterval = 2;
    protected float prevAttackTime = 0;

    [SerializeField]
    protected float damage = 10;

    protected Stronghold stronghold;

    protected int money;
    [SerializeField]
    protected float moneyFactor = 1.0f;

    protected bool isCloseToGoal = false;
    protected float closestDistanceToGoal = 150f;
    protected float totalDamage = 0;

    protected float stunTime = 0;
    protected bool isStunned = false;

    protected int quicksandApplictations = 0;

    protected float freezeTime = 0f;
    protected bool isFrozen = false;

    public Vector3 targetDirection = Vector3.positiveInfinity;

    protected float lastInjuredTime;

    protected bool isTakingDamageOverTime;
    protected float lastDamageOverTimeDamage = 0f;
    [SerializeField] protected float damageOverTimeInterval = 0.1f;
    protected float damageOverTime;

    protected UnityEngine.AI.NavMeshAgent agent;
    protected float agentSpeed;
    protected Rigidbody rb;
    [SerializeField] protected float baseDamageResistance = 1; // value of 1 means no resistance or weakness ; <1 takes more damage; >1 takes less damage
    [SerializeField] protected float maxDamageResistance = 0.3F;
    protected float damageResistance;
    [SerializeField] protected float timeToLive = 300f; // seconds
    private float spawnedTime;
    [SerializeField] protected float healthBarSizeDeltaX = 0.25f;
    [SerializeField] protected float healthBarSizeDeltaY = 0.01f;
    [SerializeField] protected float healthBarCanvasScale = 0.001f;
    [SerializeField] protected float healthBarVerticalOffset = 1.5f;
    protected Canvas healthCanvas;
    protected Image healthBar;

    protected bool isDead = false;

    public enum Typing
    {
        Earth = 0,
        Energy = 1,
        Aquatic = 2,
        Flight = 3
    }
    [SerializeField] public Typing enemyType;

    public bool isFlying = false;
    //public Vector3 velocity  = new Vector3(0.01F, 0, 0); //Temp to move enemies?

    protected virtual void Awake()
    {
        // Try to get the NavMeshAgent component attached to this GameObject
        this.agent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
        this.rb = this.GetComponent<Rigidbody>();
        if (this.agent == null)
        {
            Debug.LogError("No NavMeshAgent component was found on " + this.gameObject.name);
        }
        if (this.rb == null)
        {
            Debug.LogError("No Rigidbody component was found on " + this.gameObject.name);
        }
        this.money = (int)Mathf.RoundToInt((Mathf.Sqrt(this.health)) * this.moneyFactor);
        this.spawnedTime = Time.time;
        this.speed += UnityEngine.Random.Range(-0.25f, 0.25f);

        this.InitHealthBar();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        EnemyManager.Instance.AddEnemy(this.transform, this);
        EnemyManager.Instance.AddPath(this.transform, this.agent);
        damageResistance = baseDamageResistance;
        this.SetStunned(false);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        this.healthCanvas.transform.position = transform.position + new Vector3(0f, 1.5f, 0f);
    }

    protected virtual void FixedUpdate()
    {
        if (this.stronghold == null && EnemyManager.Instance.stronghold != null)
        {
            this.stronghold = EnemyManager.Instance.stronghold;
        }
        if (this.agent != null && this.rb != null && this.stronghold != null)
        {
            var strongholdDirection = this.stronghold.transform.position - this.transform.position;
            strongholdDirection.y = 0;
            //Enemy is next to stronghold
            if (!this.isFrozen && this.agent.hasPath && (strongholdDirection.sqrMagnitude < 1))
            {
                //Debug.Log(this.gameObject + " Stop Agent");
                this.StopAgent();
                this.isCloseToGoal = true;
            }
            else if (this.isCloseToGoal && (this.agent.remainingDistance > 1) && !this.isStunned && !this.isFrozen)
            {
                //Debug.Log(this.gameObject + " Start Agent");
                this.StartAgent();
            }
        }

        //close to goal and can attack -> do damage to stronghold and log it
        if (this.isCloseToGoal && (TowerManager.Instance.towerTimer >= (this.prevAttackTime + this.attackInterval)))
        {
            //Debug.Log(this.transform.gameObject + "hit stronghold from position: " + this.transform.position);
            this.prevAttackTime = TowerManager.Instance.towerTimer;
            EnemyManager.Instance.stronghold.TakeDamage(this.damage);
            this.totalDamage += this.damage;
        }

        this.CheckTTL();

        if (this.isTakingDamageOverTime && TowerManager.Instance.towerTimer >= (this.lastDamageOverTimeDamage + this.damageOverTimeInterval))
        {
            //Debug.Log(this.transform.gameObject + "taking DOT ->" + this.damageOverTime);
            this.TakeDamage(this.damageOverTime);
            this.lastDamageOverTimeDamage = TowerManager.Instance.towerTimer;
        }
        else if (this.isTakingDamageOverTime)
        {
            //Debug.Log(this.transform.gameObject + " -> Not Time for DOT");
        }

        if (this.agent != null && this.agent.remainingDistance < this.closestDistanceToGoal)
        {
            this.closestDistanceToGoal = this.agent.remainingDistance;
        }
        if (this.health <= 0)
        {
            this.Die();
            return;
        }
        if (this.stunTime > 0 && !this.isStunned && this.agent != null)
        {
            this.SetStunned(true);
        }
        else if (this.stunTime <= 0 && this.isStunned && this.agent != null)
        {
            //Debug.Log("Reenabling nav agent because " + this.stunTime + " and " + this.isStunned);
            this.SetStunned(false);
        }
        if (this.stunTime > 0)
        {
            //Debug.Log("Stun time: " + (this.stunTime));
            this.stunTime--;
        }
        if ((this.agent != null))
        {
            this.agent.nextPosition = this.transform.position;
        }
        if(this.isFrozen && this.freezeTime <= 0 && this.agent != null)
        {
            this.StartAgent();
        }
        if(this.freezeTime > 0)
        {
            this.freezeTime--;
        }

    }

    // Initializes the health bar
    protected virtual void InitHealthBar()
    {
        // Create the health canvas to hold the health bar
        this.healthCanvas = new GameObject("HealthCanvas").AddComponent<Canvas>();
        this.healthCanvas.renderMode = RenderMode.WorldSpace;
        this.healthCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(healthBarSizeDeltaX, healthBarSizeDeltaY);
        this.healthCanvas.transform.localScale = new Vector3(healthBarCanvasScale, healthBarCanvasScale, healthBarCanvasScale);

        // Create the health bar
        this.healthBar = new GameObject("Healthbar").AddComponent<Image>();
        this.healthBar.transform.SetParent(healthCanvas.transform);
        this.healthBar.color = Color.green;
        this.healthBar.rectTransform.sizeDelta = new Vector2(healthBarSizeDeltaX, healthBarSizeDeltaY);
        
        this.healthCanvas.transform.SetParent(this.transform);
        this.healthCanvas.transform.localPosition = new Vector3(0f, healthBarVerticalOffset, 0f);
        this.healthBar.rectTransform.localPosition = Vector3.zero;
    }

    // Returns this enemy's element type as a string.
    public Typing GetEnemyTyping(){
        return this.enemyType;
    }

    public bool ShouldBeImmune()
    {
        return TowerManager.Instance.towerTimer - this.lastInjuredTime <= this.immunityFrames;

    }

    //takes the damage to be done to the enemy; returns how much health the enemy has left
    public float TakeDamage(float damage)
    {
        float damageDealt = 0;
        if (!this.ShouldBeImmune())
        {
            damage = damage / this.damageResistance;
            damageDealt = this.health - Math.Max(0, this.health - damage);
        }

        this.health -= damageDealt;

        // Update the health bar
        float healthPercentage = this.health / 100;
        this.healthBar.rectTransform.sizeDelta = new Vector2(healthBarSizeDeltaX * healthPercentage, healthBarSizeDeltaY);
        this.healthBar.color = Color.Lerp(Color.red, Color.green, healthPercentage);

        //Debug.Log(this.gameObject + " was hit for " + damage + " damage");
        return this.health;
    }

    //Put anything that happens when killed in here
    protected void Die()
    {
        if(this.isDead) return;
        if (!EnemyManager.Instance.RemoveTransform(this.transform))
        {
            //Debug.LogError("Failed To Remove Enemy:" + this.gameObject + " From Dictionary");
        }
        //Debug.Log(this.transform.gameObject + " gave $" + this.money);
        EnemyManager.Instance.GetWallet().CollectMoney(this.money);
        LevelManager.Instance.DecreaseEnemyCount();
        PlayerDataManager.Instance.updateEnemiesKilled(this.GetEnemyTyping()); // alert the PlayerDataManager.
        PlayerDataManager.Instance.printAll();
        this.StopTakingDamageOverTime(100000f);
        LevelManager.Instance.LogEnemyTypeDamage(this.totalDamage, this.enemyType);
        LevelManager.Instance.LogEnemyTypeDistance(this.closestDistanceToGoal,this.enemyType);
        
        this.isDead = true;
        
        Destroy(this.gameObject);

        Debug.Log(this.transform.gameObject + " killed at " + this.transform.position);
    }

    // Stun for a certain amount of time.
    public void Stun(float time)
    {
        Debug.Log(this.gameObject + " stunned");
        this.stunTime = time;
    }

    // For now just toggles the NavMeshAgent.
    protected void SetStunned(bool isStunned)
    {
        this.ToggleNavAgent(isStunned);
        this.isStunned = isStunned;
    }

    // Toggles the NavMeshAgent
    // ChatGPT helped.
    protected void ToggleNavAgent(bool isStunned)
    {
        // Debug.Log("Setting nav agent to " + !isStunned);
        if (this.agent != null)
        {
            if (isStunned)
            {
                // Deactivate the NavMeshAgent
                this.agent.isStopped = true; // Stop movement
                this.agent.updatePosition = false; // Stop updating position
                this.agent.updateRotation = false; // Stop updating rotation
            }
            else
            {
                // Reactivate the NavMeshAgent
                this.agent.isStopped = false; // Resume movement
                this.agent.updatePosition = true; // Resume updating position
                this.agent.updateRotation = true; // Resume updating rotation
                this.agent.speed = this.speed;
            }
        }
        if (this.rb != null)
        {
            {
                if (isStunned)
                {
                    this.rb.isKinematic = false; // Enable physics
                }
                else
                {
                    this.rb.isKinematic = true;
                }

            }
        }
    }

    public void ApplyForce(Vector3 force)
    {
        // Debug.Log("Applying force " + force);
        if (rb != null && this.isStunned)
        {
            rb.AddForce(force, ForceMode.Force);
        }
    }
    //Takes tile and uses tile values to choose modifier values
    public virtual void ApplyTileModifiers(float elevation, float wetness)
    {
        //Debug.Log("Applying Tile Buff with: " + elevation + "/" + wetness);
    }
    public virtual void SlowAgent(float slowModifier)
    {
        this.quicksandApplictations++;
        //Debug.Log(this.transform.gameObject + " is slowed x" + this.quicksandApplictations);
        if (this.quicksandApplictations == 1)
        {
            //Debug.Log(this.transform.gameObject + " speed: " + this.agent.speed + " -> " + (this.agent.speed - slowModifier));
            this.agent.speed = this.agent.speed - slowModifier;
        }
    }
    public virtual void SpeedUpAgent(float speedModifier)
    {
        this.quicksandApplictations--;
        if (this.quicksandApplictations <= 0)
        {
            //Debug.Log(this.transform.gameObject + " is speeding up");
            this.agent.speed += speedModifier;
            this.quicksandApplictations = 0;
        }
    }
    public virtual void StopAgent()
    {
        this.agentSpeed = this.agent.speed;
        this.agent.speed = 0;
    }

    public virtual void StartAgent()
    {
        // Reactivate the NavMeshAgent
        this.agent.speed = this.agentSpeed;
        this.agentSpeed = 0;
        this.isCloseToGoal = false;
    }
    public virtual Vector3 GetVelocity()
    {
        return this.agent.velocity;
    }

    /// Kills the enemy if it's time to live is up.
    protected void CheckTTL()
    {
        if (this.timeToLive > 0 && Time.time - this.spawnedTime > this.timeToLive) {
            this.Die();
        }
    }

    public virtual void FreezeEnemy(float freezeTime)
    {
        this.isFrozen = true;
        this.freezeTime = freezeTime;
        this.StopAgent();
    }

    //damage over time for WindcallerTower -> Additive multiple windcaller projectiles do more damage per interval
    public virtual void StartTakingDamageOverTime(float damage)
    {
       
        if (this.isTakingDamageOverTime)
        {
            this.damageOverTime += damage;
            //Debug.Log(this.transform.gameObject + " add DOT ->" + damage);
        }
        else
        {
            this.damageOverTime = damage;
            this.isTakingDamageOverTime = true;
            //Debug.Log(this.transform.gameObject + " start taking DOT ->" + damage);
        }
    }
    public virtual void StopTakingDamageOverTime(float damage)
    {
        //Debug.Log(this.transform.gameObject + " decrease DOT ->" + damage);
        if (!this.isTakingDamageOverTime) { this.damageOverTime = 0; }
        this.damageOverTime -= damage;
        if(Mathf.Approximately(this.damageOverTime, 0) || this.damageOverTime <= 0)
        {
            this.damageOverTime = 0;
            this.isTakingDamageOverTime = false;
            this.lastDamageOverTimeDamage = 0;
            //Debug.Log(this.transform.gameObject + " stop taking DOT");
        }
    }

    public virtual float GetDistanceToStronghold() {
        if (isCloseToGoal)
        {
            return 0;
        }
        return this.agent.remainingDistance;
    }
}