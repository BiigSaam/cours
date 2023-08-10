using System.Collections;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public Animator animator;

    [Tooltip("From where the projectile will be shot")]
    public Transform firePoint;
    public SpriteRenderer spriteRenderer;

    [Range(0, 5)]
    public float timeDelayBetweenShots;

    public ObjectPooling bulletPooling;

    public ObjectSpawner objectSpawner;

    public float delayBetweenShotsCycles;
    public int nbOfConsecutiveShots;

    IEnumerator Start()
    {
        WaitForSeconds waiter = new WaitForSeconds(0.55f);
        while (true)
        {
            Shoot();
            yield return waiter;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(PlayAnimInterval(nbOfConsecutiveShots));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            spriteRenderer.color = new Color(1, 1, 1, 1);
            StopAllCoroutines();
        }
    }

    private IEnumerator PlayAnimInterval(int nbIterations)
    {
        spriteRenderer.color = Color.red;

        while (nbIterations > 0)
        {
            animator.SetTrigger("IsAttacking");
            nbIterations = nbIterations - 1;

            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length + timeDelayBetweenShots);
        }

        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(delayBetweenShotsCycles);
        StartCoroutine(PlayAnimInterval(nbOfConsecutiveShots));
    }

    public void Shoot()
    {
        // objectSpawner.pool.Get();
        Bullet bulletProjectile = objectSpawner.pool.Get();
        print(bulletProjectile);
        if (bulletProjectile.gameObject != null)
        {
            bulletProjectile.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation);
            // bulletProjectile.transform.right = transform.right.normalized;

            Bullet bullet = bulletProjectile.GetComponent<Bullet>();
            bullet.invoker = transform;
            bullet.Initialize();
        }
    }

    // Called from the animation's timeline
    // public void Shoot()
    // {
    //     GameObject bulletProjectile = bulletPooling.Get("bullet");

    //     if (bulletProjectile != null)
    //     {
    //         bulletProjectile.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation);

    //         Bullet bullet = bulletProjectile.GetComponent<Bullet>();
    //         bullet.invoker = transform;
    //         bullet.Initialize();
    //     }
    // }
}
