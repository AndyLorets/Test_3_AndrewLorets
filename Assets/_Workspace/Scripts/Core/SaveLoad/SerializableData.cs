using System;
using System.Collections.Generic;

/// <summary>
/// Класс, который можно легко сериализовать в JSON.
/// Хранит состояние одного инвентаря.
/// </summary>
[Serializable]
public class SerializableInventory
{
    public List<SerializableSlot> Slots;

    public SerializableInventory(InventoryModel model)
    {
        Slots = new List<SerializableSlot>();
        foreach (var slot in model.Slots)
        {
            Slots.Add(new SerializableSlot(slot));
        }
    }
}

/// <summary>
/// Класс, который можно легко сериализовать в JSON.
/// Хранит состояние одного слота.
/// </summary>
[Serializable]
public class SerializableSlot
{
    public string ItemID; 
    public int Quantity;

    public SerializableSlot(InventorySlot slot)
    {
        ItemID = slot.IsEmpty ? "" : slot.ItemData.name;
        Quantity = slot.Quantity;
    }
}