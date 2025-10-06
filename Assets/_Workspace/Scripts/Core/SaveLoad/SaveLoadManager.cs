using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveLoadManager
{
    private readonly Dictionary<string, ItemData> _itemDatabase;
    private readonly string _saveDirectoryPath;

    public SaveLoadManager()
    {
        _itemDatabase = new Dictionary<string, ItemData>();
        var allItems = Resources.LoadAll<ItemData>("Configs/Item Datas");
        foreach (var item in allItems)
        {
            if (!_itemDatabase.ContainsKey(item.name))
            {
                _itemDatabase.Add(item.name, item);
            }
            else
            {
                Debug.LogWarning($"Duplicate ItemData name found: {item.name}. Only one will be used.");
            }
        }


        _saveDirectoryPath = Path.Combine(Application.dataPath, "_Workspace", "Saves");

#if UNITY_EDITOR
        if (!Directory.Exists(_saveDirectoryPath))
        {
            Directory.CreateDirectory(_saveDirectoryPath);
            UnityEditor.AssetDatabase.Refresh();
        }
#endif
    }

    /// <summary>
    /// Сохраняет состояние инвентаря в JSON файл.
    /// </summary>
    public void SaveInventory(InventoryModel inventoryModel, string fileName)
    {
        SerializableInventory serializableInventory = new SerializableInventory(inventoryModel);
        string json = JsonUtility.ToJson(serializableInventory, true);
        string filePath = Path.Combine(_saveDirectoryPath, fileName + ".json");

        File.WriteAllText(filePath, json);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif

        Debug.Log($"Inventory saved to file: {filePath}");
    }

    /// <summary>
    /// Загружает состояние инвентаря из JSON файла.
    /// </summary>
    public bool LoadInventory(InventoryModel inventoryModel, string fileName)
    {
        string filePath = Path.Combine(_saveDirectoryPath, fileName + ".json");

        if (!File.Exists(filePath))
        {
            return false;
        }

        string json = File.ReadAllText(filePath);
        if (string.IsNullOrWhiteSpace(json))
        {
            return false;
        }

        SerializableInventory serializableInventory = JsonUtility.FromJson<SerializableInventory>(json);

        for (int i = 0; i < inventoryModel.Capacity; i++)
        {
            if (i < serializableInventory.Slots.Count)
            {
                var savedSlot = serializableInventory.Slots[i];
                if (!string.IsNullOrEmpty(savedSlot.ItemID) && _itemDatabase.TryGetValue(savedSlot.ItemID, out ItemData itemData))
                {
                    inventoryModel.Slots[i].SetItem(itemData, savedSlot.Quantity);
                }
                else
                {
                    inventoryModel.Slots[i].Clear();
                }
            }
        }

        inventoryModel.ForceUpdate();
        Debug.Log($"Inventory loaded from file: {filePath}");
        return true;
    }
}