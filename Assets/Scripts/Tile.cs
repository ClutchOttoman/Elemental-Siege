using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    // region config
    /// The elevation multiplier
    [SerializeField] public float elevation = 1F;
    /// The wetness (humidity) multiplier
    [SerializeField] public float wetness = 1F;
    // endregion

    /// The tower, if the tile has one
#nullable enable
    public GameObject? tower;
#nullable disable

    protected GameObject unplacedTower;

    // Start is called before the first frame update
    void Start()
    {
        TileManager.Instance.AddTile(this.transform, this);

        if (this.tower != null)
        {
            Vector3 placePosition = this.transform.position + new Vector3(0, 0.84F, 0);
            this.tower = Instantiate(this.tower, placePosition, this.transform.rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (this.tower == null) { return; }
    }

    public bool AttachTower(GameObject towerObject, int rotation, PlayerWallet playerWallet, int index)
    {
        var towerCost = TowerManager.Instance.GetTowerCost(index);
        if (this.tower == null && playerWallet.CanBuy(towerCost))
        {
            //Do the nescessary things to add tower
            Vector3 placePosition = this.placementPosition();
            this.tower = Instantiate(towerObject, placePosition, this.transform.rotation);
            this.tower.transform.rotation = Quaternion.AngleAxis(rotation, Vector3.up);
            BaseTower towerScript = TowerManager.Instance.GetTowerScript(this.tower.transform);
            towerScript.tile = this;
            towerScript.InitializeTileBuff(this);
            playerWallet.Buy(towerScript.cost);
            RemoveUnplacedTower();
            //this.tower.transform.localScale += new Vector3(0, 0.9F, 0);

            // Player has successfully placed tower.
            PlayerDataManager.Instance.updateTotalTowersPlaced(towerScript.GetTowerTyping()); // alert the Player Data Manager
            PlayerDataManager.Instance.printAll();

            return true;
        }
        return false;
    }

    public bool DetachTower()
    {
        if (this.tower != null)
        {
            //Do the nescessary desctructions to remove tower
            this.tower = null;
            return true;
        }
        return false;
    }

    public bool ShowUnplacedTower(GameObject unplacedTower, int rotation)
    {
        if (this.tower == null && this.unplacedTower == null)
        {
            Vector3 placePosition = this.placementPosition();
            this.unplacedTower = Instantiate(unplacedTower, placePosition, this.transform.rotation);
            this.unplacedTower.transform.rotation = Quaternion.AngleAxis(rotation, Vector3.up);
            //this.unplacedTower.transform.localScale += new Vector3(0, 0.9F, 0);
        }
        return false;
    }
    public bool RemoveUnplacedTower()
    {
        if (this.unplacedTower != null)
        {
            Destroy(this.unplacedTower);
            return true;
        }
        return false;
    }
    public void RotateUnplacedTower(int rotation)
    {
        if(this.unplacedTower == null) { return; }
        this.unplacedTower.transform.rotation = Quaternion.AngleAxis(rotation, Vector3.up);
    }

    public Vector3 placementPosition()
    {
        return this.transform.position + new Vector3(0, this.elevation/2, 0);
    }

    public string AsString(TowerPlacementController.Towers towerToBePlaced)
    {
        switch (towerToBePlaced)
        {
            case TowerPlacementController.Towers.BulwarkTower:
                return $@"Elevation: {elevation}, Wetness: {wetness}
                Bulwark Tower - Damage worsens as elevation increases
=                ";
                
            case TowerPlacementController.Towers.FireballTower:
                return $@"Elevation: {elevation}, Wetness: {wetness}
                 Fireball - Faster fire rate as humidity is lower
                ";
                
            case TowerPlacementController.Towers.TidalTower:
                return $@"Elevation: {elevation}, Wetness: {wetness}
                Tidal - Faster fire rate as humidity is higher
                ";
                
            case TowerPlacementController.Towers.WhirlwindTower:
                return $@"Elevation: {elevation}, Wetness: {wetness}
                Whirlwind - Longer projectile lifespan as elevation increases
                ";
            case TowerPlacementController.Towers.BanishmentTower:
                return $@"Elevation: {elevation}, Wetness: {wetness}
                Banishment Tower - Does not get impacted by elevation or humidity
                ";
            case TowerPlacementController.Towers.EarthquakeTower:
                return $@"Elevation: {elevation}, Wetness: {wetness}
                Earthquake Tower - Damage worsens as elevation increases
                ";
            case TowerPlacementController.Towers.QuicksandTower:
                return $@"Elevation: {elevation}, Wetness: {wetness}
                Quicksand Tower - Range worsens as elevation increases
                ";
            case TowerPlacementController.Towers.FlamethrowerTower:
                return $@"Elevation: {elevation}, Wetness: {wetness}
                Flamethrower Tower - Faster fire rate as humidity is lower
                ";
            case TowerPlacementController.Towers.WildfireTower:
                return $@"Elevation: {elevation}, Wetness: {wetness}
                Wildfire Tower - Faster fire rate as humidity is lower
                ";
            case TowerPlacementController.Towers.FreezerTower:
                return $@"Elevation: {elevation}, Wetness: {wetness}
                Freezer Tower - Longer freeze time as humidity is higher
                ";
            case TowerPlacementController.Towers.FountainTower:
                return $@"Elevation: {elevation}, Wetness: {wetness}
                Fountain Tower - Applies buffs more often as humidity increases
                ";
            case TowerPlacementController.Towers.WindcallerTower:
                return $@"Elevation: {elevation}, Wetness: {wetness}
                Windcaller Tower - Damage improves as elevation increases
                ";
            case TowerPlacementController.Towers.HurricaneTower:
                return $@"Elevation: {elevation}, Wetness: {wetness}
                Hurricane Tower - Damage improves as elevation increases
                ";
            default:
                return $@"Elevation: {elevation}, Wetness: {wetness}
                ";
        }
        
    }
}