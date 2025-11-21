using UnityEngine;

public class Despawn : MonoBehaviour
{
    [Header("Despawn Settings")]
    [Tooltip("Time in seconds before the object despawns automatically.")]
    public float lifetime = 5f;

    void Start()
    {
        // Automatically destroy the object after 'lifetime' seconds
        Destroy(gameObject, lifetime);
    }
}
