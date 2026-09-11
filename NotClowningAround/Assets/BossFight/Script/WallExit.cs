using UnityEngine;

public class WallExit : MonoBehaviour
{
    public GameObject wall;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(wall);
        }
    }
}
