using UnityEngine;

public static class BoundsUtil
{

    /// Creates a <c>Bounds</c> from two points
    public static Bounds FromAABB(Vector3 pointA, Vector3 pointB) 
    {
        // Calculate center
        Vector3 center = (pointA + pointB) / 2;

        // Calculate size
        Vector3 size = new Vector3(
            Mathf.Abs(pointA.x - pointB.x),
            Mathf.Abs(pointA.y - pointB.y),
            Mathf.Abs(pointA.z - pointB.z)
        );

        // Create the Bounds using center and size
        return new Bounds(center, size);
    }
}