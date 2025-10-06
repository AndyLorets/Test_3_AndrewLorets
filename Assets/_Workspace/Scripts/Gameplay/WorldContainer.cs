using UnityEngine;
using System;
using Zenject;

/// <summary>
/// [GAMEPLAY] - MonoBehaviour, ������� ���������� ����� 2D-������ � ������������� ��������� (������, �����, ���� � �.�.).
/// ������ ������ �� ������� � Collider2D.
/// ������� � ������ ���� ���������� ��������� �� ������ ������ ContainerData.
/// ��������� ��������� IInteractable, ����� UIManager ��� �������� � ��� ��� ��, ��� � �������.
/// </summary>
[RequireComponent(typeof(Collider2D), typeof(GuidComponent))]
public class WorldContainer : MonoBehaviour, IInteractable
{
    [Tooltip("������ �� ScriptableObject � ����������� ����� ���������� (�����������, ��������, ��������� ���)")]
    [SerializeField] private ContainerData _containerData;

    private SaveLoadManager _saveLoadManager;
    private GuidComponent _guidComponent;

    private string SAVE_FILE_NAME => $"WorldContainer_{_guidComponent.Guid}";

    /// <summary>
    /// ���������� ��������� ���������, ������������� ����� ����������� ������� � ����.
    /// ���������� �������� �� ���������� IInteractable.
    /// </summary>
    public InventoryModel Inventory { get; private set; }

    /// <summary>
    /// �������� ���������� (��������, "������� �����").
    /// ���������� �������� �� ���������� IInteractable.
    /// </summary>
    public string ContainerName => _containerData.ContainerName;

    /// <summary>
    /// �������, ���������� ��� �������������� � ���� �����������.
    /// ���������� ������� �� ���������� IInteractable.
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
    /// �������� ���������. ���� �� ����������, ��������� ���������� ����������
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
    /// ���������� ���������. 
    /// </summary>
    public void Save()
    {
        if (_saveLoadManager != null && Inventory != null && _guidComponent != null)
        {
            _saveLoadManager.SaveInventory(Inventory, SAVE_FILE_NAME);
        }
    }
}