using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Handles only the visual formatting for UI elements.
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    [SerializeField] public TMP_Text displayPlayerMoney;
    private string currencyFieldLabel = "Money: $ ";
    private string waveFieldLabel = "Wave ";
    private string strongholdHealthLabel = "Stronghold Health: ";
    private string waveRemainEnemiesLabel = "Enemies Remaining in Wave: ";
    [SerializeField] public TMP_Text displayCurrEnemyWave;
    [SerializeField] public TMP_Text displayStrongholdHealth;
    [SerializeField] public TMP_Text displayTileData;
    [SerializeField] public TMP_Text displayEnemiesLeftInWave;
    [SerializeField] public List<TMP_Text> towerCostDisplayList = new List<TMP_Text>(); // allows real-time updates linking of cost in-game by simplying modifying this list.
    protected TowerPlacementController towerPlacementController;

    // Upon loading level.
    void Awake()
    {
        if (UIManager.Instance != null && UIManager.Instance != this)
        {
            Destroy(this);
        }
        else
        {
            UIManager.Instance = this;
        }
    }

    public void updatePlayerMoney(){
        int currBalance = EnemyManager.Instance.GetWallet().GetCurrentBalance();
        UIManager.Instance.displayPlayerMoney.text = currencyFieldLabel + currBalance.ToString();
    }

    public void updateDisplayCurrEnemyWave(){
        UIManager.Instance.displayCurrEnemyWave.text = waveFieldLabel + LevelManager.Instance.currentWave + "/" + LevelManager.Instance.getTotalNumberWaves().ToString();
    }

    public void updateStrongholdHealth(){
        float percentageHealthLeft = (float) (LevelManager.Instance.stronghold.health / LevelManager.Instance.stronghold.healthMaxScale)*100.0f;

        Color healthColor = Color.Lerp(Color.red, Color.yellow, Mathf.InverseLerp(0f, 50f, percentageHealthLeft));  //  Low health
        healthColor = Color.Lerp(healthColor, Color.green, Mathf.InverseLerp(50f, 100f, percentageHealthLeft));  //  Mid health

        UIManager.Instance.displayStrongholdHealth.color = healthColor;
        UIManager.Instance.displayStrongholdHealth.text = strongholdHealthLabel + percentageHealthLeft.ToString("F0") + "%";
    }

    public void updateDisplayTileData()
    {
        if (this.towerPlacementController.tileInfoText != null) UIManager.Instance.displayTileData.text = this.towerPlacementController.tileInfoText.text;
    }

    // Displays the number of 
    public void updateRemainEnemiesWave(){
        UIManager.Instance.displayEnemiesLeftInWave.text = waveRemainEnemiesLabel + LevelManager.Instance.numberOfEnemies.ToString();
    }

    public void alertUIManagerLevelEnded(){
        if (!LevelManager.Instance.ProgressStatusLevel()){

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        this.towerPlacementController = GameObject.Find("Player").GetComponent<TowerPlacementController>();
        
        // Get tower prices. The index should match the listing order in the TowerManager.
        if (TowerManager.Instance != null && TowerManager.Instance.GetTowerCostList() != null){
            int lenCostList = TowerManager.Instance.GetTowerCostListLength();
            for (int i = 0; i < lenCostList && i < towerCostDisplayList.Count; i++){
                // Assign text to listings if it
                if (towerCostDisplayList[i] != null) towerCostDisplayList[i].text = "$" + TowerManager.Instance.GetTowerCost(i).ToString() + " each";
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        updatePlayerMoney();
        updateDisplayCurrEnemyWave();
        updateStrongholdHealth();
        updateDisplayTileData();
        updateRemainEnemiesWave();
        
    }
}
