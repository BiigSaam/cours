using UnityEngine;

public class Collectible : MonoBehaviour
{
    public CollectibleVariable data;
    public GameObject collectedEffect;
    public SpriteRenderer spriteRenderer;

    private void Awake() {
        Debug.Log("spriteRenderer " + spriteRenderer);
        spriteRenderer.sprite = data.sprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameObject effect = Instantiate(collectedEffect, transform.position, transform.rotation);
            Destroy (effect, effect.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length); 

            data.PickItem(transform.position);

            Destroy(gameObject);
        }
    }
}
