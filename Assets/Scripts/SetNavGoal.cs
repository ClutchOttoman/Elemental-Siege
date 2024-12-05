using UnityEngine;
using UnityEngine.AI;

public class NearestTargetFinder : MonoBehaviour
{
    [SerializeField]
    public string targetTag = "Target";
    public float searchRadius = 20f; // Radius for detecting targets
    public float searchInterval = 2f; // Time between searches
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
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
                if (distance < nearestDistance)
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

