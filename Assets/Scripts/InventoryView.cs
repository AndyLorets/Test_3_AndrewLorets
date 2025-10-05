using System.Collections.Generic;
using UnityEngine;

public class InventoryView
{
    private Transform _slotsContainer;
    private InventorySlotView _slotPrefab;
    private InventoryModel _inventoryModel;

    private List<InventorySlotView> _slotViews = new List<InventorySlotView>();

    // ����������� ������ Start/Awake
    public InventoryView(Transform container, InventorySlotView prefab, InventoryModel model)
    {
        _slotsContainer = container;
        _slotPrefab = prefab;
        _inventoryModel = model;

        // ������������� �� ������� ���������� ������
        _inventoryModel.OnInventoryUpdated += Redraw;
    }

    // �������� ������� ����� ��������
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

    // �����! ����� ��� �������, ����� �������� ������ ������
    public void Dispose()
    {
        _inventoryModel.OnInventoryUpdated -= Redraw;
    }
}