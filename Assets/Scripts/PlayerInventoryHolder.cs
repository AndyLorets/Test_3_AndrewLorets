using UnityEngine;

public class PlayerInventoryHolder : MonoBehaviour, IInteractable
{
    [SerializeField] private ContainerData _playerBackpackData;
    public string ContainerName => _playerBackpackData.ContainerName;
    public InventoryModel Inventory { get; private set; }

    public event System.Action<IInteractable> OnInteracted;

    void Awake()
    {
        // Создаем инвентарь игрока один раз на всю игру
        Inventory = new InventoryModel(_playerBackpackData.Capacity);
        // Тут можно добавить логику загрузки сохраненных предметов
    }

    private void OnMouseDown()
    {
        // По клику на себя, игрок открывает свой инвентарь (без второго окна)
        OnInteracted?.Invoke(this);
    }
}