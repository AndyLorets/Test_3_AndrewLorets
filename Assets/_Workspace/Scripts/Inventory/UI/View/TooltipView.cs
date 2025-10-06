using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections;

/// <summary>
/// [VIEW] - MonoBehaviour, ����������� ����������� ���������� (��������).
/// �������� ��:
/// 1. ����������� ���������� � ��������.
/// 2. �������������� ������ ������ ������� � ����������� �� ����� ������.
/// 3. ���������������� ���� �� ������ ����� � ��������, ������� ������ �� �������.
/// 4. ������� �������� ��������� � ������������.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class TooltipView : MonoBehaviour
{
    [Header("UI ��������")]
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private RectTransform _backgroundRect;

    [Header("��������� ����������")]
    [SerializeField, Tooltip("������� ������ ������ (X, Y)")] private Vector2 _padding = new Vector2(20, 20); 
    [SerializeField, Tooltip("���������� ����� ���������� � ���������")] private float _spacing = 10f;                 
    [SerializeField, Tooltip("������������ ������ ������")] private float _maxWidth = 300f;               
    [SerializeField, Tooltip("�������� �� �������")] private Vector2 _offset = new Vector2(15, -15); 

    private CanvasGroup _canvasGroup;
    private Coroutine _activeCoroutine;
    private ItemData _currentItem;

    /// <summary>
    /// ������������� ����������.
    /// </summary>
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_backgroundRect == null) _backgroundRect = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// ��������� ����� ��� ������/���������� ������� � ������� � ��������.
    /// </summary>
    public void Show(ItemData itemData)
    {
        if (_currentItem == itemData && gameObject.activeSelf) return;
        _currentItem = itemData;

        if (_activeCoroutine != null) StopCoroutine(_activeCoroutine);

        gameObject.SetActive(true);
        _activeCoroutine = StartCoroutine(ShowRoutine(itemData));
    }

    /// <summary>
    /// ��������, ���������� �� ������� ������� �����������:
    /// ������ �������� ������ �����, ��������� �������, ������������� ������, ������������� � ������ ����������.
    /// </summary>
    private IEnumerator ShowRoutine(ItemData itemData)
    {
        if (_canvasGroup.alpha > 0)
        {
            yield return _canvasGroup.DOFade(0f, 0.15f).WaitForCompletion();
        }

        _titleText.text = $"<color=green>{itemData.Name}</color>";
        _descriptionText.text = itemData.Description;

        float textWidth = _maxWidth - _padding.x * 2;
        _titleText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, textWidth);
        _descriptionText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, textWidth);

        _titleText.ForceMeshUpdate();
        _descriptionText.ForceMeshUpdate();

        Vector2 titleSize = _titleText.GetPreferredValues(textWidth, float.PositiveInfinity);
        Vector2 descriptionSize = _descriptionText.GetPreferredValues(textWidth, float.PositiveInfinity);

        float finalHeight = titleSize.y + descriptionSize.y + _spacing + _padding.y * 2;
        _backgroundRect.sizeDelta = new Vector2(_maxWidth, finalHeight);

        _titleText.rectTransform.anchoredPosition = new Vector2(_padding.x, -_padding.y);
        _descriptionText.rectTransform.anchoredPosition = new Vector2(_padding.x, -_padding.y - titleSize.y - _spacing);

        yield return null;

        Reposition();
        yield return _canvasGroup.DOFade(1f, 0.15f).WaitForCompletion();
        _activeCoroutine = null;
    }

    /// <summary>
    /// ��������� ����� ��� ������� �������.
    /// </summary>
    public void Hide()
    {
        if (!gameObject.activeSelf) return;

        _currentItem = null;
        if (_activeCoroutine != null) StopCoroutine(_activeCoroutine);
        _activeCoroutine = StartCoroutine(HideRoutine());
    }

    /// <summary>
    /// ��������, ������ ���������� ������ � �������������� GameObject.
    /// </summary>
    private IEnumerator HideRoutine()
    {
        yield return _canvasGroup.DOFade(0f, 0.15f).WaitForCompletion();
        gameObject.SetActive(false);
        _activeCoroutine = null;
    }

    /// <summary>
    /// ��������� ������� ������� ����� � ��������, �� ����� ��� ����� �� ������� ������.
    /// </summary>
    private void Reposition()
    {
        Vector2 finalPosition = (Vector2)Input.mousePosition + _offset;

        float width = _backgroundRect.rect.width;
        float height = _backgroundRect.rect.height;

        if (finalPosition.x + width > Screen.width) finalPosition.x = Screen.width - width;
        if (finalPosition.x < 0) finalPosition.x = 0;

        if (finalPosition.y - height < 0) finalPosition.y = height;
        if (finalPosition.y > Screen.height) finalPosition.y = Screen.height;

        _backgroundRect.position = finalPosition;
    }
}