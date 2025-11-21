using UnityEngine;

public class AutoDestroyParticle : MonoBehaviour
{
    private ParticleSystem ps;
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        Destroy(gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
    }
}