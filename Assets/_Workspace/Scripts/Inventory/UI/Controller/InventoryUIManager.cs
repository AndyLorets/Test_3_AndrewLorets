using UnityEngine;
using Zenject;

/// <summary>
/// [CONTROLLER / MANAGER] - Главный "дирижер" всей системы инвентаря.
/// Отвечает за:
/// 1. Связывание событий из игрового мира (клики по объектам) с UI.
/// 2. Управление открытием и закрытием окна инвентаря.
/// 3. Передачу правильных моделей данных (инвентарей) в UI для отображения.
/// 4. Управление глобальным состоянием (GameState.IsUiOpen).
/// 5. Инициацию сохранения данных.
/// </summary>
public class InventoryUIManager : IInitializable, ITickable
{
    private readonly PlayerInventoryHolder _playerInventoryHolder;
    private readonly InventoryWindowView _inventoryWindowView;
    private readonly InventoryController _inventoryController;

    private IInteractable _currentlyOpenContainer;

    /// <summary>
    /// Конструктор. Получает все необходимые зависимости через Zenject.
    /// </summary>
    public InventoryUIManager(PlayerInventoryHolder playerHolder, InventoryWindowView windowView,
                              InventoryController inventoryController)
    {
        _playerInventoryHolder = playerHolder;
        _inventoryWindowView = windowView;
        _inventoryController = inventoryController;
    }

    /// <summary>
    /// Инициализация. Вызывается Zenject'ом один раз на старте.
    /// Здесь мы подписываемся на все глобальные события.
    /// </summary>
    public void Initialize()
    {
        _playerInventoryHolder.OnInteracted += HandleInteraction;

        var containers = Object.FindObjectsOfType<WorldContainer>();
        foreach (var container in containers)
        {
            container.OnInteracted += HandleInteraction;
        }

        _inventoryWindowView.OnSortButtonClicked += HandleSortButtonClick;
        _inventoryWindowView.OnCloseButtonClicked += HideInventory;
    }

    /// <summary>
    /// Единый обработчик для взаимодействия с ЛЮБЫМ интерактивным объектом.
    /// </summary>
    private void HandleInteraction(IInteractable interactableObject)
    {
        if (interactableObject is PlayerInventoryHolder)
        {
            OpenInventory(interactableObject, null);
        }
        else
        {
            OpenInventory(_playerInventoryHolder, interactableObject);
        }
    }

    /// <summary>
    /// Общий метод для открытия окна инвентаря.
    /// </summary>
    private void OpenInventory(IInteractable primary, IInteractable secondary)
    {
        _currentlyOpenContainer = secondary;

        GameState.IsUiOpen = true;

        _inventoryWindowView.Open(primary, secondary);
        _inventoryController.SubscribeToSlots();
    }

    /// <summary>
    /// Централизованный метод для закрытия окна.
    /// </summary>
    private void HideInventory()
    {
        if (!_inventoryWindowView.IsOpen) return;

        GameState.IsUiOpen = false;
        _inventoryWindowView.Hide();
    }

    /// <summary>
    /// Обработчик события клика по любой из кнопок сортировки.
    /// </summary>
    /// <param name="panelType">Какая панель должна быть отсортирована (Игрока или Контейнера).</param>
    /// <param name="criteria">По какому критерию сортировать.</param>
    private void HandleSortButtonClick(InventoryPanelType panelType, SortCriteria criteria)
    {
        InventoryModel modelToSort = null;

        if (panelType == InventoryPanelType.Player)
        {
            modelToSort = _playerInventoryHolder.Inventory;
        }
        else if (panelType == InventoryPanelType.Container && _currentlyOpenContainer != null)
        {
            modelToSort = _currentlyOpenContainer.Inventory;
        }

        if (modelToSort != null)
        {
            modelToSort.Sort(criteria);
        }
    }

    /// <summary>
    /// Вызывается каждый кадр. Отвечает за управление вводом.
    /// </summary>
    public void Tick()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (_inventoryWindowView.IsOpen)
                HideInventory();
            else
                HandleInteraction(_playerInventoryHolder);
        }

        if (Input.GetKeyDown(KeyCode.Escape) && _inventoryWindowView.IsOpen)
        {
            HideInventory();
        }
    }
}