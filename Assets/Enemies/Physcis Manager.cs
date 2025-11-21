using UnityEngine;

public class PhysicsManager : MonoBehaviour
{
    [Tooltip("Tag for the first group of objects (e.g. Enemies)")]
    public string tagA = "Enemy";

    [Tooltip("Tag for the second group of objects (e.g. XP Orbs)")]
    public string tagB = "XP";

    [Tooltip("How often to recheck new spawns (seconds). Set to 0 to only run once.")]
    public float refreshInterval = 1f;

    void Start()
    {
        // Run once at start
        IgnoreCollisionsBetweenTags(tagA, tagB);

        // Optionally keep rechecking if you have dynamic spawns
        if (refreshInterval > 0)
            InvokeRepeating(nameof(RefreshCollisions), refreshInterval, refreshInterval);
    }

    void RefreshCollisions()
    {
        IgnoreCollisionsBetweenTags(tagA, tagB);
    }

    public static void IgnoreCollisionsBetweenTags(string tagA, string tagB)
    {
        GameObject[] groupA = GameObject.FindGameObjectsWithTag(tagA);
        GameObject[] groupB = GameObject.FindGameObjectsWithTag(tagB);

        foreach (var objA in groupA)
        foreach (var objB in groupB)
        {
            foreach (var colA in objA.GetComponentsInChildren<Collider>())
            foreach (var colB in objB.GetComponentsInChildren<Collider>())
                Physics.IgnoreCollision(colA, colB);
        }
    }
}
