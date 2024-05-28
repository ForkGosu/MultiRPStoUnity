using System.Collections;
using UnityEngine;
// using UnityEngine.Localization.Components; // 현재 Localization 비활성
using TMPro;


public class AlertOnceBase : MonoBehaviour
{
    private RectTransform m_rectTransform;
    private CanvasGroup m_canvasGroup;

    // public LocalizeStringEvent m_localizedString;
    public TextMeshProUGUI m_contentText;


    private Color m_originalTextColor;
    private Color m_originalImageColor;

    private void Awake()
    {
        m_rectTransform = GetComponent<RectTransform>();
        m_canvasGroup = GetComponent<CanvasGroup>();
    }
    public void Alert(string _context)
    {
        // m_localizedString.SetTable(null);
        m_contentText.text = _context;
    }

    public void LocalizationAlert(string _entry)
    {
        // m_localizedString.SetEntry(_entry);
    }

    public void Init()
    {
        m_rectTransform.anchoredPosition = Vector2.zero;
        m_canvasGroup.alpha = 1f;
    }

    private void OnEnable()
    {
        Init();
        StartCoroutine(AnimateUIElements());
    }

    private void OnDisable()
    {
        StopCoroutine(AnimateUIElements());
    }

    private IEnumerator AnimateUIElements()
    {
        yield return new WaitForSeconds(0.5f);
        // 각 애니메이션 코루틴 시작
        Coroutine move = StartCoroutine(MoveY(m_rectTransform, 100f, 0.8f));
        Coroutine fadeOut = StartCoroutine(FadeCanvasGroup(m_canvasGroup, 0.0f, 0.8f));

        // 모든 코루틴이 완료될 때까지 기다림
        yield return move;
        yield return fadeOut;

        // 모든 애니메이션이 완료된 후 실행될 동작
        AlertOnceManager.Instance.m_objectPool.Release(this);
    }

    private IEnumerator MoveY(RectTransform rectTransform, float targetY, float duration)
    {
        Vector3 startPosition = rectTransform.localPosition;
        Vector3 endPosition = new Vector3(startPosition.x, targetY, startPosition.z);
        float time = 0;

        while (time < duration)
        {
            rectTransform.localPosition = Vector3.Lerp(startPosition, endPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        rectTransform.localPosition = endPosition;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0;

        while (time < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }
}