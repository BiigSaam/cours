using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public FloatVariable currentHealth;
    public FloatVariable maxHealth;

    public VoidEventChannelSO onPlayerDeath;
    public Animator animator;

    public SpriteRenderer spriteRenderer;


    [Tooltip("Please uncheck it on production")]
    public bool needResetHP = true;

    private void Awake()
    {
        if (needResetHP || currentHealth.CurrentValue <= 0)
        {
            currentHealth.CurrentValue = maxHealth.CurrentValue;
        }
    }

    private void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F9))
        {
            Die();
        }
        #endif
    }

    public void TakeDamage(float damage)
    {        
        currentHealth.CurrentValue -= damage;
        if (currentHealth.CurrentValue <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        onPlayerDeath.Raise();
        transform.Rotate(0f, 0f, 45f);
        animator.SetTrigger("OnPlayerDeath");
    }

    public void OnPlayerDeathAnimationCallback()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }
}
