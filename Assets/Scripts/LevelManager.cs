using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] protected GameObject waveToSpawn;
    protected GameObject spawnedWave;
    [SerializeField] protected int totalWaves; 
    public int currentWave { get; protected set; }

    public List<EnemySpawner> enemySpawners { get; protected set; }
    public int numberOfEnemies { get; protected set; }
    public int numberOfRemainEnemies {get; protected set;} // indicates the number of remaining enemies in the current wave.

    public Stronghold stronghold;
    private bool levelInProgress = true; // indicates the status of the level. True if the level is in progress, false if the level has ended.
    private bool levelWinStatus = false; // indicates if the level has been won or lost. True if the game has won, false otherwise.
    public static LevelManager Instance { get; private set; }

    protected PlayerWallet playerWallet;

    protected float waveTotalHealth;
    [SerializeField] protected float waveRewardFactor = 1.0f;

    //max % of enemies weight thatll be added/subtracted in each adaptation
    [SerializeField] protected float waveAdaptationConstant = 0.5f;

    //holds average damage of enemy type
    protected Dictionary<BaseEnemy.Typing, float> damageTable = new Dictionary<BaseEnemy.Typing, float>();
    //holds average closest distance of each enemy type
    protected Dictionary<BaseEnemy.Typing, float> distanceTable = new Dictionary<BaseEnemy.Typing, float>();
    //holds the spawn chance weight of each type
    protected Dictionary<BaseEnemy.Typing, int> weightTable = new Dictionary<BaseEnemy.Typing, int>();

    //weight ranking greater weight, greater rank [1,4]
    protected Dictionary<BaseEnemy.Typing, int> weightRank = new Dictionary<BaseEnemy.Typing, int>();

    //enemy ranking better enemy, better rank [1,4]
    protected Dictionary<BaseEnemy.Typing, int> enemyRank = new Dictionary<BaseEnemy.Typing, int>();

    protected List<BaseEnemy.Typing> enemyTypes = new List<BaseEnemy.Typing>();

    [SerializeField] protected List<int> baseWeights = new List<int>();

    [SerializeField] protected int currentLevel = 1;
    [SerializeField] List<float> levelDifficulties = new List<float>();

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
        this.numberOfEnemies = 0;
        this.enemySpawners = new List<EnemySpawner>();
        this.currentWave = 0;

        //init weight table
        this.weightTable[BaseEnemy.Typing.Earth] = baseWeights[0];
        this.weightTable[BaseEnemy.Typing.Energy] = baseWeights[1];
        this.weightTable[BaseEnemy.Typing.Aquatic] = baseWeights[2];
        this.weightTable[BaseEnemy.Typing.Flight] = baseWeights[3];

        this.enemyTypes.Add(BaseEnemy.Typing.Earth);
        this.enemyTypes.Add(BaseEnemy.Typing.Energy);
        this.enemyTypes.Add(BaseEnemy.Typing.Aquatic);
        this.enemyTypes.Add(BaseEnemy.Typing.Flight);

    }
    // Start is called before the first frame update
    void Start()
    {
        StartNewWave();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public int getCurrentWaveNumber(){
        return this.currentWave;
    }

    public int getTotalNumberWaves(){
        return this.totalWaves;
    }

    private void StartNewWave()
    {
        if(totalWaves == currentWave)
        {
            WinLevel();
        }
        else if (this.waveToSpawn != null)
        {
            if (this.currentWave > 0)
            {
                //give wave reward
                GiveWaveReward();
                AdaptWave();
            }
            //Iterate wave number
            this.currentWave++;
            Debug.Log("Starting wave " + this.currentWave);
            //Create BaseWave Object 
            this.spawnedWave = Instantiate(this.waveToSpawn, this.transform);
            InitializeEnemyTables();
        }
    }


    public void RegisterSpawner(EnemySpawner script)
    {
        this.enemySpawners.Add(script);
        var index = Math.Max(this.currentLevel - 1, 0);
        script.levelDifficulty = this.levelDifficulties[index];
    }

    public void IncreaseEnemyCount()
    {
        this.numberOfEnemies++;
    }

    public void DecreaseEnemyCount()
    {
        this.numberOfEnemies--;
        //Wave ended -> start next one
        if (numberOfEnemies <= 0)
        {
            Destroy(this.spawnedWave);
            StartNewWave();
        }
    }

    protected void GiveWaveReward()
    {
        if(playerWallet == null) {return; }
        int reward = (int)Mathf.RoundToInt(Mathf.Sqrt(this.waveTotalHealth) * this.waveRewardFactor);
        playerWallet.CollectMoney(reward);
        Debug.Log("Giving Wave Reward: $" + reward);
    }

    public void HaltWave()
    {
        foreach(EnemySpawner spawner in this.enemySpawners)
        {
            spawner.SetSpawnerCannotSpawn();
        }
    }

    public bool WinStatusLevel(){
        // Debug.Log("Level won = " + this.levelWinStatus);
        return this.levelWinStatus;
    }

    public bool ProgressStatusLevel(){
        // Debug.Log("Level completed = " + this.levelInProgress);
        return this.levelInProgress;
    }

    // Indicates if the level has been lost.
    public void LoseLevel()
    {
        HaltWave();
        // Debug.Log("Level Lost");
        //Other Losing Stuff
        this.levelInProgress = false;

        // Signal this to the PlayerDataManager that the level has ended.
        // if (PlayerDataManager.Instance != null) PlayerDataManager.Instance.transitionStatus = true;

    }

    // Inidicates if the level has been won.
    public void WinLevel()
    {
        HaltWave();
        // Debug.Log("Level Won");
        //Other Win Stuff
        PlayerPrefs.SetInt("TutorialComplete", 1); // indicate in the save that the tutorial level has been completed.
        this.levelInProgress = false;
        this.levelWinStatus = true;

        // Signal this to the PlayerDataManager that the level has ended.
        // if (PlayerDataManager.Instance != null) PlayerDataManager.Instance.transitionStatus = true;

    }

    public void SetWallet(PlayerWallet playerWallet)
    {
        this.playerWallet = playerWallet;
    }

    public void SetWaveTotalHealth (float waveTotalHealth)
    {
        this.waveTotalHealth = waveTotalHealth;
        Debug.Log("Wave Total health set to " + this.waveTotalHealth);
    }

    public void LogEnemyTypeDamage(float damage, BaseEnemy.Typing type)
    {
        
        if (this.damageTable[type] > 0)
        {
            this.damageTable[type] += damage;
            this.damageTable[type] /= 2;
        }
        else
        {
            this.damageTable[type] = damage;
        }
        
        return;
    }

    public void LogEnemyTypeDistance(float distance, BaseEnemy.Typing type)
    {
        if(this.distanceTable[type] > 0)
        {
            this.distanceTable[type] += distance;
            this.distanceTable[type] /= 2;
        }
        else
        {
            this.distanceTable[type] = distance;
        }

        return;
    }

    protected void AdaptWave()
    {
        //Populates rank dictionaries with info
        DebugPrintEnemyDamage();
        DebugPrintEnemyDistance();
        RankEnemyTypes();
        DebugPrintEnemyRanks();
        //Apply weighting algorthim to all enemy types
        foreach (BaseEnemy.Typing type in this.enemyTypes)
        {
            //factor for choosing the stength of weight change based on how "good" they are supposed to be compared to how well the actually did compared to other enemies
            int adaptationFactor = this.enemyRank[type] - this.weightRank[type];
            float weightPercentage = (float) this.weightTable[type]/TotalWeight();
            Debug.Log(type + " wP:" + weightPercentage);

            //different formulas for +/-/0 factors
            if (adaptationFactor > 0)
            {
                // favors enemies that do better then their weight predicted
                this.weightTable[type] += Mathf.RoundToInt(adaptationFactor * (1-weightPercentage) * (this.waveAdaptationConstant * this.weightTable[type]));
            }
            else if(adaptationFactor < 0)
            {
                // does not favor enemies that do worse than their weight predicted
                this.weightTable[type] -= Mathf.RoundToInt((-adaptationFactor) * (weightPercentage) * (this.waveAdaptationConstant * this.weightTable[type]));
                Debug.Log(type + ": weight lowered by" + ((-adaptationFactor) * (weightPercentage) * (this.waveAdaptationConstant * this.weightTable[type])) + " aF: " + (-adaptationFactor) + " wP: " + weightPercentage + " aC: " + this.waveAdaptationConstant + " weight: " + this.weightTable[type]);
            }
            //when enemies do as well as predicted -> still encourage a bit of change
            else if (this.weightRank[type] < 2)
            {
                // enemies that are doing bad will still get lowered weights, but not by much
                this.weightTable[type] -= Mathf.RoundToInt((this.weightRank[type] + 1) * (weightPercentage) * (this.waveAdaptationConstant * this.weightTable[type]));
            }
            else
            {
                // enemies that are doing well get increased weights but, still not by much
                this.weightTable[type] += Mathf.RoundToInt((4-this.weightRank[type]) * (1-weightPercentage) * (this.waveAdaptationConstant * this.weightTable[type]));
            }
        }
        Debug.Log("Earth: " + this.weightTable[BaseEnemy.Typing.Earth] + ", Flight: " + this.weightTable[BaseEnemy.Typing.Flight] + ", Aquatic: " + this.weightTable[BaseEnemy.Typing.Aquatic] + ", Energy: " + this.weightTable[BaseEnemy.Typing.Energy]);
    }

    protected void RankEnemyTypes()
    {
       
        //init list
        List<BaseEnemy.Typing> enemyRankList = new List<BaseEnemy.Typing>(this.weightTable.Keys);
            // List<BaseEnemy.Typing>();

        // sort list to get ranks
        enemyRankList.Sort((a, b) => CompareEnemyTypes(a, b));

        //if weights are all the same, set them all to rank 1 so enemies that do better get a more substantial increase in weight
        if (this.currentWave == 0 || (weightTable[BaseEnemy.Typing.Earth] == weightTable[BaseEnemy.Typing.Flight] && weightTable[BaseEnemy.Typing.Earth] == weightTable[BaseEnemy.Typing.Aquatic] && weightTable[BaseEnemy.Typing.Earth] == weightTable[BaseEnemy.Typing.Energy]))
        {
            weightRank[BaseEnemy.Typing.Earth] = 1;
            weightRank[BaseEnemy.Typing.Flight] = 1;
            weightRank[BaseEnemy.Typing.Aquatic] = 1;
            weightRank[BaseEnemy.Typing.Energy] = 1;
            for (int i = 0; i < enemyRankList.Count; i++)
            {
                enemyRank[enemyRankList[i]] = i;
            }
            return;
        }

        List<BaseEnemy.Typing> weightRankList = new List<BaseEnemy.Typing>(this.weightTable.Keys);

        weightRankList.Sort((a, b) => CompareEnemyTypeWeights(a, b));

        //mark ranks on tables
        for (int i = 0; i < enemyRankList.Count; i++)
        {
            enemyRank[enemyRankList[i]] = i;
            weightRank[weightRankList[i]] = i;
        }
    }

    protected int CompareEnemyTypes(BaseEnemy.Typing a, BaseEnemy.Typing b)
    {
        //sort by damage, then closest distance, then weight -> higher weighted type is 'less' than lower weighted type
        if (damageTable[a] > damageTable[b]) 
            return 1;
        else if (damageTable[a] < damageTable[b]) 
            return -1;
        else if (distanceTable[a] < distanceTable[b])
            return 1;
        else if (distanceTable[a] > distanceTable[b])
            return -1;
        else if (weightTable[a] < weightTable[b])
            return 1;
        else
            return -1;
    }

    protected int CompareEnemyTypeWeights(BaseEnemy.Typing a, BaseEnemy.Typing b)
    {
        if (weightTable[a] > weightTable[b]) 
            return 1;
        else if (weightTable[a] < weightTable[b]) 
            return -1;
        else 
            return 0;
    }

    public int TotalWeight()
    {
        int total = 0;

        foreach (int w in this.weightTable.Values) { 
            total += w;
        }

        return total;
    }

    protected void InitializeEnemyTables()
    {
        //initalize damage table
        this.damageTable[BaseEnemy.Typing.Earth] = 0;
        this.damageTable[BaseEnemy.Typing.Energy] = 0;
        this.damageTable[BaseEnemy.Typing.Aquatic] = 0;
        this.damageTable[BaseEnemy.Typing.Flight] = 0;

        //init distance table
        this.distanceTable[BaseEnemy.Typing.Earth] = 0;
        this.distanceTable[BaseEnemy.Typing.Energy] = 0;
        this.distanceTable[BaseEnemy.Typing.Aquatic] = 0;
        this.distanceTable[BaseEnemy.Typing.Flight] = 0;

    }

    public Dictionary<BaseEnemy.Typing, int> GetWeightTable()
    {
        return this.weightTable;
    }

    protected void DebugPrintEnemyRanks()
    {
        Debug.Log("Earth: " + this.enemyRank[BaseEnemy.Typing.Earth] + ", Flight: " + this.enemyRank[BaseEnemy.Typing.Flight] + ", Aquatic: " + this.enemyRank[BaseEnemy.Typing.Aquatic] + ", Energy: " + this.enemyRank[BaseEnemy.Typing.Energy]);
    }
    protected void DebugPrintEnemyDamage()
    {
        Debug.Log("Earth: " + this.damageTable[BaseEnemy.Typing.Earth] + ", Flight: " + this.damageTable[BaseEnemy.Typing.Flight] + ", Aquatic: " + this.damageTable[BaseEnemy.Typing.Aquatic] + ", Energy: " + this.damageTable[BaseEnemy.Typing.Energy]);
    }
    protected void DebugPrintEnemyDistance()
    {
        Debug.Log("Earth: " + this.distanceTable[BaseEnemy.Typing.Earth] + ", Flight: " + this.distanceTable[BaseEnemy.Typing.Flight] + ", Aquatic: " + this.distanceTable[BaseEnemy.Typing.Aquatic] + ", Energy: " + this.distanceTable[BaseEnemy.Typing.Energy]);
    }

}
