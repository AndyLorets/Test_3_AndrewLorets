using UnityEngine;
using System;
using Zenject;

/// <summary>
/// [GAMEPLAY] - MonoBehaviour, который превращает любой 2D-объект в интерактивный контейнер (сундук, полку, труп и т.д.).
/// Должен висеть на объекте с Collider2D.
/// Создает и хранит свой уникальный инвентарь на основе данных ContainerData.
/// Реализует интерфейс IInteractable, чтобы UIManager мог работать с ним так же, как с игроком.
/// </summary>
[RequireComponent(typeof(Collider2D), typeof(GuidComponent))]
public class WorldContainer : MonoBehaviour, IInteractable
{
    [Tooltip("Ссылка на ScriptableObject с настройками этого контейнера (вместимость, название, стартовый лут)")]
    [SerializeField] private ContainerData _containerData;

    private SaveLoadManager _saveLoadManager;
    private GuidComponent _guidComponent;

    private string SAVE_FILE_NAME => $"WorldContainer_{_guidComponent.Guid}";

    /// <summary>
    /// Уникальный экземпляр инвентаря, принадлежащий этому конкретному объекту в мире.
    /// Реализация свойства из интерфейса IInteractable.
    /// </summary>
    public InventoryModel Inventory { get; private set; }

    /// <summary>
    /// Название контейнера (например, "Книжная полка").
    /// Реализация свойства из интерфейса IInteractable.
    /// </summary>
    public string ContainerName => _containerData.ContainerName;

    /// <summary>
    /// Событие, вызываемое при взаимодействии с этим контейнером.
    /// Реализация события из интерфейса IInteractable.
    /// </summary>
    public event Action<IInteractable> OnInteracted;

    [Inject]
    public void Construct(SaveLoadManager saveLoadManager)
    {
        _saveLoadManager = saveLoadManager;
    }
    private void Awake()
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
        _guidComponent = GetComponent<GuidComponent>();
        Inventory = new InventoryModel(_containerData.Capacity);

        if (!_saveLoadManager.LoadInventory(Inventory, SAVE_FILE_NAME))
        {
            foreach (var startingItem in _containerData.StartingItems)
            {
                Inventory.TryAddItem(startingItem.Item, startingItem.Quantity);
            }
        }
    }

    /// <summary>
    /// Срхранение инвентаря. 
    /// </summary>
    public void Save()
    {
        if (_saveLoadManager != null && Inventory != null && _guidComponent != null)
        {
            _saveLoadManager.SaveInventory(Inventory, SAVE_FILE_NAME);
        }
    }
}