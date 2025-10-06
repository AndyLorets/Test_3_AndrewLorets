using UnityEngine;
using System.Linq;

/// <summary>
/// [CONTROLLER] - �����, ���������� �� ��� ������ �������������� � UI ���������.
/// �� ������� ������� �� UI-������ (�����, ��������������) � ������ ������� ������� (InventoryModel)
/// ��� ��������� ������.
/// </summary>
public class InventoryController
{
    private readonly InventoryWindowView _windowView;  
    private readonly DragIconView _dragIconView;       
    private readonly TooltipView _tooltipView;         

    private InventorySlotView _draggedFromSlot;       

    /// <summary>
    /// �����������. �������� ��� ����������� View-���������� ����� Zenject.
    /// </summary>
    public InventoryController(InventoryWindowView windowView, DragIconView dragIconView, TooltipView tooltipView)
    {
        _windowView = windowView;
        _dragIconView = dragIconView;
        _tooltipView = tooltipView;
    }

    /// <summary>
    /// ������� ����� ��� "���������" �����������.
    /// ���������� UIManager'�� ������ ���, ����� ���� ��������� ����������� � ������ �������.
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
    /// ��������������� ����� ��� �������� �� ������� ������ ����������� �����.
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

    #region Event Handlers (����������� ������� �� View)

    /// <summary>
    /// �����������, ����� ������ ������ � ������� �����. ���������� ���������.
    /// </summary>
    private void HandlePointerEnter(InventoryModel model, int index)
    {
        var itemData = model.Slots[index].ItemData;
        if (itemData != null) _tooltipView.Show(itemData);
        else _tooltipView.Hide();
    }

    /// <summary>
    /// �����������, ����� ������ �������� ������� �����. ������ ���������.
    /// </summary>
    private void HandlePointerExit()
    {
        _tooltipView.Hide();
    }

    /// <summary>
    /// ����������� � ������ �������������� �������� �� �����.
    /// </summary>
    private void HandleBeginDrag(InventorySlotView slotView, InventoryModel model)
    {
        if (model.Slots[slotView.SlotIndex].IsEmpty) return;

        _draggedFromSlot = slotView;
        _draggedFromSlot.SetSelected(true); // ��������� �������� �������� ����
        _dragIconView.Show(model.Slots[slotView.SlotIndex].ItemData.Icon);
        _tooltipView.Hide();
    }

    /// <summary>
    /// ����������� � ������, ����� ������ ���� �������� ����� ��������������.
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
    /// ����������� ��� ������� ����� �� ����. ���������� �������.
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
            Debug.Log($"������� '{itemData.Name}' ������ ������������.");
        }
    }

    /// <summary>
    /// ����������� ��� ������ ����� �� ����. ����������� ������������� �� ��������.
    /// </summary>
    private void HandleRightClick(InventoryModel model, int index)
    {
        if (model.Slots[index].IsEmpty) return;

        model.RemoveItemQuantity(index, 1);
    }
    #endregion

    #region Helpers (��������������� ������)

    /// <summary>
    /// ������� UI-����, ������� � ������ ������ ��������� ��� �������� ����.
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
    /// ����������, ����� ������ (���������) ����������� ������ UI-����.
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