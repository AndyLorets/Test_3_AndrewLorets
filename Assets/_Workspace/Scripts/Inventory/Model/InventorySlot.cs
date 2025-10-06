using System;

[Serializable]
public class InventorySlot
{
    public ItemData ItemData { get; private set; }
    public int Quantity { get; private set; }

    public bool IsEmpty => ItemData == null;
    public bool IsFull => !IsEmpty && Quantity >= ItemData.MaxStackSize;

    public InventorySlot()
    {
        Clear();
    }

    /// <summary>
    /// Проверяет, можно ли добавить данный предмет в этот слот.
    /// </summary>
    /// <param name="itemToAdd">Предмет для проверки.</param>
    /// <returns>True, если предмет можно добавить.</returns>
    public bool CanAddItem(ItemData itemToAdd)
    {
        // Нельзя добавить, если:
        // 1. Слот не пустой и предметы разные.
        // 2. Слот уже полностью заполнен.
        // 3. Предмет нельзя стакать (а слот уже занят).
        return !IsEmpty && ItemData == itemToAdd && !IsFull;
    }

    public void SetItem(ItemData item, int quantity)
    {
        if (item == null || quantity <= 0)
        {
            Clear();
            return;
        }
        ItemData = item;
        Quantity = quantity;
    }

    public void Clear()
    {
        ItemData = null;
        Quantity = 0;
    }

    public void AddQuantity(int amount)
    {
        Quantity += amount;
    }

    public void RemoveQuantity(int amount)
    {
        Quantity -= amount;
        if (Quantity <= 0)
        {
            Clear();
        }
    }
}