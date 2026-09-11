using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private InputActionReference pause;
    private Animator anim;

    [SerializeField] private GameObject pauseCanvas;

    private bool gamePaused;
    private bool isPausing;
    private float timeScale;

    private void Awake()
    {
        anim = pauseCanvas.GetComponent<Animator>();
    }

    private void Update()
    {
        if (pause.action.IsPressed() && !gamePaused && !isPausing)
        {
            PauseGame();
            gamePaused = true;
            isPausing = true;
        }
        else if (pause.action.IsPressed() && gamePaused && !isPausing)
        {
            UnpauseGame();
            gamePaused = true;
            isPausing = true;
        }
    }

    private void PauseGame()
    {
        timeScale = Time.timeScale;
        Time.timeScale = 0f;
        pauseCanvas.SetActive(true);
    }

    public void UnpauseGame()
    {
        anim.SetTrigger("Unpause");
    }

    public void AnimationEnd()
    {
        isPausing = false;
        gamePaused = false;
        pauseCanvas.SetActive(false);
        Time.timeScale = timeScale;
    }

    public void AnimationBegin()
    {
        isPausing = false;
    }

}
