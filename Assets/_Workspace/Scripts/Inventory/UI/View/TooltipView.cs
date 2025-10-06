using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections;

/// <summary>
/// [VIEW] - ��������� ����������� ����������. ������������ ���� ������ � �������.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class TooltipView : MonoBehaviour
{
    [Header("UI ��������")]
    [Tooltip("������������ ��������� ������� ��� ����� �����������")]
    [SerializeField] private TextMeshProUGUI _tooltipText;
    [Tooltip("RectTransform ����, ��� ������ �� ����� ������")]
    [SerializeField] private RectTransform _backgroundRect;

    [Header("���������")]
    [Tooltip("���������� ������� �� ���� ���� �� ������")]
    [SerializeField] private Vector2 _padding = new Vector2(20, 20);
    [Tooltip("�������� �� ������� ����")]
    [SerializeField] private Vector2 _offset = new Vector2(15, -15);
    [Tooltip("������������ ������ �������. ����� ����� ������������, ���� �������� ��.")]
    [SerializeField] private float _maxWidth = 300f;
    [Tooltip("������������ �������� ���������/������������")]
    [SerializeField] private float _fadeDuration = 0.15f;

    private CanvasGroup _canvasGroup;
    private Coroutine _activeCoroutine;
    private ItemData _currentItem; // ��� �����������

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_backgroundRect == null) _backgroundRect = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// ���������� ������ � ����������� �� ItemData.
    /// </summary>
    public void Show(ItemData itemData)
    {
        if (itemData == null) return;

        if (_currentItem == itemData && gameObject.activeSelf) return;
        _currentItem = itemData;

        if (_activeCoroutine != null) StopCoroutine(_activeCoroutine);

        gameObject.SetActive(true);
        _activeCoroutine = StartCoroutine(ShowRoutine(itemData));
    }

    private IEnumerator ShowRoutine(ItemData itemData)
    {
        if (_canvasGroup.alpha > 0)
        {
            yield return _canvasGroup.DOFade(0f, _fadeDuration).WaitForCompletion();
        }

        string content = $"<b><color=#00FF00>{itemData.Name}</color></b>\n\n{itemData.Description}";
        _tooltipText.text = content;

        float textWidth = _maxWidth - _padding.x * 2;
        Vector2 preferredSize = _tooltipText.GetPreferredValues(textWidth, float.PositiveInfinity);

        _backgroundRect.sizeDelta = new Vector2(_maxWidth, preferredSize.y + _padding.y * 2);

        yield return null;

        Reposition();

        _canvasGroup.DOFade(1f, _fadeDuration);
        transform.localScale = Vector3.one * 0.95f;
        transform.DOScale(1f, _fadeDuration * 2).SetEase(Ease.OutBack);

        _activeCoroutine = null;
    }

    /// <summary>
    /// ������ �������� ������.
    /// </summary>
    public void Hide()
    {
        if (!gameObject.activeSelf) return;

        _currentItem = null;
        if (_activeCoroutine != null) StopCoroutine(_activeCoroutine);
        _activeCoroutine = StartCoroutine(HideRoutine());
    }

    private IEnumerator HideRoutine()
    {
        transform.DOKill();
        _canvasGroup.DOKill();
        yield return _canvasGroup.DOFade(0f, _fadeDuration).WaitForCompletion();
        gameObject.SetActive(false);
        _activeCoroutine = null;
    }

    /// <summary>
    /// ������������� ������ ����� � ��������, �� ����� ��� ����� �� ������� ������.
    /// </summary>
    private void Reposition()
    {
        _backgroundRect.pivot = new Vector2(0, 1);

        Vector2 finalPosition = (Vector2)Input.mousePosition + _offset;

        float width = _backgroundRect.rect.width;
        float height = _backgroundRect.rect.height;

        if (finalPosition.x + width > Screen.width)
            finalPosition.x = Input.mousePosition.x - width - _offset.x;

        if (finalPosition.y - height < 0)
            finalPosition.y = Input.mousePosition.y + height - _offset.y;

        _backgroundRect.position = finalPosition;
    }
}