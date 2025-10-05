using System;

/// <summary>
/// ������� ����������� ����� ��� �������� ����������� ��������� ����,
/// ���������� �� ����� ����� ����.
/// </summary>
public static class GameState
{
    /// <summary>
    /// ���������� true, ���� � ������ ������ ������� �����-���� ���� UI,
    /// ����������� �������������� � ������� ����� (���������, �������, ���� ����� � �.�.).
    /// </summary>
    public static bool IsUiOpen { get; set; }
}


public interface IInteractable
{
    public event Action<IInteractable> OnInteracted;
    public InventoryModel Inventory { get; }
    public string ContainerName { get; }
}