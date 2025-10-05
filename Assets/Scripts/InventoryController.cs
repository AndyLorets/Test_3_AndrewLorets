using DG.Tweening;
using System.Linq; // Нужен для .Contains()
using UnityEngine;
using Zenject;

// Реализуем IInitializable, чтобы выполнить код после того, как все зависимости будут внедрены
public class InventoryController : IInitializable
{
    // --- ЗАВИСИМОСТИ (Внедряются Zenject'ом) ---

    // Модели данных для каждого инвентаря.
    // Атрибут [Inject] с ID явно указывает, какую именно реализацию мы хотим получить.
    private readonly InventoryModel _playerInventoryModel;
    private readonly InventoryModel _containerInventoryModel;

    // View-компоненты, которыми мы будем управлять
    private readonly InventoryWindowView _windowView;
    private readonly DragIconView _dragIconView;
    private readonly TooltipView _tooltipView;

    // --- ВНУТРЕННЕЕ СОСТОЯНИЕ ---
    private Tween _hideTooltipTween;
    // Слот, из которого мы начали перетаскивание. Null, если ничего не перетаскиваем.
    private InventorySlotView _draggedFromSlot;

    public static bool IsDragging { get; private set; }

    // Конструктор, в который Zenject передаст все необходимые объекты
    public InventoryController(
        [Inject(Id = InventoryIDs.Player)] InventoryModel playerInventoryModel,
        [Inject(Id = InventoryIDs.Container)] InventoryModel containerInventoryModel,
        InventoryWindowView windowView,
        DragIconView dragIconView,
        TooltipView tooltipView)
    {
        _playerInventoryModel = playerInventoryModel;
        _containerInventoryModel = containerInventoryModel;
        _windowView = windowView;
        _dragIconView = dragIconView;
        _tooltipView = tooltipView;
    }

    // Этот метод будет вызван Zenject'ом один раз после создания контроллера
    public void Initialize()
    {
        // Подписываемся на события от каждого UI-слота в инвентаре игрока
        foreach (var slotView in _windowView.PlayerInventoryView.GetSlotViews())
        {
            SubscribeToSlotEvents(slotView, _playerInventoryModel);
        }

        // И так же подписываемся на события от слотов в инвентаре контейнера
        foreach (var slotView in _windowView.ContainerInventoryView.GetSlotViews())
        {
            SubscribeToSlotEvents(slotView, _containerInventoryModel);
        }
    }

    /// <summary>
    /// Вспомогательный метод, чтобы избежать дублирования кода подписки на события.
    /// </summary>
    private void SubscribeToSlotEvents(InventorySlotView slotView, InventoryModel model)
    {
        // Используем лямбда-выражения, чтобы передать дополнительную информацию (модель) в обработчик
        slotView.OnPointerEnterEvent += (index) => HandlePointerEnter(model, index);
        slotView.OnPointerExitEvent += HandlePointerExit;
        slotView.OnBeginDragEvent += (index) => HandleBeginDrag(slotView, model);
        slotView.OnEndDragEvent += HandleEndDrag;
        slotView.OnDoubleClickEvent += (index) => HandleDoubleClick(model, index);
    }

    #region Event Handlers (Обработчики событий от View)

    private void HandlePointerEnter(InventoryModel model, int index)
    {
        // 1. Если у нас было запланировано скрытие тултипа, отменяем его.
        _hideTooltipTween?.Kill();

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
        _hideTooltipTween = DOVirtual.DelayedCall(0.1f, () =>
        {
            _tooltipView.Hide();
        });
    }

    private void HandleBeginDrag(InventorySlotView slotView, InventoryModel model)
    {
        // Нельзя начать перетаскивание из пустого слота
        if (model.Slots[slotView.SlotIndex].IsEmpty) return;

        IsDragging = true;

        _draggedFromSlot = slotView;
        _draggedFromSlot.SetSelected(true);
        _dragIconView.Show(model.Slots[slotView.SlotIndex].ItemData.Icon);
        _tooltipView.Hide(); // Прячем тултип, чтобы не мешал
    }

    private void HandleEndDrag()
    {
        // Если мы ничего не перетаскивали, выходим
        if (_draggedFromSlot == null) return;

        IsDragging = false;

        // Ищем, над каким слотом мы отпустили мышку
        var targetPlayerSlot = FindSlotUnderCursor(_windowView.PlayerInventoryView);
        var targetContainerSlot = FindSlotUnderCursor(_windowView.ContainerInventoryView);

        // Определяем, из какой модели мы взяли предмет
        InventoryModel sourceModel = GetModelForSlot(_draggedFromSlot);
        int sourceIndex = _draggedFromSlot.SlotIndex;

        // Если отпустили над слотом игрока
        if (targetPlayerSlot != null)
        {
            TransferItem(sourceModel, _playerInventoryModel, sourceIndex, targetPlayerSlot.SlotIndex);
        }
        // Если отпустили над слотом контейнера
        else if (targetContainerSlot != null)
        {
            TransferItem(sourceModel, _containerInventoryModel, sourceIndex, targetContainerSlot.SlotIndex);
        }
        // Если отпустили вне всех слотов, можно реализовать логику "выбросить предмет"

        // Сбрасываем состояние перетаскивания
        _dragIconView.Hide();
        _draggedFromSlot.SetSelected(false);
        _draggedFromSlot = null;
    }

    private void HandleDoubleClick(InventoryModel model, int index)
    {
        if (model.Slots[index].IsEmpty) return;

        ItemData itemData = model.Slots[index].ItemData;
        Debug.Log($"Used item: {itemData.Name} from {GetModelName(model)} inventory.");

        // Пример логики использования: если это зелье, удаляем 1 штуку
        if (itemData.Type == ItemType.Potion)
        {
            model.Slots[index].RemoveQuantity(1);
            // Прямо говорим модели, что данные изменились, чтобы View перерисовалось
            // Методы TryAddItem/RemoveItem/MoveItem в модели делают это сами,
            // а тут мы меняем слот напрямую, поэтому нужен ручной вызов.
            model.OnInventoryUpdated?.Invoke();
        }
    }

    #endregion

    #region Helper Methods (Вспомогательные методы)

    /// <summary>
    /// Основная логика перемещения предмета.
    /// </summary>
    private void TransferItem(InventoryModel fromModel, InventoryModel toModel, int fromIndex, int toIndex)
    {
        // Случай 1: Перемещение внутри одного и того же инвентаря
        if (fromModel == toModel)
        {
            fromModel.MoveItem(fromIndex, toIndex);
        }
        // Случай 2: Перемещение между разными инвентарями
        else
        {
            var itemSlotToMove = fromModel.Slots[fromIndex];
            // Пытаемся добавить предмет в целевой инвентарь
            if (toModel.TryAddItem(itemSlotToMove.ItemData, itemSlotToMove.Quantity))
            {
                // Если добавление прошло успешно, удаляем предмет из исходного инвентаря
                fromModel.RemoveItem(fromIndex);
            }
        }
    }

    /// <summary>
    /// Находит UI-слот, который в данный момент находится под курсором мыши.
    /// </summary>
    private InventorySlotView FindSlotUnderCursor(InventoryView inventoryView)
    {
        foreach (var slotView in inventoryView.GetSlotViews())
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(slotView.transform as RectTransform, Input.mousePosition))
            {
                return slotView;
            }
        }
        return null;
    }

    /// <summary>
    /// Определяет, какой модели данных (`InventoryModel`) соответствует данный UI-слот (`InventorySlotView`).
    /// </summary>
    private InventoryModel GetModelForSlot(InventorySlotView slotView)
    {
        // Проверяем, принадлежит ли этот слот View инвентаря игрока
        bool isPlayerSlot = _windowView.PlayerInventoryView.GetSlotViews().Contains(slotView);
        return isPlayerSlot ? _playerInventoryModel : _containerInventoryModel;
    }

    /// <summary>
    /// Возвращает строковое имя модели для логирования.
    /// </summary>
    private string GetModelName(InventoryModel model)
    {
        return model == _playerInventoryModel ? "Player" : "Container";
    }

    #endregion
}