using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    //Live lists of all active enemies
    protected Dictionary<Transform, BaseEnemy> liveEnemyMap = new Dictionary<Transform, BaseEnemy>();
    protected Dictionary<Transform, UnityEngine.AI.NavMeshAgent> pathMap = new Dictionary<Transform, UnityEngine.AI.NavMeshAgent>();

    //Need to MANUALLY add any new enemies' prefabs & health to these lists for them to be able to spawn
    [SerializeField] protected List<BaseEnemy> EnemyList = new List<BaseEnemy>();
    [SerializeField] protected List<float> EnemyHealthList = new List<float>();
    protected Dictionary<BaseEnemy.Typing, int> typeToIndexTable = new Dictionary<BaseEnemy.Typing, int>();

    protected PlayerWallet playerWallet;

    public Stronghold stronghold;

    public bool isUI = false;

    void Awake()
    {
        InitializeTypeToIndexTable();
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

    public bool RemoveTransform(Transform transform)
    {
        bool removedFromliveEnemyMap = liveEnemyMap.Remove(transform);
        bool removedFromPathMap = pathMap.Remove(transform);
        if(removedFromliveEnemyMap && removedFromPathMap) { return true; }
        else { return false; }

    }

    public void AddEnemy(Transform transform, BaseEnemy enemy)
    {
        liveEnemyMap[transform] = enemy;
        //Debug.Log("added enemy: " + enemy.gameObject);
    }

    public BaseEnemy GetEnemy(Transform transform)
    {
        if (!liveEnemyMap.TryGetValue(transform, out BaseEnemy enemy))
        {
            enemy = transform.gameObject.GetComponent<BaseEnemy>();
            if (enemy != null)
            {
                AddEnemy(transform, enemy);
            }
        }
        return enemy;
    }

    public void AddPath(Transform transform, UnityEngine.AI.NavMeshAgent agent)
    {
        pathMap[transform] = agent;
    }
    public UnityEngine.AI.NavMeshAgent GetPath(Transform transform)
    {
        if (!pathMap.TryGetValue(transform, out UnityEngine.AI.NavMeshAgent agent))
        {
            agent = transform.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null)
            {
                AddPath(transform, agent);
            }
        }
        return agent;
    }
    public BaseEnemy GetEnemyPrefab(int i)
    {
        if(i >= EnemyList.Count) { return null; }
        return EnemyList[i];
    }
    public BaseEnemy GetEnemyPrefab(BaseEnemy.Typing type)
    {
        int i = typeToIndexTable[type];
        if (i >= EnemyList.Count) { return null; }
        return EnemyList[i];
    }
    public float GetEnemyHealth(int i)
    {
        if (i >= EnemyHealthList.Count) { return 100000; }
        return EnemyHealthList[i];
    }
    public float GetEnemyHealth(BaseEnemy.Typing type)
    {
        int i = typeToIndexTable[type];
        if(i >= EnemyHealthList.Count) { return 100000; }
        return EnemyHealthList[i];
    }
    protected void InitializeTypeToIndexTable()
    {
        typeToIndexTable[BaseEnemy.Typing.Earth] = 2;
        typeToIndexTable[BaseEnemy.Typing.Flight] = 1;
        typeToIndexTable[BaseEnemy.Typing.Aquatic] = 4;
        typeToIndexTable[BaseEnemy.Typing.Energy] = 3;

    }

    public int GetCountOfEnemyTypes() { return EnemyList.Count; }
    public void SetWallet(PlayerWallet playerWallet)
    {

        this.playerWallet = playerWallet;
    }
    public PlayerWallet GetWallet()
    {
        return this.playerWallet;
    }
}
