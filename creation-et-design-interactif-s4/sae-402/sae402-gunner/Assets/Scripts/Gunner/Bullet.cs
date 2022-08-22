using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed;

    private Rigidbody2D _rb;
    // Start is called before the first frame update

    public GameObject stream;

    void Awake()
    {
         _rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
         _moveSpeed = 10.0f;
        _rb.velocity = transform.right * _moveSpeed;

        // On veut que la balle se détruise d'elle-même 5 secondes après avoir été tirée
        Destroy(gameObject, 3f);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("feffe");
        // Debug.Log(collision.contacts[0].normal.x);
        Destroy(gameObject);
        Destroy(collision.gameObject);
        // collision.gameObject.GetComponent<SpriteRenderer>().enabled
    }

    void ShowStream()
    {
        // stream.GetComponent<Animator>().Play();
    }
}
