using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.SceneManagement;

// Uses PlayerPrefs to handle loading saves in between levels.
// Examines Tile.cs for tower placements, BaseEnemy.cs for enemy deaths, and EnemySpawner.cs files for enemy spawning.
// Is alerted by the LevelManager when to save.
public class PlayerDataManager : MonoBehaviour
{
    private class LevelDataMetrics {
        public string levelName {get; set;} = "Default Level";

        // Enemy data
        public int numEnemyTotalKilled {get; private set;} = 0;
        public int numEnemyTotalSpawned {get; private set;} = 0;
        public int currWave {get; set;} = 0;
        public int totalWaves {get; set;} = 0;

        public int numAquaticEnemyKilled {get; private set;} = 0;
        public int numAquaticEnemySpawn {get; private set;} = 0;

        public int numFlightEnemyKilled {get; private set;} = 0;
        public int numFlightEnemySpawn {get; private set;} = 0;

        public int numEarthEnemyKilled {get; private set;} = 0;
        public int numEarthEnemySpawn {get; private set;} = 0;

        public int numEnergyEnemiesKilled {get; private set;} = 0;
        public int numEnergyEnemySpawn {get; private set;} = 0;


        // Tower data
        public int numTowersPlacedTotal {get; private set;} = 0;
        public int numBulwarkTowersPlaced {get; private set;} = 0;
        public int numBanishTowersPlaced {get; private set;} = 0;
        public int numFireballTowersPlaced {get; private set;} = 0;
        public int numTidalTowersPlaced {get; private set;} = 0;
        public int numWhirlTowersPlaced {get; private set;} = 0;

        // Data level
        public bool didSucceedLevel {get; set;} = false;
        public int amountMoneyBegin {get; set;} = 0; // amount of money at the beginning of the level.
        public int amountMoneyEnd {get; set;} = 0; // amount of money at the ending of the level.

        private bool incrementEnemySpawnType(BaseEnemy.Typing enemyType){
            if (BaseEnemy.Typing.Earth == enemyType){
                this.numEarthEnemySpawn++;
                return true;
            } else if (BaseEnemy.Typing.Energy == enemyType){
                this.numEnergyEnemySpawn++;
                return true;
            } else if (BaseEnemy.Typing.Aquatic == enemyType){
                this.numAquaticEnemySpawn++;
                return true;
            } else if (BaseEnemy.Typing.Flight == enemyType){
                this.numFlightEnemySpawn++;
                return true;
            }
            return false;
        }

        public bool updateEnemySpawned(BaseEnemy.Typing enemyType){
            bool found = incrementEnemySpawnType(enemyType);
            if (found){
                this.numEnemyTotalSpawned++;
            }
            return false;
        }

        private bool incrementEnemyKilledType(BaseEnemy.Typing enemyType){
            // Debug.Log("Inside incrementEnemyKilledType()");
            // Debug.Log(enemyType);
            if (BaseEnemy.Typing.Earth == enemyType){
                this.numEarthEnemyKilled++;
                return true;
            } else if (BaseEnemy.Typing.Energy == enemyType){
                this.numEnergyEnemiesKilled++;
                return true;
            } else if (BaseEnemy.Typing.Aquatic == enemyType){
                this.numAquaticEnemyKilled++;
                return true;
            } else if (BaseEnemy.Typing.Flight == enemyType){
                this.numFlightEnemyKilled++;
                return true;
            }
            return false;
        }

        public bool updateEnemyKilled(BaseEnemy.Typing enemyType){
            bool found = incrementEnemyKilledType(enemyType);
            if (found){
                this.numEnemyTotalKilled++;
            }
            return false;
        } 

        private bool incrementTowerType(BaseTower.Elements towertype){
            if (BaseTower.Elements.Earth == towertype){
                this.numBulwarkTowersPlaced++;
                return true;
            } else if (BaseTower.Elements.Fire == towertype){
                this.numFireballTowersPlaced++;
                return true;
            } else if (BaseTower.Elements.Water == towertype){
                this.numTidalTowersPlaced++;
                return true;
            } else if (BaseTower.Elements.Air == towertype){
                this.numWhirlTowersPlaced++;
                return true;
            } else if(BaseTower.Elements.None == towertype){
                this.numBanishTowersPlaced++;
                return true;
            }
            return false;
        }

        public bool updateTowerPlaced(BaseTower.Elements towerTypePlaced){
            bool found = incrementTowerType(towerTypePlaced);
            if (found){
                this.numTowersPlacedTotal++;
            }
            return false;
        }

        // Assigns the name of the scene based off the scene name given.
        public LevelDataMetrics(){
        }

        // Converts the recorded level data in a string.
        public string AllDataString(){
            string levelName = "Level name: " + this.levelName.ToString() + "\n";
            string beginMoney = "Money at start of level = " + this.amountMoneyBegin.ToString() + "\n";
            string endMoney = "Money at end of level = " + this.amountMoneyEnd.ToString() + "\n";
            string didWin = "Level won = " + this.didSucceedLevel.ToString() + "\n";
            string playerWaveSurvive = "Player survived " + this.currWave.ToString() + " waves out of " + this.totalWaves + " total waves. \n";
            string levelStats = levelName + beginMoney + endMoney + didWin + playerWaveSurvive;

            string totalTowersPlaced = "Number of towers placed total: " + this.numTowersPlacedTotal.ToString() + "\n";
            string totalBulwark = "Number of Bulwark Towers placed: " + this.numBulwarkTowersPlaced.ToString() + "\n";
            string totalBanish = "Number of Banishment Towers placed: " + this.numBanishTowersPlaced.ToString() + "\n";
            string totalFireball = "Number of Fireball Towers placed: " + this.numFireballTowersPlaced.ToString() + "\n";
            string totalWhirl = "Number of Whirlwind Towers placed: " + this.numWhirlTowersPlaced.ToString() + "\n";
            string totalTidal = "Number of Tidal Towers placed: "  + this.numTidalTowersPlaced.ToString() + "\n";
            string towerStats = totalTowersPlaced + totalBulwark + totalBanish + totalFireball + totalWhirl + totalTidal;

            string totalSpawnEnemies = "Number of total spawned enemies: "  + this.numEnemyTotalSpawned.ToString() + "\n";
            string totalKilledEnemies = "Number of total killed enemies: " + this.numEnemyTotalSpawned.ToString() + "\n";
            string aquaticEnemyData = "Number of spawned Aquatic enemies: " + this.numAquaticEnemySpawn.ToString() + ". Number of them killed: " + this.numAquaticEnemyKilled.ToString() + "\n";
            string earthEnemyData = "Number of spawned Earth enemies: " + this.numEarthEnemySpawn.ToString() + ". Number of them killed: " + this.numEarthEnemyKilled.ToString() + "\n";
            string energyEnemyData = "Number of spawned Energy enemies: " + this.numEnergyEnemySpawn.ToString() + ". Number of them killed: " + this.numEnergyEnemiesKilled.ToString() +  "\n";
            string flightEnemyData = "Number of spawned Flight enemies: " + this.numFlightEnemySpawn.ToString() + ". Number of them killed: " + this.numFlightEnemyKilled.ToString() + "\n";
            string enemyData = totalSpawnEnemies + totalKilledEnemies + aquaticEnemyData + earthEnemyData + energyEnemyData + flightEnemyData;
            return levelStats + towerStats + enemyData;
        
        }

        public void debugPrintAllCurrData(){
            Debug.Log("Level name: " + this.levelName.ToString());
            Debug.Log("Number of towers placed total: " + this.numTowersPlacedTotal.ToString());
            Debug.Log("Number of Bulwark Towers placed: " + this.numBulwarkTowersPlaced.ToString());
            Debug.Log("Number of Banishment Towers placed: " + this.numBanishTowersPlaced.ToString());
            Debug.Log("Number of Fireball Towers placed: " + this.numFireballTowersPlaced.ToString());
            Debug.Log("Number of Whirlwind Towers placed: " + this.numWhirlTowersPlaced.ToString());
            Debug.Log("Number of Tidal Towers placed: " + this.numTidalTowersPlaced.ToString());

            Debug.Log("Number of total spawned enemies: " + this.numEnemyTotalSpawned);
            Debug.Log("Number of total killed enemies: " + this.numEnemyTotalKilled);
            Debug.Log("Number of spawned Aquatic enemies: " + this.numAquaticEnemySpawn + ". Number of them killed: " + this.numAquaticEnemyKilled);
            Debug.Log("Number of spawned Flight enemies: " + this.numFlightEnemySpawn + ". Number of them killed: " + this.numFlightEnemyKilled);
            Debug.Log("Number of spawned Energy enemies: " + this.numEnergyEnemySpawn + ". Number of them killed: " + this.numEnergyEnemiesKilled);
            Debug.Log("Number of spawned Earth enemies: " + this.numEarthEnemySpawn + ". Number of them killed: " + this.numEarthEnemyKilled);
        }

    }

    public static PlayerDataManager Instance { get; private set; }
    private string directoryFile;
    private StreamWriter stream;
    private string textFileName = "Elemental Siege Demo Session - " + DateTime.UtcNow.ToString("MM-dd-yyyy"); // Intialize connection to the textfile in the directory.
    private string dateTimeCreated = DateTime.UtcNow.ToString("MM-dd-yyyy HH:mm:ss") + " UTC"; // records the date and time.
    private string jsonString = "Elemental Siege Session - " + DateTime.UtcNow.ToString("MM-dd-yyyy HH:mm:ss") + " UTC" + "\n\n\n";
    public bool transitionStatus = false; // indicates if the level is being attempted to be left. True if it is, false otherwise.
    private LevelDataMetrics currentLevel;

    public void printAll(){
        currentLevel.debugPrintAllCurrData();
    }

    public int loadSavedMoney(){
        return PlayerPrefs.GetInt("PlayerMoney", 1000);
    }

    public void updateTotalTowersPlaced(BaseTower.Elements towerTypePlaced){
        this.currentLevel.updateTowerPlaced(towerTypePlaced);
    }

    public void updateEnemiesKilled(BaseEnemy.Typing enemyKilledType){
        this.currentLevel.updateEnemyKilled(enemyKilledType);
    }

    public void updateEnemiesSpawned(BaseEnemy.Typing enemySpawnedType){
        this.currentLevel.updateEnemySpawned(enemySpawnedType);
    }

    // Saves progress to player prefabs and to a text log file upon level swtich.
    private void SaveProgress(){

        // Debug.Log("SaveProgress() in action.");
        
        // Save data to the Player Prefs.
        int endMoney = 0;
        if (EnemyManager.Instance != null && EnemyManager.Instance.GetWallet() != null)
        {
            // Enables loading money for the next level.
            endMoney = EnemyManager.Instance.GetWallet().GetCurrentBalance();
            PlayerPrefs.SetInt("PlayerMoney", EnemyManager.Instance.GetWallet().GetCurrentBalance());
        }
        else
        {
            endMoney = 0;
        }
            
        this.currentLevel.amountMoneyEnd = endMoney;

        // Obtain win status and number of waves succeeded.
        if (LevelManager.Instance != null){
            this.currentLevel.didSucceedLevel = LevelManager.Instance.WinStatusLevel();
            this.currentLevel.currWave = (LevelManager.Instance.getCurrentWaveNumber()) - 1;
            this.currentLevel.totalWaves = LevelManager.Instance.getTotalNumberWaves();
        }

        // Log all data.
        this.jsonString = this.jsonString + this.currentLevel.AllDataString();
        PlayerPrefs.SetString("SaveLogJson", this.jsonString);
        // Debug.Log("SaveLogJson:\n" + PlayerPrefs.GetString("SaveLogJson", "N/A"));
        
        // Write data to the text log file.
        try
        {
            // If the file exists and is found in the directory...
            if (File.Exists(this.directoryFile)){

                using (this.stream = new StreamWriter(this.directoryFile, true)){
                    // Debug.Log(this.currentLevel.AllDataString());
                    this.stream.WriteLine(this.currentLevel.AllDataString());
                }

                // Debug.Log("Log file was found.");

            } else {

                // Create the file.
                using(this.stream = new StreamWriter(this.directoryFile, true)){
                    // Debug.Log(this.currentLevel.AllDataString());
                    this.stream.WriteLine(this.currentLevel.AllDataString());
                }

                // Debug.Log("Log file was not found. A new one was created instead.");
            }
        }   
        catch (System.Exception)
        {
            Debug.LogWarning("File writing went wrong.");
        }

        // Reset level data.
        this.currentLevel = new LevelDataMetrics();
        this.currentLevel.amountMoneyBegin = endMoney;
        this.transitionStatus = false;

    }

    void OnApplicationQuit(){
        
        SaveProgress();
        // Debug.Log("Releasing stream upon application exit.");

    }

    void Awake()
    {
        if (PlayerDataManager.Instance != null && PlayerDataManager.Instance != this)
        {
            Destroy(this);
        }
        else
        {
           PlayerDataManager.Instance = this;
        }
    }

    void Start(){
        
        // Initialize the data fields stored in PlayerPrefs
        PlayerPrefs.SetInt("PlayerMoney", 1000);
        PlayerPrefs.SetInt("TutorialComplete", 0);
        PlayerPrefs.SetString("SaveLogJson", "");
        this.currentLevel = new LevelDataMetrics();
        this.currentLevel.levelName = SceneManager.GetActiveScene().name;

        // Initialize save data.
        if (PlayerPrefs.GetInt("TutorialComplete", 0) == 1){
            this.currentLevel.amountMoneyBegin = PlayerPrefs.GetInt("PlayerMoney", 1000);        
        } else {
            this.currentLevel.amountMoneyBegin = 1000;
        }
        this.currentLevel.amountMoneyBegin = PlayerPrefs.GetInt("PlayerMoney", 1000);

        // Initialize the stream.
        try
        {
            string dataDirectory = Application.dataPath + Path.DirectorySeparatorChar + "gamelogs";
            this.directoryFile = dataDirectory + Path.DirectorySeparatorChar + this.textFileName + ".txt";

            // Create the data directory if it doesn't exist
            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
            }

            // If the file exists and is found in the directory...
            if (File.Exists(this.directoryFile)){

                using (this.stream = new StreamWriter(this.directoryFile, true)){
                    this.stream.WriteLine(jsonString);
                }

                // Debug.Log("Log file was found.");

            } else {

                // Create the file.
                using(this.stream = new StreamWriter(this.directoryFile, true)){
                    this.stream.WriteLine(jsonString);
                }

                // Debug.Log("Log file was not found. A new one was created instead.");
            }
        }   
        catch (System.Exception e)
        {
            Debug.LogError("File writing went wrong: " + e.Message);
        }

    }

    void Update(){

        // Determine if the manager is the in the current set scene or a new scene.
        if (transitionStatus) SaveProgress();

    }

}