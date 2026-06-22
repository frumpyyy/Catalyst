using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager m_instance { get; private set; }

    private int _atomCount;

    private int _catalystCount;

    private void Awake()
    {
        if (m_instance != null && m_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        m_instance = this;
    }

    private void OnEnable()
    {
        Atom.atomFused += reactionCompleted;
    }

    private void OnDisable()
    {
        Atom.atomFused -= reactionCompleted;
    }

    public void Start()
    {
        GameManager.m_instance.UpdateGameState(GameManager.LevelState.InPlay);
    }

    public void atomRegister() => _atomCount++;

    public void reactionCompleted()
    {
        _atomCount--;

        if (_atomCount == 0)
            StartCoroutine(WinScreen());
    }

    private IEnumerator WinScreen()
    {
        yield return new WaitForSeconds(1.0f);
        GameManager.m_instance.UpdateGameState(GameManager.LevelState.Won);
    }

    private IEnumerator LossScreen()
    {
        yield return new WaitForSeconds(1.0f);
        GameManager.m_instance.UpdateGameState(GameManager.LevelState.Lost);
    }
}
