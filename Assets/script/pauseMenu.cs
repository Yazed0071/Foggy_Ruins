using UnityEngine;

public class pauseMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject pauseMenuUI;

    [Header("Credits")]
    [SerializeField] private CreditsVideoController creditsVideo;

    private bool GamePaused = false;
    private bool isShowingCredits = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isShowingCredits)
            {
                CloseCredits();
                return;
            }

            if (GamePaused) Resume();
            else Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GamePaused = false;
        AudioListener.pause = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GamePaused = true;
        AudioListener.pause = true;
    }

    public void Credits()
    {
        AudioListener.pause = true;

        pauseMenuUI.SetActive(false);
        GamePaused = false;

        isShowingCredits = true;
        creditsVideo.PlayVideo();
    }

    public void CloseCredits()
    {
        creditsVideo.StopVideo(hide: true);
        isShowingCredits = false;

        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}
