using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [VIEW] - класс, управляющий ОДНОЙ панелью (гридом) с ячейками инвентаря.
/// Отвечает за создание и обновление UI-элементов слотов на основе данных из конкретной InventoryModel.
/// </summary>
public class InventoryView
{
    private Transform _slotsContainer;
    private InventorySlotView _slotPrefab;
    private InventoryModel _inventoryModel;

    private List<InventorySlotView> _slotViews = new List<InventorySlotView>();

    /// <summary>
    /// Конструктор. Сохраняет ссылки на зависимости и подписывается на обновления модели.
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
    /// Создает и инициализирует UI-элементы слотов (InventorySlotView) на сцене.
    /// </summary>
    public void Initialize()
    {
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
    /// Полностью перерисовывает все дочерние слоты, синхронизируя их вид с данными из модели.
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
    /// Возвращает список созданных UI-компонентов слотов.
    /// </summary>
    public List<InventorySlotView> GetSlotViews() => _slotViews;

    /// <summary>
    /// Возвращает модель данных, с которой работает это представление.
    /// </summary>
    public InventoryModel GetModel() => _inventoryModel;

    /// <summary>
    /// "Уборщик". Отписывается от событий модели, чтобы предотвратить утечки памяти.
    /// </summary>
    public void Dispose()
    {
        if (_inventoryModel != null)
        {
            _inventoryModel.OnInventoryUpdated -= Redraw;
        }
    }
}