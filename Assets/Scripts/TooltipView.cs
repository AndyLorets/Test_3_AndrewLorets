using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class TooltipView : MonoBehaviour
{
    [Header("UI Элементы")]
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private RectTransform _backgroundRect; // Ссылка на RectTransform самого TooltipPanel

    [Header("Настройки компоновки")]
    [SerializeField] private Vector2 _padding = new Vector2(20, 20); // Отступы внутри панели (X, Y)
    [SerializeField] private float _spacing = 10f; // Расстояние между заголовком и описанием
    [SerializeField] private float _maxWidth = 300f; // Максимальная ширина панели
    [SerializeField] private Vector2 _offset = new Vector2(15, -15); // Смещение от курсора

    private CanvasGroup _canvasGroup;
    private Coroutine _activeCoroutine;
    private ItemData _currentItem;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_backgroundRect == null) _backgroundRect = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }
    
    public void Show(ItemData itemData)
    {
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
            yield return _canvasGroup.DOFade(0f, 0.15f).WaitForCompletion();
        }

        _titleText.text = $"<color=green>{itemData.Name}</color>";
        _descriptionText.text = itemData.Description;

        float textWidth = _maxWidth - _padding.x * 2; // Учтем отступы с обеих сторон
        _titleText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, textWidth);
        _descriptionText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, textWidth);

        _titleText.ForceMeshUpdate();
        _descriptionText.ForceMeshUpdate();

        // Спрашиваем, какой размер был бы предпочтительным при заданной ширине
        Vector2 titleSize = _titleText.GetPreferredValues(textWidth, float.PositiveInfinity);
        Vector2 descriptionSize = _descriptionText.GetPreferredValues(textWidth, float.PositiveInfinity);
        // -------------------------

        float finalHeight = titleSize.y + descriptionSize.y + _spacing + _padding.y * 2; // Учтем отступы сверху и снизу
        _backgroundRect.sizeDelta = new Vector2(_maxWidth, finalHeight);

        // Позиционируем текст внутри фона с учетом отступов
        _titleText.rectTransform.anchoredPosition = new Vector2(_padding.x, -_padding.y);
        _descriptionText.rectTransform.anchoredPosition = new Vector2(_padding.x, -_padding.y - titleSize.y - _spacing);

        yield return null;

        Reposition();
        yield return _canvasGroup.DOFade(1f, 0.15f).WaitForCompletion();
        _activeCoroutine = null;
    }


    public void Hide()
    {
        if (!gameObject.activeSelf) return;

        _currentItem = null;
        if (_activeCoroutine != null) StopCoroutine(_activeCoroutine);
        _activeCoroutine = StartCoroutine(HideRoutine());
    }

    private IEnumerator HideRoutine()
    {
        yield return _canvasGroup.DOFade(0f, 0.15f).WaitForCompletion();
        gameObject.SetActive(false);
        _activeCoroutine = null;
    }

    private void Reposition()
    {
        // Наш pivot уже в верхнем левом углу, расчеты становятся проще
        Vector2 finalPosition = (Vector2)Input.mousePosition + _offset;
        
        float width = _backgroundRect.rect.width;
        float height = _backgroundRect.rect.height;

        if (finalPosition.x + width > Screen.width)
            finalPosition.x = Screen.width - width;
        if (finalPosition.x < 0)
            finalPosition.x = 0;

        if (finalPosition.y - height < 0)
            finalPosition.y = height;
        if (finalPosition.y > Screen.height)
            finalPosition.y = Screen.height;
        
        _backgroundRect.position = finalPosition;
    }
}