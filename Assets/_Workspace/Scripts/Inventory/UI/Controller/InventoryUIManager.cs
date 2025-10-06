using UnityEngine;
using Zenject;

/// <summary>
/// [CONTROLLER / MANAGER] - ������� "�������" ���� ������� ���������.
/// �������� ��:
/// 1. ���������� ������� �� �������� ���� (����� �� ��������) � UI.
/// 2. ���������� ��������� � ��������� ���� ���������.
/// 3. �������� ���������� ������� ������ (����������) � UI ��� �����������.
/// 4. ���������� ���������� ���������� (GameState.IsUiOpen).
/// 5. ��������� ���������� ������.
/// </summary>
public class InventoryUIManager : IInitializable, ITickable
{
    private readonly PlayerInventoryHolder _playerInventoryHolder;
    private readonly InventoryWindowView _inventoryWindowView;
    private readonly InventoryController _inventoryController;

    private IInteractable _currentlyOpenContainer;

    /// <summary>
    /// �����������. �������� ��� ����������� ����������� ����� Zenject.
    /// </summary>
    public InventoryUIManager(PlayerInventoryHolder playerHolder, InventoryWindowView windowView,
                              InventoryController inventoryController)
    {
        _playerInventoryHolder = playerHolder;
        _inventoryWindowView = windowView;
        _inventoryController = inventoryController;
    }

    /// <summary>
    /// �������������. ���������� Zenject'�� ���� ��� �� ������.
    /// ����� �� ������������� �� ��� ���������� �������.
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
    /// ������ ���������� ��� �������������� � ����� ������������� ��������.
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
    /// ����� ����� ��� �������� ���� ���������.
    /// </summary>
    private void OpenInventory(IInteractable primary, IInteractable secondary)
    {
        _currentlyOpenContainer = secondary;

        GameState.IsUiOpen = true;

        _inventoryWindowView.Open(primary, secondary);
        _inventoryController.SubscribeToSlots();
    }

    /// <summary>
    /// ���������������� ����� ��� �������� ����.
    /// </summary>
    private void HideInventory()
    {
        if (!_inventoryWindowView.IsOpen) return;

        GameState.IsUiOpen = false;
        _inventoryWindowView.Hide();
    }

    /// <summary>
    /// ���������� ������� ����� �� ����� �� ������ ����������.
    /// </summary>
    /// <param name="panelType">����� ������ ������ ���� ������������� (������ ��� ����������).</param>
    /// <param name="criteria">�� ������ �������� �����������.</param>
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
    /// ���������� ������ ����. �������� �� ���������� ������.
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