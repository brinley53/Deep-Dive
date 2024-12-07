using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenuUI; // Reference to the main menu UI GameObject

    void Start()
    {
        // Ensure the main menu is visible at the start
        mainMenuUI.SetActive(true);
        Time.timeScale = 0; // Pause the game until the start button is pressed
    }

    public void StartGame()
    {
        // Hide the main menu and start the game
        mainMenuUI.SetActive(false);
        Time.timeScale = 1; // Resume the game
    }

    public void ExitGame()
    {
        Debug.Log("Exiting Game");
        Application.Quit();
    }
}