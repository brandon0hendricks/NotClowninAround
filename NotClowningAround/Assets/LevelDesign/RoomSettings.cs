using UnityEngine;

public class RoomSettings : MonoBehaviour
{
    public bool isCameraLocked;
    public float lensSize;
    private void Start()
    {
        Physics2D.IgnoreLayerCollision(9, 10, true); //ignore collision between player and room bounds
    }
}
