using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirEnemyFlying : BaseEnemy
{
    protected float baseSpeed;
    [SerializeField]
    protected float speedFactor = 0.05F;
   
    protected Quaternion quaternion;
    [SerializeField] protected float attackRange = 4F;
    Vector3 distanceToStronghold = Vector3.positiveInfinity;

    public float elevation;
    [SerializeField] protected float flightElevation;
    [SerializeField] public float baseFlightElevation = 5f;
    [SerializeField] protected float flightElevationChangeSpeed = 0.5f;
    protected int flightDirection = 0; // -1 down -- 0 none -- +1 up



    protected override void Awake()
    {
        this.quaternion = new Quaternion();
        this.rb = this.GetComponent<Rigidbody>();
        this.money = (int)Math.Round((Mathf.Sqrt(this.health)) * this.moneyFactor, 0);
        this.baseSpeed = this.speed;
        this.InitHealthBar();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        EnemyManager.Instance.AddEnemy(this.transform, this);
        damageResistance = baseDamageResistance;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {

        if (this.stronghold != null)
        {
            distanceToStronghold = this.stronghold.transform.position - this.transform.position;
            distanceToStronghold.y = 0;
        }
        if (this.stronghold == null && EnemyManager.Instance.stronghold != null)
        {
            this.stronghold = EnemyManager.Instance.stronghold;
        }
        if (this.stronghold != null && (distanceToStronghold).sqrMagnitude < this.attackRange)
        {
            this.isCloseToGoal = true;
        }
        else
        {
            this.isCloseToGoal = false;
        }
        if (this.stronghold != null && this.distanceToStronghold.sqrMagnitude < Mathf.Pow(this.closestDistanceToGoal, 2))
        {
            this.closestDistanceToGoal = this.distanceToStronghold.magnitude;
        }

        base.CheckTTL();

        base.FixedUpdate();

        if (this.stronghold != null) {
            var goal = this.stronghold.transform.position;
            goal.y += this.baseFlightElevation;
            this.targetDirection = goal - this.transform.position; 
            this.rb.velocity = Vector3.zero;

            if (this.targetDirection.sqrMagnitude >= 1) //not changing elevation -> move toward stronghold
            {
                //Debug.Log("Moving Direction: " + this.targetDirection.normalized.x + "," + this.targetDirection.normalized.z);
                //Debug.Log(this.transform.gameObject + " at " + this.transform.position + " moving forward");
                //moving will go lower than base flight elevation
                if(this.transform.position.y + this.targetDirection.y <= this.flightElevation)
                {
                    this.targetDirection.y = 0;
                }
                this.transform.position += this.targetDirection.normalized * this.speed * Time.deltaTime;
                var lookDirection = this.targetDirection;
                lookDirection.y = 0;
                this.quaternion.SetLookRotation(lookDirection.normalized, Vector3.up);
                this.transform.rotation = this.quaternion;
            }

            if(this.targetDirection.sqrMagnitude > 1000000 && this.targetDirection != Vector3.positiveInfinity)
            {
                Debug.Log("Flight Enemy Killed Self at " + this.targetDirection.sqrMagnitude);
                this.Die();
            }
        }
    }


    public override void ApplyTileModifiers(float elevation, float wetness)
    {
        if (!Mathf.Approximately(elevation, this.elevation)) {
            this.elevation = elevation;
        }
        float speedModifier = 1 + speedFactor * (elevation - 1);
        this.speed = this.baseSpeed * speedModifier;
        SetFlightElevation(elevation);
    }

    protected void AdjustFlightElevation()
    {
       
        if (Mathf.Approximately(this.flightElevation,this.baseFlightElevation + this.elevation) || Mathf.Approximately(this.transform.position.y, this.baseFlightElevation + this.elevation))
        {
            this.flightDirection = 0;
            this.targetDirection.y = 0;   
        }
        else if(this.transform.position.y < (this.baseFlightElevation + this.elevation))
        {
            //going up
            this.flightDirection = 1;
        }
        else if (this.transform.position.y > (this.baseFlightElevation + this.elevation))
        {
            //going down
            this.flightDirection = -1;
        }
        //Debug.Log("Flying Enemy adjusting elevation to " + this.flightElevation);

    }
    public void SetFlightElevation(float elevation)
    {
        if (Mathf.Approximately(this.flightElevation, this.baseFlightElevation + this.elevation)) return;
        this.flightElevation = elevation + this.baseFlightElevation;
        this.AdjustFlightElevation();
    }

    public override Vector3 GetVelocity()
    {
        return (this.targetDirection.normalized * this.speed *Time.deltaTime);
    }

    public override float GetDistanceToStronghold()
    {
        return this.distanceToStronghold.magnitude;
    }

}