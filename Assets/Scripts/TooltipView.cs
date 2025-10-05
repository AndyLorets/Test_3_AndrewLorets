using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class TooltipView : MonoBehaviour
{
    [Header("UI ��������")]
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private RectTransform _backgroundRect; // ������ �� RectTransform ������ TooltipPanel

    [Header("��������� ����������")]
    [SerializeField] private Vector2 _padding = new Vector2(20, 20); // ������� ������ ������ (X, Y)
    [SerializeField] private float _spacing = 10f; // ���������� ����� ���������� � ���������
    [SerializeField] private float _maxWidth = 300f; // ������������ ������ ������
    [SerializeField] private Vector2 _offset = new Vector2(15, -15); // �������� �� �������

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

        float textWidth = _maxWidth - _padding.x * 2; // ����� ������� � ����� ������
        _titleText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, textWidth);
        _descriptionText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, textWidth);

        _titleText.ForceMeshUpdate();
        _descriptionText.ForceMeshUpdate();

        // ����������, ����� ������ ��� �� ���������������� ��� �������� ������
        Vector2 titleSize = _titleText.GetPreferredValues(textWidth, float.PositiveInfinity);
        Vector2 descriptionSize = _descriptionText.GetPreferredValues(textWidth, float.PositiveInfinity);
        // -------------------------

        float finalHeight = titleSize.y + descriptionSize.y + _spacing + _padding.y * 2; // ����� ������� ������ � �����
        _backgroundRect.sizeDelta = new Vector2(_maxWidth, finalHeight);

        // ������������� ����� ������ ���� � ������ ��������
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
        // ��� pivot ��� � ������� ����� ����, ������� ���������� �����
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