using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    [SerializeField]CinemachineConfiner2D roomScale;
    [SerializeField]GameObject cam;
    [SerializeField]GameObject followPoint;
    private void OnTriggerEnter2D(Collider2D collision)
    {
         changeRoomBounds(collision);
    }

    void changeRoomBounds(Collider2D collision) //this function adapts camera to room settings
    {
        Debug.Log("Camera bounds changed");
        if (collision.tag == "RoomBounds")
        {
            roomScale.BoundingShape2D = collision.GetComponent<BoxCollider2D>();
            if(collision.GetComponent<RoomSettings>() != null)
            {
                float roomlensSize = collision.GetComponent<RoomSettings>().lensSize;
                StartCoroutine(changeCamera(roomlensSize));
                bool isCameraLocked = collision.GetComponent<RoomSettings>().isCameraLocked;

                if (isCameraLocked == true)
                {
                    cam.GetComponent<CinemachineCamera>().Follow = collision.transform;
                }
                else
                {
                    cam.GetComponent<CinemachineCamera>().Follow = followPoint.transform;
                }
            }
            else //saftey measure to make sure that camera will track player if room settings are not set up correctly
            {
                cam.GetComponent<CinemachineCamera>().Follow = followPoint.transform;
            }
        }
    }

    public IEnumerator changeCamera(float cameraSize) //change camera lens size to match room settings, this is a coroutine to make the change smooth
    {
       
        while (cam.GetComponent<CinemachineCamera>().Lens.OrthographicSize != cameraSize)
        {
            cam.GetComponent<CinemachineCamera>().Lens.OrthographicSize = Mathf.Lerp(cam.GetComponent<CinemachineCamera>().Lens.OrthographicSize, cameraSize, .5f);
            yield return new WaitForEndOfFrame();
        }
    }
}
