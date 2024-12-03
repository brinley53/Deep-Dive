using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject gameOverScreen;
    private CanvasGroup canvasGroup;
    private bool isPaused = false;

    void Start()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        canvasGroup = pauseMenu.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0; // Start with 0 opacity
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
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

    public void ShowGameOverScreen()
    {
        gameOverScreen.SetActive(true);
        Time.timeScale = 0;
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}