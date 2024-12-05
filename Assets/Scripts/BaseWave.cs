using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWave : MonoBehaviour
{
    protected float waveStartTime;
    [SerializeField] protected float waveStartInterval = 10.0F;

    protected int numberOfSpawners;

    public bool waveReady = false;

    protected bool waveStarted = false;

    protected float maxTotalHealth;
    [SerializeField] protected float waveHealthMultiplier = 240.0F;

    // Start is called before the first frame update
    void Start()
    {
        waveStartTime = TowerManager.Instance.towerTimer + waveStartInterval;
        numberOfSpawners = LevelManager.Instance.enemySpawners.Count;

        // Debug.Log("Wave Starting: " + (waveStartTime));
        //waveReady = GenerateBaseEnemyWave();
        waveReady = GenerateWeightedWave();
    }

    // Update is called once per frame
    void Update()
    {
        numberOfSpawners = LevelManager.Instance.enemySpawners.Count;
        if (this.numberOfSpawners == 0)
        {
            return;
        }
        if (TowerManager.Instance.towerTimer >= waveStartTime && !waveStarted && waveReady)
        {
            // Debug.Log("Wave Started at: " + TowerManager.Instance.towerTimer);
            for (int i = 0; i < numberOfSpawners; i++)
            {
                LevelManager.Instance.enemySpawners[i].SetSpawnerCanSpawn();
            }
            waveStarted = true;
        }
    }
    //generate wave and split it across all spawners
    protected virtual bool GenerateWave()
    {
        float maxTotalHealth = LevelManager.Instance.currentWave * 120.0F;
        List<Queue<BaseEnemy>> spawnQueues = new List<Queue<BaseEnemy>>();
        return true;
    }

    //Gives the spawners their queue of enemies to spawn
    protected void SetSpawnerQueues(List<Queue<BaseEnemy>> spawnQueues)
    {
        for (int i = 0; i < numberOfSpawners; i++) {
            LevelManager.Instance.enemySpawners[i].SetSpawnerQueue(spawnQueues[i]);
        }
    }

    //For inital testing of generating waves of just base enemies
    public bool GenerateBaseEnemyWave()
    {
        //Debug.Log("Generating Base Enemy Wave");
        maxTotalHealth = LevelManager.Instance.currentWave * waveHealthMultiplier;
        List<Queue<BaseEnemy>> spawnQueues = new List<Queue<BaseEnemy>>();
        for (int i = 0; i < numberOfSpawners; i++) {
            spawnQueues.Add(new Queue<BaseEnemy>());
        }

        float waveTotalHealth = 0;
        while(waveTotalHealth < maxTotalHealth)
        {
            for (int i = 0; i < numberOfSpawners; i++) {
                if (waveTotalHealth + EnemyManager.Instance.GetEnemyHealth(0) > maxTotalHealth)
                {
                    SetSpawnerQueues(spawnQueues);
                    return true;
                }
                LevelManager.Instance.IncreaseEnemyCount();
                waveTotalHealth += EnemyManager.Instance.GetEnemyHealth(0);
                spawnQueues[i].Enqueue(EnemyManager.Instance.GetEnemyPrefab(0));
            }
        }
        SetSpawnerQueues(spawnQueues);
        return true;
    }

    public bool GenerateEvenDistributionWave()
    {
        int numberOfEnemyTypes = EnemyManager.Instance.GetCountOfEnemyTypes() - 1; //- 1 beacuse base enemy is in list at index 0
        maxTotalHealth = (LevelManager.Instance.currentWave + 1) * (LevelManager.Instance.currentWave + 1) * waveHealthMultiplier;
        List<Queue<BaseEnemy>> spawnQueues = new List<Queue<BaseEnemy>>();
        for (int i = 0; i < numberOfSpawners; i++)
        {
            spawnQueues.Add(new Queue<BaseEnemy>());
        }

        float waveTotalHealth = 0;
        while (waveTotalHealth < maxTotalHealth)
        {
            for (int i = 0; i < numberOfSpawners; i++)
            {
                // R + 1 beacuse base enemy is in list at index 0 and we dont want those to spawn
                var R = Random.Range(0,numberOfEnemyTypes) + 1;
                //Debug.Log("attempting to add Enemy: " + R + " to queue");
                if (waveTotalHealth + EnemyManager.Instance.GetEnemyHealth(R) > maxTotalHealth)
                {
                    LevelManager.Instance.SetWaveTotalHealth(waveTotalHealth);
                    SetSpawnerQueues(spawnQueues);
                    return true;
                }
                LevelManager.Instance.IncreaseEnemyCount();
                waveTotalHealth += EnemyManager.Instance.GetEnemyHealth(R);
                spawnQueues[i].Enqueue(EnemyManager.Instance.GetEnemyPrefab(R));
            }
        }
        LevelManager.Instance.SetWaveTotalHealth(waveTotalHealth);
        SetSpawnerQueues(spawnQueues);
        return true;

    }

    public bool GenerateWeightedWave()
    {
        // Debug.Log("Starting weighted wave generation");
        int startOfRange = 1, endOfRange=0, temp;
        //convert table into table of range starts for random picking
        Dictionary<BaseEnemy.Typing, int> weightTable = LevelManager.Instance.GetWeightTable();
        int totalWeight = LevelManager.Instance.TotalWeight();

        Dictionary<BaseEnemy.Typing, int> weightRangeTable = new Dictionary<BaseEnemy.Typing, int>();
        foreach (BaseEnemy.Typing type in weightTable.Keys)
        {
            endOfRange += weightTable[type];
            temp = weightTable[type];
            weightRangeTable[type] = startOfRange;
            startOfRange += temp;
        }
        // Debug.Log("Weight Ranges: " + weightRangeTable);

        int numberOfEnemyTypes = EnemyManager.Instance.GetCountOfEnemyTypes() - 1; //- 1 beacuse base enemy is in list at index 0
        maxTotalHealth = (LevelManager.Instance.currentWave + 1) * (LevelManager.Instance.currentWave + 1) * waveHealthMultiplier;
        List<Queue<BaseEnemy>> spawnQueues = new List<Queue<BaseEnemy>>();
        for (int i = 0; i < numberOfSpawners; i++)
        {
            spawnQueues.Add(new Queue<BaseEnemy>());
        }
        

        float waveTotalHealth = 0;
        while (waveTotalHealth < maxTotalHealth)
        {
            for (int i = 0; i < numberOfSpawners; i++)
            {
                //random choice from weights
                int R = Random.Range(1, totalWeight+1);
                BaseEnemy.Typing typeToSpawn = GetChoice(weightRangeTable, R);
                // Debug.Log("randomly chose to add Enemy: " + typeToSpawn + " to queue");

                if (waveTotalHealth + EnemyManager.Instance.GetEnemyHealth(typeToSpawn) > maxTotalHealth)
                {
                    LevelManager.Instance.SetWaveTotalHealth(waveTotalHealth);
                    SetSpawnerQueues(spawnQueues);
                    return true;
                }
                LevelManager.Instance.IncreaseEnemyCount();
                waveTotalHealth += EnemyManager.Instance.GetEnemyHealth(typeToSpawn);
                spawnQueues[i].Enqueue(EnemyManager.Instance.GetEnemyPrefab(typeToSpawn));
            }
        }
        LevelManager.Instance.SetWaveTotalHealth(waveTotalHealth);
        SetSpawnerQueues(spawnQueues);
        return true;
    }

    protected BaseEnemy.Typing GetChoice(Dictionary<BaseEnemy.Typing, int> weightRangeTable, int choice)
    {
        List<BaseEnemy.Typing> sortedByRangeStart = new List<BaseEnemy.Typing>(weightRangeTable.Keys);
        sortedByRangeStart.Sort((a, b) =>
        {
            if (weightRangeTable[a] < weightRangeTable[b]) return -1;
            else if (weightRangeTable[a] > weightRangeTable[b]) return 1;
            else return 0;
        });

        //pick type that choice is in range of
        if (choice < weightRangeTable[sortedByRangeStart[1]])
        {
            return sortedByRangeStart[0];
        }
        else if (choice < weightRangeTable[sortedByRangeStart[2]])
        {
            return sortedByRangeStart[1];
        }
        else if (choice < weightRangeTable[sortedByRangeStart[3]])
        {
            return sortedByRangeStart[2];
        }
        else
        {
            return sortedByRangeStart[3];
        }
    }

}
