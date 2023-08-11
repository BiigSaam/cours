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

    [HideInInspector]
    public Transform invoker;

    public void Start()
    {
        // rb.velocity = transform.right * moveSpeed;

    }

    private void OnEnable()
    {
        // rb.velocity = moveSpeed * transform.right;
        
        // autoDestroyCoroutine = StartCoroutine(AutoDestroy(delayBeforeAutodestruction));
    }

    IEnumerator AutoDestroy(float duration = 0)
    {
        yield return new WaitForSeconds(duration);
        if (invoker == null)
        {
            Destroy(gameObject);
        }
        else
        {
            invoker.GetComponent<ObjectPooling>().Release("bullet", gameObject);
            // invoker = null; 
        }
    }

    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (
    //         other.gameObject.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth)
    //     )
    //     {
    //         playerHealth.TakeDamage(damage);
    //     }

    //     animator.SetTrigger("IsCollided");
    //     // rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

    //     autoDestroyCoroutine = StartCoroutine(AutoDestroy(0.35f));
    // }

    public void OnDisable()
    {
        // rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        transform.rotation = Quaternion.identity;
        // StopCoroutine(autoDestroyCoroutine);
        // animator.ResetTrigger("IsCollided");

        // if (invoker == null)
        // {
        //     Destroy(gameObject);
        // }
    }
}
