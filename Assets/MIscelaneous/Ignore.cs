using UnityEngine;

public class Ignore : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Collider myCollider = GetComponent<Collider>();
        foreach (var other in GameObject.FindGameObjectsWithTag("Projectile"))
        {
            if (other != gameObject)
            {
                Collider otherCollider = other.GetComponent<Collider>();
                if (otherCollider != null && myCollider != null)
                    Physics.IgnoreCollision(myCollider, otherCollider);
            }
        }
    }
}
