using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float moveSpeed;
    public Rigidbody2D rb;

    public float delayBeforeAutodestruction = 2.5f;

    public Animator animator;

    public float damage = 1f;

    private Coroutine autoDestroyCoroutine;

    public ObjectPooled objectPooled;
    

    private void OnEnable()
    {
        autoDestroyCoroutine = StartCoroutine(AutoDestroy(delayBeforeAutodestruction));
    }

    IEnumerator AutoDestroy(float duration = 0)
    {
        yield return new WaitForSeconds(duration);
        
        if (objectPooled.Pool == null)
        {
            Destroy(gameObject);
        }
        else
        {
            objectPooled.Release();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (
            other.gameObject.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth)
        )
        {
            playerHealth.TakeDamage(damage);
        }

        animator.SetTrigger("IsCollided");
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

        autoDestroyCoroutine = StartCoroutine(AutoDestroy(0.35f));
    }

    public void OnDisable()
    {
        rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        rb.velocity = Vector2.zero;

        StopCoroutine(autoDestroyCoroutine);
        animator.ResetTrigger("IsCollided");

        if (objectPooled.Pool == null)
        {
            Destroy(gameObject);
        }
    }
}
