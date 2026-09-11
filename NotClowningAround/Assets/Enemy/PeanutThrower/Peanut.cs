using System.Collections;
using System.Threading;
using UnityEngine;

public class Peanut : MonoBehaviour
{

    public PeanutThrowerController parent;
    public GameObject col;
    float timer = 0.1f;

    void Start()
    {
        StartCoroutine(PeanutKiller());
    }

    void FixedUpdate()
    {
        if (timer > 0f)
        {
            col.SetActive(false);
            timer -= Time.deltaTime;
        }
        else if (timer <= 0f && !col.activeSelf)
        {
            col.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player") || collider.gameObject.CompareTag("GroundTarp") || collider.gameObject.CompareTag("GroundGrass") || collider.gameObject.CompareTag("Ground"))
        {
            parent.OnPeanutDead();
            SoundManager.PlaySound(SoundType.PeanutHit, 0.25f);
            Destroy(gameObject);
        }
    }

    private IEnumerator PeanutKiller()
    {
        yield return new WaitForSeconds(10f);
        parent.OnPeanutDead();
        Destroy(gameObject);
    }
}
