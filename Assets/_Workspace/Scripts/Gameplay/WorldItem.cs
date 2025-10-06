using UnityEngine;
using System;

[RequireComponent(typeof(Collider2D))]
public class WorldItem : MonoBehaviour
{
    [Tooltip("Данные о предмете, который представляет этот объект")]
    [SerializeField] private ItemData _itemData;
    [Tooltip("Количество предметов в этом стаке")]
    [SerializeField] private int _quantity = 1;

    public static event Action<WorldItem, Action<bool>> OnItemPickupRequested;

    private Collider2D _collider;

    /// <summary>
    /// Возвращает данные о предмете.
    /// </summary>
    public ItemData ItemData => _itemData;

    /// <summary>
    /// Возвращает количество.
    /// </summary>
    public int Quantity => _quantity;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }
    private void HandleClick()
    {
        _collider.enabled = false;

        OnItemPickupRequested?.Invoke(this, (success) => {
            if (success)
            {
                Destroy(gameObject);
            }
            else
            {
                _collider.enabled = true;
            }
        });
    }
    private void OnMouseDown()
    {
        if(GameState.IsUiOpen) return;  

        HandleClick(); 
    }
}