using UnityEngine;

/// <summary>
/// Перечисление (enum) для определения типов предметов.
/// Легко расширяется новыми типами.
/// </summary>
public enum ItemType
{
    Weapon,
    Potion,
    QuestItem
}

/// <summary>
/// [DATA] - ScriptableObject, который служит "шаблоном" или "чертежом" для всех предметов в игре.
/// Позволяет геймдизайнерам создавать и настраивать предметы как ассеты в проекте,
/// не трогая код.
/// Атрибут [CreateAssetMenu] добавляет опцию для создания таких ассетов в меню Unity (Assets/Create/...).
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
    [Tooltip("Максимальное количество предметов в одном стаке (игнорируется, если CanStack = false)")]
    public int MaxStackSize = 1;

    /// <summary>
    /// Метод, который автоматически вызывается в редакторе Unity каждый раз,
    /// когда значение изменяется в инспекторе.
    /// Используется для валидации данных и предотвращения ошибок конфигурации.
    /// </summary>
    private void OnValidate()
    {
        if (!CanStack)
        {
            MaxStackSize = 1;
        }
    }
}