using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindcallerNavController : MonoBehaviour
{
    [SerializeField]
    public string targetTag = "Target";
    public float searchRadius = 20f; // Radius for detecting targets
    public float searchInterval = 2f; // Time between searches
    private UnityEngine.AI.NavMeshAgent agent;
    [SerializeField] public float sqrTetherDistance;
    [HideInInspector] public WindcallerTower parentWindcallerTower;

    void Awake()
    {

    }
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("No NavMeshAgent found on this GameObject!");
        }

        // Start the search routine
        InvokeRepeating(nameof(SearchForTarget), 0f, searchInterval);
    }

    void SearchForTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, searchRadius);
        GameObject nearestTarget = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(targetTag))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                Vector3 parentTowerLocation = this.parentWindcallerTower.transform.position;
                parentTowerLocation.y = 0f;
                Vector3 targetLocation = hitCollider.transform.position;
                targetLocation.y = 0f;

                float sqrDistanceToTarget = (parentTowerLocation - targetLocation).sqrMagnitude;

                //can only target enemies in towers range
                if (distance < nearestDistance && sqrDistanceToTarget < sqrTetherDistance)
                {
                    nearestDistance = distance;
                    nearestTarget = hitCollider.gameObject;
                }
            }
        }

        if (nearestTarget != null)
        {
            agent.SetDestination(nearestTarget.transform.position);
        }
    }

    // Draw the detection radius in the editor for debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }
}
