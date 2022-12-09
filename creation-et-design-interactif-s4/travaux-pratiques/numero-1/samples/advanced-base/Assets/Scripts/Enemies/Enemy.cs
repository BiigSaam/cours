using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float bounce = 2f;
    public FloatVariable maxHealth;

    [ReadOnlyInspector]
    public float currentHealth = 0f;

    public SpriteRenderer spriteRenderer;
    public BoxCollider2D bc2d;
    public Rigidbody2D rb;
    public Animator animator;

    [Tooltip("Component to disable after specific event. E.g. : death")]
    public Behaviour[] listComponents;

    private void Start()
    {
        currentHealth = maxHealth?.CurrentValue ?? 1f;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (
            other.gameObject.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth) &&
            other.gameObject.CompareTag("Player") &&
            other.contacts[0].normal.y > -0.5f
            )
        {
            playerHealth.TakeDamage(1f);
        }

        if (other.contacts[0].normal.y < -0.5f)
        {
            StartCoroutine(TakeDamage(1f));
            Vector2 bounceForce = Vector2.up * bounce;
            other.gameObject.GetComponent<Rigidbody2D>().AddForce(bounceForce, ForceMode2D.Impulse);

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        if (other.gameObject.CompareTag("Saw"))
        {
            Die();
        }
    }

    IEnumerator TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (animator)
        {
            animator.SetTrigger("IsHit");
        }
        else
        {
            spriteRenderer.color = UnityEngine.Color.black;
            yield return new WaitForSeconds(0.25f);
            spriteRenderer.color = new Color(1, 1, 1, 1);
        }
    }

    private void Die()
    {
        Vector2 bounceForce = Vector2.up * 1000;
        rb.AddForce(bounceForce, ForceMode2D.Impulse);
        bc2d.enabled = false;
        gameObject.transform.Rotate(0f, 0f, 80f);

        foreach (var component in listComponents)
        {
            component.enabled = false;
        }

        foreach (Transform child in transform) {
            child.gameObject.SetActive(false);
        }
    }

    void OnBecameInvisible()
    {
        if (currentHealth <= 0)
        {
            Destroy(gameObject, 0.15f);
        }
    }
}
