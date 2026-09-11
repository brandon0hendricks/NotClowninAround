using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Handles main menu

    private Animator anim;
    private AudioSource musicManager;

    private void Start()
    {
        if (gameObject.name == "Canvas")
        {
            anim = GetComponentInChildren<Animator>();
            musicManager = GetComponentInChildren<AudioSource>();
            musicManager.Play();
        }
    }

    private void Update()
    {
        if (gameObject.name == "Canvas")
        {
            if (musicManager.volume < 0.75f)
            {
                musicManager.volume = Mathf.Lerp(musicManager.volume, 0.75f, Time.deltaTime);
            }
        }
    }

    public void StartButton()
    {
        anim.SetTrigger("Fader");
        if (gameObject.name == "Canvas")
        {
            musicManager.volume = Mathf.Lerp(musicManager.volume, 0f, Time.deltaTime * 5);
        }
    }
    
    void NextLevel()
    {
        SceneManager.LoadScene(1);
    }
}
