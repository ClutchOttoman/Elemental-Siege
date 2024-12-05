using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Helps log every move the player takes in a Elemental Siege level.
public class LevelSupervisor : MonoBehaviour
{

    // public Dictionary<string, BaseTower> 
    public int numTotalEnemiesSpawned {get; set;} = 0;
    public int numTotalEnemiesKilled {get; set;} = 0;

    public int numAquaticEnemiesKilled {get; set;} = 0;
    public int numAquaticEnemiesSpawn {get; set;} = 0;

    public int numEarthEnemiesKilled {get; set;} = 0;
    public int numEarthEnemiesSpawn {get; set;} = 0;

    public int numEnergyEnemiesKilled {get; set;} = 0;
    public int numEnergyEnemiesSpawn {get; set;} = 0;

    public int numFlightEnemiesKilled {get; set;} = 0;
    public int numFlightEnemiesSpawn {get; set;} = 0;

    public int numTotalTowersPlaced {get; set;} = 0;
    public int numBanishmentTowersPlaced {get; set;} = 0;
    public int numBulwarkTowersPlaced {get; set;} = 0;
    public int numFireballTowersPlaced {get; set;} = 0;
    public int numTidalTowersPlaced {get; set;} = 0;
    public int numWhirlwindTowersPlaced {get; set;} = 0;

    public void incrementTotalTowersPlaced(){
        this.numTotalTowersPlaced++;
    }

    public void incrementTotalEnemiesKilled(){
        this.numTotalEnemiesKilled++;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
