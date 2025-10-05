using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using DG.Tweening; // ���������� DOTween ��� ������� ���������

public class InventorySlotView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
                                IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    // --- ������ ���� ---
    [SerializeField] private Image _background;
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TextMeshProUGUI _quantityText;

    // --- ����� ��������� ��� ���������� �������� ---
    [Serializable]
    public struct SlotStateVisuals
    {
        public Color BackgroundColor;
        // ���� ����� �������� � ������ ���������, ��������, ���� �����, ������ � �.�.
    }

    [Header("��������� ���������")]
    [SerializeField] private SlotStateVisuals _normalState;
    [SerializeField] private SlotStateVisuals _hoveredState;
    [SerializeField] private SlotStateVisuals _selectedState;
    [SerializeField] private SlotStateVisuals _emptyState;
    [SerializeField] private float _colorFadeDuration = 0.1f;

    public int SlotIndex { get; private set; }

    // --- ������ ������� ---
    public event Action<int> OnPointerEnterEvent;
    public event Action OnPointerExitEvent;
    public event Action<int> OnBeginDragEvent;
    public event Action OnEndDragEvent;
    public event Action<int> OnDoubleClickEvent;

    // --- ����� ���� ��� ���������� ���������� ---
    private bool _isPointerOver;
    private bool _isSelected;
    private bool _isEmpty = true; // �������� � �������������, ��� ���� ����

    public void Init(int index)
    {
        SlotIndex = index;
    }

    public void UpdateView(InventorySlot slot)
    {
        _isEmpty = slot.IsEmpty;

        if (_isEmpty)
        {
            _itemIcon.gameObject.SetActive(false);
            _quantityText.gameObject.SetActive(false);
        }
        else
        {
            _itemIcon.gameObject.SetActive(true);
            _itemIcon.sprite = slot.ItemData.Icon;

            bool textActive = slot.ItemData.CanStack && slot.Quantity > 1;
            _quantityText.gameObject.SetActive(textActive);
            if (textActive)
            {
                _quantityText.text = slot.Quantity.ToString();
            }
        }

        // ����� ���������� ������, ��������� � ���������� ���������
        UpdateVisualState();
    }
    private void UpdateVisualState()
    {
        SlotStateVisuals targetState;

        // ����������, ����� �� ������ ������������ ���� ���� ��� ���������
        // �����, ����:
        // 1. ���� �� ������ (������� ���������).
        // ���
        // 2. ���� �������������� (����� ������� �� ����� ����, ����� ������� �������).
        bool canBeHovered = !_isEmpty || InventoryController.IsDragging;

        // ������ �����������: Selected > Hovered > Empty/Normal
        if (_isSelected)
        {
            targetState = _selectedState;
        }
        else if (_isPointerOver && canBeHovered) 
        {
            targetState = _hoveredState;
        }
        else
        {
            targetState = _isEmpty ? _emptyState : _normalState;
        }

        _background.DOColor(targetState.BackgroundColor, _colorFadeDuration);
    }

    public void SetSelected(bool isSelected)
    {
        _isSelected = isSelected;
        UpdateVisualState();
    }

    #region Pointer Events (��������� ��� ������ � �����������)

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isPointerOver = true;
        UpdateVisualState();
        OnPointerEnterEvent?.Invoke(SlotIndex);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isPointerOver = false;
        UpdateVisualState();
        OnPointerExitEvent?.Invoke();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && !_isEmpty)
        {
            OnBeginDragEvent?.Invoke(SlotIndex);
        }
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnEndDragEvent?.Invoke();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && eventData.clickCount == 2 && !_isEmpty)
        {
            OnDoubleClickEvent?.Invoke(SlotIndex);
        }
    }

    #endregion
}