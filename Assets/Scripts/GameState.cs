using System;

/// <summary>
/// Простой статический класс для хранения глобального состояния игры,
/// доступного из любой точки кода.
/// </summary>
public static class GameState
{
    /// <summary>
    /// Возвращает true, если в данный момент открыто какое-либо окно UI,
    /// блокирующее взаимодействие с игровым миром (инвентарь, диалоги, меню паузы и т.д.).
    /// </summary>
    public static bool IsUiOpen { get; set; }
}


public interface IInteractable
{
    public event Action<IInteractable> OnInteracted;
    public InventoryModel Inventory { get; }
    public string ContainerName { get; }
}