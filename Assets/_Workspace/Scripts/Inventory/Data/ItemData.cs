using UnityEngine;

/// <summary>
/// ������������ (enum) ��� ����������� ����� ���������.
/// ����� ����������� ������ ������.
/// </summary>
public enum ItemType
{
    Weapon,
    Potion,
    QuestItem
}

/// <summary>
/// [DATA] - ScriptableObject, ������� ������ "��������" ��� "��������" ��� ���� ��������� � ����.
/// ��������� �������������� ��������� � ����������� �������� ��� ������ � �������,
/// �� ������ ���.
/// ������� [CreateAssetMenu] ��������� ����� ��� �������� ����� ������� � ���� Unity (Assets/Create/...).
/// </summary>
[CreateAssetMenu(fileName = "New ItemData", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string Name;
    [TextArea(3, 10)] 
    public string Description;
    public ItemType Type;
    public Sprite Icon;

    [Header("Stacking")]
    public bool CanStack;
    [Tooltip("������������ ���������� ��������� � ����� ����� (������������, ���� CanStack = false)")]
    public int MaxStackSize = 1;

    /// <summary>
    /// �����, ������� ������������� ���������� � ��������� Unity ������ ���,
    /// ����� �������� ���������� � ����������.
    /// ������������ ��� ��������� ������ � �������������� ������ ������������.
    /// </summary>
    private void OnValidate()
    {
        if (!CanStack)
        {
            MaxStackSize = 1;
        }
    }
}