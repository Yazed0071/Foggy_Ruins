using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseMenu : MonoBehaviour
{
    [SerializeField] private GameObject loserMenuUI;
    [SerializeField] private GuardianHealth guardianHealth; // assign in Inspector or auto-find

    private bool gamePaused = false;
    private bool isShowing = false;

    private void Awake()
    {
        // Optional auto-find if not assigned in inspector
        if (guardianHealth == null)
            guardianHealth = FindFirstObjectByType<GuardianHealth>();
    }

    private void Start()
    {
        Show(false); // make sure menu is hidden at start
    }

    private void Update()
    {
        if (guardianHealth == null) return;

        // Show once when lose condition is true
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
        // restore timescale/audio before reload to avoid carry-over issues
        Time.timeScale = 1f;
        AudioListener.pause = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
