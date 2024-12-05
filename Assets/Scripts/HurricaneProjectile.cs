using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurricaneProjectile : BaseProjectile
{
    [SerializeField] protected HurricaneAOE hurricaneTemplate;

    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {

        base.Update();
    }

    protected override void OnHitEnemy(BaseEnemy enemy)
    {
        Debug.Log("Hurricane Projectile Hit Enemy");
        var hurricaneToSpawn = hurricaneTemplate.GetComponent<HurricaneAOE>();

        var hurricane = Instantiate(hurricaneToSpawn, this.transform.position,this.transform.rotation);

        HurricaneAOE hurricaneScript = hurricane.GetComponent<HurricaneAOE>();
        hurricaneScript.SetDamageOverTime(this.buffedDamage);

        Destroy(this.gameObject);

    }
    protected override void OnHitTerrain(GameObject tile)
    {
        Debug.Log("Hurricane Projectile Hit Tile");
        var hurricaneToSpawn = hurricaneTemplate.GetComponent<HurricaneAOE>();

        var hurricane = Instantiate(hurricaneToSpawn, this.transform.position, this.transform.rotation);

        HurricaneAOE hurricaneScript = hurricane.GetComponent<HurricaneAOE>();
        hurricaneScript.SetDamageOverTime(this.buffedDamage);

        Destroy(this.gameObject);
    }
}
