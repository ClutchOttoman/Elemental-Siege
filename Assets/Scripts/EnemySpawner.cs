using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : BaseTower
{
    protected Collider EnemySpawnPoint;
    [SerializeField] protected Transform FlyingEnemySpawnPoint;

    public Queue<BaseEnemy> spawnQueue { get; protected set; }
    protected bool canSpawn = false;
    protected float spawnInterval;
    protected int currentWave = 0;
    public float levelDifficulty;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        EnemySpawnPoint = this.projectileSpawnPoint.GetComponent<SphereCollider>();
        LevelManager.Instance.RegisterSpawner(this);
    }

    // Update is called once per frame
    protected override void Update()
    {
        if(this.currentWave != LevelManager.Instance.currentWave)
        {
            this.currentWave = LevelManager.Instance.currentWave;
            this.spawnInterval = ((this.attackInterval + 1) * this.levelDifficulty) / (LevelManager.Instance.currentWave + 1);
            // Debug.Log("spawnInterval for wave " + this.currentWave + ": " + this.spawnInterval);
        }
        

        if (spawnQueue != null)
        {
            if (this.canSpawn && this.spawnQueue.Count > 0 && (this.timeAtLastAttack + this.spawnInterval <= TowerManager.Instance.towerTimer))
            {
                BaseEnemy enemyToSpawn = this.spawnQueue.Dequeue();
                //Debug.Log("Spawning " + enemyToSpawn + " at " + this.gameObject);

                PlayerDataManager.Instance.updateEnemiesSpawned(enemyToSpawn.GetEnemyTyping()); // alert the PlayerDataManager.
                //PlayerDataManager.Instance.printAll();
                Spawn(enemyToSpawn);

            }
            if (spawnQueue.Count <= 0)
            {
                this.canSpawn = false;
            }
        }
    }
    //Spawner override -> can attack every interval
    protected bool Spawn(BaseEnemy enemyToSpawn)
    {
        BaseEnemy enemy;
        if (enemyToSpawn.GetEnemyTyping() == BaseEnemy.Typing.Flight)
        {
           enemy = Instantiate(enemyToSpawn, this.FlyingEnemySpawnPoint.position, this.transform.rotation);
        }
        else 
        {
            enemy = Instantiate(enemyToSpawn, this.EnemySpawnPoint.transform);
            enemy.transform.localScale = enemy.transform.localScale * 10;
        }
        //Debug.Log("Attempting to Spawn");
       
        if (enemy == null)
        {
            Debug.LogWarning("Failed to Instantiate Enemy");
            return false;
        }
   

        
        //Debug.Log("Enemy Scale: " + enemy.transform.localScale);
        this.timeAtLastAttack = TowerManager.Instance.towerTimer;
        return true;
    }
    //Spawn Enemy at target location

    public void SetSpawnerQueue(Queue<BaseEnemy> newSpawnQueue)
    {
        //Debug.Log("Setting " + this.gameObject + "'s spawn queue");
        this.spawnQueue = newSpawnQueue;
    }

    public void SetSpawnerCanSpawn()
    {
        this.canSpawn = true;
    }

    public void SetSpawnerCannotSpawn()
    {
        this.canSpawn = false;
    }

}