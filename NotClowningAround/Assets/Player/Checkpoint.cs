using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public static Checkpoint instance;

    public bool checkpointing = false;

    public Transform playerLoc;
    public float savedHealth;

    private void Awake()
    {
        if (instance == null)
        {
            Debug.Log("Checkpoint instance created.");
            instance = this;
            playerLoc = gameObject.transform;
            savedHealth = gameObject.GetComponent<PlayerHealth>().currentHealth;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Checkpoint"))
        {
            playerLoc = gameObject.transform;
            Debug.Log("Checkpoint reached!");
            checkpointing = true;
        }
    }
}
