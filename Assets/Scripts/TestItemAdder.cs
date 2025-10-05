using UnityEngine;
using Zenject;

public class TestItemAdder : MonoBehaviour
{
    [SerializeField] private ItemData _sword;
    [SerializeField] private ItemData _potion;

    // ����������� ��� ���������, ������ �� ����� ID
    [Inject(Id = InventoryIDs.Player)]
    private InventoryModel _playerInventory;

    [Inject(Id = InventoryIDs.Container)]
    private InventoryModel _containerInventory;


    void Update()
    {
        // �������� ��� ������
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (_playerInventory.TryAddItem(_sword))
            {
                Debug.Log("Added Sword to Player");
            }
        }

        // �������� 3 ����� � ��������� (������)
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (_containerInventory.TryAddItem(_potion, 3))
            {
                Debug.Log("Added 3 Potions to Container");
            }
        }
    }
}