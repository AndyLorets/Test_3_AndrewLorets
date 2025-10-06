using UnityEngine;
using System;

[RequireComponent(typeof(Collider2D))]
public class WorldItem : MonoBehaviour
{
    [Tooltip("������ � ��������, ������� ������������ ���� ������")]
    [SerializeField] private ItemData _itemData;
    [Tooltip("���������� ��������� � ���� �����")]
    [SerializeField] private int _quantity = 1;

    public static event Action<WorldItem, Action<bool>> OnItemPickupRequested;

    private Collider2D _collider;

    /// <summary>
    /// ���������� ������ � ��������.
    /// </summary>
    public ItemData ItemData => _itemData;

    /// <summary>
    /// ���������� ����������.
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