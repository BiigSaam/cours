using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RockHead2 : MonoBehaviour
{
    public Vector3[] directions = new Vector3[4];
    public float range = 100;
    public Rigidbody2D rb;

    public LayerMask listTriggerLayers;
    private Collider2D currentTrigger;
    public float speed; // 30

    // public Transform[] dirs = new Tr
    public List<Transform> resultsList = new List<Transform>();

    private Vector3 destination;

    public GameObject[] listTriggers;
    private float delayBetweenMoves = 1.2f;

    private int currentIndex = 1;

    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        // EnableTriggers();

        
        // CheckForTriggers();
    }

    void EnableTriggers()
    {
        for (int i = 0; i < listTriggers.Length; i++)
        {
            listTriggers[i].SetActive(i == currentIndex);
        }
    }
    private void FixedUpdate()
    {
        foreach(var x in resultsList) {
            Debug.Log("x.localPosition.normalized " + x.localPosition.normalized);
            Debug.DrawRay(transform.position, x.localPosition.normalized  * range, Color.red);
        }
        // rb.AddForce(destination * speed, ForceMode2D.Impulse);
    }

    private void CheckForTriggers()
    {
        CalculateDirections();

        // Check in all directions if detects trigger 
        for (int i = 0; i < directions.Length; i++)
        {
            Debug.DrawRay(transform.position, directions[i], Color.red);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directions[i], range, listTriggerLayers);
            if (hit.collider != null && rb.velocity == Vector2.zero && currentTrigger != hit.collider)
            {
                StartCoroutine(ChangeDirection(-hit.normal));
                currentTrigger = hit.collider;
            }
        }
    }

    IEnumerator ChangeDirection(Vector2 dir)
    {
        yield return new WaitForSeconds(delayBetweenMoves);
        destination = dir;
        currentIndex = (currentIndex + 1) % listTriggers.Length;

        if (dir.x == 0)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        }

        EnableTriggers();
    }

    private void CalculateDirections()
    {
        directions[0] = transform.right * range; // Right direction
        directions[1] = -transform.right * range; // Left direction
        directions[2] = transform.up * range; // Up direction
        directions[3] = -transform.up * range; // Down direction
    }

    // private void OnCollisionStay2D(Collision2D other) {
    //     if(rb.velocity == Vector2.zero){
    //         if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0)) {
    //         if(destination.y > 0) {
    //             animator.SetTrigger("HitTop");
    //         } else if (destination.y < 0) {
    //             animator.SetTrigger("HitBottom");
    //         }

    //         if(destination.x > 0) {
    //             animator.SetTrigger("HitRight");
    //         } else if (destination.x < 0) {
    //             animator.SetTrigger("HitLeft");
    //         }
    //         }
    //         CheckForTriggers();
    //     }
    // }
}
