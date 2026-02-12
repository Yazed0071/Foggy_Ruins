using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseMenu : MonoBehaviour
{
    [SerializeField] private GameObject loserMenuUI;
    [SerializeField] private GuardianHealth guardianHealth;

    private bool gamePaused = false;
    private bool isShowing = false;

    private bool GamePaused = false;
    private bool isShowingCredits = false;

    private void Awake()
    {
        if (guardianHealth == null)
            guardianHealth = FindFirstObjectByType<GuardianHealth>();
    }

    private void Start()
    {
        Show(false);
    }

    private void Update()
    {
        if (guardianHealth == null) return;

        if (!isShowing && guardianHealth.IsShowLoseMenu())
        {
            Show(true);
        }
    }

    private void Show(bool show)
    {
        isShowing = show;

        if (loserMenuUI != null)
            loserMenuUI.SetActive(show);

        gamePaused = show;
        Time.timeScale = show ? 0f : 1f;
        AudioListener.pause = gamePaused;
    }

    [SerializeField] private CreditsVideoController creditsVideo;
    public void Credits()
    {
        AudioListener.pause = true;

        loserMenuUI.SetActive(false);
        GamePaused = false;

        isShowingCredits = true;
        creditsVideo.PlayVideo();
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

    public void Retry()
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
