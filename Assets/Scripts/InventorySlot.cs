using System;

[Serializable]
public class InventorySlot
{
    public ItemData ItemData { get; private set; }
    public int Quantity { get; private set; }

    public bool IsEmpty => ItemData == null;

    public InventorySlot()
    {
        Clear();
    }

    public void SetItem(ItemData item, int quantity)
    {
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