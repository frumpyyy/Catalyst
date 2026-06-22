using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager m_instance { get; private set; }

    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject loseUI;

    /// <summary>
    /// checking for a duplicate uimanager, if there is one that isnt the current manager then delete the current manager
    /// </summary>
    private void Awake()
    {
        if (m_instance != null && m_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        m_instance = this;
    }


    public void StateChanged(GameManager.LevelState newState)
    {
        //i was going to do a switch statement here however in the off chance that UI is incorrectly triggered the switch statement would only turn on
        //the win or lose ui not turning the opposite off, meaning code would have to be duplicated within the switch to perform this funcitonality.
        bool gameWon = newState == GameManager.LevelState.Won;
        bool gameLost = newState == GameManager.LevelState.Lost;

        winUI.SetActive(gameWon);
        loseUI.SetActive(gameLost);
    }

    //functions for UI buttons to restart lvl and skip to next
    public void Restart()
    {
        GameManager.m_instance.RestartLevel();
    }

    public void NextLevel()
    {

        GameManager.m_instance.LoadNext();
    }


}
