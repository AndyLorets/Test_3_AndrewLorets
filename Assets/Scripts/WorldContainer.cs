using UnityEngine;
using System;

[RequireComponent(typeof(Collider2D))]
public class WorldContainer : MonoBehaviour, IInteractable
{
    [SerializeField] private ContainerData _containerData;

    // Свойства и события, необходимые для интерфейса IInteractable
    public InventoryModel Inventory { get; private set; }
    public string ContainerName => _containerData.ContainerName;
    public event Action<IInteractable> OnInteracted;

    private void Awake()
    {
        Inventory = new InventoryModel(_containerData.Capacity);
        foreach (var startingItem in _containerData.StartingItems)
        {
            Inventory.TryAddItem(startingItem.Item, startingItem.Quantity);
        }
    }

    // Логика клика остается, так как это основная функция контейнера
    private void OnMouseDown()
    {
        OnInteracted?.Invoke(this);
    }
}

// Простой интерфейс для всех интерактивных объектов
public interface IInteractable
{
    public event Action<IInteractable> OnInteracted;
    public InventoryModel Inventory { get; }
    public string ContainerName { get; }
}