using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager m_instance { get; private set; }

    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject loseUI;

    [SerializeField] private GameObject comboUI;
    [SerializeField] private CanvasGroup comboCanvasGroup;
    [SerializeField] private TextMeshProUGUI comboText;


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

    public void UpdateComboChain(int chain)
    {
        if (chain == 0)
        {
            StopAllCoroutines();
            return;
        }


        comboUI.SetActive(true);

        string cText = chain switch
        {
            < 3 => $"COMBO x{chain}",
            < 6 => $"GREAT! x{chain}",
            < 9 => $"AWESOME! x{chain}",
            < 12 => $"INCREDIBLE! x{chain}",
            < 15 => $"UNSTOPPABLE! x{chain}",
            _ => $"CATALYTIC! x{chain}"
        };

        comboText.text = $"{cText}";

        StopAllCoroutines();

        StartCoroutine(ComboAnimation());
    }

    private IEnumerator ComboAnimation()
    {
        RectTransform rectTransform = comboUI.GetComponent<RectTransform>();

        comboCanvasGroup.alpha = 1.0f;
        comboCanvasGroup.interactable = false;
        comboCanvasGroup.blocksRaycasts = false;

        rectTransform.localScale = Vector3.zero;

        Vector2 startPosition = rectTransform.anchoredPosition;
        Vector2 endPosition = startPosition + Vector2.up * 50.0f;

        float timeOffset = 0.0f;

        while (timeOffset < 1.0f)
        {
            timeOffset += Time.deltaTime * 2.0f;

            float easedEffect = Mathf.Sin(timeOffset * Mathf.PI * 0.5f);

            float overShoot = Mathf.Sin(timeOffset * Mathf.PI);
            rectTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 1.15f, overShoot);

            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, easedEffect);

            yield return null;

        }


        comboUI.SetActive(false);
        comboCanvasGroup.alpha = 0.0f;


    }

}
