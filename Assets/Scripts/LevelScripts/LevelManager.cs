using System;
using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager m_instance { get; private set; }

    public static event Action<int> OnShot;

    private int _atomCount;

    private int _comboChain;
    private Coroutine _chainMultiResetCoroutine;

    [SerializeField] private int _catalystsAllowed = 3;
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
        Atom.m_atomFused += reactionCompleted;
    }

    private void OnDisable()
    {
        Atom.m_atomFused -= reactionCompleted;
    }

    public void Start()
    {
        _catalystCount = _catalystsAllowed;
        GameManager.m_instance.UpdateGameState(GameManager.LevelState.InPlay);
        OnShot?.Invoke(_catalystCount);
    }

    public void atomRegister() => _atomCount++;

    public bool TryShoot()
    {
        if (_catalystCount <= 0) return false;

        _catalystCount--;
        OnShot?.Invoke(_catalystCount);

        if (_catalystCount <= 0)
            StartCoroutine(CheckLoss());

        return true;
    }

    public void reactionCompleted()
    {
        _atomCount--;
        _comboChain++;

        UIManager.m_instance.UpdateComboChain(_comboChain);

        if (_chainMultiResetCoroutine != null)
            StopCoroutine(_chainMultiResetCoroutine);

        _chainMultiResetCoroutine = StartCoroutine(ResetChainMultiplier());

        if (_atomCount <= 0)
        {
            StartCoroutine(WinScreen());
            return;
        }

    }

    public bool isLastAtom() => _atomCount == 1;

    private IEnumerator ResetChainMultiplier()
    {
        yield return new WaitForSeconds(2.0f);
        _comboChain = 0;
        UIManager.m_instance.UpdateComboChain(_comboChain);
    }

    private IEnumerator WinScreen()
    {
        yield return new WaitForSeconds(1.0f);
        GameManager.m_instance.UpdateGameState(GameManager.LevelState.Won);
    }

    private IEnumerator CheckLoss()
    {
        yield return new WaitForSeconds(3.0f);//slight grace period for explosions to occur etc
        if (GameManager.m_instance.m_gameState == GameManager.LevelState.InPlay)
            GameManager.m_instance.UpdateGameState(GameManager.LevelState.Lost);
    }
}
