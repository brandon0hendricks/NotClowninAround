using Unity.VisualScripting;
using UnityEngine;

public class CameraLocker : MonoBehaviour
{
    // This will move the object that locks the player's camera during the boss fight
    [SerializeField] private Transform player;
    [SerializeField] private Transform lockPosition;
    [SerializeField] private float cameraMoveSpeed;
    private float xLoc;

    public bool cameraLock;

    private void Start()
    {
        xLoc = player.position.x;
    }

    void Update()
    {
        transform.position = Position();
        if (cameraLock && transform.position != lockPosition.position)
        {
            xLoc = Mathf.Lerp(transform.position.x, lockPosition.position.x, cameraMoveSpeed);
        }
        else if (!cameraLock && transform.position != player.position)
        {
            xLoc = Mathf.Lerp(transform.position.x, player.position.x, cameraMoveSpeed);
        }
    }

    private Vector2 Position()
    {
        return new Vector2(xLoc, player.position.y);
    }
}
