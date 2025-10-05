using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ContainerData", menuName = "Inventory/Container Data")]
public class ContainerData : ScriptableObject
{
    public string ContainerName;    
    public int Capacity;
    public List<StartingItem> StartingItems;
}

[System.Serializable]
public struct StartingItem
{
    public ItemData Item;
    public int Quantity;
}