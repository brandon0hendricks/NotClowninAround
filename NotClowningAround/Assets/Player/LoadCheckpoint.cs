using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadCheckpoint : MonoBehaviour
{
    public GameObject player;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        playerHealth = player.GetComponent<PlayerHealth>();
        player.transform.position = Checkpoint.instance.playerLoc.position;
        playerHealth.currentHealth = Checkpoint.instance.savedHealth;
    }

    public void ReloadScene()
    {
        Debug.Log("Reloading scene...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
