using UnityEngine;

public class BossWall : MonoBehaviour
{
    // This is for the wall that follows the player up

    [SerializeField] private BossFightManager fightManager;
    [SerializeField] private GameObject wall;
    [SerializeField] private GameObject wallBlocker;
    [SerializeField] private float moveSpeed;

    [SerializeField] private Rigidbody2D rb;

    private void Update()
    {
        if (fightManager.fightOngoing && !wall.activeSelf)
        {
            wall.SetActive(true);
            wallBlocker.SetActive(true);

        }
        MoveUp();
    }
    private void MoveUp()
    {
        rb.linearVelocityY = moveSpeed;
    }
}
