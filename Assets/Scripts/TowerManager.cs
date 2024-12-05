using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance { get; private set; }
    public float towerTimer { get; private set; }

    [SerializeField] protected List<GameObject> TowerList = new List<GameObject>();
    [SerializeField] protected List<GameObject> UnplacedTowerList = new List<GameObject>();
    [SerializeField] protected List<int> towerCostList = new List<int>();
    protected Dictionary<Transform, BaseTower> towerMap = new Dictionary<Transform, BaseTower>();

    void Awake()
    {
        if (TowerManager.Instance != null && TowerManager.Instance != this)
        {
            Destroy(this);
        }
        else
        {
            TowerManager.Instance = this;
            this.towerTimer = Time.time;
        }

    }

    // Update is called once per frame
    void Update()
    {
        towerTimer = Time.time;
    }

    public void RegisterTower(Transform transform, BaseTower baseTower)
    {
        this.towerMap[transform] = baseTower;
    }
    public BaseTower GetTowerScript(Transform transform)
    {
        if (!this.towerMap.TryGetValue(transform, out BaseTower res))
        {
            res = transform.gameObject.GetComponent<BaseTower>();
            if (res != null)
            {
                RegisterTower(transform, res);
            }
        }
        return res;
    }

    public GameObject GetTowerPrefab(int index)
    {
        if (index >= TowerList.Count)
        {
            return null;
        }
        return TowerList[index];
    }

    public GameObject GetUnplacedTower(int index)
    {
        if (index >= UnplacedTowerList.Count)
        {
            return null;
        }
        return UnplacedTowerList[index];
    }

    public List<int> GetTowerCostList(){
        if (this.towerCostList != null) return this.towerCostList;
        return null;
    }

    public int GetTowerCostListLength(){
        if (this.towerCostList != null) return this.towerCostList.Count;
        return 0;
    }

    public int GetTowerCost(int index)
    {
        if (index >= towerCostList.Count)
        {
            return int.MaxValue;
        }
        return towerCostList[index];
    }

}
