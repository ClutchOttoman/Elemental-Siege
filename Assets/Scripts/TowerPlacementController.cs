using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerPlacementController : MonoBehaviour
{
    [SerializeField] protected GameObject mainCameraObject;
    protected Camera mainCamera;
    [SerializeField] protected AudioSource audioSource;
    protected Ray ray;
    protected RaycastHit hitData;
    protected RaycastHit prevHitData;
    protected Tile tileScript;
    protected Tile prevTileScript;
    protected bool placementModeOn;
    [SerializeField] protected float maxRaycastDistance = 150.0F;
    protected int tilesLayerAsMask = 1 << 7;
    protected GameObject unplacedTower = null;
    protected GameObject tower = null;
    protected int towerRotation = 0;
    [SerializeField] protected PlayerWallet myWallet;
    protected int index = 0;
    [SerializeField] public Text tileInfoText;
    protected Towers towerToBePlaced;  

    public enum Towers
    {
        BaseTower,
        BulwarkTower,
        FireballTower,
        TidalTower,
        WhirlwindTower,
        BanishmentTower,
        EarthquakeTower,
        QuicksandTower,
        FlamethrowerTower,
        WildfireTower,
        FreezerTower,
        FountainTower,
        WindcallerTower,
        HurricaneTower
    }
    void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        this.mainCamera = this.mainCameraObject.GetComponent<Camera>();
        this.placementModeOn = false;
        Debug.Log("Tower Placement Start");
    }

    void Update()
    {
        //Currently set to 't' for testing -> click t to toggle tower placement mode
        /*if (Input.GetButtonDown("ToggleTowerPlacement"))
        {
            //PlaceBulwarkTower();
            //PlaceFireballTower();
            //PlaceTidalTower();
            //PlaceWhirlwindTower();
            //PlaceBanishmentTower();
        }*/
        
        // Ensure that the tower isn't being placed in a screen area designed as a UI bound.
        // Prevents unintentional tower placement.
        if (!PlayerController.Instance.checkInUIBounds() && !PlayerController.Instance.isInMenu){
           
            if (this.placementModeOn && Input.GetButtonDown("ExitTowerPlacement"))
            {
                Debug.Log("Placement Mode Off");
                this.placementModeOn = false;
                // if (PlayerController.Instance != null) PlayerController.Instance.setTowerPlacementMode(false);
                if (this.prevTileScript != null)
                {
                    this.prevTileScript.RemoveUnplacedTower();
                }
            }

            if (this.placementModeOn && Input.GetButtonDown("PlaceTower") && this.tileScript != null)
            {
                if(this.tileScript.AttachTower(this.tower, this.towerRotation, this.myWallet, this.index))
                {
                    this.tileScript.RemoveUnplacedTower();
                    this.audioSource.PlayOneShot(this.audioSource.clip);
                }
                
            }

            if (this.placementModeOn && Input.GetButtonDown("RotateTower"))
            {
                this.towerRotation = (this.towerRotation + 45) % 360;
                if (this.tileScript != null)
                {
                    this.tileScript.RotateUnplacedTower(this.towerRotation);
                }
            }
            
        }
        
    }

    void FixedUpdate()
    {
        if (this.placementModeOn)
        {
            GetTileCollider();
            // UIManager.Instance.displayTileData.text = this.tileInfoText.text; // update tile info.
        }
    }

    protected void GetTileCollider()
    {
        //ray orgin at center of viewport which is the Main Camera
        //Debug.Log("Attempting to find tile to place tower on");
        this.ray = mainCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        if (Physics.Raycast(ray, out hitData, this.maxRaycastDistance, this.tilesLayerAsMask, QueryTriggerInteraction.Collide) && hitData.transform != this.prevHitData.transform)
        {
            if (this.prevTileScript != null)
            {
                this.prevTileScript.RemoveUnplacedTower();
            }
            this.prevHitData = hitData;
            //Debug.Log("Ray Hit: " + hitData.transform.name);
            this.tileScript = TileManager.Instance.GetTile(hitData.transform);
            this.prevTileScript = this.tileScript;

            if (unplacedTower != null)
            {
                this.tileScript.ShowUnplacedTower(unplacedTower, this.towerRotation);
            }

            // Update the text of the UI element
            if (this.tileInfoText != null)
            {
                this.tileInfoText.text = this.tileScript.AsString(this.towerToBePlaced);
            }
        }
    }

    public void PlaceBulwarkTower()
    {
        //set tower to be placed as bulwark
        this.tower = TowerManager.Instance.GetTowerPrefab((int)Towers.BulwarkTower);
        this.unplacedTower = TowerManager.Instance.GetUnplacedTower((int)Towers.BulwarkTower);
        this.index = (int)Towers.BulwarkTower;
        this.towerToBePlaced = Towers.BulwarkTower;
        Debug.Log("Placing: " + this.tower);
        this.placementModeOn = true;
        // if (PlayerController.Instance != null) PlayerController.Instance.setTowerPlacementMode(true);
    }

    public void PlaceFireballTower()
    {
        //set tower to be placed as fireball
        this.tower = TowerManager.Instance.GetTowerPrefab((int)Towers.FireballTower);
        this.unplacedTower = TowerManager.Instance.GetUnplacedTower((int)Towers.FireballTower);
        this.index = (int)Towers.FireballTower;
        this.towerToBePlaced = Towers.FireballTower;
        Debug.Log("Placing: " + this.tower);
        this.placementModeOn = true;
        // if (PlayerController.Instance != null) PlayerController.Instance.setTowerPlacementMode(true);
    }

    public void PlaceTidalTower()
    {
        //set tower to be placed as Tidal
        this.tower = TowerManager.Instance.GetTowerPrefab((int)Towers.TidalTower);
        this.unplacedTower = TowerManager.Instance.GetUnplacedTower((int)Towers.TidalTower);
        this.index = (int)Towers.TidalTower;
        this.towerToBePlaced = Towers.TidalTower;
        Debug.Log("Placing: " + this.tower);
        this.placementModeOn = true;
        // if (PlayerController.Instance != null) PlayerController.Instance.setTowerPlacementMode(true);
    }

    public void PlaceWhirlwindTower()
    {
        //set tower to be placed as Whirlwind
        this.tower = TowerManager.Instance.GetTowerPrefab((int)Towers.WhirlwindTower);
        this.unplacedTower = TowerManager.Instance.GetUnplacedTower((int)Towers.WhirlwindTower);
        this.index = (int)Towers.WhirlwindTower;
        this.towerToBePlaced = Towers.WhirlwindTower;
        Debug.Log("Placing: " + this.tower);
        this.placementModeOn = true;
        // if (PlayerController.Instance != null) PlayerController.Instance.setTowerPlacementMode(true);
    }

    public void PlaceBanishmentTower()
    {
        //set tower to be placed as Banishment
        this.tower = TowerManager.Instance.GetTowerPrefab((int)Towers.BanishmentTower);
        this.unplacedTower = TowerManager.Instance.GetUnplacedTower((int)Towers.BanishmentTower);
        this.index = (int)Towers.BanishmentTower;
        this.towerToBePlaced = Towers.BanishmentTower;
        Debug.Log("Placing: " + this.tower);
        this.placementModeOn = true;
        // if (PlayerController.Instance != null) PlayerController.Instance.setTowerPlacementMode(true);
    }

    public void PlaceEarthquakeTower()
    {
        //set tower to be placed as Earthquake
        this.tower = TowerManager.Instance.GetTowerPrefab((int)Towers.EarthquakeTower);
        this.unplacedTower = TowerManager.Instance.GetUnplacedTower((int)Towers.EarthquakeTower);
        this.index = (int)Towers.EarthquakeTower;
        this.towerToBePlaced = Towers.EarthquakeTower;
        Debug.Log("Placing: " + this.tower);
        this.placementModeOn = true;
        // if (PlayerController.Instance != null) PlayerController.Instance.setTowerPlacementMode(true);
    }

    public void PlaceQuicksandTower()
    {
        //set tower to be placed as Quicksand
        this.tower = TowerManager.Instance.GetTowerPrefab((int)Towers.QuicksandTower);
        this.unplacedTower = TowerManager.Instance.GetUnplacedTower((int)Towers.QuicksandTower);
        this.index = (int)Towers.QuicksandTower;
        this.towerToBePlaced = Towers.QuicksandTower;
        Debug.Log("Placing: " + this.tower);
        this.placementModeOn = true;
        // if (PlayerController.Instance != null) PlayerController.Instance.setTowerPlacementMode(true);
    }

    public void PlaceFlamethrowerTower()
    {
        //set tower to be placed as Flamethrower
        this.tower = TowerManager.Instance.GetTowerPrefab((int)Towers.FlamethrowerTower);
        this.unplacedTower = TowerManager.Instance.GetUnplacedTower((int)Towers.FlamethrowerTower);
        this.index = (int)Towers.FlamethrowerTower;
        this.towerToBePlaced = Towers.FlamethrowerTower;
        Debug.Log("Placing: " + this.tower);
        this.placementModeOn = true;
        // if (PlayerController.Instance != null) PlayerController.Instance.setTowerPlacementMode(true);
    }

    public void PlaceWildfireTower()
    {
        //set tower to be placed as Wildfire
        this.tower = TowerManager.Instance.GetTowerPrefab((int)Towers.WildfireTower);
        this.unplacedTower = TowerManager.Instance.GetUnplacedTower((int)Towers.WildfireTower);
        this.index = (int)Towers.WildfireTower;
        this.towerToBePlaced = Towers.WildfireTower;
        Debug.Log("Placing: " + this.tower);
        this.placementModeOn = true;
        // if (PlayerController.Instance != null) PlayerController.Instance.setTowerPlacementMode(true);
    }

    public void PlaceFreezerTower()
    {
        //set tower to be placed as Freezer
        this.tower = TowerManager.Instance.GetTowerPrefab((int)Towers.FreezerTower);
        this.unplacedTower = TowerManager.Instance.GetUnplacedTower((int)Towers.FreezerTower);
        this.index = (int)Towers.FreezerTower;
        this.towerToBePlaced = Towers.FreezerTower;
        Debug.Log("Placing: " + this.tower);
        this.placementModeOn = true;
        // if (PlayerController.Instance != null) PlayerController.Instance.setTowerPlacementMode(true);
    }

    public void PlaceFountainTower()
    {
        //set tower to be placed as Fountain
        this.tower = TowerManager.Instance.GetTowerPrefab((int)Towers.FountainTower);
        this.unplacedTower = TowerManager.Instance.GetUnplacedTower((int)Towers.FountainTower);
        this.index = (int)Towers.FountainTower;
        this.towerToBePlaced = Towers.FountainTower;
        Debug.Log("Placing: " + this.tower);
        this.placementModeOn = true;
        // if (PlayerController.Instance != null) PlayerController.Instance.setTowerPlacementMode(true);
    }

    public void PlaceWindcallerTower()
    {
        //set tower to be placed as Windcaller
        this.tower = TowerManager.Instance.GetTowerPrefab((int)Towers.WindcallerTower);
        this.unplacedTower = TowerManager.Instance.GetUnplacedTower((int)Towers.WindcallerTower);
        this.index = (int)Towers.WindcallerTower;
        this.towerToBePlaced = Towers.WindcallerTower;
        Debug.Log("Placing: " + this.tower);
        this.placementModeOn = true;
        // if (PlayerController.Instance != null) PlayerController.Instance.setTowerPlacementMode(true);
    }

    public void PlaceHurricaneTower()
    {
        //set tower to be placed as Hurricane
        this.tower = TowerManager.Instance.GetTowerPrefab((int)Towers.HurricaneTower);
        this.unplacedTower = TowerManager.Instance.GetUnplacedTower((int)Towers.HurricaneTower);
        this.index = (int)Towers.HurricaneTower;
        this.towerToBePlaced = Towers.HurricaneTower;
        Debug.Log("Placing: " + this.tower);
        this.placementModeOn = true;
        // if (PlayerController.Instance != null) PlayerController.Instance.setTowerPlacementMode(true);
    }

    public void StopTowerPlacement()
    {
        Debug.Log("Placement Mode Off");
        this.placementModeOn = false;
        // if (PlayerController.Instance != null) PlayerController.Instance.setTowerPlacementMode(false);
        if (this.prevTileScript != null)
        {
            this.prevTileScript.RemoveUnplacedTower();
        }
        if (this.tileInfoText != null)
        {
            this.tileInfoText.text = $@" ";
        }
    }
}
