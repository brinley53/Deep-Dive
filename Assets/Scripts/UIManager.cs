/*
UIManager.cs
Description: This script manages the user interface elements such as pause menu, lose menu, and main menu in a Unity game.
Programmer: Gianni Louisa, Brinley Hull, Ben Renner, Connor Bennudriti, Kyle Moore
Date Created: 12/6/2024
Other sources of code: ChatGPT, Unity Documentation, Unity Forums, Youtube tutorials

Revisions:
- Added required comments (12/8/2024, Gianni-Louisa)
- 1 high score (12/8/2024, KyleMoore12)
- Pushed change to fix my stupidness (12/7/2024, Gianni-Louisa)
- Added background music and fixed some errors with player object not being referenced (12/7/2024, cbennudr)
- Added main menu and updated exit buttons (12/6/2024, Gianni-Louisa)
- Added lose and pause screen (12/6/2024, Gianni-Louisa)


Preconditions:
- The GameObject this script is attached to must have references to the UI elements (pauseMenu, loseMenu, gameOverScreen, mainMenuUI).
- The 'pauseMenu' must have a CanvasGroup component for fade effects.

Postconditions:
- The UI elements will be shown or hidden based on user interactions.
- The game will pause or resume based on user interactions.

Acceptable Input:
- 'pauseMenu', 'loseMenu', 'gameOverScreen', and 'mainMenuUI' should be valid GameObjects.
- 'canvasGroup' should be a valid CanvasGroup component.

Unacceptable Input:
- Null or unassigned UI elements will result in a null reference error.

Error and Exception Conditions:
- NullReferenceException if UI elements are not assigned.

Side Effects:
- Modifies the active state of UI elements.
- Changes the game's time scale.

Invariants:
- The game is paused when the pause menu or lose menu is active.
- The main menu is active at the start of the game.

Known Faults:
- None documented.

*/

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

// Class: UIManager
// Description: Manages the display and interaction of UI elements in the game.
public class UIManager : MonoBehaviour
{
    public GameObject pauseMenu; // UI element for the pause menu
    public GameObject loseMenu; // UI element for the lose menu
    public GameObject gameOverScreen; // UI element for the game over screen
    public GameObject mainMenuUI; // UI element for the main menu
    private CanvasGroup canvasGroup; // CanvasGroup for fade effects on the pause menu
    private bool isPaused = false; // Tracks if the game is currently paused
    private bool isLose = false; // Tracks if the lose menu is currently active
    private bool isMainMenuActive = true; // Tracks if the main menu is currently active

    // Method: Start
    // Description: Initializes the UI state and sets the main menu active.
    void Start()
    {
        Time.timeScale = 0; // Pause the game initially
        pauseMenu.SetActive(false); // Hide the pause menu
        mainMenuUI.SetActive(true); // Show the main menu
        canvasGroup = pauseMenu.GetComponent<CanvasGroup>(); // Get the CanvasGroup component
        canvasGroup.alpha = 0; // Set the initial alpha for fade effect
        Debug.Log("Main menu should be active now."); // Log the initial state
    }

    // Method: Update
    // Description: Checks for user input to toggle menus.
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // Check for Escape key press
        {
            TogglePause(); // Toggle the pause menu
        }
        if (Input.GetKeyDown(KeyCode.K)) // Check for K key press
        {
            ToggleLoseMenu(); // Toggle the lose menu
        }
    }

    // Method: TogglePause
    // Description: Toggles the pause menu and game state.
    public void TogglePause()
    {
        isPaused = !isPaused; // Toggle the paused state
        pauseMenu.SetActive(isPaused); // Show or hide the pause menu
        Time.timeScale = isPaused ? 0 : 1; // Pause or resume the game
    }

    // Method: ToggleLoseMenu
    // Description: Toggles the lose menu and game state.
    public void ToggleLoseMenu()
    {
        if (loseMenu == null) // Check if loseMenu is assigned
        {
            Debug.LogError("Lose menu is not assigned."); // Log error if not assigned
            return; // Exit the method
        }
        isLose = !isLose; // Toggle the lose state
        loseMenu.SetActive(isLose); // Show or hide the lose menu
        Time.timeScale = isLose ? 0 : 1; // Pause or resume the game

        Debug.Log($"Lose menu toggled. isLose: {isLose}"); // Log the lose state
    }

    // Method: ToggleMainMenu
    // Description: Toggles the main menu and game state.
    public void ToggleMainMenu()
    {
        isMainMenuActive = !isMainMenuActive; // Toggle the main menu state
        mainMenuUI.SetActive(isMainMenuActive); // Show or hide the main menu
        Time.timeScale = isMainMenuActive ? 0 : 1; // Pause or resume the game
    }

    // Method: FadeIn
    // Description: Coroutine to fade in the pause menu.
    private IEnumerator FadeIn()
    {
        float duration = 2f; // Duration of the fade effect
        float targetAlpha = 0.95f; // Target alpha value for the fade
        float startAlpha = canvasGroup.alpha; // Initial alpha value
        float elapsedTime = 0f; // Elapsed time for the fade

        while (elapsedTime < duration) // Loop until the fade is complete
        {
            elapsedTime += Time.unscaledDeltaTime; // Increment elapsed time
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration); // Lerp the alpha value
            yield return null; // Wait for the next frame
        }

        canvasGroup.alpha = targetAlpha; // Ensure the final alpha is set
    }

    // Method: QuitGame
    // Description: Quits the game and returns to the main menu.
    public void QuitGame()
    {
        RestartGame(); // Restart the game
        ToggleMainMenu(); // Show the main menu
    }

    // Method: RestartGame
    // Description: Restarts the current game scene.
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
    }

    // Method: ReturnToMainMenu
    // Description: Loads the main menu scene.
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Load the main menu scene
    }
}