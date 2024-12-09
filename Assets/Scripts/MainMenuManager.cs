/*
MainMenuManager.cs
Description: Manages the main menu interactions in a Unity game, including starting the game, displaying high scores, and exiting the game.
Programmer: Gianni Louisa, Brinley Hull, Ben Renner, Connor Bennudriti, Kyle Moore
Date Created: 12/6/2024
Other sources of code: ChatGPT, Unity Documentation, Unity Forums, Youtube tutorials

Revisions:
- Added required comments (12/8/2024, Gianni-Louisa)
- Fix Scoreboard (12/8/2024, KyleMoore12)
- 1 high score (12/8/2024, KyleMoore12)
- Added main menu and updated exit buttons (12/6/2024, Gianni-Louisa)


Preconditions:
- The GameObject this script is attached to must have references to the main menu UI and scoreboard UI.
- 'highScoreText' must be assigned a valid TMP_Text component in the Unity Editor.

Postconditions:
- The main menu UI will be displayed or hidden based on user interactions.
- The game will start or pause based on user interactions.
- The high score will be displayed when requested.

Acceptable Input:
- 'highScoreText' should be a valid TMP_Text component.
- 'Scoreboard' and 'mainMenuUI' should be valid GameObjects.

Unacceptable Input:
- Null or unassigned 'highScoreText', 'Scoreboard', or 'mainMenuUI' will result in a null reference error.

Error and Exception Conditions:
- NullReferenceException if 'highScoreText', 'Scoreboard', or 'mainMenuUI' are not assigned.

Side Effects:
- Modifies the active state of UI elements.
- Changes the game's time scale.

Invariants:
- The game is paused when the main menu is active.
- The high score text is updated with the latest score from PlayerPrefs.

Known Faults:
- None documented.

*/

using UnityEngine;
using TMPro;

// Class: MainMenuManager
// Description: Handles the main menu operations such as starting the game, showing scores, and exiting.
public class MainMenuManager : MonoBehaviour
{
    public TMP_Text highScoreText; // UI element for displaying the high score
    public GameObject Scoreboard; // Reference to the scoreboard UI
    public GameObject mainMenuUI; // Reference to the main menu UI

    // Method: Start
    // Description: Initializes the main menu state and pauses the game.
    void Start()
    {
        float SavedHighScore; // Variable to store the saved high score
        Scoreboard.SetActive(false); // Hide the scoreboard initially
        mainMenuUI.SetActive(true); // Show the main menu UI
        Time.timeScale = 0; // Pause the game until the start button is pressed
    }

    // Method: StartGame
    // Description: Starts the game by hiding the main menu and resuming time.
    public void StartGame()
    {
        mainMenuUI.SetActive(false); // Hide the main menu UI
        Time.timeScale = 1; // Resume the game
    }

    // Method: OpenScores
    // Description: Displays the scoreboard and updates the high score text.
    public void OpenScores()
    {
        Debug.Log(PlayerPrefs.GetFloat("SavedHighScore")); // Log the saved high score for debugging
        Scoreboard.SetActive(true); // Show the scoreboard UI
        float temp = PlayerPrefs.GetFloat("SavedHighScore"); // Retrieve the saved high score
        highScoreText.text = "High Score: " + temp.ToString(); // Update the high score text
    }

    // Method: ExitGame
    // Description: Exits the game application.
    public void ExitGame()
    {
        Debug.Log("Exiting Game"); // Log the exit action for debugging
        Application.Quit(); // Quit the application
    }

    // Method: LeaveScores
    // Description: Hides the scoreboard UI.
    public void LeaveScores()
    {
        Scoreboard.SetActive(false); // Hide the scoreboard UI
    }
}