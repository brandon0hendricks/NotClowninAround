using System.Collections;
using UnityEngine;

public class Bouncepad : MonoBehaviour
{
    public float bounciness;
    private Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
       if (collision.collider.CompareTag("Player"))
        {
            anim.SetTrigger("Bounce");
            Rigidbody2D rb = collision.collider.GetComponent<Rigidbody2D>();
            StartCoroutine(Bounce(rb));
        }
    }

    private IEnumerator Bounce(Rigidbody2D rb)
    {
        float currentVelocity = rb.linearVelocity.y;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * bounciness, ForceMode2D.Impulse);
        yield return null;
    }
}
