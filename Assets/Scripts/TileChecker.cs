using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileChecker : MonoBehaviour
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
        BaseEnemy self = EnemyManager.Instance.GetEnemy(this.transform.parent.transform);
        Tile tile = TileManager.Instance.GetTile(other.transform);
        if (tile != null && self != null)
        {
            self.ApplyTileModifiers(tile.elevation, tile.wetness);
            //Debug.Log(self + " is on tile: "+ tile);
        }
        else
        {
            Debug.LogWarning("tile or enemy is null for tile check; enemy: " + self + " tile: " + tile);
            return;
        }
    }
}
