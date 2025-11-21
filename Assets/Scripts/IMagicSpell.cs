using UnityEngine;

public interface IMagicSpell
{
    int ManaCost { get; }
    float Cooldown { get; }
    void Cast(Vector3 direction, Transform launchPoint);
}
