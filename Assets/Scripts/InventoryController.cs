using UnityEngine;
using System.Linq;

public class InventoryController
{
    // Зависимости, получаемые через конструктор
    private readonly InventoryWindowView _windowView;
    private readonly DragIconView _dragIconView;
    private readonly TooltipView _tooltipView;

    // Внутреннее состояние
    private InventorySlotView _draggedFromSlot;

    public InventoryController(InventoryWindowView windowView, DragIconView dragIconView, TooltipView tooltipView)
    {
        _windowView = windowView;
        _dragIconView = dragIconView;
        _tooltipView = tooltipView;
    }

    /// <summary>
    /// Этот метод вызывается UIManager'ом КАЖДЫЙ РАЗ при открытии инвентаря.
    /// Он подписывает контроллер на события от НОВЫХ, только что созданных UI-слотов.
    /// </summary>
    public void SubscribeToSlots()
    {
        if (_windowView.PlayerInventoryView != null)
        {
            foreach (var slotView in _windowView.PlayerInventoryView.GetSlotViews())
            {
                // Передаем модель напрямую из View
                SubscribeToSlotEvents(slotView, _windowView.PlayerInventoryView.GetModel());
            }
        }

        if (_windowView.ContainerInventoryView != null)
        {
            foreach (var slotView in _windowView.ContainerInventoryView.GetSlotViews())
            {
                SubscribeToSlotEvents(slotView, _windowView.ContainerInventoryView.GetModel());
            }
        }
    }

    private void SubscribeToSlotEvents(InventorySlotView slotView, InventoryModel model)
    {
        // Отписываемся от старых событий на всякий случай, чтобы избежать дубликатов
        slotView.OnPointerEnterEvent -= (index) => HandlePointerEnter(model, index);
        slotView.OnPointerExitEvent -= HandlePointerExit;
        slotView.OnBeginDragEvent -= (index) => HandleBeginDrag(slotView, model);
        slotView.OnEndDragEvent -= HandleEndDrag;
        slotView.OnDoubleClickEvent -= (index) => HandleDoubleClick(model, index);

        // Подписываемся заново
        slotView.OnPointerEnterEvent += (index) => HandlePointerEnter(model, index);
        slotView.OnPointerExitEvent += HandlePointerExit;
        slotView.OnBeginDragEvent += (index) => HandleBeginDrag(slotView, model);
        slotView.OnEndDragEvent += HandleEndDrag;
        slotView.OnDoubleClickEvent += (index) => HandleDoubleClick(model, index);
    }

    #region Event Handlers

    private void HandlePointerEnter(InventoryModel model, int index)
    {
        var itemData = model.Slots[index].ItemData;
        if (itemData != null)
        {
            _tooltipView.Show(itemData);
        }
        else
        {
            _tooltipView.Hide();
        }
    }

    private void HandlePointerExit()
    {
        _tooltipView.Hide();
    }

    private void HandleBeginDrag(InventorySlotView slotView, InventoryModel model)
    {
        if (model.Slots[slotView.SlotIndex].IsEmpty) return;

        _draggedFromSlot = slotView;
        _draggedFromSlot.SetSelected(true);
        _dragIconView.Show(model.Slots[slotView.SlotIndex].ItemData.Icon);
        _tooltipView.Hide();
    }

    private void HandleEndDrag()
    {
        if (_draggedFromSlot == null) return;

        InventoryModel sourceModel = GetModelForSlot(_draggedFromSlot);
        int sourceIndex = _draggedFromSlot.SlotIndex;

        // Ищем целевой слот
        var targetSlotView = FindSlotUnderCursor();

        if (targetSlotView != null)
        {
            InventoryModel targetModel = GetModelForSlot(targetSlotView);
            int targetIndex = targetSlotView.SlotIndex;

            // Используем новый централизованный метод для перемещения
            InventoryModel.TransferItem(sourceModel, sourceIndex, targetModel, targetIndex);
        }

        _dragIconView.Hide();
        _draggedFromSlot.SetSelected(false);
        _draggedFromSlot = null;
    }

    private void HandleDoubleClick(InventoryModel model, int index)
    {
        if (model.Slots[index].IsEmpty) return;
        ItemData itemData = model.Slots[index].ItemData;

        Debug.Log($"Used item: {itemData.Name}");

        if (itemData.Type == ItemType.Potion)
        {
            model.Slots[index].RemoveQuantity(1);
            model.OnInventoryUpdated?.Invoke();
        }
    }

    #endregion

    #region Helpers

    private InventorySlotView FindSlotUnderCursor()
    {
        var allViews = _windowView.PlayerInventoryView.GetSlotViews();
        if (_windowView.ContainerInventoryView != null)
        {
            allViews = allViews.Concat(_windowView.ContainerInventoryView.GetSlotViews()).ToList();
        }

        foreach (var slotView in allViews)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(slotView.transform as RectTransform, Input.mousePosition))
            {
                return slotView;
            }
        }
        return null;
    }

    private InventoryModel GetModelForSlot(InventorySlotView slotView)
    {
        if (_windowView.PlayerInventoryView.GetSlotViews().Contains(slotView))
        {
            return _windowView.PlayerInventoryView.GetModel();
        }
        if (_windowView.ContainerInventoryView != null && _windowView.ContainerInventoryView.GetSlotViews().Contains(slotView))
        {
            return _windowView.ContainerInventoryView.GetModel();
        }
        return null;
    }

    #endregion
}