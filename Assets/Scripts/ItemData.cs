using UnityEngine;

public enum ItemType
{
    Weapon,
    Potion,
    QuestItem
}

[CreateAssetMenu(fileName = "New ItemData", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string Name;
    public string Description;
    public ItemType Type;
    public Sprite Icon;

    [Header("Stacking")]
    public bool CanStack;
    public int MaxStackSize = 1;

    private void OnValidate()
    {
        if (!CanStack)
        {
            MaxStackSize = 1;
        }
    }
}