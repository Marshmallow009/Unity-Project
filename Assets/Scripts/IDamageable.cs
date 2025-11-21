public interface IDamageable
{
    void TakeDamage(int amount);
    event System.Action OnDeath;
}