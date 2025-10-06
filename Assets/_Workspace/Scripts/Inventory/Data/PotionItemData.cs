using UnityEngine;

[CreateAssetMenu(fileName = "New Potion", menuName = "Inventory/Items/Potion")]
public class PotionItemData : ItemData, IUsable
{
    [Header("Potion Specific")]
    public int SpecificAmount = 50;

    /// <summary>
    /// ���������� ������ �������������.
    /// </summary>
    public void Use()
    {
        Debug.Log($"������������ ����� '{Name}': {SpecificAmount}");
    }
}