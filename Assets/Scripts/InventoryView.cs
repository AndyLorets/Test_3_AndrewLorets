using System.Collections.Generic;
using UnityEngine;

public class InventoryView
{
    private Transform _slotsContainer;
    private InventorySlotView _slotPrefab;
    private InventoryModel _inventoryModel;

    private List<InventorySlotView> _slotViews = new List<InventorySlotView>();

    // Конструктор вместо Start/Awake
    public InventoryView(Transform container, InventorySlotView prefab, InventoryModel model)
    {
        _slotsContainer = container;
        _slotPrefab = prefab;
        _inventoryModel = model;

        // Подписываемся на событие обновления модели
        _inventoryModel.OnInventoryUpdated += Redraw;
    }

    // Вызываем вручную после создания
    public void Initialize()
    {
        for (int i = 0; i < _inventoryModel.Slots.Count; i++)
        {
            InventorySlotView slotView = Object.Instantiate(_slotPrefab, _slotsContainer);
            slotView.Init(i);
            _slotViews.Add(slotView);
        }
        Redraw();
    }

    public void Redraw()
    {
        for (int i = 0; i < _slotViews.Count; i++)
        {
            _slotViews[i].UpdateView(_inventoryModel.Slots[i]);
        }
    }

    public List<InventorySlotView> GetSlotViews() => _slotViews;

    // Важно! Метод для отписки, чтобы избежать утечек памяти
    public void Dispose()
    {
        _inventoryModel.OnInventoryUpdated -= Redraw;
    }
}