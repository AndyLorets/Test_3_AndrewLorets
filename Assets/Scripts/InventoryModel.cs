using System;
using System.Collections.Generic;
using System.Linq;

public class InventoryModel
{
    public Action OnInventoryUpdated;

    public List<InventorySlot> Slots { get; private set; }
    private int _capacity;

    public InventoryModel(int capacity)
    {
        _capacity = capacity;
        Slots = new List<InventorySlot>(capacity);
        for (int i = 0; i < capacity; i++)
        {
            Slots.Add(new InventorySlot());
        }
    }

    public bool TryAddItem(ItemData item, int quantity = 1)
    {
        // Поиск слота, где уже есть такой предмет и есть место
        if (item.CanStack)
        {
            foreach (var slot in Slots.Where(s => !s.IsEmpty && s.ItemData == item))
            {
                int canAdd = item.MaxStackSize - slot.Quantity;
                if (canAdd >= quantity)
                {
                    slot.AddQuantity(quantity);
                    OnInventoryUpdated?.Invoke();
                    return true;
                }
                else if (canAdd > 0)
                {
                    slot.AddQuantity(canAdd);
                    quantity -= canAdd;
                }
            }
        }

        // Поиск первого пустого слота для остатка или для нового предмета
        foreach (var slot in Slots.Where(s => s.IsEmpty))
        {
            slot.SetItem(item, quantity);
            OnInventoryUpdated?.Invoke();
            return true;
        }

        // Инвентарь полон
        return false;
    }

    public void MoveItem(int fromIndex, int toIndex)
    {
        InventorySlot fromSlot = Slots[fromIndex];
        InventorySlot toSlot = Slots[toIndex];

        if (fromSlot.IsEmpty) return;

        // Если целевой слот пуст, просто перемещаем
        if (toSlot.IsEmpty)
        {
            toSlot.SetItem(fromSlot.ItemData, fromSlot.Quantity);
            fromSlot.Clear();
        }
        // Если предметы одинаковые и можно стакать
        else if (fromSlot.ItemData == toSlot.ItemData && toSlot.ItemData.CanStack)
        {
            int canAddToStack = toSlot.ItemData.MaxStackSize - toSlot.Quantity;
            int amountToMove = Math.Min(fromSlot.Quantity, canAddToStack);

            if (amountToMove > 0)
            {
                toSlot.AddQuantity(amountToMove);
                fromSlot.RemoveQuantity(amountToMove);
            }
        }
        // Если слоты заняты разными предметами, меняем их местами
        else
        {
            ItemData tempItem = toSlot.ItemData;
            int tempQuantity = toSlot.Quantity;

            toSlot.SetItem(fromSlot.ItemData, fromSlot.Quantity);
            fromSlot.SetItem(tempItem, tempQuantity);
        }

        OnInventoryUpdated?.Invoke();
    }

    public void RemoveItem(int index)
    {
        if (index < 0 || index >= Slots.Count || Slots[index].IsEmpty)
            return;

        Slots[index].Clear();
        OnInventoryUpdated?.Invoke();
    }
}