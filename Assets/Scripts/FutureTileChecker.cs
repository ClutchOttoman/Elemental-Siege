using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FutureTileChecker : MonoBehaviour
{
    protected int tileLayerAsMask = 1 << 7;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (null == other)
        {
            Debug.LogWarning("collider is null for tile check");
            return;
        }

        int collisionLayerAsMask = 1 << other.transform.gameObject.layer;

        //not tile collider
        if ((collisionLayerAsMask & this.tileLayerAsMask) <= 0)
        {
            return;
        }
        AirEnemyFlying self = (AirEnemyFlying) EnemyManager.Instance.GetEnemy(this.transform.parent.transform);
        Tile tile = TileManager.Instance.GetTile(other.transform);
        if (tile != null && self != null)
        {
            //self.SetFlightElevation(tile.elevation);
            Debug.Log(self + " is flying to elevation: "+ tile.elevation);
        }
        else
        {
            Debug.LogWarning("tile or enemy is null for future tile check; enemy: " + self + " tile: " + tile);
            return;
        }
    }
}
