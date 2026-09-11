using UnityEngine;

public class EndGame : MonoBehaviour
{
    public AudioSource music;
    public AudioSource sfx;
    public void Update()
    {
        music.volume -= Time.deltaTime / 5;
        sfx.volume -= Time.deltaTime / 5;
    }
}
