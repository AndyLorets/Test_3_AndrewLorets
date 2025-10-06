using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// [MODEL] - Главный класс, отвечающий за логику и хранение состояния одного инвентаря.
/// Содержит список слотов и методы для управления ими.
/// </summary>
public class InventoryModel
{
    /// <summary>
    /// Событие, которое вызывается каждый раз, когда содержимое инвентаря изменяется.
    /// UI (InventoryView) подписывается на него, чтобы знать, когда нужно перерисоваться.
    /// </summary>
    public event Action OnInventoryUpdated;

    /// <summary>
    /// Список слотов, составляющих этот инвентарь.
    /// </summary>
    public List<InventorySlot> Slots { get; private set; }

    /// <summary>
    /// Вместимость инвентаря (количество слотов).
    /// </summary>
    public int Capacity => Slots.Count;

    /// <summary>
    /// Конструктор. Создает инвентарь с заданным количеством пустых слотов.
    /// </summary>
    public InventoryModel(int capacity)
    {
        Slots = new List<InventorySlot>(capacity);
        for (int i = 0; i < capacity; i++)
        {
            Slots.Add(new InventorySlot());
        }
    }

    /// <summary>
    /// Пытается добавить предмет в инвентарь.
    /// Сначала ищет существующие стаки, затем - пустые слоты.
    /// </summary>
    /// <returns>True, если все предметы удалось добавить. False, если инвентарь полон.</returns>
    public bool TryAddItem(ItemData item, int quantity = 1)
    {
        if (item == null || quantity <= 0) return false;

        if (item.CanStack)
        {
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

        OnInventoryUpdated?.Invoke();
        return quantity <= 0;
    }

    /// <summary>
    /// Статический метод для перемещения предмета между двумя инвентарями (или внутри одного).
    /// Это центральная точка для всей логики drag & drop.
    /// </summary>
    /// <param name="fromModel">Инвентарь-источник.</param>
    /// <param name="fromIndex">Индекс слота-источника.</param>
    /// <param name="toModel">Целевой инвентарь.</param>
    /// <param name="toIndex">Индекс целевого слота.</param>
    public static void TransferItem(InventoryModel fromModel, int fromIndex, InventoryModel toModel, int toIndex)
    {
        InventorySlot fromSlot = fromModel.Slots[fromIndex];
        if (fromSlot.IsEmpty) return;

        if (fromModel == toModel)
        {
            fromModel.MoveItemWithin(fromIndex, toIndex);
        }
        else
        {
            InventorySlot toSlot = toModel.Slots[toIndex];

            if (toSlot.IsEmpty || toSlot.CanAddItem(fromSlot.ItemData))
            {
                int maxStackSize = fromSlot.ItemData.MaxStackSize;
                int canAddToStack = toSlot.IsEmpty ? maxStackSize : maxStackSize - toSlot.Quantity;
                int amountToMove = Mathf.Min(fromSlot.Quantity, canAddToStack);

                if (amountToMove > 0)
                {
                    if (toSlot.IsEmpty) toSlot.SetItem(fromSlot.ItemData, amountToMove);
                    else toSlot.AddQuantity(amountToMove);

                    fromSlot.RemoveQuantity(amountToMove);
                }
            }
            else
            {
                ItemData tempItem = toSlot.ItemData;
                int tempQuantity = toSlot.Quantity;
                toSlot.SetItem(fromSlot.ItemData, fromSlot.Quantity);
                fromSlot.SetItem(tempItem, tempQuantity);
            }
        }

        fromModel.OnInventoryUpdated?.Invoke();
        if (fromModel != toModel) toModel.OnInventoryUpdated?.Invoke();
    }

    /// <summary>
    /// Приватный метод для перемещения предмета внутри этого же инвентаря.
    /// </summary>
    private void MoveItemWithin(int fromIndex, int toIndex)
    {
        InventorySlot fromSlot = Slots[fromIndex];
        InventorySlot toSlot = Slots[toIndex];

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
        else
        {
            ItemData tempItem = toSlot.ItemData;
            int tempQuantity = toSlot.Quantity;
            toSlot.SetItem(fromSlot.ItemData, fromSlot.Quantity);
            fromSlot.SetItem(tempItem, tempQuantity);
        }
    }

    /// <summary>
    /// Удаляет предмет из указанного слота.
    /// </summary>
    public void RemoveItemQuantity(int index, int count)
    {
        if (index < 0 || index >= Capacity || Slots[index].IsEmpty) return;

        Slots[index].RemoveQuantity(count);
        OnInventoryUpdated?.Invoke();
    }

    /// <summary>
    /// Полностью удаляет предмет из указанного слота.
    /// </summary>
    public void RemoveItem(int index)
    {
        if (index < 0 || index >= Capacity || Slots[index].IsEmpty) return;

        Slots[index].Clear();
        OnInventoryUpdated?.Invoke();
    }

    /// <summary>
    /// Публичный метод для принудительного вызова события OnInventoryUpdated извне.
    /// Используется после загрузки данных, чтобы UI перерисовался.
    /// </summary>
    public void ForceUpdate()
    {
        OnInventoryUpdated?.Invoke();
    }

    /// <summary>
    /// Сортирует слоты инвентаря по заданному критерию.
    /// </summary>
    /// <param name="criteria">Критерий для основной сортировки.</param>
    public void Sort(SortCriteria criteria)
    {
        IOrderedEnumerable<InventorySlot> sortedSlots;

        switch (criteria)
        {
            case SortCriteria.ByType:
                sortedSlots = Slots.OrderBy(slot => slot.IsEmpty)
                                   .ThenBy(slot => slot.ItemData?.Type);
                break;
            case SortCriteria.ByName:
                sortedSlots = Slots.OrderBy(slot => slot.IsEmpty)
                                   .ThenBy(slot => slot.ItemData?.Name);
                break;
            case SortCriteria.ByQuantity:
                sortedSlots = Slots.OrderBy(slot => slot.IsEmpty)
                                   .ThenByDescending(slot => slot.Quantity);
                break;
            default:
                sortedSlots = Slots.OrderBy(slot => slot.IsEmpty)
                                   .ThenBy(slot => slot.ItemData?.Name);
                break;
        }

        Slots = sortedSlots.ThenBy(slot => slot.ItemData?.Name).ToList();

        OnInventoryUpdated?.Invoke();
    }

}
public enum SortCriteria
{
    ByType,
    ByName,
    ByQuantity
}