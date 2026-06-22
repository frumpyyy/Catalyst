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
