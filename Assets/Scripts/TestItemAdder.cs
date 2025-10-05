using UnityEngine;
using Zenject;

public class TestItemAdder : MonoBehaviour
{
    [SerializeField] private ItemData _sword;
    [SerializeField] private ItemData _potion;

    // Запрашиваем оба инвентаря, каждый со своим ID
    [Inject(Id = InventoryIDs.Player)]
    private InventoryModel _playerInventory;

    [Inject(Id = InventoryIDs.Container)]
    private InventoryModel _containerInventory;


    void Update()
    {
        // Добавить меч игроку
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (_playerInventory.TryAddItem(_sword))
            {
                Debug.Log("Added Sword to Player");
            }
        }

        // Добавить 3 зелья в контейнер (сундук)
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (_containerInventory.TryAddItem(_potion, 3))
            {
                Debug.Log("Added 3 Potions to Container");
            }
        }
    }
}