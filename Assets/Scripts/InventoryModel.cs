using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine; // ��� Debug.Log

public class InventoryModel
{
    // ���������� 'event' ��� ������������, ����� ����� ����� �� ��� ������� �������
    public Action OnInventoryUpdated;

    public List<InventorySlot> Slots { get; private set; }
    public int Capacity => Slots.Count;

    public InventoryModel(int capacity)
    {
        Slots = new List<InventorySlot>(capacity);
        for (int i = 0; i < capacity; i++)
        {
            Slots.Add(new InventorySlot());
        }
    }

    public bool TryAddItem(ItemData item, int quantity = 1)
    {
        if (item == null || quantity <= 0) return false;

        if (item.CanStack)
        {
            // ������� �������� �������� � ������������ �����
            foreach (var slot in Slots.Where(s => s.CanAddItem(item)))
            {
                int canAdd = item.MaxStackSize - slot.Quantity;
                int amountToAdd = Mathf.Min(quantity, canAdd);
                slot.AddQuantity(amountToAdd);
                quantity -= amountToAdd;
                if (quantity <= 0)
                {
                    OnInventoryUpdated?.Invoke();
                    return true;
                }
            }
        }

        // ���� �������� �������� (��� ������� �� ���������), ���� ������ �����
        if (quantity > 0)
        {
            foreach (var slot in Slots.Where(s => s.IsEmpty))
            {
                int amountToAdd = Mathf.Min(quantity, item.MaxStackSize);
                slot.SetItem(item, amountToAdd);
                quantity -= amountToAdd;
                if (quantity <= 0)
                {
                    OnInventoryUpdated?.Invoke();
                    return true;
                }
            }
        }

        // ���� ���-�� ��������, �� �� ��, �� ����� ��������� UI
        if (quantity < item.MaxStackSize)
        {
            OnInventoryUpdated?.Invoke();
        }

        return quantity <= 0;
    }

    // ����� �����: ���������������� ������ �����������
    public static void TransferItem(InventoryModel fromModel, int fromIndex, InventoryModel toModel, int toIndex)
    {
        InventorySlot fromSlot = fromModel.Slots[fromIndex];
        if (fromSlot.IsEmpty) return;

        // ���� ���������� ������ ������ ���������
        if (fromModel == toModel)
        {
            fromModel.MoveItemWithin(fromIndex, toIndex);
        }
        // ���� ���������� ����� ������� �����������
        else
        {
            // �������� �������� � ������� ���� (�� ����� ���� �� ������)
            InventorySlot toSlot = toModel.Slots[toIndex];

            // ���� ������� ���� ���� ��� �������� ���������� � ����� �������
            if (toSlot.IsEmpty || (toSlot.CanAddItem(fromSlot.ItemData)))
            {
                int canAddToStack = toSlot.ItemData != null ? toSlot.ItemData.MaxStackSize - toSlot.Quantity : fromSlot.ItemData.MaxStackSize;
                int amountToMove = Mathf.Min(fromSlot.Quantity, canAddToStack);

                if (amountToMove > 0)
                {
                    // ���� ������� ���� ��� ����, ������������� �������. ���� ��� - ��������� ����������
                    if (toSlot.IsEmpty) toSlot.SetItem(fromSlot.ItemData, amountToMove);
                    else toSlot.AddQuantity(amountToMove);

                    fromSlot.RemoveQuantity(amountToMove);
                }
            }
            // ���� ����� ������ ������� ����������, ������ �� �������
            else
            {
                ItemData tempItem = toSlot.ItemData;
                int tempQuantity = toSlot.Quantity;

                toSlot.SetItem(fromSlot.ItemData, fromSlot.Quantity);
                fromSlot.SetItem(tempItem, tempQuantity);
            }
        }

        // ��������� ��� ���������
        fromModel.OnInventoryUpdated?.Invoke();
        toModel.OnInventoryUpdated?.Invoke();
    }

    private void MoveItemWithin(int fromIndex, int toIndex)
    {
        InventorySlot fromSlot = Slots[fromIndex];
        InventorySlot toSlot = Slots[toIndex];

        // ���� �������� ���������� � ���������, ����������
        if (!toSlot.IsEmpty && toSlot.CanAddItem(fromSlot.ItemData))
        {
            int canAddToStack = toSlot.ItemData.MaxStackSize - toSlot.Quantity;
            int amountToMove = Mathf.Min(fromSlot.Quantity, canAddToStack);
            if (amountToMove > 0)
            {
                toSlot.AddQuantity(amountToMove);
                fromSlot.RemoveQuantity(amountToMove);
            }
        }
        // � ��������� ������� ������ ������ �������
        else
        {
            ItemData tempItem = toSlot.ItemData;
            int tempQuantity = toSlot.Quantity;
            toSlot.SetItem(fromSlot.ItemData, fromSlot.Quantity);
            fromSlot.SetItem(tempItem, tempQuantity);
        }
    }

    public void RemoveItem(int index)
    {
        if (index < 0 || index >= Slots.Count || Slots[index].IsEmpty) return;

        Slots[index].Clear();
        OnInventoryUpdated?.Invoke();
    }
}