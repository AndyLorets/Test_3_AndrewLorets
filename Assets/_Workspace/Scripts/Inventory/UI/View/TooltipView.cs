using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections;

/// <summary>
/// [VIEW] - Управляет всплывающей подсказкой. Рассчитывает свой размер и позицию.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class TooltipView : MonoBehaviour
{
    [Header("UI Элементы")]
    [Tooltip("Единственный текстовый элемент для всего содержимого")]
    [SerializeField] private TextMeshProUGUI _tooltipText;
    [Tooltip("RectTransform фона, чей размер мы будем менять")]
    [SerializeField] private RectTransform _backgroundRect;

    [Header("Настройки")]
    [Tooltip("Внутренние отступы от края фона до текста")]
    [SerializeField] private Vector2 _padding = new Vector2(20, 20);
    [Tooltip("Смещение от курсора мыши")]
    [SerializeField] private Vector2 _offset = new Vector2(15, -15);
    [Tooltip("Максимальная ширина тултипа. Текст будет переноситься, если превысит ее.")]
    [SerializeField] private float _maxWidth = 300f;
    [Tooltip("Длительность анимации появления/исчезновения")]
    [SerializeField] private float _fadeDuration = 0.15f;

    private CanvasGroup _canvasGroup;
    private Coroutine _activeCoroutine;
    private ItemData _currentItem; // Для оптимизации

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_backgroundRect == null) _backgroundRect = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Показывает тултип с информацией из ItemData.
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
    /// Плавно скрывает тултип.
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
    /// Позиционирует тултип рядом с курсором, не давая ему выйти за пределы экрана.
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