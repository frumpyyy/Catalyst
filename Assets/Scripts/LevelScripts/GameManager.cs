using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //game manager should be a singleton instance, only one ever active
    public static GameManager m_instance { get; private set; }

    //level state enum to describe the current state of the game, won, lost, in play
    public enum LevelState
    {
        InPlay,
        Won,
        Lost
    };

    //current state var used mainly for UI and scene loading, for example when game is won, ui will be notified from update game state func and show win screen
    public LevelState m_gameState { get; private set; }

    /// <summary>
    /// checking for a duplicate gamemanager, if there is one that isnt the current manager then delete the current manager
    /// </summary>
    private void Awake()
    {
        if (m_instance != null && m_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        m_instance = this;

        //allows for score tracking and play state between scenes (levels in this case)
        DontDestroyOnLoad(gameObject);
    }

    public void UpdateGameState(LevelState newState)
    {
        m_gameState = newState;
        UIManager.m_instance?.StateChanged(newState);
    }

    public void LoadNext()
    {
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        int totalScenes = SceneManager.sceneCountInBuildSettings;

        int nextIndex = buildIndex + 1;

        if (nextIndex >= totalScenes)
            nextIndex = 0;

        SceneManager.LoadScene(nextIndex);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
