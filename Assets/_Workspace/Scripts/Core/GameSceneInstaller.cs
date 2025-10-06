using Zenject;
using UnityEngine;

public class GameSceneInstaller : MonoInstaller
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
        // ������ ���������� �� �����
        Container.Bind<PlayerInventoryHolder>().FromInstance(_playerInventoryHolderInstance).AsSingle();
        Container.Bind<InventoryWindowView>().FromInstance(_inventoryWindowInstance).AsSingle();

        // ������������ Transform ������ ��� ����������� ID, ����� �� ���� �������� � ������� Transform'���
        Container.Bind<Transform>()
                 .WithId("PlayerTransform")
                 .FromInstance(_playerInventoryHolderInstance.transform)
                 .AsSingle();

        // ������ ��������������� View...
        Container.Bind<DragIconView>().FromComponentInNewPrefab(_dragIconViewPrefab)
            .UnderTransform(_mainCanvas.transform).AsSingle();
        Container.Bind<TooltipView>().FromComponentInNewPrefab(_tooltipViewPrefab)
            .UnderTransform(_mainCanvas.transform).AsSingle();

        // ������ �����������
        Container.Bind<InventoryController>().AsSingle();
        Container.Bind<SaveLoadManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<InventoryUIManager>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<ItemPickupController>().AsSingle().NonLazy();

    }
}