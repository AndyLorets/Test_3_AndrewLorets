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

    [Serializable]
    public struct SlotStateVisuals
    {
        public Color BackgroundColor;
    }

    [Header("Настройки состояний")]
    [SerializeField] private SlotStateVisuals _normalState;
    [SerializeField] private SlotStateVisuals _hoveredState;
    [SerializeField] private SlotStateVisuals _selectedState;
    [SerializeField] private SlotStateVisuals _emptyState;
    [SerializeField] private float _colorFadeDuration = 0.1f;

    public int SlotIndex { get; private set; }

    public event Action<int> OnPointerEnterEvent;
    public event Action OnPointerExitEvent;
    public event Action<int> OnBeginDragEvent;
    public event Action OnEndDragEvent;
    public event Action<int> OnDoubleClickEvent;

    // --- УПРАВЛЕНИЕ СОСТОЯНИЕМ ---
    private bool _isPointerOver;
    private bool _isSelected;
    private bool _isEmpty = true;
    private bool _isGlobalDragging; // НОВОЕ ПОЛЕ: знает, тащит ли кто-то предмет ВООБЩЕ

    public void Init(int index)
    {
        SlotIndex = index;
        // При инициализации сразу устанавливаем правильный пустой цвет
        _background.color = _emptyState.BackgroundColor;
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
        SlotStateVisuals targetState;

        bool canBeHovered = !_isEmpty || _isGlobalDragging;

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

        _background.DOColor(targetState.BackgroundColor, _colorFadeDuration).SetEase(Ease.OutQuad);
    }

    // --- ПУБЛИЧНЫЕ МЕТОДЫ ДЛЯ КОНТРОЛЛЕРА ---

    public void SetSelected(bool isSelected)
    {
        _isSelected = isSelected;
        UpdateVisualState();
    }

    // НОВЫЙ МЕТОД: Слот получает уведомление о глобальном состоянии перетаскивания
    public void OnGlobalDragStateChanged(bool isDragging)
    {
        _isGlobalDragging = isDragging;
        // Перерисовываем себя, если мышь сейчас над нами,
        // так как наше состояние "можно ли подсвечивать" могло измениться
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
    }

    #endregion
}