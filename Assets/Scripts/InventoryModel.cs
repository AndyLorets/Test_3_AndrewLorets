using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine; // Для Debug.Log

public class InventoryModel
{
    // Используем 'event' для безопасности, чтобы никто извне не мог вызвать событие
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
            // Сначала пытаемся добавить в существующие стаки
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

        // Если остались предметы (или предмет не стакается), ищем пустые слоты
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

        // Если что-то добавили, но не всё, всё равно обновляем UI
        if (quantity < item.MaxStackSize)
        {
            OnInventoryUpdated?.Invoke();
        }

        return quantity <= 0;
    }

    // НОВЫЙ МЕТОД: централизованная логика перемещения
    public static void TransferItem(InventoryModel fromModel, int fromIndex, InventoryModel toModel, int toIndex)
    {
        InventorySlot fromSlot = fromModel.Slots[fromIndex];
        if (fromSlot.IsEmpty) return;

        // Если перемещаем внутри одного инвентаря
        if (fromModel == toModel)
        {
            fromModel.MoveItemWithin(fromIndex, toIndex);
        }
        // Если перемещаем между разными инвентарями
        else
        {
            // Пытаемся добавить в целевой слот (он может быть не пустым)
            InventorySlot toSlot = toModel.Slots[toIndex];

            // Если целевой слот пуст или предметы одинаковые и можно стакать
            if (toSlot.IsEmpty || (toSlot.CanAddItem(fromSlot.ItemData)))
            {
                int canAddToStack = toSlot.ItemData != null ? toSlot.ItemData.MaxStackSize - toSlot.Quantity : fromSlot.ItemData.MaxStackSize;
                int amountToMove = Mathf.Min(fromSlot.Quantity, canAddToStack);

                if (amountToMove > 0)
                {
                    // Если целевой слот был пуст, устанавливаем предмет. Если нет - добавляем количество
                    if (toSlot.IsEmpty) toSlot.SetItem(fromSlot.ItemData, amountToMove);
                    else toSlot.AddQuantity(amountToMove);

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
        }

        // Обновляем оба инвентаря
        fromModel.OnInventoryUpdated?.Invoke();
        toModel.OnInventoryUpdated?.Invoke();
    }

    private void MoveItemWithin(int fromIndex, int toIndex)
    {
        InventorySlot fromSlot = Slots[fromIndex];
        InventorySlot toSlot = Slots[toIndex];

        // Если предметы одинаковые и стакаются, объединяем
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
        // В остальных случаях просто меняем местами
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