using UnityEngine;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public TMP_Text highScoreText;
    public GameObject Scoreboard; // Reference to the scoreboard
    public GameObject mainMenuUI; // Reference to the main menu UI GameObject

    void Start()
    {
        float SavedHighScore;
        // Ensure the main menu is visible at the start
        Scoreboard.SetActive(false);
        mainMenuUI.SetActive(true);
        Time.timeScale = 0; // Pause the game until the start button is pressed
    }

    public void StartGame()
    {
        // Hide the main menu and start the game
        mainMenuUI.SetActive(false);
        Time.timeScale = 1; // Resume the game
    }

    public void OpenScores()
    {
        Debug.Log(PlayerPrefs.GetFloat("SavedHighScore"));
        Scoreboard.SetActive(true);
        float temp = PlayerPrefs.GetFloat("SavedHighScore");
        highScoreText.text = temp.ToString();
    }
    public void ExitGame()
    {
        Debug.Log("Exiting Game");
        Application.Quit();
    }
    public void LeaveScores()
    {
        Scoreboard.SetActive(false);
    }
}