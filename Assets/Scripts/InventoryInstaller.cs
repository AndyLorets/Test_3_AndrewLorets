using Zenject;
using UnityEngine;

public class InventoryInstaller : MonoInstaller
{
    [Header("���������� �� �����")]
    [SerializeField] private Canvas _mainCanvas;
    [SerializeField] private PlayerInventoryHolder _playerInventoryHolderInstance; 
    [SerializeField] private InventoryWindowView _inventoryWindowInstance;

    [Header("������� ��� ��������")]
    [SerializeField] private DragIconView _dragIconViewPrefab;
    [SerializeField] private TooltipView _tooltipViewPrefab;

    public override void InstallBindings()
    {
        // --- VIEW ---
        // ������ ����������, ������� ��� ���������� �� �����.
        // Zenject ������ ��������, ��� �� �����.
        Container.Bind<PlayerInventoryHolder>().FromInstance(_playerInventoryHolderInstance).AsSingle();
        Container.Bind<InventoryWindowView>().FromInstance(_inventoryWindowInstance).AsSingle();

        // ������ ��������������� View, ������� ����� ������� �� ��������.
        Container.Bind<DragIconView>().FromComponentInNewPrefab(_dragIconViewPrefab)
            .UnderTransform(_mainCanvas.transform).AsSingle();

        Container.Bind<TooltipView>().FromComponentInNewPrefab(_tooltipViewPrefab)
            .UnderTransform(_mainCanvas.transform).AsSingle();

        // --- CONTROLLERS ---
        // ������ ����������, ���������� �� ������ drag & drop � �.�.
        // �� ������ �� ������� �� ������� ��������, ������� ������� �������.
        Container.Bind<InventoryController>().AsSingle();

        // ������ ������� "�������", ������� ��������� ��� � UI.
        // BindInterfacesAndSelfTo + NonLazy �������� ��� ��������� �� ������
        // � ���������� ����������� �� ��� ������ ������� (� ����� ������ Initialize).
        Container.BindInterfacesAndSelfTo<InventoryUIManager>().AsSingle().NonLazy();
    }
}