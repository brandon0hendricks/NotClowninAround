using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    [SerializeField] Animator fader;
    public SaveData saveData;

    public void restartLevel()
    {
        fader.SetTrigger("Fade");
        saveData.checkpointLocation = new Vector2(0,0);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void reloadLevel()
    {
        fader.SetTrigger("Fade");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
