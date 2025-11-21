using UnityEngine;

public class IgnorePlayer : MonoBehaviour
{
    void Start()
    {
        // Get ALL colliders on this rubble piece (including children)
        Collider[] rubbleColliders = GetComponentsInChildren<Collider>();

        // Get ALL colliders on the player
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            Collider[] playerColliders = player.GetComponentsInChildren<Collider>();

            // Ignore each rubble collider with each player collider
            foreach (Collider rubbleCol in rubbleColliders)
            {
                foreach (Collider playerCol in playerColliders)
                {
                    Physics.IgnoreCollision(rubbleCol, playerCol);
                }
            }
        }
    }
}
