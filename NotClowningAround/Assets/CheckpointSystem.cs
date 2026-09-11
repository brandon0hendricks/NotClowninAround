using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    
    public SaveData saveData;
    private Vector2 RESET_VECTOR = new Vector2(0,0);
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Checkpoint"))
        {
            saveData.checkpointLocation = new Vector2(transform.position.x,transform.position.y);
            Debug.Log("Checkpoint reached and data saved!");
        }
    }

    private void Awake()
    {
        if (saveData.checkpointLocation != RESET_VECTOR)
        {
            transform.position = saveData.checkpointLocation;
            Debug.Log("Player respawned at checkpoint!");
        }
    }
}


