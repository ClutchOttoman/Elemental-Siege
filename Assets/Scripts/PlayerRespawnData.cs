using UnityEngine;

/// Data about where the player can go, and where they should respawn if they leave the bounds
public class PlayerRespawnData
{
    /// The bounds
    public Bounds bounds;
    /// The respawn point
    public Vector3 spawnPoint;
    /// The respawn point's rotation data
    public Quaternion spawnPointRotation;

    public PlayerRespawnData(Bounds bounds, Vector3 spawnPoint, Quaternion spawnPointRotation)
    {
        this.bounds = bounds;
        this.spawnPoint = spawnPoint;
        this.spawnPointRotation = spawnPointRotation;
    }
}