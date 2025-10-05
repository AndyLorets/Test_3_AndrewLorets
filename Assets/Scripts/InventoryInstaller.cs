using Zenject;
using UnityEngine;

public class InventoryInstaller : MonoInstaller
{
    [SerializeField] private Canvas _mainCanvas;
    [SerializeField] private PlayerInventoryHolder _playerInventoryHolderInstance; 
    [SerializeField] private InventoryWindowView _inventoryWindowInstance;
    [SerializeField] private DragIconView _dragIconViewPrefab;
    [SerializeField] private TooltipView _tooltipViewPrefab;

    public override void InstallBindings()
    {
        Container.Bind<PlayerInventoryHolder>().FromInstance(_playerInventoryHolderInstance).AsSingle();
        Container.Bind<InventoryWindowView>().FromInstance(_inventoryWindowInstance).AsSingle();

        Container.Bind<DragIconView>().FromComponentInNewPrefab(_dragIconViewPrefab)
            .UnderTransform(_mainCanvas.transform).AsSingle();

        Container.Bind<TooltipView>().FromComponentInNewPrefab(_tooltipViewPrefab)
            .UnderTransform(_mainCanvas.transform).AsSingle();

        Container.Bind<InventoryController>().AsSingle();

        Container.BindInterfacesAndSelfTo<InventoryUIManager>().AsSingle().NonLazy();
    }
}