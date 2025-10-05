using UnityEngine;
using System;

[RequireComponent(typeof(Collider2D))]
public class WorldContainer : MonoBehaviour, IInteractable
{
    [SerializeField] private ContainerData _containerData;

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

    private void OnMouseDown()
    {
        if (GameState.IsUiOpen) return;

        OnInteracted?.Invoke(this);
    }
}