using UnityEngine;

public class PlayerInventoryHolder : MonoBehaviour, IInteractable
{
    [SerializeField] private ContainerData _playerBackpackData;
    public string ContainerName => _playerBackpackData.ContainerName;
    public InventoryModel Inventory { get; private set; }

    public event System.Action<IInteractable> OnInteracted;

    void Awake()
    {
        Inventory = new InventoryModel(_playerBackpackData.Capacity);
    }

    private void OnMouseDown()
    {
        if (GameState.IsUiOpen) return;

        OnInteracted?.Invoke(this);
    }
}