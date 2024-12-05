using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulwarkTower : BaseTower
{
    //value to multiply damage by when elevation is increased
    [SerializeField] float percentageMultiplier = 0.5f;
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

    public override void InitializeTileBuff(Tile tile)
    {
        base.InitializeTileBuff(tile);
        this.elementalMultiplier = this.tile.elevation * (1 - this.fountainElevationMultiplier);
        this.buff = -(this.elementalMultiplier * percentageMultiplier);
        //Debug.Log(this.gameObject + " buff: " + this.buff);
    }

}
