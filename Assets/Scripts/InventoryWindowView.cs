using UnityEngine;
using DG.Tweening;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class InventoryWindowView : MonoBehaviour
{
    [SerializeField] private Transform _playerSlotsContainer;
    [SerializeField] private Transform _containerSlotsContainer;
    [SerializeField] private InventorySlotView _slotPrefab;
    [SerializeField] private TextMeshProUGUI _playerInventoryTitle; // <-- ДОБАВЛЕНО
    [SerializeField] private TextMeshProUGUI _containerInventoryTitle; // <-- ДОБАВЛЕНО

    // Сделаем их публичными, чтобы контроллер мог к ним обращаться
    public InventoryView PlayerInventoryView { get; private set; }
    public InventoryView ContainerInventoryView { get; private set; }


    private CanvasGroup _canvasGroup;
    public bool IsOpen { get; private set; }

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        // Важно: на старте очищаем панели, чтобы не было "мусора" от прошлых открытий
        ClearPanels();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Главный метод. Открывает окно с инвентарями и их названиями.
    /// </summary>
    public void Open(IInteractable player, IInteractable container) // <-- Изменили параметры
    {
        IsOpen = true;
        gameObject.SetActive(true);

        ClearPanels();

        // --- ЛОГИКА ОБНОВЛЕНА ---

        // 1. Устанавливаем название и View для игрока (всегда)
        _playerInventoryTitle.text = player.ContainerName;
        PlayerInventoryView = new InventoryView(_playerSlotsContainer, _slotPrefab, player.Inventory);
        PlayerInventoryView.Initialize();

        // 2. Устанавливаем название и View для контейнера (только если он есть)
        bool hasContainer = container != null;
        _containerSlotsContainer.gameObject.SetActive(hasContainer);
        _containerInventoryTitle.gameObject.SetActive(hasContainer);

        if (hasContainer)
        {
            _containerInventoryTitle.text = container.ContainerName;
            ContainerInventoryView = new InventoryView(_containerSlotsContainer, _slotPrefab, container.Inventory);
            ContainerInventoryView.Initialize();
        }

        // Плавно показываем окно
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.DOFade(1f, 0.3f);
    }

    public void Hide()
    {
        IsOpen = false;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.DOFade(0f, 0.3f).OnComplete(() =>
        {
            gameObject.SetActive(false);
            ClearPanels(); // Очищаем после закрытия
        });
    }

    private void ClearPanels()
    {
        // Уничтожаем старые UI-слоты
        foreach (Transform child in _playerSlotsContainer) Destroy(child.gameObject);
        foreach (Transform child in _containerSlotsContainer) Destroy(child.gameObject);

        // Отписываемся от событий старых моделей, чтобы избежать утечек памяти
        PlayerInventoryView?.Dispose();
        ContainerInventoryView?.Dispose();
        PlayerInventoryView = null;
        ContainerInventoryView = null;
    }
}