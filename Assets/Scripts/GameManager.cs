using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton for convenient access (optional pattern)
    public static GameManager Instance { get; private set; }

    // Number of times the player has been hit
    [SerializeField] private int hits = 0;

    // Public variable for UI to read the converted letter grade
    public string CurrentLetterGrade = "A+";

    // Whether the game has ended
    private bool isGameOver = false;

    // Public read-only accessor for other systems (UI, etc.)
    public bool IsGameOver => isGameOver;

    // Expose hits count if needed by other systems
    public int Hits => hits;

    // Predefined grade steps up to F. After F, additional hits produce F-, F--, ...
    private static readonly string[] GradeSteps =
    {
        "A+", "A", "A-", 
        "B+", "B", "B-", 
        "C+", "C", "C-", 
        "D+", "D", "D-",
        "F"
    };

    private void Awake()
    {
        // Simple singleton initialization. If you want the GameManager to persist across scenes,
        // add DontDestroyOnLoad(gameObject) here — but be mindful of ResetGame() / scene reload behavior.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        UpdateLetterGrade();
    }

    // Public: call this to register that the player was hit once
    public void RegisterPlayerHit()
    {
        if (isGameOver) return;

        hits++;
        UpdateLetterGrade();
    }

    // Converts hits -> letter grade and writes to CurrentLetterGrade (for UI)
    private void UpdateLetterGrade()
    {
        int fIndex = GradeSteps.Length - 1; // index of "F"
        if (hits <= fIndex)
        {
            CurrentLetterGrade = GradeSteps[hits];
            return;
        }

        // For hits beyond index of F, produce F-, F--, F---, ...
        int extraDashes = hits - fIndex;
        CurrentLetterGrade = "F" + new string('-', extraDashes);
    }

    // Public: end the game (stops further hit counting). Adjust as needed (UI, scoreboard, etc.)
    public void EndGame()
    {
        if (isGameOver) return;
        isGameOver = true;

        // Pause the game; UI or other systems can respond to isGameOver / CurrentLetterGrade
        Time.timeScale = 0f;
    }

    // Public: reset the scene and game state
    public void ResetGame()
    {
        // Ensure normal time scale before reloading
        Time.timeScale = 1f;

        // Optionally reset local state (scene reload will recreate objects)
        hits = 0;
        CurrentLetterGrade = "A+";
        isGameOver = false;

        // Reload the active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
