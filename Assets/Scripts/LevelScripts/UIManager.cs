using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager m_instance { get; private set; }

    [SerializeField] private GameObject _winUI;
    [SerializeField] private GameObject _loseUI;
    [SerializeField] private GameObject _ammoUI;
    [SerializeField] private TextMeshProUGUI _ammoText;

    [SerializeField] private GameObject _comboUI;
    [SerializeField] private CanvasGroup _comboCanvasGroup;
    [SerializeField] private TextMeshProUGUI _comboText;
    [SerializeField] private ParticleSystem _comboParticles;


    private Coroutine _comboRoutine;


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


        _comboUI.SetActive(true);
        _comboCanvasGroup.interactable = false;
        _comboCanvasGroup.blocksRaycasts = false;
        HideComboUI();

    }

    private void HideComboUI()
    {
        _comboCanvasGroup.alpha = 0f;
        _comboParticles.Stop();
        _comboParticles.Clear();
    }

    private void OnEnable()
    {
        LevelManager.OnShot += UpdateAmmoCounter;
    }

    private void OnDisable()
    {
        LevelManager.OnShot -= UpdateAmmoCounter;
    }


    public void StateChanged(GameManager.LevelState newState)
    {
        bool gameWon = newState == GameManager.LevelState.Won;
        bool gameLost = newState == GameManager.LevelState.Lost;

        _ammoUI.SetActive(false);
        HideComboUI();
        _winUI.SetActive(gameWon);
        _loseUI.SetActive(gameLost);
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

    public void UpdateAmmoCounter(int ammo)
    {
        _ammoText.text = $"Ammo: {ammo}";
    }

    public void UpdateComboChain(int chain)
    {

        if (chain == 0)
        {
            HideComboUI();
            return;
        }

        _comboUI.SetActive(true);

        string cText = chain switch
        {
            < 3 => $"COMBO x{chain}",
            < 6 => $"GREAT! x{chain}",
            < 9 => $"AWESOME! x{chain}",
            < 12 => $"INCREDIBLE! x{chain}",
            < 15 => $"UNSTOPPABLE! x{chain}",
            _ => $"CATALYTIC! x{chain}"
        };

        _comboText.text = $"{cText}";
        _comboParticles.Play();

        if (_comboRoutine != null) StopCoroutine(_comboRoutine);


        _comboRoutine = StartCoroutine(ComboAnimation());
    }

    private IEnumerator ComboAnimation()
    {
        RectTransform rectTransform = _comboText.GetComponent<RectTransform>();

        _comboCanvasGroup.alpha = 1.0f;

        rectTransform.localScale = Vector3.zero;

        Vector2 startPosition = rectTransform.anchoredPosition;
        Vector2 endPosition = startPosition + Vector2.up * 15.0f;

        float duration = 0.4f;
        float timeOffset = 0.0f;

        while (timeOffset < duration)
        {
            timeOffset += Time.deltaTime;

            float n = timeOffset / duration;

            float move = Mathf.SmoothStep(0f, 1.0f, n);

            float scale = Mathf.Lerp(0.8f, 1.0f, move);

            rectTransform.localScale = Vector3.one * scale;

            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, move);

            yield return null;

        }

    }

}
