using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance { get; private set; }

    //Live lists of all active enemies
    protected Dictionary<Transform, Tile> tileMap = new Dictionary<Transform, Tile>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }


    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddTile(Transform transform, Tile tile)
    {
        tileMap[transform] = tile;
        //Debug.Log("tile: " + tile + " added");
    }

    public Tile GetTile(Transform transform) {
        if (!tileMap.TryGetValue(transform, out Tile tile))
        {
            tile = transform.gameObject.GetComponent<Tile>();
            if (tile != null)
            {
                AddTile(transform, tile);
            }
        }
        return tile;
    }
}
