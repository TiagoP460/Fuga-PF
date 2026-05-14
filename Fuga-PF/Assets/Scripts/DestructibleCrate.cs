using UnityEngine;

public class DestructibleCrate : MonoBehaviour
{
    public int health = 2;

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            DestroyCrate();
        }
    }

    void DestroyCrate()
    {
        Destroy(gameObject);
    }
}