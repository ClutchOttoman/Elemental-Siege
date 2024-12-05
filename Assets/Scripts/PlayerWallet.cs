using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    [SerializeField] protected int money = 1000;
    // Start is called before the first frame update
    void Start()
    {
        // Handle tutorial level loading.
        if (PlayerPrefs.GetInt("TutorialComplete", 0) == 1){
            //this.money = 1500;
            //this.money = PlayerDataManager.Instance.loadSavedMoney(); // only if the tutorial is complete.
            // Debug.Log("Money loaded");
        } else {
            this.money = 1000;
            // Debug.Log("Tutorial money given");
        }
        
        EnemyManager.Instance.SetWallet(this);
        LevelManager.Instance.SetWallet(this);


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetCurrentBalance(){
        return this.money;
    }

    //for UI to know if tower is purchasable
    public bool CanBuy(int cost)
    {
        if (cost <= this.money)
        {
            return true;
        }
        else return false;
    }
    //for money to be removed when tower is actually placed
    public void Buy(int cost)
    {
        this.money -= cost;
        Debug.Log("transaction cost: " + cost + "money: " + this.money); 
        return;
    }
    public void CollectMoney(int reward) {
        Debug.Log("Collecting: " + reward);
        this.money += reward;
    }
   }
