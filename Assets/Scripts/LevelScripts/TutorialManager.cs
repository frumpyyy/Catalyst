using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup _hintCanvas;
    [SerializeField] private TextMeshProUGUI _hintText;
    [SerializeField] private float _hintFadeDuration = 0.5f;
    [SerializeField] private GameObject _atomPrefab;

    private Coroutine _fadeRoutine;
    private Coroutine _watcherRoutine;

    private enum TutorialState
    {
        Def,
        fired,
        moved,
        fused
    };

    private TutorialState _state;

    private void Start()
    {
        Atom.m_atomFused += UserFusion;
        UpdateHint("Aim by moving your mouse");
        _watcherRoutine = StartCoroutine(TutorialWatcher());
    }

    private void OnDestroy()
    {
        Atom.m_atomFused -= UserFusion;
    }

    private void UpdateHint(string hint)
    {
        if (_fadeRoutine != null)
            StopCoroutine(_fadeRoutine);

        _hintText.text = hint;

        _fadeRoutine = StartCoroutine(FadeRoutine(1.0f));
    }

    private IEnumerator TutorialWatcher()
    {
        yield return new WaitUntil(() => Mouse.current.delta.ReadValue().magnitude > 0.0f);
        if (_state == TutorialState.Def) UpdateHint("Use LEFT CLICK to fire a catalyst");

        yield return new WaitUntil(() => _state == TutorialState.fired);
        UpdateHint("Use RIGHT CLICK to reposition the catalyst launcher");

        yield return new WaitUntil(() => _state == TutorialState.moved);
        UpdateHint("Hit atoms together to cause a chain reaction! Atoms must first be hit by a catalyst to become primed for a merge (purple). Once merged they become explosive (red) and are actived by any atom collision!");

        GameObject newCatalyst = Instantiate(_atomPrefab, new Vector3(-2.0f, 0.0f, 0.0f), Quaternion.identity);
        GameObject newCatalyst1 = Instantiate(_atomPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        GameObject newCatalyst2 = Instantiate(_atomPrefab, new Vector3(2.0f, 0.0f, 0.0f), Quaternion.identity);

        yield return new WaitUntil(() => _state == TutorialState.fused);
        UpdateHint("When an explosion happens all nearby atoms become primed to merge leading to some cool combos! The goal is to explode all atoms leaving just one that can be exploded by shooting it directly.");


    }

    private IEnumerator FadeRoutine(float alphaTarget)
    {
        float startAlpha = _hintCanvas.alpha;
        float fadeTimer = 0.0f;

        while (fadeTimer < _hintFadeDuration)
        {
            fadeTimer += Time.deltaTime;
            _hintCanvas.alpha = Mathf.Lerp(startAlpha, alphaTarget, fadeTimer / _hintFadeDuration);
            yield return null;
        }
        _hintCanvas.alpha = alphaTarget;
        _fadeRoutine = null;
    }

    private IEnumerator FadeOut()
    {
        if (_fadeRoutine != null)
        {
            StopCoroutine(_fadeRoutine);
            _fadeRoutine = null;
        }

        _fadeRoutine = StartCoroutine(FadeRoutine(0.0f));

        yield return new WaitForSeconds(_hintFadeDuration);

        if (_watcherRoutine != null)
        {
            StopCoroutine(_watcherRoutine);
            _watcherRoutine = null;
        }
    }

    public void UserFired()
    {
        _state = TutorialState.fired;
    }

    public void UserMoved()
    {
        _state = TutorialState.moved;
    }

    public void UserFusion()
    {
        if (_state == TutorialState.fused) return;
        _state = TutorialState.fused;
        StartCoroutine(FadeOut());
    }

}
