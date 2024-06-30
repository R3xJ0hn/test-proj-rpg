using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] float health = 100f;

    public bool IsDead { get; private set; } = false;

    public void TakeDamage(float damage)
    {
        health = Mathf.Max(health - damage, 0);

        if (health == 0)
            DeathBehaviour();
    }

    private void DeathBehaviour()
    {
        if (IsDead) return;
        IsDead = true;
        GetComponent<Animator>().SetTrigger("die");
    }
}
