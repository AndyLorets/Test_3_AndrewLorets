using UnityEngine;
using Zenject;

/// <summary>
/// [GAMEPLAY] - MonoBehaviour, который "держит" инвентарь игрока.
/// Этот компонент должен висеть на игровом объекте Player.
/// Он создает и владеет экземпляром InventoryModel для игрока, делая его доступным
/// для других систем.
/// Реализует интерфейс IInteractable, чтобы на игрока можно было, например, кликнуть
/// для открытия его инвентаря.
/// </summary>
public class PlayerInventoryHolder : MonoBehaviour, IInteractable
{
    [Tooltip("Ссылка на ScriptableObject с настройками инвентаря игрока (вместимость, название)")]
    [SerializeField] private ContainerData _playerBackpackData;

    /// <summary>
    /// Возвращает название инвентаря игрока (например, "Рюкзак").
    /// Реализация свойства из интерфейса IInteractable.
    /// </summary>
    public string ContainerName => _playerBackpackData.ContainerName;

    /// <summary>
    /// Экземпляр инвентаря, принадлежащий игроку.
    /// Это "единственный источник правды" о предметах игрока.
    /// Реализация свойства из интерфейса IInteractable.
    /// </summary>
    public InventoryModel Inventory { get; private set; }

    /// <summary>
    /// Событие, вызываемое при взаимодействии с игроком.
    /// Реализация события из интерфейса IInteractable.
    /// </summary>
    public event System.Action<IInteractable> OnInteracted;

    private SaveLoadManager _saveLoadManager;

    private const string SAVE_FILE_NAME = "PlayerInventory";


    [Inject]
    public void Construct(SaveLoadManager saveLoadManager)
    {
        _saveLoadManager = saveLoadManager;
    }

    void Awake()
    {
        Load();
    }

    private void OnMouseDown()
    {
        if (GameState.IsUiOpen) return;

        OnInteracted?.Invoke(this);
    }
    private void OnApplicationQuit()
    {
        Save();
    }

    /// <summary>
    /// Загрузка инвентаря. Если не получилось, заполняем стартовыми предметами
    /// </summary>
    private void Load()
    {
        Inventory = new InventoryModel(_playerBackpackData.Capacity);

        if (!_saveLoadManager.LoadInventory(Inventory, SAVE_FILE_NAME))
        {
            foreach (var startingItem in _playerBackpackData.StartingItems)
            {
                Inventory.TryAddItem(startingItem.Item, startingItem.Quantity);
            }
        }
    }

    /// <summary>
    /// Сохранения инвентаря.
    /// </summary>
    private void Save()
    {
        _saveLoadManager.SaveInventory(Inventory, SAVE_FILE_NAME);
    }
}