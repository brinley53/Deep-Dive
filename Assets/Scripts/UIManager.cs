using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject loseMenu;
    public GameObject gameOverScreen;
    public GameObject mainMenuUI;
    private CanvasGroup canvasGroup;
    private bool isPaused = false;
    private bool isLose = false;
    private bool isMainMenuActive = true;

    void Start()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(false);
        mainMenuUI.SetActive(true);
        canvasGroup = pauseMenu.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        Debug.Log("Main menu should be active now.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            ToggleLoseMenu();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void ToggleLoseMenu()
    {
        if (loseMenu == null)
        {
            Debug.LogError("Lose menu is not assigned.");
            return;
        }
        isLose = !isLose;
        loseMenu.SetActive(isLose);
        Time.timeScale = isLose ? 0 : 1;

        Debug.Log($"Lose menu toggled. isLose: {isLose}");
    }

    public void ToggleMainMenu()
    {
        isMainMenuActive = !isMainMenuActive;
        mainMenuUI.SetActive(isMainMenuActive);
        Time.timeScale = isMainMenuActive ? 0 : 1;
    }

    private IEnumerator FadeIn()
    {
        float duration = 2f;
        float targetAlpha = 0.95f;
        float startAlpha = canvasGroup.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    public void QuitGame()
    {
        RestartGame();
        ToggleMainMenu();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}