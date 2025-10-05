using System.Collections.Generic;
using UnityEngine;

// Этот класс не MonoBehaviour. Он управляет группой UI-элементов слотов (InventorySlotView),
// связывая их с конкретной моделью данных (InventoryModel).
public class InventoryView
{
    private Transform _slotsContainer;
    private InventorySlotView _slotPrefab;
    private InventoryModel _inventoryModel;

    private List<InventorySlotView> _slotViews = new List<InventorySlotView>();

    /// <summary>
    /// Конструктор. Сохраняет ссылки на всё необходимое и подписывается на обновления модели.
    /// </summary>
    public InventoryView(Transform container, InventorySlotView prefab, InventoryModel model)
    {
        _slotsContainer = container;
        _slotPrefab = prefab;
        _inventoryModel = model;

        if (_inventoryModel != null)
        {
            _inventoryModel.OnInventoryUpdated += Redraw;
        }
    }

    /// <summary>
    /// Создает UI-элементы слотов на сцене. Вызывается один раз после создания View.
    /// </summary>
    public void Initialize()
    {
        // Очищаем предыдущие слоты на всякий случай, если они остались
        foreach (Transform child in _slotsContainer)
        {
            Object.Destroy(child.gameObject);
        }
        _slotViews.Clear();

        if (_inventoryModel == null) return;

        for (int i = 0; i < _inventoryModel.Capacity; i++)
        {
            InventorySlotView slotView = Object.Instantiate(_slotPrefab, _slotsContainer);
            slotView.Init(i);
            _slotViews.Add(slotView);
        }
        Redraw();
    }

    /// <summary>
    /// Полностью перерисовывает все слоты на основе текущего состояния модели.
    /// Вызывается, когда модель сообщает об изменениях (OnInventoryUpdated).
    /// </summary>
    public void Redraw()
    {
        if (_inventoryModel == null) return;

        for (int i = 0; i < _slotViews.Count; i++)
        {
            _slotViews[i].UpdateView(_inventoryModel.Slots[i]);
        }
    }

    /// <summary>
    /// Возвращает список UI-компонентов слотов. Нужно для InventoryController, чтобы подписаться на события.
    /// </summary>
    public List<InventorySlotView> GetSlotViews() => _slotViews;

    /// <summary>
    /// Возвращает модель данных, с которой работает это View.
    /// Критически важно для InventoryController, чтобы он знал, какой инвентарь изменяет.
    /// </summary>
    public InventoryModel GetModel() => _inventoryModel; // <-- ВОТ ЭТОТ МЕТОД НУЖНО БЫЛО ДОБАВИТЬ

    /// <summary>
    /// "Уборщик". Отписывается от событий модели, чтобы избежать утечек памяти,
    /// когда этот View больше не нужен (например, при закрытии окна инвентаря).
    /// </summary>
    public void Dispose()
    {
        if (_inventoryModel != null)
        {
            _inventoryModel.OnInventoryUpdated -= Redraw;
        }
    }
}