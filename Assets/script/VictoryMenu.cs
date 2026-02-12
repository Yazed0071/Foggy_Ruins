using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryMenu : MonoBehaviour
{
    [SerializeField] private GameObject victoryMenuUI;

    private bool gamePaused = false;
    private bool isShowing = false;


    public void Show(bool show)
    {
        isShowing = show;

        if (victoryMenuUI != null)
            victoryMenuUI.SetActive(show);

        gamePaused = show;
        Time.timeScale = show ? 0f : 1f;
        AudioListener.pause = gamePaused;
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

    public void Restart()
    {
        RestartCurrentScene();
    }

    public void RestartCurrentScene()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}