using System.Collections.Generic;
using UnityEngine;

// ���� ����� �� MonoBehaviour. �� ��������� ������� UI-��������� ������ (InventorySlotView),
// �������� �� � ���������� ������� ������ (InventoryModel).
public class InventoryView
{
    private Transform _slotsContainer;
    private InventorySlotView _slotPrefab;
    private InventoryModel _inventoryModel;

    private List<InventorySlotView> _slotViews = new List<InventorySlotView>();

    /// <summary>
    /// �����������. ��������� ������ �� �� ����������� � ������������� �� ���������� ������.
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
    /// ������� UI-�������� ������ �� �����. ���������� ���� ��� ����� �������� View.
    /// </summary>
    public void Initialize()
    {
        // ������� ���������� ����� �� ������ ������, ���� ��� ��������
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
    /// ��������� �������������� ��� ����� �� ������ �������� ��������� ������.
    /// ����������, ����� ������ �������� �� ���������� (OnInventoryUpdated).
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
    /// ���������� ������ UI-����������� ������. ����� ��� InventoryController, ����� ����������� �� �������.
    /// </summary>
    public List<InventorySlotView> GetSlotViews() => _slotViews;

    /// <summary>
    /// ���������� ������ ������, � ������� �������� ��� View.
    /// ���������� ����� ��� InventoryController, ����� �� ����, ����� ��������� ��������.
    /// </summary>
    public InventoryModel GetModel() => _inventoryModel; // <-- ��� ���� ����� ����� ���� ��������

    /// <summary>
    /// "�������". ������������ �� ������� ������, ����� �������� ������ ������,
    /// ����� ���� View ������ �� ����� (��������, ��� �������� ���� ���������).
    /// </summary>
    public void Dispose()
    {
        if (_inventoryModel != null)
        {
            _inventoryModel.OnInventoryUpdated -= Redraw;
        }
    }
}