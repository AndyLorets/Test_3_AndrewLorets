using Zenject;
using UnityEngine;

public class InventoryInstaller : MonoInstaller
{
    [Header("������� �� �����")]
    [SerializeField] private Canvas _mainCanvas;
    [SerializeField] private InventoryWindowView _inventoryWindowInstance;

    [Header("������� ��� ��������")]
    [SerializeField] private DragIconView _dragIconViewPrefab;
    [SerializeField] private TooltipView _tooltipViewPrefab;

    private const int PLAYER_INVENTORY_CAPACITY = 20;
    private const int CONTAINER_INVENTORY_CAPACITY = 12;

    public override void InstallBindings()
    {
        // --- MODEL ---
        Container.Bind<InventoryModel>()
                 .WithId(InventoryIDs.Player)
                 .FromMethod(_ => new InventoryModel(PLAYER_INVENTORY_CAPACITY))
                 .AsCached();

        Container.Bind<InventoryModel>()
                 .WithId(InventoryIDs.Container)
                 .FromMethod(_ => new InventoryModel(CONTAINER_INVENTORY_CAPACITY))
                 .AsCached();

        // --- VIEW ---
        // --- ��������� 2: ���������� FromInstance() ---
        // ������ ���������� ��������� InventoryWindowView, ������� ��� ���� �� �����
        Container.Bind<InventoryWindowView>()
                 .FromInstance(_inventoryWindowInstance)
                 .AsSingle();

        // ��������� View ������� �� ��������, ��� � ������
        Container.Bind<DragIconView>().FromComponentInNewPrefab(_dragIconViewPrefab)
            .UnderTransform(_mainCanvas.transform).AsSingle();

        Container.Bind<TooltipView>().FromComponentInNewPrefab(_tooltipViewPrefab)
            .UnderTransform(_mainCanvas.transform).AsSingle();

        // --- CONTROLLERS ---
        Container.BindInterfacesAndSelfTo<InventoryController>().AsSingle().NonLazy();
    }
}