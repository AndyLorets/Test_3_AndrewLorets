using UnityEngine;
using Zenject;

public class InventoryUIManager : IInitializable, ITickable
{
    private readonly PlayerInventoryHolder _playerInventoryHolder;
    private readonly InventoryWindowView _inventoryWindowView;
    private readonly InventoryController _inventoryController;

    public InventoryUIManager(PlayerInventoryHolder playerHolder, InventoryWindowView windowView, InventoryController inventoryController)
    {
        _playerInventoryHolder = playerHolder;
        _inventoryWindowView = windowView;
        _inventoryController = inventoryController;
    }

    public void Initialize()
    {
        _playerInventoryHolder.OnInteracted += OpenPlayerInventory;

        var containers = Object.FindObjectsOfType<WorldContainer>();
        foreach (var container in containers)
        {
            container.OnInteracted += OpenContainerInventory;
        }
    }
    private void OpenPlayerInventory(IInteractable player)
    {
        _inventoryWindowView.Open(player, null);
        _inventoryController.SubscribeToSlots();

        GameState.IsUiOpen = true; 
    }
    private void OpenContainerInventory(IInteractable container)
    {
        _inventoryWindowView.Open(_playerInventoryHolder, container);
        _inventoryController.SubscribeToSlots();
    }
    public void Tick()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && _inventoryWindowView.IsOpen)
        {
            _inventoryWindowView.Hide();
        }
    }
}