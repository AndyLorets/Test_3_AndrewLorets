using Zenject;
using UnityEngine;

public class GameSceneInstaller : MonoInstaller
{
    [Header("Компоненты со сцены")]
    [SerializeField] private Canvas _mainCanvas;
    [SerializeField] private PlayerInventoryHolder _playerInventoryHolderInstance; 
    [SerializeField] private InventoryWindowView _inventoryWindowInstance;

    [Header("Префабы для создания")]
    [SerializeField] private DragIconView _dragIconViewPrefab;
    [SerializeField] private TooltipView _tooltipViewPrefab;

    public override void InstallBindings()
    {
        // Биндим компоненты со сцены
        Container.Bind<PlayerInventoryHolder>().FromInstance(_playerInventoryHolderInstance).AsSingle();
        Container.Bind<InventoryWindowView>().FromInstance(_inventoryWindowInstance).AsSingle();

        // Регистрируем Transform игрока под специальным ID, чтобы не было путаницы с другими Transform'ами
        Container.Bind<Transform>()
                 .WithId("PlayerTransform")
                 .FromInstance(_playerInventoryHolderInstance.transform)
                 .AsSingle();

        // Биндим вспомогательные View...
        Container.Bind<DragIconView>().FromComponentInNewPrefab(_dragIconViewPrefab)
            .UnderTransform(_mainCanvas.transform).AsSingle();
        Container.Bind<TooltipView>().FromComponentInNewPrefab(_tooltipViewPrefab)
            .UnderTransform(_mainCanvas.transform).AsSingle();

        // Биндим контроллеры
        Container.Bind<InventoryController>().AsSingle();
        Container.Bind<SaveLoadManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<InventoryUIManager>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<ItemPickupController>().AsSingle().NonLazy();

    }
}