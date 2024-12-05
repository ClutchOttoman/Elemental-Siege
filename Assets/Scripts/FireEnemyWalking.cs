using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEnemyWalking : BaseEnemy
{
    //[SerializeField] protected float speedMultiplier = 0.3f;
    protected float baseAgentSpeed;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        this.baseAgentSpeed = this.agent.speed;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void ApplyTileModifiers(float elevation, float wetness)
    {
        //get difference from two for true modifier value  i.e 0.9 wetness -> 1.1 speed modifier ; 1.5 wetness -> 0.5 speed modifier
        float speedModifier = (2 - wetness);
        this.agent.speed = this.baseAgentSpeed * speedModifier;

        //Debug.Log("Enemy: " + this.transform.gameObject + " speed modifier =" + speedModifier);
    }
}
