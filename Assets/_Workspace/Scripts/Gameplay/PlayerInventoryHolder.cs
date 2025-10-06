using UnityEngine;
using Zenject;

/// <summary>
/// [GAMEPLAY] - MonoBehaviour, ������� "������" ��������� ������.
/// ���� ��������� ������ ������ �� ������� ������� Player.
/// �� ������� � ������� ����������� InventoryModel ��� ������, ����� ��� ���������
/// ��� ������ ������.
/// ��������� ��������� IInteractable, ����� �� ������ ����� ����, ��������, ��������
/// ��� �������� ��� ���������.
/// </summary>
public class PlayerInventoryHolder : MonoBehaviour, IInteractable
{
    [Tooltip("������ �� ScriptableObject � ����������� ��������� ������ (�����������, ��������)")]
    [SerializeField] private ContainerData _playerBackpackData;

    /// <summary>
    /// ���������� �������� ��������� ������ (��������, "������").
    /// ���������� �������� �� ���������� IInteractable.
    /// </summary>
    public string ContainerName => _playerBackpackData.ContainerName;

    /// <summary>
    /// ��������� ���������, ������������� ������.
    /// ��� "������������ �������� ������" � ��������� ������.
    /// ���������� �������� �� ���������� IInteractable.
    /// </summary>
    public InventoryModel Inventory { get; private set; }

    /// <summary>
    /// �������, ���������� ��� �������������� � �������.
    /// ���������� ������� �� ���������� IInteractable.
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
    /// �������� ���������. ���� �� ����������, ��������� ���������� ����������
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
    /// ���������� ���������.
    /// </summary>
    private void Save()
    {
        _saveLoadManager.SaveInventory(Inventory, SAVE_FILE_NAME);
    }
}