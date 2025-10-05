using Zenject;
using UnityEngine;

public class InventoryInstaller : MonoInstaller
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
        // --- VIEW ---
        // Биндим компоненты, которые уже существуют на сцене.
        // Zenject просто запомнит, где их найти.
        Container.Bind<PlayerInventoryHolder>().FromInstance(_playerInventoryHolderInstance).AsSingle();
        Container.Bind<InventoryWindowView>().FromInstance(_inventoryWindowInstance).AsSingle();

        // Биндим вспомогательные View, которые нужно создать из префабов.
        Container.Bind<DragIconView>().FromComponentInNewPrefab(_dragIconViewPrefab)
            .UnderTransform(_mainCanvas.transform).AsSingle();

        Container.Bind<TooltipView>().FromComponentInNewPrefab(_tooltipViewPrefab)
            .UnderTransform(_mainCanvas.transform).AsSingle();

        // --- CONTROLLERS ---
        // Биндим контроллер, отвечающий за логику drag & drop и т.д.
        // Он больше не зависит от моделей напрямую, поэтому биндинг простой.
        Container.Bind<InventoryController>().AsSingle();

        // Биндим ГЛАВНЫЙ "дирижер", который связывает мир и UI.
        // BindInterfacesAndSelfTo + NonLazy заставит его создаться на старте
        // и немедленно подписаться на все нужные события (в своем методе Initialize).
        Container.BindInterfacesAndSelfTo<InventoryUIManager>().AsSingle().NonLazy();
    }
}