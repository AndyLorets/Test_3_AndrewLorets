using UnityEngine;
using DG.Tweening; 
using Zenject;

[RequireComponent(typeof(CanvasGroup))]
public class InventoryWindowView : MonoBehaviour
{
    [SerializeField] private Transform _playerSlotsContainer;
    [SerializeField] private Transform _containerSlotsContainer;
    [SerializeField] private InventorySlotView _slotPrefab; 

    public InventoryView PlayerInventoryView { get; private set; }
    public InventoryView ContainerInventoryView { get; private set; }

    private CanvasGroup _canvasGroup;
    private bool _isOpen = false;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        Hide(immediate: true);
    }

    // Этот метод будет вызван Zenject'ом после создания этого объекта
    [Inject]
    public void Construct(
        [Inject(Id = InventoryIDs.Player)] InventoryModel playerInventory,
        [Inject(Id = InventoryIDs.Container)] InventoryModel containerInventory)
    {
        // Создаем и настраиваем View для инвентаря игрока
        PlayerInventoryView = new InventoryView(_playerSlotsContainer, _slotPrefab, playerInventory);
        PlayerInventoryView.Initialize();

        // Создаем и настраиваем View для инвентаря контейнера
        ContainerInventoryView = new InventoryView(_containerSlotsContainer, _slotPrefab, containerInventory);
        ContainerInventoryView.Initialize();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Toggle();
        }
    }
    public void Toggle()
    {
        if (_isOpen) Hide();
        else Show();
    }

    public void Show()
    {
        _isOpen = true;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.DOFade(1f, 0.3f);
    }

    public void Hide(bool immediate = false)
    {
        _isOpen = false;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        if (immediate) _canvasGroup.alpha = 0f;
        else _canvasGroup.DOFade(0f, 0.3f);
    }
}