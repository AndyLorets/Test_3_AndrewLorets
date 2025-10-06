using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;

public class InventorySlotView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
                                IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeField] private Image _background;
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TextMeshProUGUI _quantityText;

    [Header("Настройки состояний")]
    [SerializeField] private Color _normalStateColor;
    [SerializeField] private Color _hoveredStateColor;
    [SerializeField] private Color _selectedStateColor;
    [SerializeField] private Color _emptyStateColor;
    [SerializeField] private float _colorFadeDuration = 0.1f;

    public int SlotIndex { get; private set; }

    public event Action<int> OnPointerEnterEvent;
    public event Action OnPointerExitEvent;
    public event Action<int> OnBeginDragEvent;
    public event Action OnEndDragEvent;
    public event Action<int> OnDoubleClickEvent;
    public event Action<int> OnRightClickEvent;

    private bool _isPointerOver;
    private bool _isSelected;
    private bool _isEmpty = true;
    private bool _isGlobalDragging; 

    public void Init(int index)
    {
        SlotIndex = index;
        _background.color = _emptyStateColor;
    }

    public void UpdateView(InventorySlot slot)
    {
        _isEmpty = slot.IsEmpty;
        _itemIcon.gameObject.SetActive(!_isEmpty);
        _quantityText.gameObject.SetActive(!_isEmpty && slot.ItemData.CanStack && slot.Quantity > 1);

        if (!_isEmpty)
        {
            _itemIcon.sprite = slot.ItemData.Icon;
            if (_quantityText.gameObject.activeSelf)
            {
                _quantityText.text = slot.Quantity.ToString();
            }
        }
        UpdateVisualState();
    }

    private void UpdateVisualState()
    {
        Color targetColor;

        bool canBeHovered = !_isEmpty || _isGlobalDragging;

        if (_isSelected)
        {
            targetColor = _selectedStateColor;
        }
        else if (_isPointerOver && canBeHovered)
        {
            targetColor = _hoveredStateColor;
        }
        else
        {
            targetColor = _isEmpty ? _emptyStateColor : _normalStateColor;
        }

        _background.DOColor(targetColor, _colorFadeDuration).SetEase(Ease.OutQuad);
    }


    public void SetSelected(bool isSelected)
    {
        _isSelected = isSelected;
        UpdateVisualState();
    }

    public void OnGlobalDragStateChanged(bool isDragging)
    {
        _isGlobalDragging = isDragging;

        if (_isPointerOver)
        {
            UpdateVisualState();
        }
    }

    #region Pointer Events

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
        else if (eventData.button == PointerEventData.InputButton.Right && !_isEmpty)
        {
            OnRightClickEvent?.Invoke(SlotIndex);
        }
    }

    #endregion
}