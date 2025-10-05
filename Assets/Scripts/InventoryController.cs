using DG.Tweening;
using System.Linq; // ����� ��� .Contains()
using UnityEngine;
using Zenject;

// ��������� IInitializable, ����� ��������� ��� ����� ����, ��� ��� ����������� ����� ��������
public class InventoryController : IInitializable
{
    // --- ����������� (���������� Zenject'��) ---

    // ������ ������ ��� ������� ���������.
    // ������� [Inject] � ID ���� ���������, ����� ������ ���������� �� ����� ��������.
    private readonly InventoryModel _playerInventoryModel;
    private readonly InventoryModel _containerInventoryModel;

    // View-����������, �������� �� ����� ���������
    private readonly InventoryWindowView _windowView;
    private readonly DragIconView _dragIconView;
    private readonly TooltipView _tooltipView;

    // --- ���������� ��������� ---
    private Tween _hideTooltipTween;
    // ����, �� �������� �� ������ ��������������. Null, ���� ������ �� �������������.
    private InventorySlotView _draggedFromSlot;

    public static bool IsDragging { get; private set; }

    // �����������, � ������� Zenject �������� ��� ����������� �������
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

    // ���� ����� ����� ������ Zenject'�� ���� ��� ����� �������� �����������
    public void Initialize()
    {
        // ������������� �� ������� �� ������� UI-����� � ��������� ������
        foreach (var slotView in _windowView.PlayerInventoryView.GetSlotViews())
        {
            SubscribeToSlotEvents(slotView, _playerInventoryModel);
        }

        // � ��� �� ������������� �� ������� �� ������ � ��������� ����������
        foreach (var slotView in _windowView.ContainerInventoryView.GetSlotViews())
        {
            SubscribeToSlotEvents(slotView, _containerInventoryModel);
        }
    }

    /// <summary>
    /// ��������������� �����, ����� �������� ������������ ���� �������� �� �������.
    /// </summary>
    private void SubscribeToSlotEvents(InventorySlotView slotView, InventoryModel model)
    {
        // ���������� ������-���������, ����� �������� �������������� ���������� (������) � ����������
        slotView.OnPointerEnterEvent += (index) => HandlePointerEnter(model, index);
        slotView.OnPointerExitEvent += HandlePointerExit;
        slotView.OnBeginDragEvent += (index) => HandleBeginDrag(slotView, model);
        slotView.OnEndDragEvent += HandleEndDrag;
        slotView.OnDoubleClickEvent += (index) => HandleDoubleClick(model, index);
    }

    #region Event Handlers (����������� ������� �� View)

    private void HandlePointerEnter(InventoryModel model, int index)
    {
        // 1. ���� � ��� ���� ������������� ������� �������, �������� ���.
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
        // ������ ������ �������������� �� ������� �����
        if (model.Slots[slotView.SlotIndex].IsEmpty) return;

        IsDragging = true;

        _draggedFromSlot = slotView;
        _draggedFromSlot.SetSelected(true);
        _dragIconView.Show(model.Slots[slotView.SlotIndex].ItemData.Icon);
        _tooltipView.Hide(); // ������ ������, ����� �� �����
    }

    private void HandleEndDrag()
    {
        // ���� �� ������ �� �������������, �������
        if (_draggedFromSlot == null) return;

        IsDragging = false;

        // ����, ��� ����� ������ �� ��������� �����
        var targetPlayerSlot = FindSlotUnderCursor(_windowView.PlayerInventoryView);
        var targetContainerSlot = FindSlotUnderCursor(_windowView.ContainerInventoryView);

        // ����������, �� ����� ������ �� ����� �������
        InventoryModel sourceModel = GetModelForSlot(_draggedFromSlot);
        int sourceIndex = _draggedFromSlot.SlotIndex;

        // ���� ��������� ��� ������ ������
        if (targetPlayerSlot != null)
        {
            TransferItem(sourceModel, _playerInventoryModel, sourceIndex, targetPlayerSlot.SlotIndex);
        }
        // ���� ��������� ��� ������ ����������
        else if (targetContainerSlot != null)
        {
            TransferItem(sourceModel, _containerInventoryModel, sourceIndex, targetContainerSlot.SlotIndex);
        }
        // ���� ��������� ��� ���� ������, ����� ����������� ������ "��������� �������"

        // ���������� ��������� ��������������
        _dragIconView.Hide();
        _draggedFromSlot.SetSelected(false);
        _draggedFromSlot = null;
    }

    private void HandleDoubleClick(InventoryModel model, int index)
    {
        if (model.Slots[index].IsEmpty) return;

        ItemData itemData = model.Slots[index].ItemData;
        Debug.Log($"Used item: {itemData.Name} from {GetModelName(model)} inventory.");

        // ������ ������ �������������: ���� ��� �����, ������� 1 �����
        if (itemData.Type == ItemType.Potion)
        {
            model.Slots[index].RemoveQuantity(1);
            // ����� ������� ������, ��� ������ ����������, ����� View ��������������
            // ������ TryAddItem/RemoveItem/MoveItem � ������ ������ ��� ����,
            // � ��� �� ������ ���� ��������, ������� ����� ������ �����.
            model.OnInventoryUpdated?.Invoke();
        }
    }

    #endregion

    #region Helper Methods (��������������� ������)

    /// <summary>
    /// �������� ������ ����������� ��������.
    /// </summary>
    private void TransferItem(InventoryModel fromModel, InventoryModel toModel, int fromIndex, int toIndex)
    {
        // ������ 1: ����������� ������ ������ � ���� �� ���������
        if (fromModel == toModel)
        {
            fromModel.MoveItem(fromIndex, toIndex);
        }
        // ������ 2: ����������� ����� ������� �����������
        else
        {
            var itemSlotToMove = fromModel.Slots[fromIndex];
            // �������� �������� ������� � ������� ���������
            if (toModel.TryAddItem(itemSlotToMove.ItemData, itemSlotToMove.Quantity))
            {
                // ���� ���������� ������ �������, ������� ������� �� ��������� ���������
                fromModel.RemoveItem(fromIndex);
            }
        }
    }

    /// <summary>
    /// ������� UI-����, ������� � ������ ������ ��������� ��� �������� ����.
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
    /// ����������, ����� ������ ������ (`InventoryModel`) ������������� ������ UI-���� (`InventorySlotView`).
    /// </summary>
    private InventoryModel GetModelForSlot(InventorySlotView slotView)
    {
        // ���������, ����������� �� ���� ���� View ��������� ������
        bool isPlayerSlot = _windowView.PlayerInventoryView.GetSlotViews().Contains(slotView);
        return isPlayerSlot ? _playerInventoryModel : _containerInventoryModel;
    }

    /// <summary>
    /// ���������� ��������� ��� ������ ��� �����������.
    /// </summary>
    private string GetModelName(InventoryModel model)
    {
        return model == _playerInventoryModel ? "Player" : "Container";
    }

    #endregion
}