using UnityEngine;
using System.Linq;

/// <summary>
/// [CONTROLLER] - класс, отвечающий за всю логику взаимодействия с UI инвентаря.
/// Он слушает события от UI-слотов (клики, перетаскивания) и отдает команды моделям (InventoryModel)
/// для изменения данных.
/// </summary>
public class InventoryController
{
    private readonly InventoryWindowView _windowView;  
    private readonly DragIconView _dragIconView;       
    private readonly TooltipView _tooltipView;         

    private InventorySlotView _draggedFromSlot;       

    /// <summary>
    /// Конструктор. Получает все необходимые View-компоненты через Zenject.
    /// </summary>
    public InventoryController(InventoryWindowView windowView, DragIconView dragIconView, TooltipView tooltipView)
    {
        _windowView = windowView;
        _dragIconView = dragIconView;
        _tooltipView = tooltipView;
    }

    /// <summary>
    /// Главный метод для "активации" контроллера.
    /// Вызывается UIManager'ом КАЖДЫЙ РАЗ, когда окно инвентаря открывается с новыми слотами.
    /// </summary>
    public void SubscribeToSlots()
    {
        if (_windowView.PlayerInventoryView != null)
        {
            foreach (var slotView in _windowView.PlayerInventoryView.GetSlotViews())
            {
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

    /// <summary>
    /// Вспомогательный метод для подписки на события одного конкретного слота.
    /// </summary>
    private void SubscribeToSlotEvents(InventorySlotView slotView, InventoryModel model)
    {
        slotView.OnPointerEnterEvent -= (index) => HandlePointerEnter(model, index);
        slotView.OnPointerExitEvent -= HandlePointerExit;
        slotView.OnBeginDragEvent -= (index) => HandleBeginDrag(slotView, model);
        slotView.OnEndDragEvent -= HandleEndDrag;
        slotView.OnDoubleClickEvent -= (index) => HandleDoubleClick(model, index);
        slotView.OnRightClickEvent -= (index) => HandleRightClick(model, index);

        slotView.OnPointerEnterEvent += (index) => HandlePointerEnter(model, index);
        slotView.OnPointerExitEvent += HandlePointerExit;
        slotView.OnBeginDragEvent += (index) => HandleBeginDrag(slotView, model);
        slotView.OnEndDragEvent += HandleEndDrag;
        slotView.OnDoubleClickEvent += (index) => HandleDoubleClick(model, index);
        slotView.OnRightClickEvent += (index) => HandleRightClick(model, index);
    }

    #region Event Handlers (Обработчики событий от View)

    /// <summary>
    /// Срабатывает, когда курсор входит в границы слота. Показывает подсказку.
    /// </summary>
    private void HandlePointerEnter(InventoryModel model, int index)
    {
        var itemData = model.Slots[index].ItemData;
        if (itemData != null) _tooltipView.Show(itemData);
        else _tooltipView.Hide();
    }

    /// <summary>
    /// Срабатывает, когда курсор покидает границы слота. Прячет подсказку.
    /// </summary>
    private void HandlePointerExit()
    {
        _tooltipView.Hide();
    }

    /// <summary>
    /// Срабатывает в начале перетаскивания предмета из слота.
    /// </summary>
    private void HandleBeginDrag(InventorySlotView slotView, InventoryModel model)
    {
        if (model.Slots[slotView.SlotIndex].IsEmpty) return;

        _draggedFromSlot = slotView;
        _draggedFromSlot.SetSelected(true); // Визуально выделяем исходный слот
        _dragIconView.Show(model.Slots[slotView.SlotIndex].ItemData.Icon);
        _tooltipView.Hide();
    }

    /// <summary>
    /// Срабатывает в момент, когда кнопка мыши отпущена после перетаскивания.
    /// </summary>
    private void HandleEndDrag()
    {
        if (_draggedFromSlot == null) return;

        InventoryModel sourceModel = GetModelForSlot(_draggedFromSlot);
        int sourceIndex = _draggedFromSlot.SlotIndex;

        var targetSlotView = FindSlotUnderCursor();

        if (targetSlotView != null)
        {
            InventoryModel targetModel = GetModelForSlot(targetSlotView);
            int targetIndex = targetSlotView.SlotIndex;

            InventoryModel.TransferItem(sourceModel, sourceIndex, targetModel, targetIndex);
        }

        _dragIconView.Hide();
        _draggedFromSlot.SetSelected(false);
        _draggedFromSlot = null;
    }

    /// <summary>
    /// Срабатывает при двойном клике на слот. Использует предмет.
    /// </summary>
    private void HandleDoubleClick(InventoryModel model, int index)
    {
        if (model.Slots[index].IsEmpty) return;

        ItemData itemData = model.Slots[index].ItemData;

        if (itemData is IUsable usableItem)
        {
            usableItem.Use();
            model.RemoveItemQuantity(index, 1);
        }
        else
        {
            Debug.Log($"Предмет '{itemData.Name}' нельзя использовать.");
        }
    }

    /// <summary>
    /// Срабатывает при правом клике на слот. Запрашивает подтверждение на удаление.
    /// </summary>
    private void HandleRightClick(InventoryModel model, int index)
    {
        if (model.Slots[index].IsEmpty) return;

        model.RemoveItemQuantity(index, 1);
    }
    #endregion

    #region Helpers (Вспомогательные методы)

    /// <summary>
    /// Находит UI-слот, который в данный момент находится под курсором мыши.
    /// </summary>
    private InventorySlotView FindSlotUnderCursor()
    {
        var playerSlots = _windowView.PlayerInventoryView.GetSlotViews();
        var allSlots = (_windowView.ContainerInventoryView != null)
            ? playerSlots.Concat(_windowView.ContainerInventoryView.GetSlotViews())
            : playerSlots;

        foreach (var slotView in allSlots)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(slotView.transform as RectTransform, Input.mousePosition))
            {
                return slotView;
            }
        }
        return null;
    }

    /// <summary>
    /// Определяет, какой Модели (инвентарю) принадлежит данный UI-слот.
    /// </summary>
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