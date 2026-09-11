using UnityEngine;

public class Music : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private BossFightManager manager;

    [SerializeField] private AudioClip normal;
    [SerializeField] private AudioClip fight;

    void Update()
    {
        if (manager.fightOngoing && musicSource.clip != fight)
        {
            musicSource.clip = fight;
            musicSource.Play();
        }
        else if (!manager.fightOngoing && musicSource.clip != normal)
        {
            musicSource.clip = normal;
            musicSource.Play();
        }
    }

    // I am SO FUCKING TIRED of working on this GODDAMN GAME
    // After this script I AM DONE
    //IDGAF
}
