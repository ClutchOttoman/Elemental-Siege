using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballTower : BaseTower
{
    [SerializeField] protected float attackIntervalMultiplier = 3F;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    //low wetness -> faster fire rate ~ 33% better at lowest  High wetness -> lower fire rate ~ 25% worse at highest
    public override void InitializeTileBuff(Tile tile)
    {
        base.InitializeTileBuff(tile);
        float wetness = tile.wetness * (1 - (this.fountainBuff * this.fountainWetnessMultiplier));
        this.attackInterval =  this.baseAttackInterval * (wetness);
    }
}

